using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using UnityEditor.Rendering;
using System.Collections;
using System;

namespace Editor
{
	[InitializeOnLoad]
	public class OculusQuestSettings : EditorWindow
	{
		static bool FinalBuild = false;

		#region Items

		List<List<Item>> items;

		List<Item> projectItems;
		List<Item> audioItems;
		List<Item> graphicItems;
		List<Item> playerItems;
		List<Item> qualityItems;
		List<Item> buildItems;

		public class Item
		{
			const string ignore = "ignore.";
			const string useRecommended = "Use recommended ({0})";
			const string currentValue = " (current = {0})";

			public delegate bool DelegateIsReady();
			public delegate string DelegateGetCurrent();
			public delegate void DelegateSet();

			public DelegateIsReady IsCorrect;
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
				if (ignored || IsCorrect())
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
		#endregion

		#region Graphics Tier
		public static void SetGraphicsTier(int i)
		{
			TierSettings ts = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Android, (GraphicsTier) i);
			ts.standardShaderQuality = ShaderQuality.High;
			ts.reflectionProbeBoxProjection = false;
			ts.reflectionProbeBlending = false;
			ts.detailNormalMap = false;
			ts.semitransparentShadows = false;
			ts.enableLPPV = false;
			ts.cascadedShadowMaps = false;
			ts.prefer32BitShadowMaps = false;
			ts.hdr = false;
			ts.renderingPath = RenderingPath.Forward;
			ts.realtimeGICPUUsage = RealtimeGICPUUsage.Low;
			EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Android, (GraphicsTier) i, ts);
		}

