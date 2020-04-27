using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VRVision.Quest
{
	//QSDK v2.4 Revised WTY
	[DisallowMultipleComponent]
	public class Emission : MonoBehaviour
	{
		[Help("Adds emission effect to object's material.")]
		[SerializeField] private bool startEnabled;
		[Header("Flash")]
		[SerializeField] private bool flashEffect;

		[Header("Settings")]
		[SerializeField] private Color emissionColor = Color.white;
		[Tooltip("Set value higher than 1 to add white")][SerializeField] private float emissionIntensity = 1;
		[Space]
		[ReadOnly][SerializeField] private Renderer[] renderersOutput;

		void Awake()
		{
			renderersOutput = GetComponentsInChildren<Renderer>();
			foreach (Renderer r in renderersOutput)
				r.material.EnableKeyword("_EMISSION");
		}

		void Start()
		{
			if (!startEnabled)
				Toggle(false);
		}

		public void Toggle(bool state)
		{
			this.enabled = state;
		}

		private void OnEnable()
		{
			foreach (Renderer r in renderersOutput)
				r.material.SetColor("_EmissionColor", emissionColor * emissionIntensity);
		}
		private void OnDisable()
		{
			foreach (Renderer r in renderersOutput)
				r.material.SetColor("_EmissionColor", emissionColor * 0);
		}

		public void SetColor(Color color)
		{
			emissionColor = color;
		}

		public void ToggleFlash(bool state)
		{
			flashEffect = state;
		}

		void Update()
		{
			if (flashEffect)
				foreach (Renderer r in renderersOutput)
					r.material.SetColor("_EmissionColor", emissionColor * Mathf.PingPong(Time.realtimeSinceStartup, 1) * emissionIntensity);
		}


	}
}