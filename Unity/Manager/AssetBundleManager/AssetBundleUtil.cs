using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UniFramework.Net
{
	public class AssetBundleUtil
	{
		public static string GetPlatformName ()
		{
			#if UNITY_EDITOR
			return GetPlatformForAssetBundles (EditorUserBuildSettings.activeBuildTarget);
			#else
		return GetPlatformForAssetBundles(Application.platform);
			#endif
		}

		#if UNITY_EDITOR
		public static string GetPlatformForAssetBundles (BuildTarget target)
		{
			switch (target) {
			case BuildTarget.Android:
				return "Android";
			case BuildTarget.iOS:
				return "iOS";
			case BuildTarget.WebGL:
				return "WebGL";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "Windows";
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
				return "OSX";
			default:
				return null;
			}
		}
		#endif

		public static string GetPlatformForAssetBundles (RuntimePlatform platform)
		{
			switch (platform) {
			case RuntimePlatform.Android:
				return "Android";
			case RuntimePlatform.IPhonePlayer:
				return "iOS";
			case RuntimePlatform.WebGLPlayer:
				return "WebGL";
			case RuntimePlatform.WindowsPlayer:
				return "Windows";
			case RuntimePlatform.OSXPlayer:
				return "OSX";
			default:
				return null;
			}
		}

		public static string GetStreamingAssetsPath ()
		{
			if (Application.isEditor)
				return "file://" + System.Environment.CurrentDirectory.Replace ("\\", "/"); // Use the build output folder directly.
		else if (Application.isWebPlayer)
				return System.IO.Path.GetDirectoryName (Application.absoluteURL).Replace ("\\", "/") + "/StreamingAssets";
			else if (Application.isMobilePlatform || Application.isConsolePlatform)
				return Application.streamingAssetsPath;
			else // For standalone player.
			return "file://" + Application.streamingAssetsPath;
		}
	}
}