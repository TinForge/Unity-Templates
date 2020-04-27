using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VRVision.Quest
{
	//QSDK v2.4 Revised WTY
	public class CollisionUnityEvents : MonoBehaviour
	{
		[Help("Put this on objects you want to collide/trigger. Invokes UnityEvents using Collider/Trigger callbacks. Requires a collider component on the same object. Does not include Collision/Collider argument.")]
		[SerializeField] private bool printDebugLogs = true;

		public MyEvent onEnter;
		public MyEvent onStay;
		public MyEvent onExit;

		private void Awake()
		{
			if (GetComponent<Collider>() == null)
				Debug.LogError("CollisionUnityEvents needs a collider");
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (printDebugLogs)
				Debug.Log(name + " :OnCollisionEnter");
			onEnter?.Invoke();
		}

		private void OnCollisionStay(Collision collision)
		{
			if (printDebugLogs)
				Debug.Log(name + " :OnCollisionStay");
			onStay?.Invoke();
		}

		private void OnCollisionExit(Collision collision)
		{
			if (printDebugLogs)
				Debug.Log(name + " :OnCollisionExit");
			onExit?.Invoke();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (printDebugLogs)
				Debug.Log(name + " :OnTriggerEnter");
			onEnter?.Invoke();
		}

		private void OnTriggerStay(Collider other)
		{
			if (printDebugLogs)
				Debug.Log(name + " :OnTriggerStay");
			onStay?.Invoke();
		}

		private void OnTriggerExit(Collider other)
		{
			if (printDebugLogs)
				Debug.Log(name + " :OnTriggerExit");
			onExit?.Invoke();
		}

		/// <summary>
		/// Toggles InvokingEvents if Collision Callbacks are received
		/// </summary>
		public void ToggleInvokeEvents(bool state)
		{
			printDebugLogs = state;
		}

		[Serializable]
		public class MyEvent : UnityEvent { }
	}
}
