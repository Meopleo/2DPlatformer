using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JellyEffect : MonoBehaviour
{
    public enum JellyState { Flying, Sticking, Bouncing }
    private JellyState currentState = JellyState.Flying;

    [Header("Jigli Beállítások")]
    [SerializeField] private Transform visuals;
    [SerializeField] private float stretchFactor = 0.08f;
    [SerializeField] private float maxStretch = 0.6f;
    [SerializeField] private float recoverySpeed = 10f;

    [Header("Fizikai Pontosság (Radius)")]
    [SerializeField] private float offsetMultiplier = 2.65f;

    private Vector3 originalScale;
    private Rigidbody2D rb;
    private Vector3 targetScale;
    private Quaternion targetRotation;
    private Vector3 targetLocalPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (visuals == null) return;

        originalScale = visuals.localScale;
        targetScale = originalScale;
        targetRotation = Quaternion.identity;
        targetLocalPosition = Vector3.zero;
    }

    void Update()
    {
        if (visuals == null) return;

        float currentSpeed = rb.linearVelocity.magnitude;

        if (currentState == JellyState.Sticking && currentSpeed > 0.5f)
        {
            currentState = JellyState.Flying;
            StopAllCoroutines();
            targetLocalPosition = Vector3.zero;
        }

        if (currentState == JellyState.Flying)
        {
            if (currentSpeed > 0.2f)
            {
                float stretch = currentSpeed * stretchFactor;
                stretch = Mathf.Clamp(stretch, 0f, maxStretch);

                targetScale.y = originalScale.y * (1f + stretch);
                targetScale.x = originalScale.x * (1f - stretch * 0.4f);

                float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg - 90f;
                targetRotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                targetScale = originalScale;
                targetRotation = Quaternion.identity;
            }

            visuals.localScale = Vector3.Lerp(visuals.localScale, targetScale, Time.deltaTime * recoverySpeed);
            visuals.localPosition = Vector3.Lerp(visuals.localPosition, targetLocalPosition, Time.deltaTime * recoverySpeed);
        }

        visuals.rotation = Quaternion.Lerp(visuals.rotation, targetRotation, Time.deltaTime * recoverySpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;
        StopAllCoroutines();

        Vector2 wallNormal = collision.GetContact(0).normal;
        float angle = Mathf.Atan2(-wallNormal.y, -wallNormal.x) * Mathf.Rad2Deg - 90f;
        targetRotation = Quaternion.Euler(0, 0, angle);
        visuals.rotation = targetRotation;

        if (collision.gameObject.CompareTag("Sticky"))
        {
            StartCoroutine(StickySplatRoutine(impactForce));
        }
        else
        {
            StartCoroutine(RealisticBounceRoutine(impactForce));
        }
    }

    // --- JAVÍTVA: Finomabb felkenődés pici érintésnél ---
    private IEnumerator StickySplatRoutine(float force)
    {
        currentState = JellyState.Sticking;

        // A minimum határt 0.4-ről levettem 0.05-re! (Kis koccanás = kis lapulás)
        float squash = Mathf.Clamp(force * 0.05f, 0.05f, 1.2f);

        targetScale = new Vector3(originalScale.x * (1f + squash), originalScale.y * (1f - squash * 0.5f), originalScale.z);

        float flatCutOffset = offsetMultiplier * 0.15f;
        targetLocalPosition = targetRotation * new Vector3(0, (originalScale.y - targetScale.y) * offsetMultiplier + flatCutOffset, 0);

        float duration = 0.6f;
        float time = 0f;
        float intensity = Mathf.Clamp(force * 0.1f, 0.2f, 1.5f);

        while (time < duration)
        {
            time += Time.deltaTime;
            float dampening = Mathf.Exp(-time * 10f);
            float wobble = Mathf.Cos(time * 40f) * dampening * intensity;

            visuals.localScale = new Vector3(
                targetScale.x * (1f + wobble * 0.4f),
                targetScale.y * (1f - wobble * 0.5f),
                originalScale.z
            );

            float currentShift = (originalScale.y - visuals.localScale.y) * offsetMultiplier + flatCutOffset;
            visuals.localPosition = targetRotation * new Vector3(0, currentShift, 0);

            yield return null;
        }

        visuals.localScale = targetScale;
        visuals.localPosition = targetLocalPosition;
    }

    // --- JAVÍTVA: Finomabb rezgés pici érintésnél ---
    private IEnumerator RealisticBounceRoutine(float force)
    {
        currentState = JellyState.Bouncing;

        float duration = 0.8f;
        float time = 0f;
        // A minimum rezgést is sokkal kisebbre vettem
        float intensity = Mathf.Clamp(force * 0.08f, 0.15f, 1.5f);

        while (time < duration)
        {
            time += Time.deltaTime;

            float dampening = Mathf.Exp(-time * 8f);
            float wobble = Mathf.Cos(time * 45f) * dampening * intensity;

            visuals.localScale = new Vector3(
                originalScale.x * (1f + wobble),
                originalScale.y * (1f - wobble * 0.8f),
                originalScale.z
            );

            float dynamicSink = Mathf.Max(0, wobble) * offsetMultiplier * 0.7f;
            visuals.localPosition = targetRotation * new Vector3(0, dynamicSink, 0);

            yield return null;
        }

        visuals.localScale = originalScale;
        visuals.localPosition = Vector3.zero;
        currentState = JellyState.Flying;
    }
}