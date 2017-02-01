using System;
using UnityEditor;

namespace UniFramework.Editor
{
	public static class AssetBundleMenu
	{
	
		[MenuItem ("Build/CI/Build IOS AssetBundle")]
		static void  BuildIOSAssetBundle ()
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
			AssetBundleBuilder.BuildAssetBundle (BuildTarget.iOS, AssetBundleBuildSetting.RelativeOutputPath);
		}

		[MenuItem ("Build/CI/Build Android AssetBundle")]
		static void  BuildAndroidAssetBundle ()
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
			AssetBundleBuilder.BuildAssetBundle (BuildTarget.Android, AssetBundleBuildSetting.RelativeOutputPath);
		}
	}

}


