using UnityEngine;
using System.Collections.Generic;

namespace VRVision.Quest
{
	/// <summary>
	/// A debugging tool that will play a sound when an error occurs on the headset
	/// </summary>
	public class ErrorSounder : MonoBehaviour
	{
		public AudioSource errorSound;

		public List<AudioClip> sounds;

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
			{
				int i = Random.Range(0, sounds.Count);
				errorSound.clip = sounds[i];
				errorSound.Play();
			}
		}
	}
}