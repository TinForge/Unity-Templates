using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRVision.EditorUtility
{
	public class DevInfo : MonoBehaviour
	{
		[TextArea(5, 25)]
		[SerializeField] private string text;
	}
}