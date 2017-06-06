using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UniFramework.Extension
{
	public static class GameObjectExtension
	{

		public static T GetOrAddComponent<T> (this GameObject self) where T: Component
		{
			return  GetOrAddComponent (self, typeof(T)) as T;
		}

		public static Component GetOrAddComponent (this GameObject self, System.Type type)
		{
			Component result = self.GetComponent (type);
			if (result == null) {
				result = self.AddComponent (type);
			}
			return result;
		}

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
	}


	public static class GameObjectPauseExtension 
	{
		public static void Pause(this GameObject self)
		{
			MonoBehaviour[] comps = self.GetComponentsInChildren<MonoBehaviour>();
			for(int i = 0 ; i < comps.Length ; i++)
			{	
				comps[i].enabled = false;
			}

			self.GetComponent<ParticleSystem>().Play();
			self.GetComponent<Animation>().Play();
			self.GetComponent<Animator>().speed = 1;
			self.GetComponent<Rigidbody>().SetPlaySpeed(1);
			self.GetComponent<Rigidbody2D>().simulated = true;
		}
		public static void Resume(this GameObject self)
		{
			MonoBehaviour[] comps = self.GetComponentsInChildren<MonoBehaviour>();
			for(int i = 0 ; i < comps.Length ; i++)
			{	
				comps[i].enabled = true;
			}	

			self.GetComponent<ParticleSystem>().Pause();
			self.GetComponent<Animation>().Stop();
			self.GetComponent<Animator>().speed = 0;
			self.GetComponent<Rigidbody>().SetPlaySpeed(0);
			self.GetComponent<Rigidbody2D>().simulated = false;
		}
	
	}
}

