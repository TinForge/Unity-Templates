using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Mail;
using System.Net;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.SceneManagement;

public class LaunchAPK : MonoBehaviour
{
	/// <summary>
	/// Launches an APK, doesn't destroy current app instance
	/// </summary>
	public static void Load(string bundle)
	{
#if UNITY_ANDROID
			bool fail = false;
			string bundleId = bundle; // your target bundle id
			print(bundleId);
			AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

			AndroidJavaObject launchIntent = null;
			try
			{
				launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
			}
			catch (System.Exception e)
			{
				fail = true;
			}

			if (fail)
			{
				Debug.Log("Failed to load APK");
			}
			else //open the app
				ca.Call("startActivity", launchIntent);

			up.Dispose();
			ca.Dispose();
			packageManager.Dispose();
			launchIntent.Dispose();
#endif
	}

	/// <summary>
	/// Launches an APK, destroys current app instance
	/// </summary>
	public static void LaunchApkKillCurrent(string bundle)
	{
		Load(bundle);
#if UNITY_ANDROID
			System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
	}
}
