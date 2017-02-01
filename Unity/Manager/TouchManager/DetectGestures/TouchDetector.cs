using UnityEngine;
using System.Collections;

namespace UniFramework.TouchInput
{
	public class TouchDetector : AbstractGestureDetector
	{
		public event System.Action<CustomInput> OnTouchStart;
		public event System.Action<CustomInput> OnTouchEnd;


		public override void Enqueue (CustomInput currentInput)
		{
			if (currentInput.IsDown) {
				if (OnTouchStart != null)
					OnTouchStart (currentInput);
			}
			if (currentInput.IsUp) {
				if (OnTouchEnd != null)
					OnTouchEnd (currentInput);
			}	
		}
	}
}
