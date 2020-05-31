using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shifts pitch of sound.
/// Configured to engine sound - quiet low pitch oscillates to loud high pitch
/// </summary>
public class PitchShift : MonoBehaviour
{
	[SerializeField] private AudioSource audioSource;

	[Header("Pitch")]
	[SerializeField] [Range(0, 2)] private float minPitch;
	[SerializeField] [Range(0, 2)] private float maxPitch;

	[Header("Volume")]
	[SerializeField] [Range(0, 2)] private float minVolume;
	[SerializeField] [Range(0, 2)] private float maxVolume;

	[Header("ReadOnly")]
	[ReadOnly] [SerializeField] private float currentRate;
	[ReadOnly] [SerializeField] private float targetRate;


	public void SetTargetRate(float targetRate)
	{
		this.targetRate = targetRate;
	}

	private void Update()
	{
		currentRate = Mathf.MoveTowards(currentRate, targetRate, 0.02f);

		audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, currentRate);
		audioSource.volume = Mathf.Lerp(minVolume, maxVolume, currentRate);
	}
}
