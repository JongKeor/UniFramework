using UnityEngine;
using System.Collections;

namespace UniFramework.TimeScale
{
	public static class MonoBehaviourTimeExtension
	{
		public static float CurrentDeltaTime (this MonoBehaviour self)
		{
//			return  TimeScaleManager.Instance.DeltaTime (self.gameObject);
			return Time.deltaTime;
		}

		public static float CurrentTimeScale (this MonoBehaviour self)
		{
//			return  TimeScaleManager.Instance.TimeScale (self.gameObject);
			return Time.timeScale;
		}
	}
}


