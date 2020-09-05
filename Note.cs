using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinForge.EditorTools
{
	public class Note : MonoBehaviour
	{
		[TextArea(5, 25)]
		[SerializeField] private string text;
	}
}