using UnityEngine;
using System.Collections;

namespace UniFramework.TimeScale
{
	public class PartileSystemTimeScaleModifier : ITimeScaleModifier
	{

		public void ApplyTimeScale (GameObject obj, float timeScale)
		{
			ParticleSystem ps = obj.GetComponent<ParticleSystem> ();
			if (ps != null) {
				if (ps.isPlaying) {
					var main =  ps.main ;
					main.simulationSpeed = timeScale;
				}
			}
		}
	}
}

