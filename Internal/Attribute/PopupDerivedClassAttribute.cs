using UnityEngine;
using System.Collections;

namespace UniFramework
{
	public class PopupDerivedClassAttribute : PropertyAttribute
	{
		public System.Type type;

		public PopupDerivedClassAttribute (System.Type t)
		{
			type = t;

		}
	}
}
