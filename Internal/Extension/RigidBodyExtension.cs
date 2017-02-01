using UnityEngine;
using System.Collections;
using UniFramework.Extension.Helper;

namespace UniFramework.Extension
{
	public static class RigidbodyExtension
	{
		public static void SetPlaySpeed (this Rigidbody body, float speed)
		{

			if (body.velocity.sqrMagnitude != 0 || body.angularVelocity.sqrMagnitude != 0 || body.IsPaused ()) {
				RigidbodyHelper helper = body.GetComponent<RigidbodyHelper> ();
				if (helper == null) {
					helper = body.gameObject.AddComponent<RigidbodyHelper> ();
				}
				helper.rigid = body;
				helper.SetPlaySpeed (speed);
			}
		}

		public static bool IsPaused (this Rigidbody body)
		{
			RigidbodyHelper helper = body.GetComponent<RigidbodyHelper> ();
			if (helper == null) {
				return false;
			}
			return helper.isPause;

		}

	}
}
