using UnityEngine;

public class FollowObject : MonoBehaviour
{
	[Help("Follows a target transform. Lerp for smooth tracking. Adjustable position/rotation. Teleport Snapping.")]
	[SerializeField] private bool startEnabled = true;
	[Space]
	[SerializeField] private Transform target;
	[Range(0, 1)] [SerializeField] private float lerpRate = 0.05f;

	[Header("Settings")]
	[SerializeField] private bool useLocalSpace;
	[SerializeField] private Vector3 positionOffset;
	[SerializeField] private Vector3 rotationOffset;
	[SerializeField] private RotationMode rotationMode;

	[Header("Teleport")]
	[SerializeField] private bool useTeleport = false;
	[SerializeField] private float teleportThreshold = 1;

	[Header("Output")]
	[ReadOnly] [SerializeField] private float distanceOutput;

	//Private
	private Transform cam; //For looking at camera

	//Property
	public bool IsFollowing { get; private set; }

	private void Start()
	{
		if (target == null)
			Debug.LogWarning("FollowObject has nothing to follow");

		cam = FindObjectOfType<OVRCameraRig>().centerEyeAnchor;

		if (cam == null && rotationMode == RotationMode.LookAtCamera)
			cam = Camera.main.transform;
		if (cam == null && rotationMode == RotationMode.LookAtCamera)
			Debug.LogError("FollowObject couldn't find camera");

		Toggle(startEnabled);
	}

	void LateUpdate()
	{
		//Null check
		if (target == null)
			return;

		//Teleport
		if (useTeleport)
		{
			distanceOutput = Vector3.Distance(transform.position, target.TransformPoint(positionOffset));
			if (distanceOutput > positionOffset.magnitude + teleportThreshold)
			{
				transform.position = target.TransformPoint(positionOffset);
			}
		}

		//Position
		if (useLocalSpace)
			transform.position = Vector3.Lerp(transform.position, target.TransformPoint(positionOffset), lerpRate);
		else
			transform.position = Vector3.Lerp(transform.position, target.position + positionOffset, lerpRate);

		//Rotation
		if (rotationMode == RotationMode.LookAtCamera)
			transform.rotation = Quaternion.LookRotation(cam.position - transform.position) * Quaternion.Euler(rotationOffset);
		else if (rotationMode == RotationMode.YAxisOnly)
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(target.eulerAngles.y, Vector3.up) * Quaternion.Euler(rotationOffset), lerpRate);
		else if (rotationMode == RotationMode.FullRotation)
			transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation * Quaternion.Euler(rotationOffset), lerpRate);
	}

	/// <summary>
	/// Set Target for Object to follow
	/// </summary>
	public void SetTarget(Transform target)
	{
		this.target = target;
	}

	/// <summary>
	/// Activate/Deactivate this component
	/// </summary>
	public void Toggle(bool state)
	{
		IsFollowing = state;
		gameObject.SetActive(state);
	}


	public enum RotationMode { NoRotation, LookAtCamera, YAxisOnly, FullRotation }
}