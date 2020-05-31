using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticCoroutine
{
	private class CoroutineHolder : MonoBehaviour { }

	private static CoroutineHolder holder;

	private static CoroutineHolder Holder
	{
		get
		{
			if (holder == null)
				holder = new GameObject("Static Coroutine Holder").AddComponent<CoroutineHolder>();
			return holder;
		}
	}

	public static void StartCoroutine(IEnumerator coroutine)
	{
		Holder.StartCoroutine(coroutine);
	}
}