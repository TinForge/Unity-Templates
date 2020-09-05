using System.Collections;

using UnityEngine;

/// <summary>
/// Fades UI CanvasGroup out using an animation curve for quick & easy customization.
/// Attach this to a UI canvas with a CanvasGroup and it'll follow the alphaCurve
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class AlphaAnimator : MonoBehaviour
{
	[Help("Animate alpha for UI. Requires CanvasGroup. Set StartMode or manually call Play().")]
	[SerializeField] private StartMode startMode;
	[SerializeField] private RepeatMode repeatMode;
	[SerializeField] private bool destroyOnFinish = false;

	[Header("Animation Settings")]
	[SerializeField] private float animationTime = 4;
	[Tooltip("Alpha of 0 should evaluate to 0. Alpha of 1 should evaluate to 1.")]
	[SerializeField] private AnimationCurve alphaCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

	//Private
	private CanvasGroup cg;

	private void Awake()
	{
		cg = GetComponent<CanvasGroup>();
		if (cg == null)
			Debug.LogError("Couldn't find " + cg.GetType().ToString() + " in " + GetType().ToString());
	}

	private void OnEnable()
	{
		switch (startMode)
		{
		case StartMode.Nothing:
			break;
		case StartMode.SetStartAlpha:
			SetCG(0);
			break;
		case StartMode.SetEndAlpha:
			SetCG(1);
			break;
		case StartMode.Play:
			PlayForward();
			break;
		case StartMode.PlayReverse:
			PlayReverse();
			break;
		}
	}


	/// <summary>
	/// Plays forward 0-1
	/// </summary>
	public void PlayForward()
	{
		StopAllCoroutines();
		StartCoroutine(AnimateRoutine(0, 1));
	}

	/// <summary>
	/// Plays reverse 1-0
	/// </summary>
	public void PlayReverse()
	{
		StopAllCoroutines();
		StartCoroutine(AnimateRoutine(1, 0));
	}

	/// <summary>
	/// Automatically plays forward/reverse based on current value. Won't play if alpha is not 0 or 1.
	/// </summary>
	public void PlayToggle()
	{
		if (cg.alpha == 0f)
		{
			StopAllCoroutines();
			StartCoroutine(AnimateRoutine(0, 1));
		}
		else if (cg.alpha == 1f)
		{
			StopAllCoroutines();
			StartCoroutine(AnimateRoutine(1, 0));
		}
	}

	/// <summary>
	/// Plays forward(true)/reverse(false) based on bool
	/// </summary>
	public void Play(bool forward)
	{
		StopAllCoroutines();
		if (forward)
			StartCoroutine(AnimateRoutine(0, 1));
		else
			StartCoroutine(AnimateRoutine(1, 0));
	}

	/// <summary>
	/// Set custom start/end (values 0-1 accepted)
	/// </summary>
	public void Play(float start, float end)
	{
		StopAllCoroutines();
		StartCoroutine(AnimateRoutine(start, end));
	}

	private IEnumerator AnimateRoutine(float start, float end)
	{
		if (cg == null)
		{
			Debug.Log("No CanvasGroup");
			yield break;
		}

		if (cg.alpha == end) //if CG is already at the result we want
			yield break;

		float t = 0;
		while (t < 1)
		{
			float lerp = Mathf.Lerp(start, end, t);
			SetCG(alphaCurve.Evaluate(lerp));
			t += Time.deltaTime / animationTime;
			yield return null;
		}
		SetCG(alphaCurve.Evaluate(end));

		if (destroyOnFinish)
			Destroy(gameObject);
		else if (repeatMode == RepeatMode.Loop)
			StartCoroutine(AnimateRoutine(start, end));
		else if (repeatMode == RepeatMode.PingPong)
			StartCoroutine(AnimateRoutine(end, start));
	}

	public void Stop()
	{
		StopAllCoroutines();
	}

	private void SetCG(float alpha)
	{
		cg.alpha = alpha;
		cg.blocksRaycasts = alpha != 0;
		cg.interactable = alpha != 0;
	}

	public void SetRepeatMode(RepeatMode mode)
	{
		repeatMode = mode;
	}

	public enum StartMode { Nothing, SetStartAlpha, SetEndAlpha, Play, PlayReverse }
	public enum RepeatMode { Nothing, Loop, PingPong }

}
