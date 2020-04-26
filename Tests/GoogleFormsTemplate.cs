using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

//https://www.youtube.com/watch?v=fPfH8ZLcrmY

public class GoogleFormsTemplate : MonoBehaviour
{
	private const string kGFormBaseURL = "https://docs.google.com/forms/example/";

	private static string formName = "entry.391122070";
	private static string formLocation = "entry.1472257687";
	private static string formScore = "entry.1647030640";

	void Start()
	{
		StartCoroutine(SendGFormData("1", "2", "3"));
	}

	private static IEnumerator SendGFormData(string nameField, string locationField, string scoreField)
	{
		WWWForm form = new WWWForm();
		form.AddField(formName, nameField);
		form.AddField(formLocation, locationField);
		form.AddField(formScore, scoreField);
		string urlGFormResponse = kGFormBaseURL + "formResponse";
		using (UnityWebRequest www = UnityWebRequest.Post(urlGFormResponse, form))
		{
			yield return www.SendWebRequest();
			Debug.Log("Data Submitted");
		}
	}
}