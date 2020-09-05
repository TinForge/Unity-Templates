using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class VelocityTracker : MonoBehaviour
{
	[Help("Calculates the velocity of the object without needing a RigidBody. Calculated using Transform.position deltas.")]
	[Range(0, 4)] [SerializeField] private float multiplier = 1;

	Vector3 lastPos = Vector3.zero;
	Vector3 vD = Vector3.zero; //velocity delta
	Vector3 vF = Vector3.zero; //velocity final

	Quaternion lastRot = Quaternion.identity;
	Quaternion deltaRotation = Quaternion.identity; //angular velocity delta
	Vector3 avF = Vector3.zero; //angular velocity final

	[Header("Output")]
	[ReadOnly] [SerializeField] private float velocityMagnitude = 0;
	[ReadOnly] [SerializeField] private float angularVelocityMagnitude = 0;

	private void OnEnable()
	{
		vF = Vector3.zero;
		avF = Vector3.zero;
	}

	private void OnDisable()
	{
		vF = Vector3.zero;
		avF = Vector3.zero;
	}

	private void FixedUpdate()
	{
		//Velocity
		vD = (transform.position - lastPos) / Time.deltaTime;
		lastPos = transform.position;
		vF = Vector3.Lerp(vF, vD, 0.1f);
		velocityMagnitude = vF.magnitude * multiplier;

		//Angular Velocity
		deltaRotation = transform.rotation * Quaternion.Inverse(lastRot);
		lastRot = transform.rotation;
		deltaRotation.ToAngleAxis(out var angle, out var axis);
		angle *= Mathf.Deg2Rad;
		avF = (1.0f / Time.deltaTime) * angle * axis;
		angularVelocityMagnitude = avF.magnitude * multiplier;
	}

	/// <summary>
	/// Get the calculated velocity from Transform.position deltas.
	/// </summary>
	/// <returns></returns>
	public Vector3 GetVelocity()
	{
		return vF * multiplier;
	}

	public Vector3 GetAngularVelocity()
	{
		return avF * multiplier;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawRay(transform.position, vF * multiplier);
	}
}