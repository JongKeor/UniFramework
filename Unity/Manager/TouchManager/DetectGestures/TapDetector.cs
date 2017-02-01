using UnityEngine;
using System.Collections;

namespace UniFramework.TouchInput
{
	public class TapDetector : AbstractGestureDetector
	{

		public const float TAP_TIME = 0.2f;

		public event System.Action<CustomInput> OnTap;

		public CustomInput FirstInput = null;

		public override void Enqueue (CustomInput currentInput)
		{
			if (currentInput.IsDown) {
				FirstInput = currentInput;
			} else if (currentInput.IsUp) {
				if (FirstInput != null) {
					if (currentInput.Time - FirstInput.Time < TAP_TIME) {
						if (OnTap != null)
							OnTap (currentInput);
					}
					FirstInput = null;
				}
			}
		}

	}
}

