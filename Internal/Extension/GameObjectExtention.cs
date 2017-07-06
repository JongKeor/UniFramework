using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UniFramework.Extension
{
    public static class GameObjectExtension
    {

        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            return GetOrAddComponent(self, typeof(T)) as T;
        }

        public static Component GetOrAddComponent(this GameObject self, System.Type type)
        {
            Component result = self.GetComponent(type);
            if (result == null)
            {
                result = self.AddComponent(type);
            }
            return result;
        }

        public static GameObject FindChildObjectByName(this GameObject self, string strName)
        {
            Transform[] AllData = self.GetComponentsInChildren<Transform>(true);
            GameObject target = null;
            for (int i = 0; i < AllData.Length; i++)
            {
                if (AllData[i].name.Equals(strName))
                {
                    target = AllData[i].gameObject;
                    break;
                }
            }
            return target;
        }

        public static GameObject FindAndCreateChildObjectByName(this GameObject self, string strName ,params System.Type[] components)
        {
            GameObject ret = FindChildObjectByName(self, strName);
            if (ret == null)
            {
                ret = new GameObject(strName,components);
                ret.transform.SetParent(self.transform);
                ret.transform.localPosition = Vector3.zero;
                ret.transform.localRotation = Quaternion.identity;
                ret.transform.localScale = Vector3.one;
            }
            return ret;
        }

        public static GameObject[] FindChlidObjectsByName(this GameObject self, string strName)
        {
            List<GameObject> ret = new List<GameObject>();
            Transform[] AllData = self.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < AllData.Length; i++)
            {
                if (AllData[i].name.Contains(strName))
                {
                    ret.Add(AllData[i].gameObject);
                }
            }
            return ret.ToArray();
        }
    }


    public static class GameObjectPauseExtension
    {
        public static void Pause(this GameObject self)
        {
            MonoBehaviour[] comps = self.GetComponentsInChildren<MonoBehaviour>();
            for (int i = 0; i < comps.Length; i++)
            {
                comps[i].enabled = false;
            }




            ParticleSystem[] pss =  self.GetComponentsInChildren<ParticleSystem>();
			foreach(var ps in pss){
				ps.Pause();
			}
            Animation[] anis =  self.GetComponentsInChildren<Animation>();
			foreach(var ani in anis){
				ani.Stop();
			}

			Animator[] ats =  self.GetComponentsInChildren<Animator>();
			foreach(var at in ats){
				at.speed = 0;
			}

			Rigidbody[] rds =  self.GetComponentsInChildren<Rigidbody>();
			foreach(var rd in rds){
				rd.SetPlaySpeed(0);
			}

			Rigidbody2D[] rd2s =  self.GetComponentsInChildren<Rigidbody2D>();
			foreach(var rd in rd2s){
				 rd.simulated = false;
			}
            
            
           
        }
        public static void Resume(this GameObject self)
        {
            MonoBehaviour[] comps = self.GetComponentsInChildren<MonoBehaviour>();
            for (int i = 0; i < comps.Length; i++)
            {
                comps[i].enabled = true;
            }


            ParticleSystem[] pss =  self.GetComponentsInChildren<ParticleSystem>();
			foreach(var ps in pss){
				ps.Play();
			}
            Animation[] anis =  self.GetComponentsInChildren<Animation>();
			foreach(var ani in anis){
				ani.Play();
			}

			Animator[] ats =  self.GetComponentsInChildren<Animator>();
			foreach(var at in ats){
				at.speed = 1;
			}

			Rigidbody[] rds =  self.GetComponentsInChildren<Rigidbody>();
			foreach(var rd in rds){
				rd.SetPlaySpeed(1);
			}

			Rigidbody2D[] rd2s =  self.GetComponentsInChildren<Rigidbody2D>();
			foreach(var rd in rd2s){
				 rd.simulated = true;
			}

        }

    }
}

