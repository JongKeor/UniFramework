using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UniFramework.Extension.Helper
{

	public class RigidbodyHelper  : MonoBehaviour
	{
		public Rigidbody rigid;
		private float preSpeed = 1f;
		public bool isPause = false;
		private Vector3 savedVelocity;
		private Vector3 savedAngulaVelocity;

		public void SetPlaySpeed (float s)
		{
			if (!isPause) {
				if (s == 0) {
					savedVelocity = rigid.velocity;
					savedAngulaVelocity = rigid.angularVelocity;
					isPause = true;
				} else {
					float changepercent = s / preSpeed;
					rigid.velocity *= changepercent;
					rigid.angularVelocity *= changepercent;
					preSpeed = s;
				}
			} else {
				isPause = false;
				float changepercent = s / preSpeed;
				rigid.velocity = savedVelocity * changepercent;
				rigid.angularVelocity = savedAngulaVelocity * changepercent;
				preSpeed = s;
			}

		}

		void FixedUpdate ()
		{
			if (rigid != null) {
				if (isPause) {
					rigid.Sleep ();
				}
			}
		}

	}
}

