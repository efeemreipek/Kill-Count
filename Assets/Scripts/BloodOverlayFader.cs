using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodOverlayFader : MonoBehaviour
{
    [SerializeField] private float overlayInTime = 0.5f;
    [SerializeField] private float overlayOutTime = 0.25f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        Health.OnDamageTaken += Health_OnDamageTaken;
    }
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    private void OnDisable()
    {
        Health.OnDamageTaken -= Health_OnDamageTaken;
    }

    private void Health_OnDamageTaken()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        float time = 0f;

        while(time < overlayInTime)
        {
            canvasGroup.alpha = 0f;
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / overlayInTime);
            yield return null;
        }
        time = 0f;
        while (time < overlayOutTime)
        {
            canvasGroup.alpha = 1f;
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / overlayInTime);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
