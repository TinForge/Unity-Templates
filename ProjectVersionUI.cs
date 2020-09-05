using UnityEngine;
using UnityEngine.UI;

public class ProjectVersionUI : MonoBehaviour
{
	[Help("Displays project version on UI text. Will automatically get Text on component")]
	[SerializeField] private string prefix = "v";
	private Text versionUI;

	private void Start()
	{
		if (versionUI == null)
			versionUI = GetComponent<Text>();
		if (versionUI != null)
			versionUI.text = prefix + Application.version;
	}
}