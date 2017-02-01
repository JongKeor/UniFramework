using System;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace UniFramework.Editor
{
	public static class ProjectBuilderMenu
	{
		private const string Argument_OutputDir = "-outputDir";
		private const string Argument_KeystorePath = "-keystorePath";
		private const string Argument_KeystorePass = "-keystorePass";
		private const string Argument_KeyaliasName = "-keyaliasName";
		private const string Argument_KeyaliasPass = "-keyaliasPass";
		private const string Argument_DefineSymbols = "-D";
		private const string Default_OutputPath = "build";


		[MenuItem ("Build/CI/Build IOS")]
		public static void PerformiOSTestServerBuild ()
		{ 
			BuildOptions opt = BuildOptions.None;

			PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
			string buildPath = GetCommandArg(Argument_OutputDir,Default_OutputPath);
			Directory.CreateDirectory (buildPath);

			string original =  PlayerSettings.GetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android);

			PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS, original+ ";" +UnitySymbolsFromArg());

			if (ProjectBuilder.GenericBuild (ProjectBuilder.FindEnabledEditorScenes (), buildPath, BuildTarget.iOS, opt)) {
				PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS, original);
				EditorApplication.Exit (0);
			} else {
				EditorApplication.Exit (1);
			}

		}

		[MenuItem ("Build/CI/Build Android")]
		public static void PerformAndroidBuild ()
		{
			BuildOptions opt = BuildOptions.None;
			PlayerSettings.Android.keystoreName = GetCommandArg(Argument_KeystorePath);
			PlayerSettings.Android.keystorePass = GetCommandArg(Argument_KeystorePass);
			PlayerSettings.Android.keyaliasName = GetCommandArg(Argument_KeyaliasName);
			PlayerSettings.Android.keyaliasPass = GetCommandArg(Argument_KeyaliasPass);

			string buildPath = GetCommandArg(Argument_OutputDir,Default_OutputPath);
			Directory.CreateDirectory (buildPath);
			string original =  PlayerSettings.GetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android);
			PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS, original+ ";" +UnitySymbolsFromArg());
			if (ProjectBuilder.GenericBuild (ProjectBuilder.FindEnabledEditorScenes (), Path.Combine(buildPath ,PlayerSettings.productName + ".apk"), BuildTarget.Android, opt)) {
				PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS, original);
				EditorApplication.Exit (0);
			} else {
				PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS, original);
				EditorApplication.Exit (1);
			}
		}


		private static string UnitySymbolsFromArg(){
			string[] result =  FindArgsStartWith("-D");
			StringBuilder sb = new StringBuilder();
			foreach (var arg in result)
			{
				sb.Append(arg.Substring(2));
				sb.Append(";");
			}

			return sb.ToString();
		}

		private static bool IsExistCommandArg(string name)
		{
			var args = System.Environment.GetCommandLineArgs();
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == name)
				{
					return true;
				}
			}
			return false;
		}

		private static string[] FindArgsStartWith(string name)
		{
			List<string> argList = new List<string>();
			var args = System.Environment.GetCommandLineArgs();
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].StartsWith(name))
				{
					argList.Add(args[i]);
				}
			}
			return argList.ToArray();
		}

		private static string GetCommandArg(string name , string defaultValue = "")
		{
			var args = System.Environment.GetCommandLineArgs();
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == name && args.Length > i + 1)
				{
					return args[i + 1];
				}
			}
			return defaultValue;
		}
	}
}


