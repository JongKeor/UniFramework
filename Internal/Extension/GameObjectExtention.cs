using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UniFramework.Extension
{
	public static class GameObjectExtension
	{

		public static T GetOrAddComponent<T> (this GameObject child) where T: Component
		{
			return  GetOrAddComponent (child, typeof(T)) as T;
		}

		public static Component GetOrAddComponent (this GameObject child, System.Type type)
		{
			Component result = child.GetComponent (type);
			if (result == null) {
				result = child.gameObject.AddComponent (type);
			}
			return result;
		}
		
		//
		//	public static void IterateChildren (this GameObject self, System.Func<GameObject, bool>  childHandler)
		//	{
		//		foreach (Transform child in self.transform) {
		//			if (childHandler (child.gameObject)) {
		//				child.gameObject.IterateChildren (childHandler);
		//			}
		//		}
		//	}

		public static GameObject FindChildObjectByName (this GameObject self, string strName)
		{
			Transform[] AllData = self.GetComponentsInChildren<Transform> (true);
			GameObject target = null;
			for (int i = 0; i < AllData.Length; i++) {
				if (AllData [i].name.Equals (strName)) {
					target = AllData [i].gameObject;
					break;
				}
			}
			return target;
		}

		public static GameObject FindAndCreateChildObjectByName (this GameObject self, string strName)
		{
			GameObject ret = FindChildObjectByName (self, strName);
			if (ret == null) {
				ret = new  GameObject (strName);
				ret.transform.parent = self.transform;
				ret.transform.localPosition = Vector3.zero;
				ret.transform.localRotation = Quaternion.identity;
				ret.transform.localScale = Vector3.one;
			}
			return ret;
		}

		public static GameObject[] FindChlidObjectsByName (this GameObject self, string strName)
		{
			List<GameObject> ret = new List<GameObject> ();
			Transform[] AllData = self.GetComponentsInChildren<Transform> (true);

			for (int i = 0; i < AllData.Length; i++) {
				if (AllData [i].name.Contains (strName)) {
					ret.Add (AllData [i].gameObject);
				}
			}
			return ret.ToArray ();
		}

		public static float GetColliderXZRadius (this GameObject obj)
		{
			CapsuleCollider capsule = obj.GetComponent<CapsuleCollider> ();
			if (capsule != null) {
				return capsule.radius;
			}
			BoxCollider box = obj.GetComponent<BoxCollider> ();
			if (box != null) {
				return box.bounds.size.x < box.bounds.size.z ? box.bounds.size.z : box.bounds.size.x;
			}

			SphereCollider sphere = obj.GetComponent<SphereCollider> ();
			if (sphere != null) {
				return sphere.radius;
			}

			CharacterController cc = obj.GetComponent<CharacterController> ();
			if (cc != null) {
				return cc.radius;
			}

			MeshCollider mesh = obj.GetComponent<MeshCollider> ();
			if (mesh != null) {
				return mesh.bounds.size.x < mesh.bounds.size.z ? mesh.bounds.size.z : mesh.bounds.size.x;
			}

			return 0;
		}

		public static Vector3 GetColliderCenter (this GameObject obj)
		{
			if (obj == null) {
				return Vector3.zero;
			}
			CapsuleCollider capsule = obj.GetComponent<CapsuleCollider> ();
			if (capsule != null) {
				return obj.transform.TransformPoint (capsule.center);
			}
			BoxCollider box = obj.GetComponent<BoxCollider> ();
			if (box != null) {
				return obj.transform.TransformPoint (box.center);
			}
			SphereCollider sphere = obj.GetComponent<SphereCollider> ();
			if (sphere != null) {
				return obj.transform.TransformPoint (sphere.center);
			}

			CharacterController cc = obj.GetComponent<CharacterController> ();
			if (cc != null) {
				return obj.transform.TransformPoint (cc.center);
			}

			MeshCollider mesh = obj.GetComponent<MeshCollider> ();
			if (mesh != null) {
				return obj.transform.TransformPoint (mesh.bounds.center);
			}

			return Vector3.zero;
		}

	}
}