		public static bool CheckGraphicsTier(int i)
		{
			bool flag = true;
			TierSettings ts = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Android, (GraphicsTier) i);
			flag &= ts.standardShaderQuality == ShaderQuality.High;
			flag &= ts.reflectionProbeBoxProjection == false;
			flag &= ts.reflectionProbeBlending == false;
			flag &= ts.detailNormalMap == false;
			flag &= ts.semitransparentShadows == false;
			flag &= ts.enableLPPV == false;
			flag &= ts.cascadedShadowMaps == false;
			flag &= ts.prefer32BitShadowMaps == false;
			flag &= ts.hdr == false;
			flag &= ts.renderingPath == RenderingPath.Forward;
			flag &= ts.realtimeGICPUUsage == RealtimeGICPUUsage.Low;
			return flag;
		}
		#endregion

		#region Batching

		// Enable/Disable static batching
		public static void SetStaticBatchingValue(bool value)
		{
			PlayerSettings[] playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
			if (playerSettings == null)
				return;
			SerializedObject playerSettingsSerializedObject = new SerializedObject(playerSettings);
			SerializedProperty batchingSettings = playerSettingsSerializedObject.FindProperty("m_BuildTargetBatching");
			// Not sure how these couldn't exist
			if (batchingSettings == null)
				return;
			// Iterate over all platforms
			for (int i = 0; i < batchingSettings.arraySize; i++)
			{
				SerializedProperty batchingArrayValue = batchingSettings.GetArrayElementAtIndex(i);
				if (batchingArrayValue == null)
					continue;
				IEnumerator batchingEnumerator = batchingArrayValue.GetEnumerator();
				if (batchingEnumerator == null)
					continue;
				while (batchingEnumerator.MoveNext())
				{
					SerializedProperty property = (SerializedProperty) batchingEnumerator.Current;
					if (property != null && property.name == "m_StaticBatching")
						property.boolValue = value;
				}
			}
			playerSettingsSerializedObject.ApplyModifiedProperties();
		}

		public static bool GetStaticBatchingValue()
		{
			bool flag = true;

			PlayerSettings[] playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
			if (playerSettings == null)
				return false;
			SerializedObject playerSettingsSerializedObject = new SerializedObject(playerSettings);
			SerializedProperty batchingSettings = playerSettingsSerializedObject.FindProperty("m_BuildTargetBatching");
			// Not sure how these couldn't exist
			if (batchingSettings == null)
				return false;
			// Iterate over all platforms
			for (int i = 0; i < batchingSettings.arraySize; i++)
			{
				SerializedProperty batchingArrayValue = batchingSettings.GetArrayElementAtIndex(i);
				if (batchingArrayValue == null)
					continue;
				IEnumerator batchingEnumerator = batchingArrayValue.GetEnumerator();
				if (batchingEnumerator == null)
					continue;
				while (batchingEnumerator.MoveNext())
				{
					SerializedProperty property = (SerializedProperty) batchingEnumerator.Current;
					if (property != null && property.name == "m_StaticBatching")
						flag &= property.boolValue;
				}
			}
			return flag;
		}

		public static void SetDynamicBatchingValue(bool value)
		{
			PlayerSettings[] playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
			if (playerSettings == null)
				return;
			SerializedObject playerSettingsSerializedObject = new SerializedObject(playerSettings);
			SerializedProperty batchingSettings = playerSettingsSerializedObject.FindProperty("m_BuildTargetBatching");
			// Not sure how these couldn't exist
			if (batchingSettings == null)
				return;
			// Iterate over all platforms
			for (int i = 0; i < batchingSettings.arraySize; i++)
			{
				SerializedProperty batchingArrayValue = batchingSettings.GetArrayElementAtIndex(i);
				if (batchingArrayValue == null)
				{
					continue;
				}
				IEnumerator batchingEnumerator = batchingArrayValue.GetEnumerator();
				if (batchingEnumerator == null)
				{
					continue;
				}
				while (batchingEnumerator.MoveNext())
				{
					SerializedProperty property = (SerializedProperty) batchingEnumerator.Current;
					if (property != null && property.name == "m_DynamicBatching")
					{
						property.boolValue = value;
					}
				}
			}
			playerSettingsSerializedObject.ApplyModifiedProperties();
		}

		public static bool GetDynamicBatchingValue()
		{
			bool flag = true;

			PlayerSettings[] playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
			if (playerSettings == null)
				return false;
			SerializedObject playerSettingsSerializedObject = new SerializedObject(playerSettings);
			SerializedProperty batchingSettings = playerSettingsSerializedObject.FindProperty("m_BuildTargetBatching");
			// Not sure how these couldn't exist
			if (batchingSettings == null)
				return false;
			// Iterate over all platforms
			for (int i = 0; i < batchingSettings.arraySize; i++)
			{
				SerializedProperty batchingArrayValue = batchingSettings.GetArrayElementAtIndex(i);
				if (batchingArrayValue == null)
					continue;
				IEnumerator batchingEnumerator = batchingArrayValue.GetEnumerator();
				if (batchingEnumerator == null)
					continue;
				while (batchingEnumerator.MoveNext())
				{
					SerializedProperty property = (SerializedProperty) batchingEnumerator.Current;
					if (property != null && property.name == "m_DynamicBatching")
						flag &= property.boolValue;
				}
			}
			return flag;
		}

		#endregion

		#region VR Support

		public static bool GetVirtualRealitySupported(BuildTargetGroup group)
		{
			return PlayerSettings.GetVirtualRealitySupported(group);
		}

		public static void SetVirtualRealitySupported(BuildTargetGroup group, bool set)
		{
			PlayerSettings.SetVirtualRealitySupported(group, set);
		}

		public static string[] GetVirtualRealitySDKs(BuildTargetGroup group)
		{
			return PlayerSettings.GetVirtualRealitySDKs(group);
		}

		public static void SetVirtualRealitySDKs(BuildTargetGroup group, string[] devices)
		{
			PlayerSettings.SetVirtualRealitySDKs(group, devices);
		}

		public static bool GetMobileMTRendering(BuildTargetGroup group)
		{
			return PlayerSettings.GetMobileMTRendering(group);
		}

		public static void SetMobileMTRendering(BuildTargetGroup group, bool set)
		{
			PlayerSettings.SetMobileMTRendering(group, set);
		}

		#endregion

		#region Items

		static string GetOVRManager() { var ovr = Type.GetType("OVRManager"); if (ovr != null) return ovr.GetMethod("utilitiesVersion").Invoke(null,null).ToString(); else return "Not Found"; }

		static List<Item> GetProjectItems()
		{
			var unityVersion = new Item("Unity Version", "2019.1.2f1")
			{
				IsCorrect = () => { return Application.unityVersion == "2019.1.2f1"; },
				GetCurrent = () => { return Application.unityVersion; },
				Set = () => { Debug.Log("Open this project in 2019.1.2f1"); }
			};

			var oculusVersion = new Item("Oculus Integrations Version", GetOVRManager())
			{
				IsCorrect = () => { return GetOVRManager() == "1.41.0"; },
				GetCurrent = () => { return GetOVRManager(); },
				Set = () => { Debug.Log("Use Oculus Integrations 1.41.0"); }
			};

			return new List<Item>()
			{
				unityVersion,
				oculusVersion
			};
		}

		static List<Item> GetAudioItems()
		{
			var spatializerPlugin = new Item("Spatializer Plugin", "Oculus Spatializer")
			{
				IsCorrect = () => { return AudioSettings.GetSpatializerPluginName() == "OculusSpatializer"; },
				GetCurrent = () => { return AudioSettings.GetSpatializerPluginName(); },
				Set = () => { AudioSettings.SetSpatializerPluginName("OculusSpatializer"); }
			};

			/* No scripting reference for Ambisonic Decoder Plugin
			var ambisonicDecoderPlugin = new Item("Ambisonic Decoder", "Oculus Spatializer")
			{
				IsCorrect = () => { return AudioSettings.ambi == "OculusSpatializer"; },
				GetCurrent = () => { return AudioSettings.GetSpatializerPluginName(); },
				Set = () => { AudioSettings.SetSpatializerPluginName("OculusSpatializer"); }
			};   */

			return new List<Item>()
			{
				spatializerPlugin,
			};
		}

		static List<Item> GetGraphicItems()
		{
			var graphicsTier1 = new Item("Graphics Tier 1", "Custom Set")
			{
				IsCorrect = () => { return CheckGraphicsTier(0); },
				GetCurrent = () => { return CheckGraphicsTier(0) ? "Custom Settings" : "Incorrect Settings"; },
				Set = () => { SetGraphicsTier(0); }
			};

			var graphicsTier2 = new Item("Graphics Tier 2", "Custom Set")
			{
				IsCorrect = () => { return CheckGraphicsTier(1); },
				GetCurrent = () => { return CheckGraphicsTier(1) ? "Custom Settings" : "Incorrect Settings"; },
				Set = () => { SetGraphicsTier(1); }
			};

			var graphicsTier3 = new Item("Graphics Tier 3", "Custom Set")
			{
				IsCorrect = () => { return CheckGraphicsTier(2); },
				GetCurrent = () => { return CheckGraphicsTier(2) ? "Custom Settings" : "Incorrect Settings"; },
				Set = () => { SetGraphicsTier(2); }
			};

			return new List<Item>()
			{
				graphicsTier1,
				graphicsTier2,
				graphicsTier3
			};
		}

		static List<Item> GetPlayerItems()
		{
			var disableDepthStencil = new Item("Disable Depth and Stencil", true.ToString())
			{
				IsCorrect = () => { return PlayerSettings.Android.disableDepthAndStencilBuffers == true; },
				GetCurrent = () => { return PlayerSettings.Android.disableDepthAndStencilBuffers.ToString(); },
				Set = () => { PlayerSettings.Android.disableDepthAndStencilBuffers = true; }
			};

			var splashScreen = new Item("Splash Screen", false.ToString())
			{
				IsCorrect = () => { return PlayerSettings.SplashScreen.show == false; },
				GetCurrent = () => { return PlayerSettings.SplashScreen.show.ToString(); },
				Set = () => { PlayerSettings.SplashScreen.show = false; }
			};

			var openGLES3 = new Item("Graphics API", "OpenGLES 3.0")
			{
				IsCorrect = () => { return PlayerSettings.GetGraphicsAPIs(BuildTarget.Android)[0].ToString() == "OpenGLES3"; },
				GetCurrent = () => { return PlayerSettings.GetGraphicsAPIs(BuildTarget.Android)[0].ToString(); },
				Set = () => { PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 }); }
			};

			var enableMTRendering = new Item("Multi Threaded Rendering", true.ToString())
			{
				IsCorrect = () => { return GetMobileMTRendering(BuildTargetGroup.Android); },
				GetCurrent = () => { return GetMobileMTRendering(BuildTargetGroup.Android).ToString(); },
				Set = () => { SetMobileMTRendering(BuildTargetGroup.Android, true); }
			};

			var staticBatching = new Item("Static Batching", true.ToString())
			{
				IsCorrect = () => { return GetStaticBatchingValue() == true; },
				GetCurrent = () => { return GetStaticBatchingValue().ToString(); },
				Set = () => { SetStaticBatchingValue(true); }
			};

			var dynamicBatching = new Item("Dynamic Batching", true.ToString())
			{
				IsCorrect = () => { return GetDynamicBatchingValue() == true; },
				GetCurrent = () => { return GetDynamicBatchingValue().ToString(); },
				Set = () => { SetDynamicBatchingValue(true); }
			};

			var gpuSkinning = new Item("GPU Skinning", true.ToString())
			{
				IsCorrect = () => { return PlayerSettings.gpuSkinning; },
				GetCurrent = () => { return PlayerSettings.gpuSkinning.ToString(); },
				Set = () => { PlayerSettings.gpuSkinning = true; }
			};

			var graphicsJobs = new Item("Graphics Jobs", false.ToString())
			{
				IsCorrect = () => { return !PlayerSettings.graphicsJobs; },
				GetCurrent = () => { return PlayerSettings.graphicsJobs.ToString(); },
				Set = () => { PlayerSettings.graphicsJobs = false; }
			};

			var androidMinSDK = new Item("Android Min SDK version", AndroidSdkVersions.AndroidApiLevel21.ToString())
			{
				IsCorrect = () => { return PlayerSettings.Android.minSdkVersion >= AndroidSdkVersions.AndroidApiLevel21; },
				GetCurrent = () => { return PlayerSettings.Android.minSdkVersion.ToString(); },
				Set = () => { PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21; }
			};

			var androidTargetSDK = new Item("Android Target SDK version", AndroidSdkVersions.AndroidApiLevelAuto.ToString())
			{
				IsCorrect = () => { return PlayerSettings.Android.targetSdkVersion == AndroidSdkVersions.AndroidApiLevelAuto; },
				GetCurrent = () => { return PlayerSettings.Android.targetSdkVersion.ToString(); },
				Set = () => { PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto; }
			};

			return new List<Item>()
			{
				disableDepthStencil,
				splashScreen,
				openGLES3,
				enableMTRendering,
				staticBatching,
				dynamicBatching,
				gpuSkinning,
				graphicsJobs,
				gpuSkinning,
				androidMinSDK,
				androidTargetSDK
			};
		}

		static List<Item> GetQualityItems()
		{

			var pixelLightCount = new Item("Pixel Light Count", "1")
			{
				IsCorrect = () => { return QualitySettings.pixelLightCount == 1; },
				GetCurrent = () => { return QualitySettings.pixelLightCount.ToString(); },
				Set = () => { QualitySettings.pixelLightCount = 1; }
			};

			var textureQuality = new Item("Texture Quality", "Full Res")
			{
				IsCorrect = () => { return QualitySettings.masterTextureLimit == 0; },
				GetCurrent = () => { return "enum " + QualitySettings.masterTextureLimit.ToString(); },
				Set = () => { QualitySettings.masterTextureLimit = 0; }
			};

			var anisotropicFiltering = new Item("Anisotrophic Filtering", "Per Texture")
			{
				IsCorrect = () => { return QualitySettings.anisotropicFiltering == AnisotropicFiltering.Enable; },
				GetCurrent = () => { return QualitySettings.anisotropicFiltering.ToString(); },
				Set = () => { QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable; }
			};

			var antiAliasing = new Item("Anti Aliasing", "2X or 4X")
			{
				IsCorrect = () => { return QualitySettings.antiAliasing == 2 || QualitySettings.antiAliasing == 4 || QualitySettings.antiAliasing == 8; },
				GetCurrent = () => { return QualitySettings.antiAliasing.ToString(); },
				Set = () => { QualitySettings.antiAliasing = 4; }
			};

			var softParticles = new Item("Soft Particles", false.ToString())
			{
				IsCorrect = () => { return !QualitySettings.softParticles; },
				GetCurrent = () => { return QualitySettings.softParticles.ToString(); },
				Set = () => { QualitySettings.softParticles = false; }
			};

			var vSync = new Item("V Sync", false.ToString())
			{
				IsCorrect = () => { return QualitySettings.vSyncCount == 0; },
				GetCurrent = () => { return "enum " + QualitySettings.vSyncCount.ToString(); },
				Set = () => { QualitySettings.vSyncCount = 0; }
			};

			return new List<Item>()
			{
				pixelLightCount,
				textureQuality,
				anisotropicFiltering,
				antiAliasing,
				softParticles,
				vSync,
			};
		}

		static List<Item> GetBuildItems()
		{
			var packageName = new Item("Package Name", "Custom Set") //Correct setting
			{
				IsCorrect = () => { return PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) != "com.Unity.unitySample" || PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) != "com.Oculus.unitySample"; }, //return true for correct setting
				GetCurrent = () => { return PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android); }, //display what it is currently
				Set = () => { Debug.Log("Set custom Package Name"); } //what it should be set to
			};

			var buildTarget = new Item("Build target", BuildTarget.Android.ToString())
			{
				IsCorrect = () => { return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android; },
				GetCurrent = () => { return EditorUserBuildSettings.activeBuildTarget.ToString(); },
				Set = () => { EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android); }
			};

			var textureCompression = new Item("Texture Compression", "ASTC")
			{
				IsCorrect = () => { return EditorUserBuildSettings.androidBuildSubtarget == MobileTextureSubtarget.ASTC; },
				GetCurrent = () => { return EditorUserBuildSettings.androidBuildSubtarget.ToString(); },
				Set = () => { EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC; }
			};

			var vrSupported = new Item("Virtual Reality Supported", true.ToString()) //Correct setting
			{
				IsCorrect = () => { return PlayerSettings.virtualRealitySupported; }, //return true for correct setting
				GetCurrent = () => { return PlayerSettings.virtualRealitySupported.ToString(); }, //display what it is currently
				Set = () => { PlayerSettings.virtualRealitySupported = true; PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, new string[] { "Oculus" }); } //what it should be set to
			};

			var stereoRenderingMode = new Item("Stereo Rendering Mode", (FinalBuild ? "SinglePass / MultiPass" : "MultiPass")) //Correct setting
			{
				IsCorrect = () => { if (!FinalBuild) return PlayerSettings.stereoRenderingPath == StereoRenderingPath.MultiPass; else return true; }, //return true for correct setting
				GetCurrent = () => { return PlayerSettings.stereoRenderingPath.ToString(); }, //display what it is currently
				Set = () => { if (!FinalBuild) PlayerSettings.stereoRenderingPath = StereoRenderingPath.MultiPass; } //what it should be set to
			};

			var scriptingBackend = new Item("Scripting Backend", (FinalBuild ? "IL2CPP" : "Mono")) //Correct setting
			{
				IsCorrect = () => { return PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) == (FinalBuild ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x); }, //return true for correct setting
				GetCurrent = () => { return PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android).ToString(); }, //display what it is currently
				Set = () => { PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, (FinalBuild ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x)); } //what it should be set to
			};

			var devBuild = new Item("Development Build", (FinalBuild ? false.ToString() : true.ToString())) //Correct setting
			{
				IsCorrect = () => { return EditorUserBuildSettings.development == FinalBuild ? false : true; }, //return true for correct setting
				GetCurrent = () => { return EditorUserBuildSettings.development.ToString(); }, //display what it is currently
				Set = () => { EditorUserBuildSettings.development = FinalBuild ? false : true; } //what it should be set to
			};

			//write external permission
			//ambisonic

			return new List<Item>()
			{
				packageName,
				buildTarget,
				textureCompression,
				vrSupported,
				stereoRenderingMode,
				scriptingBackend,
				devBuild
			};
		}

		#endregion

		#region GUI

		Vector2 scrollPosition;

		public void OnGUI()
		{
			EditorGUILayout.HelpBox("Recommended project settings for Quest:", MessageType.Info);

			GUILayout.FlexibleSpace();

			if (items == null)
				return;

			int notReadyItems = 0;
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

			foreach (var item in items)
			{
				foreach (var entry in item)
					if (entry.Show())
						notReadyItems++;
				GUILayout.Space(5);
				GUILayout.Label("", GUI.skin.horizontalSlider);
				GUILayout.Space(5);
			}
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Displaying Settings For " + (FinalBuild ? "Final Build" : "Development Build"));
			if (GUILayout.Button("Switch"))
			{
				FinalBuild = !FinalBuild;
				UpdateWithClearIgnore();
			}
			if (GUILayout.Button("Clear All Ignores"))
				foreach (var item in items)
					foreach (var entry in item)
						entry.CleanIgnore();

			GUILayout.EndHorizontal();
			GUILayout.EndScrollView();

			GUILayout.FlexibleSpace();

			GUILayout.BeginHorizontal();
			if (notReadyItems > 0)
			{
				if (GUILayout.Button("Accept All"))
				{
					foreach (var item in items)
						foreach (var entry in item)
						{
							// Only set those that have not been explicitly ignored.
							if (!entry.IsIgnored)
								entry.Set();
						}

					UnityEditor.EditorUtility.DisplayDialog("Accept All", "YEEEEEET!", "Ok");

					Close();
				}

				if (GUILayout.Button("Ignore All"))
				{
					if (UnityEditor.EditorUtility.DisplayDialog("Ignore All", "Are you sure?", "Yes, Ignore All", "Cancel"))
					{
						foreach (var item in items)
							foreach (var entry in item)
							{
								// Only ignore those that do not currently match our recommended settings.
								if (!entry.IsCorrect())
									entry.Ignore();
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
		#endregion

		#region Editor

		static OculusQuestSettings window;

		static OculusQuestSettings()
		{
			EditorApplication.update += Update;
		}

		[UnityEditor.MenuItem("VRVision/Quest Project Settings", priority = 1)]
		static void UpdateWithClearIgnore()
		{
			var projectItems = GetProjectItems();
			var audioItems = GetAudioItems();
			var graphicItems = GetGraphicItems();
			var playerItems = GetPlayerItems();
			var qualityItems = GetQualityItems();
			var buildItems = GetBuildItems();
			UpdateInner(new List<List<Item>>() { projectItems, audioItems, graphicItems, playerItems, qualityItems, buildItems }, true);
		}

		static void Update()
		{
			Debug.Log("Checking for correct Quest settings...");
			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
			{
				var projectItems = GetProjectItems();
				var audioItems = GetAudioItems();
				var graphicItems = GetGraphicItems();
				var playerItems = GetPlayerItems();
				var qualityItems = GetQualityItems();
				var buildItems = GetBuildItems();
				UpdateInner(new List<List<Item>>() { projectItems, audioItems, graphicItems, playerItems, qualityItems, buildItems }, false);
			}
			EditorApplication.update -= Update;
		}


		public static void UpdateInner(List<List<Item>> items, bool forceShow)
		{
			bool show = forceShow;
			if (!forceShow)
			{
				foreach (var item in items)
				{
					foreach (var entry in item)
						show |= !entry.IsIgnored && !entry.IsCorrect();
				}
			}

			if (show)
			{
				window = GetWindow<OculusQuestSettings>(true);
				window.minSize = new Vector2(640, 900); //original 640 x 320
				window.items = items;
			}
		}

		#endregion

	}
}