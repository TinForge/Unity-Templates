using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RepetitiveMotion : MonoBehaviour
{
	[Help("Ping pong position/rotation for dynamic scenery. Completely overrides position/rotation.")]

	[SerializeField] private bool smoothStep;
	[SerializeField] private bool localSpace;
	[Space]
	[SerializeField] private bool usePosition;
	[SerializeField] private float posTime = 1;
	[SerializeField] private float posX = 0;
	[SerializeField] private float posY = 0;
	[SerializeField] private float posZ = 0;
	[Space]
	[SerializeField] private bool useRotation;
	[SerializeField] private float rotTime = 1;
	[SerializeField] private float rotX = 0;
	[SerializeField] private float rotY = 0;
	[SerializeField] private float rotZ = 0;

	private Vector3 posOffset;
	private Vector3 rotOffset;
	private float t = 0;
	private float posLerp;
	private float rotLerp;

	void Start()
	{
		posOffset = localSpace ? transform.localPosition : transform.position;
		rotOffset = localSpace ? transform.localEulerAngles : transform.eulerAngles;
	}

	void Update()
	{
		t += Time.deltaTime;
		posLerp = Mathf.PingPong(t / posTime, 1);
		rotLerp = Mathf.PingPong(t / rotTime, 1);

		if (smoothStep)
		{
			posLerp = Mathf.Cos(posLerp * Mathf.PI);
			rotLerp = Mathf.Cos(rotLerp * Mathf.PI);
		}

		float pX = posX * posLerp;
		float pY = posY * posLerp;
		float pZ = posZ * posLerp;

		float rX = rotX * rotLerp;
		float rY = rotY * rotLerp;
		float rZ = rotZ * rotLerp;

		if (usePosition)
			if (localSpace)
				transform.localPosition = posOffset + new Vector3(pX, pY, pZ);
			else
				transform.position = posOffset + new Vector3(pX, pY, pZ);
		if (useRotation)
			if (localSpace)
				transform.localEulerAngles = rotOffset + new Vector3(rX, rY, rZ);
			else
				transform.eulerAngles = rotOffset + new Vector3(rX, rY, rZ);

	}
}