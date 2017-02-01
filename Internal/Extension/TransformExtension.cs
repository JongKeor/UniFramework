using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UniFramework.Extension
{
	public  static class TransformExtension
	{
		#region Position

		public static void SetX (this Transform transform, float x)
		{
			var newPosition = new Vector3 (x, transform.position.y, transform.position.z);
			transform.position = newPosition;
		}

		public static void SetY (this Transform transform, float y)
		{
			var newPosition = new Vector3 (transform.position.x, y, transform.position.z);

			transform.position = newPosition;
		}

		public static void SetZ (this Transform transform, float z)
		{
			var newPosition = new Vector3 (transform.position.x, transform.position.y, z);

			transform.position = newPosition;
		}

		public static void SetXY (this Transform transform, float x, float y)
		{
			var newPosition = new Vector3 (x, y, transform.position.z);
			transform.position = newPosition;
		}

		public static void SetXZ (this Transform transform, float x, float z)
		{
			var newPosition = new Vector3 (x, transform.position.y, z);
			transform.position = newPosition;
		}

		public static void SetYZ (this Transform transform, float y, float z)
		{
			var newPosition = new Vector3 (transform.position.x, y, z);
			transform.position = newPosition;
		}


		public static void SetXYZ (this Transform transform, float x, float y, float z)
		{
			var newPosition = new Vector3 (x, y, z);
			transform.position = newPosition;
		}


		public static void TranslateX (this Transform transform, float x)
		{
			var offset = new Vector3 (x, 0, 0);

			transform.position += offset;
		}

		public static void TranslateY (this Transform transform, float y)
		{
			var offset = new Vector3 (0, y, 0);

			transform.position += offset;
		}

		public static void TranslateZ (this Transform transform, float z)
		{
			var offset = new Vector3 (0, 0, z);
			transform.position += offset;
		}

		public static void TranslateXY (this Transform transform, float x, float y)
		{
			var offset = new Vector3 (x, y, 0);
			transform.position += offset;
		}

		public static void TranslateXZ (this Transform transform, float x, float z)
		{
			var offset = new Vector3 (x, 0, z);
			transform.position += offset;
		}

		public static void TranslateYZ (this Transform transform, float y, float z)
		{
			var offset = new Vector3 (0, y, z);
			transform.position += offset;
		}

		public static void TranslateXYZ (this Transform transform, float x, float y, float z)
		{
			var offset = new Vector3 (x, y, z);
			transform.position += offset;
		}

		public static void SetLocalX (this Transform transform, float x)
		{
			var newPosition = new Vector3 (x, transform.localPosition.y, transform.localPosition.z);
			transform.localPosition = newPosition;
		}

		public static void SetLocalY (this Transform transform, float y)
		{
			var newPosition = new Vector3 (transform.localPosition.x, y, transform.localPosition.z);
			transform.localPosition = newPosition;
		}

		public static void SetLocalZ (this Transform transform, float z)
		{
			var newPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, z);
			transform.localPosition = newPosition;
		}

		public static void SetLocalXY (this Transform transform, float x, float y)
		{
			var newPosition = new Vector3 (x, y, transform.localPosition.z);
			transform.localPosition = newPosition;
		}

		public static void SetLocalXZ (this Transform transform, float x, float z)
		{
			var newPosition = new Vector3 (x, transform.localPosition.z, z);
			transform.localPosition = newPosition;
		}

		public static void SetLocalYZ (this Transform transform, float y, float z)
		{
			var newPosition = new Vector3 (transform.localPosition.x, y, z);
			transform.localPosition = newPosition;
		}

		public static void SetLocalXYZ (this Transform transform, float x, float y, float z)
		{
			var newPosition = new Vector3 (x, y, z);
			transform.localPosition = newPosition;
		}

		public static void ResetPosition (this Transform transform)
		{
			transform.position = Vector3.zero;
		}

		public static void ResetLocalPosition (this Transform transform)
		{
			transform.localPosition = Vector3.zero;
		}

		#endregion

		#region Scale

		public static void SetScaleX (this Transform transform, float x)
		{
			var newScale = new Vector3 (x, transform.localScale.y, transform.localScale.z);
			transform.localScale = newScale;
		}

		public static void SetScaleY (this Transform transform, float y)
		{
			var newScale = new Vector3 (transform.localScale.x, y, transform.localScale.z);
			transform.localScale = newScale;
		}


		public static void SetScaleZ (this Transform transform, float z)
		{
			var newScale = new Vector3 (transform.localScale.x, transform.localScale.y, z);
			transform.localScale = newScale;
		}

		public static void SetScaleXY (this Transform transform, float x, float y)
		{
			var newScale = new Vector3 (x, y, transform.localScale.z);
			transform.localScale = newScale;
		}

		public static void SetScaleXZ (this Transform transform, float x, float z)
		{
			var newScale = new Vector3 (x, transform.localScale.y, z);
			transform.localScale = newScale;
		}


		public static void SetScaleYZ (this Transform transform, float y, float z)
		{
			var newScale = new Vector3 (transform.localScale.x, y, z);
			transform.localScale = newScale;
		}

		public static void SetScaleXYZ (this Transform transform, float x, float y, float z)
		{
			var newScale = new Vector3 (x, y, z);
			transform.localScale = newScale;
		}

		public static void ScaleByX (this Transform transform, float x)
		{
			transform.localScale = new Vector3 (transform.localScale.x * x, transform.localScale.y, transform.localScale.z);
		}


		public static void ScaleByY (this Transform transform, float y)
		{
			transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y * y, transform.localScale.z);
		}


		public static void ScaleByZ (this Transform transform, float z)
		{
			transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y, transform.localScale.z * z);
		}


		public static void ScaleByXY (this Transform transform, float x, float y)
		{
			transform.localScale = new Vector3 (transform.localScale.x * x, transform.localScale.y * y, transform.localScale.z);
		}


		public static void ScaleByXZ (this Transform transform, float x, float z)
		{
			transform.localScale = new Vector3 (transform.localScale.x * x, transform.localScale.y, transform.localScale.z * z);
		}


		public static void ScaleByYZ (this Transform transform, float y, float z)
		{
			transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y * y, transform.localScale.z * z);
		}

		public static void ScaleByXY (this Transform transform, float r)
		{
			transform.ScaleByXY (r, r);
		}

		public static void ScaleByXZ (this Transform transform, float r)
		{
			transform.ScaleByXZ (r, r);
		}

		public static void ScaleByYZ (this Transform transform, float r)
		{
			transform.ScaleByYZ (r, r);
		}

		public static void ScaleByXYZ (this Transform transform, float x, float y, float z)
		{
			transform.localScale = new Vector3 (
				x, y, z);
		}

		public static void ScaleByXYZ (this Transform transform, float r)
		{
			transform.ScaleByXYZ (r, r, r);
		}


		public static void ResetScale (this Transform transform)
		{
			transform.localScale = Vector3.one;
		}

		#endregion

		#region Rotation

		public static void RotateAroundX (this Transform transform, float angle)
		{
			var rotation = new Vector3 (angle, 0, 0);
			transform.Rotate (rotation);
		}

		public static void RotateAroundY (this Transform transform, float angle)
		{
			var rotation = new Vector3 (0, angle, 0);
			transform.Rotate (rotation);
		}

		public static void RotateAroundZ (this Transform transform, float angle)
		{
			var rotation = new Vector3 (0, 0, angle);
			transform.Rotate (rotation);
		}

		public static void SetRotationX (this Transform transform, float angle)
		{
			transform.eulerAngles = new Vector3 (angle, 0, 0);
		}

		public static void SetRotationY (this Transform transform, float angle)
		{
			transform.eulerAngles = new Vector3 (0, angle, 0);
		}

		public static void SetRotationZ (this Transform transform, float angle)
		{
			transform.eulerAngles = new Vector3 (0, 0, angle);
		}

		public static void SetLocalRotationX (this Transform transform, float angle)
		{
			transform.localRotation = Quaternion.Euler (new Vector3 (angle, 0, 0));
		}

		public static void SetLocalRotationY (this Transform transform, float angle)
		{
			transform.localRotation = Quaternion.Euler (new Vector3 (0, angle, 0));
		}

		public static void SetLocalRotationZ (this Transform transform, float angle)
		{
			transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, angle));
		}

		public static void ResetRotation (this Transform transform)
		{
			transform.rotation = Quaternion.identity;
		}

		public static void ResetLocalRotation (this Transform transform)
		{
			transform.localRotation = Quaternion.identity;
		}

		#endregion

		#region All

		/**
			Resets the ;local position, local rotation, and local scale.
		*/

		public static void ResetLocal (this Transform transform)
		{
			transform.ResetLocalRotation (); 
			transform.ResetLocalPosition ();
			transform.ResetScale ();

		}

		/**
			Resets the position, rotation, and local scale.
		*/

		public static void Reset (this Transform transform)
		{
			transform.ResetRotation ();
			transform.ResetPosition ();
			transform.ResetScale ();
		}

		#endregion

		#region Children

		public static void DestroyChildren (this Transform transform)
		{
			//Add children to list before destroying
			//otherwise GetChild(i) may bomb out
			var children = new List<Transform> ();

			for (var i = 0; i < transform.childCount; i++) {
				var child = transform.GetChild (i);
				children.Add (child);
			}

			foreach (var child in children) {
				UnityEngine.Object.Destroy (child.gameObject);
			}
		}

		public static void DestroyChildrenImmediate (this Transform transform)
		{
			//Add children to list before destroying
			//otherwise GetChild(i) may bomb out
			var children = new List<Transform> ();

			for (var i = 0; i < transform.childCount; i++) {
				var child = transform.GetChild (i);
				children.Add (child);
			}

			foreach (var child in children) {
				UnityEngine.Object.DestroyImmediate (child.gameObject);
			}
		}

		public static List<Transform> GetChildren (this Transform transform)
		{
			var children = new List<Transform> ();

			for (var i = 0; i < transform.childCount; i++) {
				var child = transform.GetChild (i);
				children.Add (child);
			}

			return children;
		}

		public static void Sort (this Transform transform, Func<Transform, IComparable> sortFunction)
		{
			var children = transform.GetChildren ();
			var sortedChildren = children.OrderBy (sortFunction).ToList ();

			for (int i = 0; i < sortedChildren.Count (); i++) {
				sortedChildren [i].SetSiblingIndex (i);
			}
		}

		public static void SortAlphabetically (this Transform transform)
		{
			transform.Sort (t => t.name);
		}

		public static IEnumerable<Transform> SelfAndAllChildren (this Transform transform)
		{
			var openList = new Queue<Transform> ();

			openList.Enqueue (transform);

			while (openList.Any ()) {
				var currentChild = openList.Dequeue ();

				yield return currentChild;

				var children = transform.GetChildren ();

				foreach (var child in children) {
					openList.Enqueue (child);
				}
			}
		}

		#endregion
	}
}