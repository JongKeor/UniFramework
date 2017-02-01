using UnityEngine;
using System.Collections;

namespace UniFramework.TouchInput
{
	public class DragDetector : AbstractGestureDetector
	{
		public event System.Action<CustomInput> OnDrag;

		public override void Enqueue (CustomInput currentInput)
		{
			if (currentInput.IsDrag) {
				if (OnDrag != null)
					OnDrag (currentInput);
			}
		}
	}
}
