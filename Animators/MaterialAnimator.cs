using System.Collections;

using UnityEngine;

/// <summary>
/// Fades material color using an animation curve for quick & easy customization.
/// Attach this to a gameObject with a material and it'll follow the alphaCurve
/// </summary>
[RequireComponent(typeof(Renderer))]
public class MaterialAnimator : MonoBehaviour
{
	[Help("Animate color/alpha for material. Set StartMode or manually call Play().")]
	[SerializeField] private StartMode startMode;
	[SerializeField] private RepeatMode repeatMode;
	[SerializeField] private bool destroyOnFinish = false;

	[Header("Animation Settings")]
	public float animationTime = 5;
	[Tooltip("Alpha of 0 should evaluate to 0. Alpha of 1 should evaluate to 1.")]
	[SerializeField] private AnimationCurve alphaCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

	[SerializeField] private Color startColor = Color.clear;
	[SerializeField] private Color endColor;

	[Header("Other Settings")]
	[SerializeField] private string shaderName = "_MainColor";

	private new Renderer renderer;

	protected void Awake()
	{
		renderer = GetComponent<Renderer>();
		if (renderer == null)
			Debug.LogError("Couldn't find " + renderer.GetType().ToString() + " in " + GetType().ToString());
	}

	private void OnEnable()
	{
		switch (startMode)
		{
		case StartMode.Nothing:
			break;
		case StartMode.SetStartColor:
			renderer.material.SetColor(shaderName, startColor);
			break;
		case StartMode.SetEndColor:
			renderer.material.SetColor(shaderName, endColor);
			break;
		case StartMode.Play:
			PlayForward();
			break;
		case StartMode.PlayReverse:
			PlayReverse();
			break;
		}
	}

	public void Hide()
	{
		renderer.material.SetColor(shaderName, startColor);
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
	/// Automatically plays forward/reverse based on current value. Won't play if color is not startColor/endColor.
	/// </summary>
	public void PlayToggle()
	{
		if (renderer.material.GetColor(shaderName) == startColor)
		{
			StopAllCoroutines();
			StartCoroutine(AnimateRoutine(0, 1));
		}
		else if (renderer.material.GetColor(shaderName) == endColor)
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
		if (renderer.material.GetColor(shaderName) == (end == 0 ? startColor : endColor)) //if material color is already end color.
			yield break;

		float t = 0;
		while (t <= 1)
		{
			float a = Mathf.Lerp(start, end, t);
			renderer.material.SetColor(shaderName, Color.Lerp(startColor, endColor, alphaCurve.Evaluate(a)));
			t += Time.deltaTime / animationTime;
			yield return null;
		}
		renderer.material.SetColor(shaderName, Color.Lerp(startColor, endColor, alphaCurve.Evaluate(end)));

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

	public void SetRepeatMode(RepeatMode mode)
	{
		repeatMode = mode;
	}

	public enum StartMode { Nothing, SetStartColor, SetEndColor, Play, PlayReverse }
	public enum RepeatMode { Nothing, Loop, PingPong }
}