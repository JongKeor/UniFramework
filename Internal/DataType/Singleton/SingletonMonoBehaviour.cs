using UnityEngine;
using System.Collections;

namespace UniFramework.Generic
{
	public class SingletonMonoBehaviour<T> : MonoBehaviour  where T : MonoBehaviour
	{
		private static T _instance;

		private static object _lock = new object ();

		public static T Instance {
			get {
				if (applicationIsQuitting) {
//					Debug.LogWarning ("[Singleton] Instance '" + typeof(T) +
//					"' already destroyed on application quit." +
//					" Won't create again - returning null.");
					return null;
				}

				lock (_lock) {
					if (_instance == null) {
						_instance = (T)FindObjectOfType (typeof(T));

						if (FindObjectsOfType (typeof(T)).Length > 1) {
							Debug.LogError ("[Singleton] Something went really wrong " +
							" - there should never be more than 1 singleton!" +
							" Reopening the scene might fix it.");
							return _instance;
						}

						if (_instance == null) {
							GameObject singleton = new GameObject ();
							_instance = singleton.AddComponent<T> ();
							singleton.name = "(singleton) " + typeof(T).ToString ();

							DontDestroyOnLoad (singleton);

							Debug.Log ("[Singleton] An instance of " + typeof(T) +
							" is needed in the scene, so '" + singleton +
							"' was created with DontDestroyOnLoad.");
						} else {
							Debug.Log ("[Singleton] Using instance already created: " +
							_instance.gameObject.name);
						}
					}

					return _instance;
				}
			}
		}

		private static bool applicationIsQuitting = false;
		public void OnApplicationQuit(){
			applicationIsQuitting = true;
		}

		public void OnDestroy ()
		{
			
		}
	}

//	public class SingletonMonoBehaviour<S,M>   where S : SingletonMonoBehaviour<S,M>, new()  where M : MonoBehaviour
//	{
//		private static S _instance;
//
//		private static object _lock = new object ();
//
//		public static S Instance {
//			get {
//				lock (_lock) {
//					if (_instance == null) {
//						_instance = new S ();
//						_instance.monobehaviour = (M)GameObject.FindObjectOfType (typeof(M));
//
//						if (GameObject.FindObjectsOfType (typeof(M)).Length > 1) {
//							Debug.LogError ("[Singleton] Something went really wrong " +
//							" - there should never be more than 1 singleton!" +
//							" Reopening the scene might fix it.");
//							return _instance;
//						}
//
//						if (_instance == null) {
//							GameObject singleton = new GameObject ();
//							_instance.monobehaviour = singleton.AddComponent<M> ();
//							singleton.name = "(singleton) " + typeof(M).ToString ();
//
//							GameObject.DontDestroyOnLoad (singleton);
//
//							Debug.Log ("[Singleton] An instance of " + typeof(M) +
//							" is needed in the scene, so '" + singleton +
//							"' was created with DontDestroyOnLoad.");
//						} else {
//							Debug.Log ("[Singleton] Using instance already created: " +
//							_instance.monobehaviour.gameObject.name);
//						}
//					}
//
//					return _instance;
//				}
//			}
//		}
//
//		private M monobehaviour;
//
//		public M Behaviour {
//			get {
//				return monobehaviour;
//			}
//		}
//	}
}