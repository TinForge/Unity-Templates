using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

[InitializeOnLoad]
public class QuestSettings : EditorWindow
{
	List<Item> items;

	public class Item
	{
		const string ignore = "ignore.";
		const string useRecommended = "Use recommended ({0})";
		const string currentValue = " (current = {0})";

		public delegate bool DelegateIsReady();
		public delegate string DelegateGetCurrent();
		public delegate void DelegateSet();

		public DelegateIsReady IsReady;
		public DelegateGetCurrent GetCurrent;
		public DelegateSet Set;

		public string title { get; private set; }
		public string recommended { get; private set; }

		public Item(string title, string recommended)
		{
			this.title = title;
			this.recommended = recommended;
		}

		public bool IsIgnored { get { return EditorPrefs.HasKey(ignore + title); } }

		public void Ignore()
		{
			EditorPrefs.SetBool(ignore + title, true);
		}

		public void CleanIgnore()
		{
			EditorPrefs.DeleteKey(ignore + title);
		}

		// Return true when setting is not ready.
		public bool Show()
		{
			bool ignored = IsIgnored;
			GUILayout.Label(title + string.Format(currentValue, GetCurrent()) + (IsIgnored ? " (ignored)" : ""));
			if (ignored || IsReady())
				return false;

			GUILayout.BeginHorizontal();
			if (GUILayout.Button(string.Format(useRecommended, recommended)))
				Set();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Ignore"))
				Ignore();
			GUILayout.EndHorizontal();
			return true;
		}
	}

	#region version_compatible

	public static bool GetVirtualRealitySupported(BuildTargetGroup group)
	{
#if UNITY_2019_1_OR_NEWER
		return PlayerSettings.GetVirtualRealitySupported(group);
#else
        return UnityEditorInternal.VR.VREditor.GetVREnabledOnTargetGroup(group);
#endif
	}

	public static void SetVirtualRealitySupported(BuildTargetGroup group, bool set)
	{
#if UNITY_2019_1_OR_NEWER
		PlayerSettings.SetVirtualRealitySupported(group, set);
#else
        UnityEditorInternal.VR.VREditor.SetVREnabledOnTargetGroup(group, set);
#endif
	}

	public static string[] GetVirtualRealitySDKs(BuildTargetGroup group)
	{
#if UNITY_2019_1_OR_NEWER
		return PlayerSettings.GetVirtualRealitySDKs(group);
#else
        return UnityEditorInternal.VR.VREditor.GetVREnabledDevicesOnTargetGroup(group);
#endif
	}

	public static void SetVirtualRealitySDKs(BuildTargetGroup group, string[] devices)
	{
#if UNITY_2019_1_OR_NEWER
		PlayerSettings.SetVirtualRealitySDKs(group, devices);
#else
        UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(group, devices);
#endif
	}

	public static bool GetMobileMTRendering(BuildTargetGroup group)
	{
#if UNITY_2019_1_OR_NEWER
		return PlayerSettings.GetMobileMTRendering(group);
#else
        return PlayerSettings.mobileMTRendering;
#endif
	}

	public static void SetMobileMTRendering(BuildTargetGroup group, bool set)
	{
#if UNITY_2019_1_OR_NEWER
		PlayerSettings.SetMobileMTRendering(group, set);
#else
        PlayerSettings.mobileMTRendering = set;
#endif
	}
	#endregion

	public static List<string> GetDefineSymbols(BuildTargetGroup group)
	{
		//https://github.com/UnityCommunity/UnityLibrary/blob/master/Assets/Scripts/Editor/AddDefineSymbols.cs
		var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
		return symbols.Split(';').ToList();
	}


	static List<Item> GetItems()
	{
		var buildTarget = new Item("Build target", BuildTarget.Android.ToString())
		{
			IsReady = () => { return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android; },
			GetCurrent = () => { return EditorUserBuildSettings.activeBuildTarget.ToString(); },
			Set = () => { EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android); }
		};

		var vrSupported = new Item("Virtual Reality Supported", true.ToString()) //Correct setting
		{
			IsReady = () => { return PlayerSettings.virtualRealitySupported; }, //return true for correct setting
			GetCurrent = () => { return PlayerSettings.virtualRealitySupported.ToString(); }, //display what it is currently
			Set = () => { PlayerSettings.virtualRealitySupported = true; } //what it should be set to
		};

