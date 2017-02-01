using UnityEngine;
using System.Collections;

public class ApplicationMeta  : MonoBehaviour
{
	public static string messageBoxScene = "MessageBox";

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void OnBeforeSceneLoadRuntimeMethod()
	{
		Debug.Log("Before scene loaded");
	}

}

