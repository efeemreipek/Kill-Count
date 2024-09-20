using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public static CameraShake Instance { get; private set; }

	private Vector3 originalPos;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(this);
			return;
		}
		Instance = this;

		originalPos = transform.localPosition;
	}

	public void Shake(float duration, float amount)
	{
		StopAllCoroutines();
		StartCoroutine(ShakeCoroutine(duration, amount));
	}

	public IEnumerator ShakeCoroutine(float duration, float amount)
	{
		float endTime = Time.time + duration;

		while (Time.time < endTime)
		{
			transform.localPosition = originalPos + Random.insideUnitSphere * amount;

			duration -= Time.deltaTime;

			yield return null;
		}

		transform.localPosition = originalPos;
	}

}
