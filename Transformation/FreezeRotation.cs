using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
	[Help("Sets a constant world rotation regardless of parent transform.")]
	[SerializeField] private Vector3 rot;

	private void Update()
	{
		transform.eulerAngles = rot;
	}
}
