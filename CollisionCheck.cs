using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollisionCheck : MonoBehaviour
{
	[Help("Checks if colliding with anything. Requires Rigidbody & collider. You can specify transform names to ignore.")]
	[SerializeField] private CollisionType collisionType;
	[SerializeField] private bool matchExactName;
	[Space]
	[Help("Ignores if object name contains the phrase. If matchExactName is enabled, ignores exact matches only")]
	[SerializeField] private List<string> ignoreList;
	[Header("Read Only")]
	[ReadOnly] [SerializeField] private bool isColliding;
	[ReadOnly] [SerializeField] private int collisionCount;
	[ReadOnly] [SerializeField] private List<Transform> triggers;

	public Action CollisionEntered;
	public Action CollisionExited;

	public bool IsColliding
	{
		get { return triggers.Count > 0; }
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collisionType == CollisionType.Collision)
		{
			if (!Ignore(collision.transform.name))
			{
				if (!triggers.Contains(collision.transform))
				{
					triggers.Add(collision.transform);
					isColliding = IsColliding;
					collisionCount = triggers.Count;
					CollisionEntered?.Invoke();
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (collisionType == CollisionType.Trigger)
		{
			if (!Ignore(other.name))
			{
				if (!triggers.Contains(other.transform))
				{
					triggers.Add(other.transform);
					isColliding = IsColliding;
					collisionCount = triggers.Count;
					CollisionEntered?.Invoke();
				}
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collisionType == CollisionType.Collision)
		{
			if (triggers.Contains(collision.transform))
			{
				triggers.Remove(collision.transform);
				isColliding = IsColliding;
				collisionCount = triggers.Count;
				CollisionExited?.Invoke();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (collisionType == CollisionType.Trigger)
		{
			if (triggers.Contains(other.transform))
			{
				triggers.Remove(other.transform);
				isColliding = IsColliding;
				collisionCount = triggers.Count;
				CollisionExited?.Invoke();
			}
		}
	}

	private bool Ignore(string name)
	{
		if (matchExactName)
		{
			return ignoreList.Contains(name);
		}
		else
		{
			foreach (string s in ignoreList)
				if (name.Contains(s))
					return true;
			return false;
		}
	}

	public enum CollisionType { Collision, Trigger }
}