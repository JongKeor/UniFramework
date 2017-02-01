using UnityEngine;
using System.Collections;

namespace UniFramework.Components
{
	public class UIHud : MonoBehaviour
	{
		public GameObject target;
		public Vector3 worldOffset;
		private RectTransform rect;
		private Canvas canvas;
		private RectTransform canvasRect;

		protected void Awake ()
		{
			rect = GetComponent<RectTransform> ();
			canvas = GetComponentInParent<Canvas> ();
			canvasRect = canvas.GetComponent<RectTransform> ();

		}

		protected void LateUpdate ()
		{
			if (target == null) {
				Destroy (gameObject);
				return;
			}
			Vector2 screenPos = RectTransformUtility.WorldToScreenPoint (Camera.main, target.transform.position + worldOffset);

			Vector2 pos = Vector2.zero;

			RectTransformUtility.ScreenPointToLocalPointInRectangle (canvasRect, screenPos, canvas.worldCamera, out pos);
			rect.localPosition = pos;
		}

	}
}

