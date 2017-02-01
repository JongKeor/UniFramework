using UnityEngine;
using System.Collections;

namespace UniFramework.TimeScale
{
	public interface ITimeScaleModifier
	{
		void ApplyTimeScale (GameObject obj, float timeScale);
	}
}
