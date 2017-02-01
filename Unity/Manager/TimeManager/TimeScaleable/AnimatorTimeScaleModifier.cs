using UnityEngine;
using System.Collections;

namespace UniFramework.TimeScale
{
	public class AnimatorTimeScaleModifier : ITimeScaleModifier
	{

		public void ApplyTimeScale (GameObject obj, float timeScale)
		{
			Animator macanim = obj.GetComponent<Animator> ();
			if (macanim != null)
				macanim.speed = timeScale;
		}
	}
}

