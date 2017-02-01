using UnityEngine;
using System.Collections;

namespace UniFramework.TimeScale
{
	public class AnimationTimeScaleModifier : ITimeScaleModifier
	{

		public void ApplyTimeScale (GameObject obj, float timeScale)
		{

			Animation ani = obj.GetComponent<Animation> ();
			if (ani != null)
				foreach (AnimationState thisAnimationState in ani)
					ani [thisAnimationState.name].speed = timeScale;
		
		}
	}
}

