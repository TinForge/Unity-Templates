using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Rotate : MonoBehaviour
{
	[SerializeField] private bool localSpace;
	[SerializeField] private Vector3 speed;

	void Update()
	{
		if (localSpace)
			transform.localEulerAngles += (speed * Time.deltaTime);
		else
			transform.eulerAngles += (speed * Time.deltaTime);
	}
}
