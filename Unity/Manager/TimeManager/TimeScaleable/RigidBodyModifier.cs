using UnityEngine;
using System.Collections;
using UniFramework.Extension;

namespace UniFramework.TimeScale
{
	public class RigidBodyModifier : ITimeScaleModifier
	{
		public void ApplyTimeScale (GameObject obj, float timeScale)
		{

			Rigidbody rbRigidbody = obj.GetComponent<Rigidbody> ();
			if (rbRigidbody != null)
				rbRigidbody.SetPlaySpeed (timeScale);


		}
	}
}

