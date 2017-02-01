#define MERGE_SCENE

using System;
using UniFramework.Generic;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UniFramework;
using System.Collections;
using UnityEngine;
using UniFramework.Extension;




namespace UniFramework
{
	


	public class GameSceneManager : SingletonMonoBehaviour<GameSceneManager>
	{
		public event System.Action<SceneInfo> onSceneLoad = delegate {
			
		};
		public event System.Action<SceneInfo> onSceneUnLoad = delegate {
			
		};
		public SceneInfo Root{
			get {
				return root;
			}
		}

		protected SceneInfo root;
		protected Dictionary<Scene, SceneInfo> loadedSceneInfo = new Dictionary<Scene, SceneInfo> ();

		protected List<Scene> unloadingScene = new List<Scene>();



		#if MERGE_SCENE
		protected Dictionary<Scene, Scene> newSceneinfos = new Dictionary<Scene, Scene> ();
		#endif
		protected void Awake ()
		{
			SceneManager.sceneLoaded += SceneManager_sceneLoaded; 
			SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
//			SceneManager.activeSceneChanged += (arg0, arg1) => {
//				Debug.Log("ActiveSceneChanged " +  arg0.name + " "  + arg1.name+ " "  + Time.frameCount);
//			};
		}

		private void SceneManager_sceneLoaded (Scene scene, LoadSceneMode mode)
		{
//			SceneInfo info = GetSceneInfo (scene);
		}

		private void SceneManager_sceneUnloaded (Scene scene)
		{	
			SceneInfo info ;
			if (loadedSceneInfo.TryGetValue (scene, out info)) {
				info.InvokeClose ();
				onSceneUnLoad (info);
				UnregisterSceneInfo (scene);
			} 


		}



		public SceneInfo GetSceneInfo (Scene scene)
		{
			Scene t = scene ;
			#if MERGE_SCENE
			Scene newScene = new Scene ();
			if (newSceneinfos.TryGetValue (scene, out newScene)) {
				t = newScene;
			}
			#endif
				
			SceneInfo sceneInfo;
			if (loadedSceneInfo.TryGetValue (t, out sceneInfo)) {
				return sceneInfo;
			} 
			// For First Scene
			if (sceneInfo == null) {
				sceneInfo = new SceneInfo (t);
				root = sceneInfo;
				RegisterSceneInfo (t, sceneInfo);
				StartCoroutine (WaitForReadyCoroutine (sceneInfo));
			}

			return sceneInfo;
		}

		private void RegisterSceneInfo (Scene scene, SceneInfo info)
		{
			loadedSceneInfo.Add (scene, info);
		}

		private void UnregisterSceneInfo (Scene scene)
		{
			loadedSceneInfo.Remove (scene);
		}
		//		public void LoadScene (SceneInfo info)
		//		{
		//			 SceneManager.LoadScene (info.Name, info.Mode);
		//			var currentScene = SceneManager.GetSceneAt(SceneManager.sceneCount -1);
		//			RegisterSceneInfo(currentScene , info );
		//		}

		public AsyncTask LoadSceneAsync (SceneInfo info, System.Action onLoad, System.Action<Exception> onFail)
		{
			return this.StartCoroutineEx (LoadSceneCoroutine (info, onLoad, onFail));
		}

		private IEnumerator LoadSceneCoroutine (SceneInfo info, System.Action onLoad, System.Action<Exception> onFail)
		{
			
			AsyncOperation ay = null;
			#if MERGE_SCENE
			Scene newScene = new Scene ();
			#endif
			Scene currentScene = new Scene ();
			if (info.SceneData.State == CachedState.NotCached || info.SceneData.State == CachedState.Fail) {
				var isdone = info.SceneData.Cache (
					             (s, f) => {
						try {
							if(info.Mode == LoadSceneMode.Single){
								for(int i = 0 ; i < SceneManager.sceneCount ; i++)
								{
									unloadingScene.Add(SceneManager.GetSceneAt(i));
								}
							}
							ay = SceneManager.LoadSceneAsync (info.Name, info.Mode);
							info.SetAsyncOperaction (ay);
							currentScene = SceneManager.GetSceneAt (SceneManager.sceneCount - 1);
							if(info.Mode == LoadSceneMode.Single){
								root = info;
							}
							#if MERGE_SCENE
							if (info.Mode == LoadSceneMode.Additive) {
								newScene = SceneManager.CreateScene (currentScene.name + currentScene.GetHashCode ().ToString ());
								newSceneinfos.Add (currentScene, newScene);
								RegisterSceneInfo (newScene, info);
								s (newScene);
							} else {
								RegisterSceneInfo (currentScene, info);
								s (currentScene);
							}
							#else 
							RegisterSceneInfo(currentScene , info );
							s (currentScene);
							#endif
							return AsyncTaskInternal.Complete ();
						} catch (Exception ex) {
							f (ex);
							return AsyncTaskInternal.Complete ();
						}
					});
				while (isdone.keepWaiting) {
					yield return null;
				}
			}
			if (info.SceneData.State == CachedState.Cached) {
				while (!ay.isDone) {
					yield return null;
				}
				info.SetAsyncOperaction (null);
				#if MERGE_SCENE
				if (info.Mode == LoadSceneMode.Additive) {
					newSceneinfos.Remove (currentScene);
					SceneManager.MergeScenes (currentScene, newScene);
				}
				#endif
				info.DisableScene ();
				info.OnOpen += (obj) => {
					onLoad ();
				};

			} else {
				onFail (new Exception ("Fail to Load  Scene '" + info.Name + "'"));
			}
			yield return StartCoroutine(WaitForReadyCoroutine(info));
		}

		private IEnumerator WaitForReadyCoroutine(SceneInfo info){
			while (!info.IsLoaded) {
				
				yield return null;
			}
			info.InvokeLoad();
			info.DisableScene ();
			while (!info.IsReady ()) {
				
				yield return null;
			}
			info.EnableScene ();
			onSceneLoad (info);
			info.InvokeOpen ();
		}

		public void CloseScene (SceneInfo info)
		{
			
			if (info.SceneData.State == CachedState.Cached) {
				if(unloadingScene.Contains(info.SceneData.Value))return ;	
				if (SceneManager.sceneCount - unloadingScene.Count != 1) {
					StartCoroutine (UnloadingScene (info.SceneData.Value));
				}
			}
		}

		private IEnumerator UnloadingScene (Scene scene)
		{
			unloadingScene.Add(scene);
			#if MERGE_SCENE
			AsyncOperation op = SceneManager.UnloadSceneAsync (scene.name);
			#else
				AsyncOperation op =  SceneManager.UnloadSceneAsync (scene);
			#endif
			if (op != null) {
				while (!op.isDone)
					yield return null;
			}
//			else {
//				Debug.LogWarning("There is no Scene '" + scene.name +"'");
//			}
			unloadingScene.Remove(scene);
		}

	}



}

