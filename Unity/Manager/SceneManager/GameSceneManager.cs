// #define MERGE_SCENE

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
        public event System.Action<SceneInfo> onSceneLoad = delegate
        {

        };
        public event System.Action<SceneInfo> onSceneUnLoad = delegate
        {

        };
        public SceneInfo Root
        {
            get
            {
                return root;
            }
        }

        protected SceneInfo root;
        protected Dictionary<Scene, SceneInfo> loadedSceneInfo = new Dictionary<Scene, SceneInfo>();





#if MERGE_SCENE
        protected Dictionary<Scene, Scene> newSceneinfos = new Dictionary<Scene, Scene>();
#endif
        protected void Awake()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
            SceneManager.activeSceneChanged += (arg0, arg1) =>
            {
                Debug.Log("ActiveSceneChanged " + arg0.name + " " + arg1.name + " " + Time.frameCount);
            };


        }

        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneInfo sceneInfo = GetSceneInfo(scene);

            // For First Scene
            if (sceneInfo == null)
            {
                sceneInfo = new SceneInfo(scene);
                root = sceneInfo;
                AddSceneInfo(scene, sceneInfo);
            }
            ISceneController controller = null;
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (GameObject robj in rootObjects)
            {
                controller = robj.GetComponent<ISceneController>();
                if (controller != null) break;
            }
            controller.MySceneInfo = sceneInfo;
            sceneInfo.RegisterSceneController(controller);
            onSceneLoad(sceneInfo);
            sceneInfo.InvokeOpen();
        }

        private void SceneManager_sceneUnloaded(Scene scene)
        {
            SceneInfo info;
            if (loadedSceneInfo.TryGetValue(scene, out info))
            {
                info.InvokeClose();
                onSceneUnLoad(info);
                RemoveSceneInfo(scene);
            }

        }



        public SceneInfo GetSceneInfo(Scene scene)
        {
            Scene t = scene;
#if MERGE_SCENE
            Scene newScene = new Scene();
            if (newSceneinfos.TryGetValue(scene, out newScene))
            {
                t = newScene;
            }
#endif

            SceneInfo sceneInfo;
            if (loadedSceneInfo.TryGetValue(t, out sceneInfo))
            {
                return sceneInfo;
            }


            return sceneInfo;
        }

        private void AddSceneInfo(Scene scene, SceneInfo info)
        {
            info.SceneData = scene;
            loadedSceneInfo.Add(scene, info);
        }

        private void RemoveSceneInfo(Scene scene)
        {
            loadedSceneInfo.Remove(scene);
        }
        //		public void LoadScene (SceneInfo info)
        //		{
        //			 SceneManager.LoadScene (info.Name, info.Mode);
        //			var currentScene = SceneManager.GetSceneAt(SceneManager.sceneCount -1);
        //			RegisterSceneInfo(currentScene , info );
        //		}

        public void LoadScene(SceneInfo info, System.Action<SceneInfo> onLoad)
        {
#if MERGE_SCENE
            Scene newScene = new Scene();
#endif
            Scene currentScene = new Scene();

            if (info.Mode == LoadSceneMode.Single)
            {
                // for (int i = 0; i < SceneManager.sceneCount; i++)
                // {
                //     unloadingScene.Add(SceneManager.GetSceneAt(i));
                // }
            }

            SceneManager.LoadScene(info.Name, info.Mode);
            currentScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            if (info.Mode == LoadSceneMode.Single)
            {
                root = info;
            }


#if MERGE_SCENE
            if (info.Mode == LoadSceneMode.Additive)
            {
                newScene = SceneManager.CreateScene(currentScene.name + currentScene.GetHashCode().ToString());
                newSceneinfos.Add(currentScene, newScene);
                AddSceneInfo(newScene, info);
            }
            else
            {
                AddSceneInfo(currentScene, info);
            }
#else
            AddSceneInfo(currentScene, info);
#endif
            info.OnOpen += _ =>
            {
                onLoad(_);
            };

#if MERGE_SCENE
            if (info.Mode == LoadSceneMode.Additive)
            {
                newSceneinfos.Remove(currentScene);
                SceneManager.MergeScenes(currentScene, newScene);
                
            }
#endif

        }


        public void CloseScene(SceneInfo info)
        {

            StartCoroutine(UnloadingScene(info.SceneData));


        }

        private IEnumerator UnloadingScene(Scene scene)
        {

#if MERGE_SCENE
            AsyncOperation op = SceneManager.UnloadSceneAsync(scene.name);
#else
            AsyncOperation op = SceneManager.UnloadSceneAsync(scene);
#endif
            if (op != null)
            {
                while (!op.isDone)
                    yield return null;
            }

        }

    }



}

