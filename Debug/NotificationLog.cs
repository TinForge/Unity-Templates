using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationLog : MonoBehaviour
{
	public static NotificationLog instance;

	public List<RectTransform> log = new List<RectTransform>();

	[SerializeField] private GameObject notificationPrefab;

	[SerializeField] private Transform parent;

	void Awake()
	{
		if (instance != this && instance != null)
			Destroy(instance.gameObject);

		instance = this;
	}

	void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}

	void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception)
			Message(stackTrace);
	}

	public void Message(string message)
	{
		foreach (RectTransform rt in log)
			if (rt != null)
				rt.anchoredPosition += Vector2.up * (rt.rect.height * 1.2f);
			else
				log.Remove(rt);

		RectTransform notification = Instantiate(notificationPrefab, parent).GetComponent<RectTransform>();
		notification.GetComponentInChildren<Text>().text = message;
		log.Add(notification);
		StartCoroutine(Remove(notification, 4));
	}

	private IEnumerator Remove(RectTransform rt, float delay)
	{
		Vector3 origin = rt.anchoredPosition;
		CanvasGroup cg = rt.GetComponent<CanvasGroup>();

		float t = 0;

		while (t <= 1)
		{
			cg.alpha = t * t;
			t += Time.deltaTime;
			yield return null;
		}
		cg.alpha = 1;

		t = 0;
		while (t <= 1)
		{
			//rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, origin + Vector3.up * 1, 0.1f);
			t += Time.deltaTime / 6;
			yield return null;
		}

		t = 0;
		while (t <= 1)
		{
			cg.alpha = 1 - (t * t);
			t += Time.deltaTime;
			yield return null;
		}
		cg.alpha = 0;

		Destroy(rt.gameObject);
		log.Remove(rt);

	}
}

/*
public class NotificationModule : MonoBehaviour
{
	[SerializeField] private CanvasGroup cg;
	[SerializeField] private Text content;

	public void SetText(string message)
	{
		content.text = message;
	}

	void Start()
	{
		StartCoroutine(AnimateIn());
	}

	public void Destroy()
	{
		StartCoroutine(AnimateOut());
	}

	public void MoveUp()
	{
		StartCoroutine(ShiftUp());
	}

	private IEnumerator ShiftUp()
	{
		float t = 0;
		RectTransform rt = GetComponent<RectTransform>();
		Vector3 start = rt.anchoredPosition;
		Vector3 end = start + Vector3.up * rt.rect.height;

		while (t <= 1)
		{
			rt.anchoredPosition = Vector3.Lerp(start, end, t * t);
			t += Time.deltaTime * 2;
			yield return null;
		}
		cg.alpha = 1;

	}

	private IEnumerator AnimateIn()
	{
		float t = 0;

		while (t <= 1)
		{
			cg.alpha = t * t;
			t += Time.deltaTime;
			yield return null;
		}
		cg.alpha = 1;

	}

	private IEnumerator AnimateOut()
	{
		float t = 0;

		while (t <= 1)
		{
			cg.alpha = 1 - (t * t);
			t += Time.deltaTime;
			yield return null;
		}
		cg.alpha = 0;
		Destroy(gameObject);
	}

}
*/
