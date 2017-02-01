using UnityEngine;
using System.Collections;


namespace UniFramework.TouchInput
{
	public class DoubleTapDetector : AbstractGestureDetector
	{
		public const float DOUBLE_TAP_TIME = 0.3f;
		public CustomInput FirstUpInput = null;

		public event System.Action<CustomInput> OnDoubleTap;

		public override void Enqueue (CustomInput currentInput)
		{
			if (FirstUpInput == null) {
				if (currentInput.IsUp) {
					FirstUpInput = currentInput;
				}
			} else {
			
				if (currentInput.IsUp) {
					if (currentInput.Time - FirstUpInput.Time < DOUBLE_TAP_TIME) {
						if (OnDoubleTap != null)
							OnDoubleTap (currentInput);
						FirstUpInput = null;
					} else {
						FirstUpInput = currentInput;
					}

			
				}
			}
		}

	}
}

