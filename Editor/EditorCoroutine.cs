using System;
using System.Collections;
using UnityEditor;


public class EditorCoroutine
{
	public static EditorCoroutine start (IEnumerator _routine)
	{
		EditorCoroutine coroutine = new EditorCoroutine (_routine);
		coroutine.start ();
		return coroutine;
	}

	readonly IEnumerator routine;

	EditorCoroutine (IEnumerator _routine)
	{
		routine = _routine;
	}

	void start ()
	{
		//Debug.Log("start");
		EditorApplication.update += update;
	}

	public void stop ()
	{
		//Debug.Log("stop");
		EditorApplication.update -= update;
	}

	void update ()
	{
			
		if (!routine.MoveNext ()) {
			stop ();
		}
	}
}


