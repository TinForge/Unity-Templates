using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace VRVision.Tools
{
	[InitializeOnLoad]
	public class TextureImporter : EditorWindow
	{
		static TextureImporter window;

		static string folder = "";

		static new string name = "";
		static string type = "texture2D";

		static TextureImporter()
		{
			EditorApplication.update += Update;
		}

		[UnityEditor.MenuItem("VRVision/Texture Importer")]
		static void UpdateWithClearIgnore()
		{
			FindAssets();

			if (true)
			{
				window = GetWindow<TextureImporter>(true);
				window.minSize = new Vector2(750, 900); //original 640 x 320
			}
		}

		static void Update()
		{
			FindAssets();
			EditorApplication.update -= Update;
		}

		static string[] guids;
		static string[] paths;
		static List<Texture2D> assets;

		public static void FindAssets()
		{
			guids = AssetDatabase.FindAssets("t:" + type, new[] { "Assets/" + folder });

			paths = new string[guids.Length];
			for (int i = 0; i < paths.Length; i++)
				paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

			assets = new List<Texture2D>();
			for (int i = 0; i < paths.Length; i++)
			{
				Texture2D t = (Texture2D) AssetDatabase.LoadAssetAtPath(paths[i], typeof(Texture2D));
				if (name != "")
				{
					if (t.name.ToLower().Contains(name.ToLower()))
						assets.Add(t);
				}
				else
				{
					assets.Add(t);
				}
			}
		}

		public static bool IsNormalMap(Texture2D t)
		{
			var importer = (UnityEditor.TextureImporter) AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(t));
			if (importer.textureType == TextureImporterType.NormalMap)
				return true;
			return false;
		}

		public static bool IsFormatted(Texture2D t)
		{
			var importer = (UnityEditor.TextureImporter) AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(t));
			TextureImporterPlatformSettings t1 = new TextureImporterPlatformSettings { overridden = true, name = "Android", maxTextureSize = 4096, format = TextureImporterFormat.RGBA32, compressionQuality = 0 };
			TextureImporterPlatformSettings t2 = importer.GetPlatformTextureSettings("Android");

			if (t1.overridden == t2.overridden && t1.maxTextureSize == t2.maxTextureSize && t1.format == t2.format)
				return true;
			return false;
		}

		public static void ApplyNormalMap(List<Texture2D> list)
		{
			foreach (Texture2D t in list)
			{
				var importer = (UnityEditor.TextureImporter) AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(t));
				importer.normalmap = true;
			}
		}

		public static void ApplyFormat(List<Texture2D> list)
		{
			foreach (Texture2D t in list)
			{
				var importer = (UnityEditor.TextureImporter) AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(t));
				TextureImporterPlatformSettings tips = new TextureImporterPlatformSettings { overridden = true, name = "Android", maxTextureSize = 4096, format = TextureImporterFormat.RGBA32, compressionQuality = 0 };
				importer.SetPlatformTextureSettings(tips);
			}
		}

		Vector2 scrollPosition;
		public void OnGUI()
		{
			var resourcePath = GetResourcePath();
			var logo = AssetDatabase.LoadAssetAtPath<Texture2D>(resourcePath + "/vr-vision-logo.png");
			var rect = GUILayoutUtility.GetRect(position.width, 150, GUI.skin.box);
			if (logo)
				GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);

			GUILayout.Label("Search within folder: Assets/");
			folder = GUILayout.TextField(folder);

			GUILayout.Label("Find by name:");
			name = GUILayout.TextField(name);

			if (GUILayout.Button("Search"))
				FindAssets();

			if (assets.Count == 0)
			{
				if (folder == "")
					EditorGUILayout.HelpBox("Need to define a folder to search in", MessageType.Info);
				return;
			}
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUILayout.BeginVertical();

			for (int i = 0; i < assets.Count; i++)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(assets[i], GUILayout.Width(50), GUILayout.Height(50)))
					Selection.objects = new UnityEngine.Object[] { assets[i] };
				GUILayout.BeginVertical();
				GUILayout.Label(assets[i].name);
				GUILayout.Label(paths[i]);
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();

				GUILayout.Width(100);
				if (IsNormalMap(assets[i]))
					GUILayout.Space(100);
				else if (GUILayout.Button("Set Normal Map"))
					ApplyNormalMap(new List<Texture2D> { assets[i] });

				GUILayout.Width(100);
				if (IsFormatted(assets[i]))
					GUILayout.Space(100);
				else if (GUILayout.Button("Set Format"))
					ApplyFormat(new List<Texture2D> { assets[i] });

				if (GUILayout.Button("Remove"))
					assets.Remove(assets[i]);

				GUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();
			GUILayout.EndScrollView();


			GUILayout.BeginHorizontal();

			GUILayout.Label("Found " + assets.Count + " Textures");

			if (GUILayout.Button("Apply Normal Map to ALL"))
				ApplyNormalMap(assets);

			if (GUILayout.Button("Apply Format Settings to ALL"))
				ApplyFormat(assets);

			GUILayout.EndHorizontal();
		}

		string GetResourcePath()
		{
			var ms = MonoScript.FromScriptableObject(this);
			var path = AssetDatabase.GetAssetPath(ms);
			path = Path.GetDirectoryName(path);
			return path;
		}

	}
}