		var oculusSDK = new Item("Virtual Reality SDK", "Oculus only")
		{
			IsReady = () => { return PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.Android)[0].ToString() == "Oculus" && PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.Android).Length == 1; },
			GetCurrent = () => { return "check it yourself chonker"; },
			Set = () => { PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, new string[] {"Oculus"}); }
		};

		var textureCompression = new Item("Texture Compression", "ASTC")
		{
			IsReady = () => { return EditorUserBuildSettings.androidBuildSubtarget == MobileTextureSubtarget.ASTC; },
			GetCurrent = () => { return EditorUserBuildSettings.androidBuildSubtarget.ToString(); },
			Set = () => { EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC; }
		};

		var stereoRenderingMode = new Item("Stereo Rendering Mode", "SinglePass") //Correct setting
		{
			IsReady = () => { return PlayerSettings.stereoRenderingPath == StereoRenderingPath.SinglePass; }, //return true for correct setting
			GetCurrent = () => { return PlayerSettings.stereoRenderingPath.ToString(); }, //display what it is currently
			Set = () => { PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass; } //what it should be set to
		};

		var pixelLightCount = new Item("Pixel Light Count", "1")
		{
			IsReady = () => { return QualitySettings.pixelLightCount == 1; },
			GetCurrent = () => { return QualitySettings.pixelLightCount.ToString(); },
			Set = () => { QualitySettings.pixelLightCount = 1; }
		};

		var textureQuality = new Item("Texture Quality", "Full Res")
		{
			IsReady = () => { return QualitySettings.masterTextureLimit == 0; },
			GetCurrent = () => { return "enum " + QualitySettings.masterTextureLimit.ToString(); },
			Set = () => { QualitySettings.masterTextureLimit = 0; }
		};

		var anisotropicFiltering = new Item("Anisotrophic Filtering", "Per Texture")
		{
			IsReady = () => { return QualitySettings.anisotropicFiltering == AnisotropicFiltering.Enable; },
			GetCurrent = () => { return QualitySettings.anisotropicFiltering.ToString(); },
			Set = () => { QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable; }
		};

		var antiAliasing = new Item("Anti Aliasing", "2X or 4X")
		{
			IsReady = () => { return QualitySettings.antiAliasing == 2 || QualitySettings.antiAliasing == 4; },
			GetCurrent = () => { return QualitySettings.antiAliasing.ToString(); },
			Set = () => { QualitySettings.antiAliasing = 2; }
		};

		var softParticles = new Item("Soft Particles", false.ToString())
		{
			IsReady = () => { return !QualitySettings.softParticles; },
			GetCurrent = () => { return QualitySettings.softParticles.ToString(); },
			Set = () => { QualitySettings.softParticles = false; }
		};

		var vSync = new Item("V Sync", false.ToString())
		{
			IsReady = () => { return QualitySettings.vSyncCount == 0; },
			GetCurrent = () => { return "enum " + QualitySettings.vSyncCount.ToString(); },
			Set = () => { QualitySettings.vSyncCount = 0; }
		};

		var enableMTRendering = new Item("Multi Threaded Rendering", true.ToString())
		{
			IsReady = () => { return GetMobileMTRendering(BuildTargetGroup.Android); },
			GetCurrent = () => { return GetMobileMTRendering(BuildTargetGroup.Android).ToString(); },
			Set = () => { SetMobileMTRendering(BuildTargetGroup.Android, true); }
		};

		var gpuSkinning = new Item("GPU Skinning", true.ToString())
		{
			IsReady = () => { return PlayerSettings.gpuSkinning; },
			GetCurrent = () => { return PlayerSettings.gpuSkinning.ToString(); },
			Set = () => { PlayerSettings.gpuSkinning = true; }
		};

		var graphicsJobs = new Item("Graphics Jobs", false.ToString())
		{
			IsReady = () => { return !PlayerSettings.graphicsJobs; },
			GetCurrent = () => { return PlayerSettings.graphicsJobs.ToString(); },
			Set = () => { PlayerSettings.graphicsJobs = false; }
		};

		var AndroidMinSDK = new Item("Android Min SDK version", AndroidSdkVersions.AndroidApiLevel19.ToString())
		{
			IsReady = () => { return PlayerSettings.Android.minSdkVersion >= AndroidSdkVersions.AndroidApiLevel19; },
			GetCurrent = () => { return PlayerSettings.Android.minSdkVersion.ToString(); },
			Set = () => { PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19; }
		};

		var AndroidTargetSDK = new Item("Android Target SDK version", AndroidSdkVersions.AndroidApiLevelAuto.ToString())
		{
			IsReady = () => { return PlayerSettings.Android.targetSdkVersion == AndroidSdkVersions.AndroidApiLevelAuto; },
			GetCurrent = () => { return PlayerSettings.Android.targetSdkVersion.ToString(); },
			Set = () => { PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto; }
		};
		

		return new List<Item>()
		{
			buildTarget,
			vrSupported,
			oculusSDK,
			textureCompression,
			stereoRenderingMode,
			pixelLightCount,
			textureQuality,
			anisotropicFiltering,
			antiAliasing,
			softParticles,
			vSync,
			enableMTRendering,
			gpuSkinning,
			graphicsJobs,
			AndroidMinSDK,
			AndroidTargetSDK
		};
	}

	static QuestSettings window;

	static QuestSettings()
	{
		EditorApplication.update += Update;
	}

	[UnityEditor.MenuItem("VRVision/Quest Project Settings")]
	static void UpdateWithClearIgnore()
	{
		var items = GetItems();
		UpdateInner(items, true);
	}

	static void Update()
	{
		Debug.Log("Check for prefered editor settings");
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
		{
			var items = GetItems();
			UpdateInner(items, false);
		}
		EditorApplication.update -= Update;
	}

	public static void UpdateInner(List<Item> items, bool forceShow)
	{
		bool show = forceShow;
		if (!forceShow)
		{
			foreach (var item in items)
			{
				show |= !item.IsIgnored && !item.IsReady();
			}
		}

		if (show)
		{
			window = GetWindow<QuestSettings>(true);
			window.minSize = new Vector2(640, 320);
			window.items = items;
		}
	}

	Vector2 scrollPosition;

	string GetResourcePath()
	{
		var ms = MonoScript.FromScriptableObject(this);
		var path = AssetDatabase.GetAssetPath(ms);
		path = Path.GetDirectoryName(path);
		return path;
	}

	public void OnGUI()
	{
		var resourcePath = GetResourcePath();
		var logo = AssetDatabase.LoadAssetAtPath<Texture2D>(resourcePath + "/vr-vision-logo.png");
		var rect = GUILayoutUtility.GetRect(position.width, 150, GUI.skin.box);
		if (logo)
			GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);

		EditorGUILayout.HelpBox("Recommended project settings for Quest:", MessageType.Warning);

		if (items == null)
			return;

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		int notReadyItems = 0;
		foreach (var item in items)
		{
			if (item.Show())
				notReadyItems++;
		}

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		if (GUILayout.Button("Clear All Ignores"))
		{
			foreach (var item in items)
			{
				item.CleanIgnore();
			}
		}

		GUILayout.EndHorizontal();

		GUILayout.EndScrollView();

		GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
		if (notReadyItems > 0)
		{
			if (GUILayout.Button("Accept All"))
			{
				foreach (var item in items)
				{
					// Only set those that have not been explicitly ignored.
					if (!item.IsIgnored)
						item.Set();
				}

				EditorUtility.DisplayDialog("Accept All", "YEEEEEET!", "Ok");

				Close();
			}

			if (GUILayout.Button("Ignore All"))
			{
				if (EditorUtility.DisplayDialog("Ignore All", "Are you sure?", "Yes, Ignore All", "Cancel"))
				{
					foreach (var item in items)
					{
						// Only ignore those that do not currently match our recommended settings.
						if (!item.IsReady())
							item.Ignore();
					}

					Close();
				}
			}
		}
		else
		{
			if (GUILayout.Button("Close"))
				Close();
		}
		GUILayout.EndHorizontal();
	}
}
