using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VRVision.Quest
{
	//QSDK v2.4 Revised WTY
	public class CollisionPointerEvents : MonoBehaviour
	{
		[Help("Put this on objects you want to call PointerEvents on. Invokes PointerEvents using Collider/Trigger callbacks. Requires a collider component on the same object.")]
		[SerializeField] private bool printDebugLogs = true;


		private void Awake()
		{
			if (GetComponent<Collider>() == null)
				Debug.LogError("CollisionPointerEvents needs a collider");
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (printDebugLogs)
				Debug.Log(name + " :OnCollisionEnter");
			ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current),ExecuteEvents.pointerEnterHandler);
		}

		/*
		private void OnCollisionStay(Collision collision)
		{
			if (printDebugLogs)
				Debug.Log(name + " :OnCollisionStay");
		}
		*/

		private void OnCollisionExit(Collision collision)
		{
			if (printDebugLogs)
				Debug.Log(name + " :OnCollisionExit");
			ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (printDebugLogs)
				Debug.Log(name + " :OnTriggerEnter");
			ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
		}

		/*
		private void OnTriggerStay(Collider other)
		{
			if (printDebugLogs)
				Debug.Log(name + " :OnTriggerStay");
		}
		*/

		private void OnTriggerExit(Collider other)
		{
			if (printDebugLogs)
				Debug.Log(name + " :OnTriggerExit");
			ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
		}

	}
}
