using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UniEditorUtil
{
	public enum EWindow
	{
		GameView,
		SceneView,
		SceneHierarchyWindow,
		ProjectBrowser,
		InspectorWindow,
		ConsoleWindow,
		Animation,
	}

	public static string MakeAbsolute(string basePath ,string relativePath)
	{
		if (!basePath.EndsWith("/"))
			basePath += "/";
		
		var ret = new System.Uri(new System.Uri(basePath), relativePath );
		return ret.AbsolutePath;
	}

	public static string MakeRelative(string filePath, string referencePath)
	{
		if (!referencePath.EndsWith("/"))
			referencePath += "/";
		var fileUri = new System.Uri(filePath);
		var referenceUri = new System.Uri(referencePath);
		return referenceUri.MakeRelativeUri(fileUri).ToString();
	}

	public static void GetFileInfo(AudioClip clip, out string fileName, out string fileExtension)
	{
		string filePath = AssetDatabase.GetAssetPath(clip);
		System.IO.FileInfo fi = new System.IO.FileInfo(filePath);
		fileName = fi.Name;
		fileExtension = fi.Extension;
	}
	
	public static void GetFileInfo(out System.IO.FileInfo o_fileInfo, Object i_obj)
	{
		string filePath = AssetDatabase.GetAssetPath(i_obj);
		o_fileInfo = new System.IO.FileInfo(filePath);
	}
	
	public static string [] GetExtFiles(string i_strPath, string i_strExtensions, System.IO.SearchOption i_eSearchOption)
	{
		if (string.IsNullOrEmpty(i_strPath) == true || 
			string.IsNullOrEmpty(i_strExtensions) == true)
		{
			return null;
		}
		
		List<string> lstFiles = new List<string>();
		
		if (string.Compare(i_strExtensions, "*.*", true) == 0)
		{
			string [] strFiles = System.IO.Directory.GetFiles(i_strPath);
			lstFiles.AddRange(strFiles);
		}
		else
		{
			string [] exts = i_strExtensions.Split('|');

			for (int nIndex = 0; nIndex < exts.Length; nIndex++)
			{
				string [] strFiles = System.IO.Directory.GetFiles(i_strPath, exts[nIndex], i_eSearchOption);
				lstFiles.AddRange(strFiles);
			}
		}
		
		return lstFiles.ToArray();
	}


	
	public static void DisplayProgressBar(string title, Object obj, int curIndex, int maxCount)
	{
		string fileInfo = string.Empty;
		
		float ratio = (float)curIndex / (float)maxCount;
		float percent = ratio * 100.0f;
		
		if (obj != null)
		{
			string filePath = AssetDatabase.GetAssetPath(obj);
			fileInfo = string.Format("{0} ({1:F1}%)", filePath, percent);
		}
		else
		{
			fileInfo = string.Format("{0:F1}%", percent);
		}

		EditorUtility.DisplayProgressBar(title, fileInfo, ratio);
	}
	
	public static void DisplayProgressBar(string title, string msg, int curIndex, int maxCount)
	{
		string fileInfo = string.Empty;
		
		float ratio = (float)curIndex / (float)maxCount;
		float percent = ratio * 100.0f;

		if (string.IsNullOrEmpty(msg))
		{
			fileInfo = string.Format("{0:F1}%", percent);
		}
		else
		{
			fileInfo = string.Format("{0} {1:F1}%", msg, percent);
		}

		EditorUtility.DisplayProgressBar(title, fileInfo, ratio);
	}
	
	public static void GetGameObjectPath(GameObject go, out string path)
	{
		path = string.Empty;

		if (go == null)
		{
			return;
		}

		path = go.name;
		Transform t = go.transform;

		while (t.parent != null)
		{
			path = t.parent.name + "/" + path;
			t = t.parent;
		}
	}
	
	public static void OutputGameObjectPos(string title, GameObject go)
	{
		string strPath = string.Empty;
		GetGameObjectPath(go, out strPath);
		Debug.Log(string.Format("[{0}] {1}", title, strPath));
	}
	
	public static void FindMissingScripts(GameObject go, ref int o_count)
	{
		if (go == null)
		{
			return;
		}

		Component [] components = go.GetComponents<Component>();
		
		foreach (Component com in components)
		{
			if (com == null)
			{
				o_count++;
				OutputGameObjectPos("MissingScript", go);
			}
		}

		for (int index = 0; index < go.transform.childCount; index++)
		{
			Transform childTrans = go.transform.GetChild(index);
			FindMissingScripts(childTrans.gameObject, ref o_count);
		}
	}
	
	public static void SceneLabel(Color i_color, Vector3 i_pos, string i_text)
	{
		GUIStyle style = new GUIStyle();
		style.normal.textColor = i_color;
		Handles.Label(i_pos, i_text, style);
	}
	
	public static void SceneLine(Color i_color, Vector3 i_p1, Vector3 i_p2)
	{
		Handles.color = i_color;
		Handles.DrawLine(i_p1, i_p2);
	}
	
	public static void SceneWireDiscUp(Color i_color, Vector3 i_pos, float i_radius)
	{
		Handles.color = i_color;
		Handles.DrawWireDisc(i_pos, Vector3.up, i_radius);
	}
	
	public static void InspectorLine()
	{
		GUILayout.Box( "", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(0.1f) } );
	}

	public static EditorWindow GetEditorWindow(EWindow i_eWindow)
	{
		string strSource = "";
		switch (i_eWindow)
		{
		case EWindow.GameView:				strSource = "UnityEditor.GameView";					break;
		case EWindow.SceneView:				strSource = "UnityEditor.SceneView";				break;
		case EWindow.SceneHierarchyWindow:	strSource = "UnityEditor.SceneHierarchyWindow";		break;
		case EWindow.ProjectBrowser:		strSource = "UnityEditor.ProjectBrowser";			break;
		case EWindow.InspectorWindow:		strSource = "UnityEditor.InspectorWindow";			break;
		case EWindow.ConsoleWindow:			strSource = "UnityEditor.ConsoleWindow";			break;
		case EWindow.Animation:				strSource = "UnityEditor.AnimationWindow";			break;
		}

		string strDest = string.Empty;
		EditorWindow [] windows = Resources.FindObjectsOfTypeAll(typeof(EditorWindow)) as EditorWindow [];

		foreach (EditorWindow window in windows)
		{
			strDest = window.GetType().ToString();
			//Debug.Log(strDest);
			if (string.Compare(strSource, strDest, true) == 0)
			{
				return window;
			}
		}

		return null;
	}

    public static GUIStyle GetGuiStyle(string i_name)
    {
        foreach (GUIStyle style in GUI.skin)
        {
            if (string.Compare(style.name, i_name, true) == 0)
            {
                return style;
            }
        }
        return null;
    }
	
	public static AnimationEvent CopyAnimationEvent(AnimationEvent i_source)
	{
		if (i_source == null)
		{
			return null;
		}

		AnimationEvent aniEvent = new AnimationEvent();
		aniEvent.time = i_source.time;
		aniEvent.functionName = i_source.functionName;
		aniEvent.floatParameter = i_source.floatParameter;
		aniEvent.intParameter = i_source.intParameter;
		aniEvent.stringParameter = i_source.stringParameter;
		aniEvent.objectReferenceParameter = i_source.objectReferenceParameter;
		
		return aniEvent;
	}
	
	public static string GetAssetPathCut(string i_strPath)
	{
		string strPath = i_strPath;
		string strAssets = "Assets/";
		
		if (strPath.Contains(strAssets) == true)
		{
			int nFindIndex = strPath.IndexOf(strAssets);
			if (nFindIndex != -1)
			{
				strPath = strPath.Substring(nFindIndex + strAssets.Length);
			}
		}
		
		return strPath;
	}
	
	public static string GetAssetFullPath(string i_strPath)
	{
		string strPath = Application.dataPath + "/" + i_strPath;
		
		if (string.IsNullOrEmpty(strPath))
		{
			strPath = Application.dataPath;
		}
		
		return strPath;
	}

    public static GUIStyle CreateGUIStyle(int fontSize, Color color, FontStyle eFont)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        style.fontSize = fontSize;
        style.fontStyle = eFont;
        return style;
    }

    public static bool IsCompareStr(string strSource, string strDest, bool bIgonreCase = false)
    {
        return (string.Compare(strSource, strDest, bIgonreCase) == 0);
    }

    public static bool IsCompareIncludeStr(string strSource, string strDest, bool bIgonreCase = false)
    {
        if (string.IsNullOrEmpty(strSource) == true &&
            string.IsNullOrEmpty(strDest) == true)
        {
            return true;
        }

        if (string.IsNullOrEmpty(strSource) == false &&
            string.IsNullOrEmpty(strDest) == false)
        {
            string _strSource = strSource;
            string _strDest = strDest;

            if (bIgonreCase == true)
            {
                _strSource = strSource.ToLower();
                _strDest = strDest.ToLower();
            }

            if (_strSource.IndexOf(_strDest) > -1)
            {
                return true;
            }
        }

        return false;
    }
}
