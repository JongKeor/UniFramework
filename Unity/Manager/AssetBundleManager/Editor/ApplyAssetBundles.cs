using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;


internal static class ContinuationManager
{
    private class Job
    {
        public Job (Func<bool> completed, Action continueWith)
        {
            Completed = completed;
            ContinueWith = continueWith;
        }

        public Func<bool> Completed { get; private set; }

        public Action ContinueWith { get; private set; }
    }

    private static readonly List<Job> jobs = new List<Job> ();

    public static void Add (Func<bool> completed, Action continueWith)
    {
        if (!jobs.Any ())
            EditorApplication.update += Update;
        jobs.Add (new Job (completed, continueWith));
    }

    private static void Update ()
    {
        for (int i = 0; i >= 0; --i) {
            var jobIt = jobs [i];
            if (jobIt.Completed ()) {
                jobIt.ContinueWith ();
                jobs.RemoveAt (i);
            }
        }
        if (!jobs.Any ())
            EditorApplication.update -= Update;
    }
}

//
//
//public class ApplyAssetBundles : EditorWindow
//{
//    public const string EXCLUDE_EXTIONSION_DSSTORE = ".DS_Store";
//    public const string EXCLUDE_EXTIONSION_ZIP = ".zip";
//
//    // variable
//    private string fileList = "";
//    public BuildAssetBundles.ResourceServer currentServer;
//
//
//    [MenuItem ("Window/Star/Asset Bundles Tool")]
//    private static void Init ()
//    {
//        ApplyAssetBundles window = EditorWindow.GetWindow (typeof(ApplyAssetBundles)) as ApplyAssetBundles;
//        window.position = new Rect (Screen.width / 2, Screen.height / 2, 300, 500);
//    }
//
//
//    void OnGUI ()
//    {
//        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
//        string outputPath = BuildAssetBundles.GetAssetBundlePath (target);
//
//        GUILayout.BeginVertical ();
//
//        if (GUILayout.Button ("Build Asset")) {
//            BuildAsset (target, outputPath);
//            File (target);
//        }
//        GUILayout.EndVertical ();
//
//
//        GUILayout.BeginHorizontal ();
//        currentServer = (BuildAssetBundles.ResourceServer)EditorGUILayout.EnumPopup (currentServer);
//        if (GUILayout.Button ("Apply")) {
//            string zipoutputPath = BuildAssetBundles.GetZipPath (target);
//			if (Zip (zipoutputPath,outputPath)) {
//				
//				EditorUtility.DisplayProgressBar("Apply", "Wait !!", 0);
//				BuildAssetBundles.Apply (zipoutputPath , currentServer ,true, (WWW www)=>{
//					if(www == null){
//						Debug.LogError("unexpected exception");
//						return;
//					}
//					if (!string.IsNullOrEmpty (www.error)) {
//						Debug.Log ("failed : " + www.error);
//					} else {
//						Debug.Log ("good : " + www.text);
//					}
//                    EditorUtility.ClearProgressBar();
//				});
//            } else {
//                Debug.LogWarning ("faild to compress zip ");
//            }
//        }
//        GUILayout.EndHorizontal ();
//
//        fileList = EditorGUILayout.TextArea (fileList);
//    }
//
//
//    public static void BuildAsset (BuildTarget buildTarget, string outputPath)
//    {
//        List<string> changedAssetBundleList = BuildAssetBundles.BuildAssetBundle (buildTarget, outputPath);
//
//        foreach (string assetBundle  in changedAssetBundleList) {
//            Debug.Log (assetBundle);
//        }
//
//        if (changedAssetBundleList.Count > 0) {
//            foreach (BuildAssetBundles.ResourceServer ser in Enum.GetValues( typeof(BuildAssetBundles.ResourceServer))) {
//                BuildAssetBundles.SetChangedList (ser, buildTarget, changedAssetBundleList);
//            }
//        }
//    }
//
//	
//
//
//    public string[] FindUpdateFile (BuildTarget buildTarget)
//    {
//        return AssetDatabase.GetAllAssetBundleNames ();		
//    }
//
//    public void File (BuildTarget buildTarget)
//    {
//        fileList = "";
//        string[] files = FindUpdateFile (buildTarget);
//
//        foreach (string str in files) {
//            if (files.Last ().Equals (str)) {
//                fileList += str;
//            } else {
//                fileList += str + "\n";
//            }
//        }
//    }
//    
//	private  bool Zip (string outputPath, string srcPath )
//    {
//		Debug.Log ("output path : " + outputPath); 
//		Debug.Log ("src path : " + srcPath);
//        if (fileList == string.Empty) {
//            Debug.Log ("there is no files to compress");
//            return false;
//        }
//        List<string> ret = new List<string> ();
//
//        string[] list = fileList.Split ('\n');
//
//        foreach (string t in list) {
//			ret.Add (Path.Combine (srcPath, t));
//			ret.Add (Path.Combine (srcPath, t + "." + BuildAssetBundles.INCLUDE_EXTENSION_MANIFEST));
//        }
//        return ZipUtil.Zip (outputPath, ret.ToArray ());
//    }
//	
//
//
//
//
//}
