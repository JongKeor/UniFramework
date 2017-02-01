using UnityEngine;
using System.Collections;

namespace UniFramework.TouchInput
{
	public abstract class AbstractGestureDetector : MonoBehaviour
	{
		public abstract void Enqueue (CustomInput currentInput);
	}
}
