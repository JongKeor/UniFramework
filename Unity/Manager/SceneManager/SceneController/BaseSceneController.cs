﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniFramework.Extension;
using UniFramework.Generic;
using System;




namespace UniFramework
{
    public class PopupArgs
    {
        [HideInInspector]
        public string name = null;
        [HideInInspector]
        public Dictionary<string, object> args = null;
        [HideInInspector]
        public System.Action<BaseSceneController> onOpen;
        [HideInInspector]
        public System.Action<BaseSceneController> onClose;
    }

    public class BaseSceneController : MonoBehaviour, ISceneController
    {
        private const string GAMEOBJECT_SHEILD = "_Sheild";

        public SceneInfo Root
        {
            get
            {
                return GameSceneManager.Instance.Root;
            }
        }

        public SceneInfo MySceneInfo
        {
            get
            {
                return sceneInfo;
            }
            set
            {
                sceneInfo = value;
            }
        }

        public Scene Scene
        {
            get
            {
                return gameObject.scene;
            }
        }

        public SceneInfo Parent
        {
            get
            {
                return MySceneInfo.Parent;
            }
        }

        public int SceneDepth
        {
            get
            {
                if (Parent == null) return 0;

                var parentcontroller = Parent.GetSceneController<BaseSceneController>();
                if (parentcontroller == null)
                {
                    return 0;
                }
                return parentcontroller.SceneDepth + 1;
            }
        }


        public SceneInfo[] Childs
        {
            get
            {
                return childs.ToArray();
            }
        }

        public SceneInfo CurrentPopup
        {
            get
            {
                return currentPopup;
            }
        }


        public Canvas[] canvases;

        public EventSystem eventSystem;


        protected SceneInfo sceneInfo;
        protected SceneInfo parent = null;
        protected List<SceneInfo> childs = new List<SceneInfo>();

        protected bool isApplicationQuit = false;

        private SceneInfo currentPopup;
        private Queue<PopupArgs> popupQueue = new Queue<PopupArgs>();

        private SceneInfo currentSinglePopup;
        private int popupSingleCount = 0;

        #region Unity Callback

        protected virtual void Awake()
        {
            GameSceneManager.Instance.GetSceneInfo(this.Scene);

            Debug.Log("Awake " + name + " " + Time.frameCount + " ");

        }
        protected virtual void Reset()
        {
            canvases = System.Array.FindAll<Canvas>(GameObject.FindObjectsOfType<Canvas>(),
                (canvas) => canvas.gameObject.scene == gameObject.scene
            );

            eventSystem = System.Array.Find<EventSystem>(GameObject.FindObjectsOfType<EventSystem>(),
                (es) => es.gameObject.scene == gameObject.scene
            );

        }

        protected void OnDestroy()
        {
            if (!isApplicationQuit)
            {
                if (GameSceneManager.Instance != null)
                {

                    popupQueue.Clear();
                    if (currentPopup != null)
                        GameSceneManager.Instance.CloseScene(currentPopup);

                    if (currentPopup != null)
                        GameSceneManager.Instance.CloseScene(currentSinglePopup);

                    foreach (var child in this.Childs)
                    {
                        GameSceneManager.Instance.CloseScene(child);
                    }

                    foreach (var child in this.Childs)
                    {
                        GameSceneManager.Instance.CloseScene(child);
                    }
                    GameSceneManager.Instance.CloseScene(MySceneInfo);
                    MySceneInfo.UnRegisterSceneController();
                }
            }
        }


        protected void OnApplicationQuit()
        {
            isApplicationQuit = true;
        }

        #endregion

        #region implemented interface members of ISceneController


        public virtual void OnOpen(Dictionary<string, object> arguments)
        {

            Debug.Log("OnOpen " + name + " " + Time.frameCount + " " + gameObject.scene.GetHashCode());
            if (Parent != null)
            {
                SetSceneCanvasOrder(SceneDepth);
            }
        }



        public virtual void OnClose()
        {

            Debug.Log("OnClose " + name + " " + Time.frameCount);
        }

        public virtual void OnActive()
        {
            Debug.Log("OnActive " + name + " " + Time.frameCount);
        }

        public virtual void OnDeactive()
        {
            Debug.Log("OnDeactive " + name + " " + Time.frameCount);
        }

        public virtual void OnOpenChildScene(SceneInfo info)
        {

        }

        public virtual void OnCloseChildScene(SceneInfo info)
        {

        }

        #endregion



        #region SceneManagement

        public void Close()
        {
            if (Parent != null)
            {
                var container = Parent.GetSceneController<SceneContainerController>();
                if (container != null)
                    container.ContainerChildClose();
            }
            else
            {
                GameSceneManager.Instance.CloseScene(MySceneInfo);
            }
        }


        public void Active()
        {
            foreach (var child in this.Childs)
            {
                child.ActiveScene();
            }
            MySceneInfo.ActiveScene();
        }

        public void Deactive()
        {
            foreach (var child in this.Childs)
            {
                child.DeactiveScene();

            }
            MySceneInfo.DeactiveScene();
        }

        public void Additive(string name, Dictionary<string, object> args = null, System.Action<BaseSceneController> onOpen = null, System.Action<BaseSceneController> onClose = null)
        {
            if (MySceneInfo.SceneType == SceneType.Popup)
            {
                Debug.LogWarning("Cannot Additive Scene in Popup Scene ");
                return;
            }
            Root.GetSceneController<BaseSceneController>().eventSystem.enabled = false;
            SceneInfo newScene = new SceneInfo(MySceneInfo, name, LoadSceneMode.Additive, args,
                                     (info) =>
                                     {
                                         if (onOpen != null)
                                             onOpen(info.GetSceneController<BaseSceneController>());
                                     },
                                     (info) =>
                                     {
                                         if (onClose != null)
                                             onClose(info.GetSceneController<BaseSceneController>());
                                     });
            newScene.OnClose += (obj) =>
            {
                this.childs.Remove(newScene);
            };
            this.childs.Add(newScene);
            GameSceneManager.Instance.LoadScene(newScene, _ =>
            {
                newScene.GetSceneController<BaseSceneController>().eventSystem.enabled = false;
                Root.GetSceneController<BaseSceneController>().eventSystem.enabled = true;
            });

        }

        public void Switch(string name, Dictionary<string, object> args = null, System.Action<BaseSceneController> onOpen = null, System.Action<BaseSceneController> onClose = null)
        {
            if (MySceneInfo.SceneType == SceneType.Popup)
            {
                Debug.LogWarning("Cannot Switch Scene  in Popup Scene ");
                return;
            }
            Root.GetSceneController<BaseSceneController>().eventSystem.enabled = false;
            SceneInfo newScene = new SceneInfo(name, LoadSceneMode.Single, args,
                                     (info) =>
                                     {
                                         if (onOpen != null)
                                             onOpen(info.GetSceneController<BaseSceneController>());
                                     },
                                     (info) =>
                                     {
                                         if (onClose != null)
                                             onClose(info.GetSceneController<BaseSceneController>());
                                     });
            GameSceneManager.Instance.LoadScene(newScene, _ =>
            {
                Root.GetSceneController<BaseSceneController>().eventSystem.enabled = true;
            });

        }

        public void PopupUniqueScene(string name, Dictionary<string, object> args = null, System.Action<BaseSceneController> onOpen = null, System.Action<BaseSceneController> onClose = null)
        {
            popupSingleCount++;
            if (currentSinglePopup != null)
            {
                var rootController = Root.GetSceneController<BaseSceneController>();
                rootController.SetEnableCanvas(false);
                Root.GetSceneController<BaseSceneController>().eventSystem.enabled = false;
                SceneInfo newScene = new SceneInfo(name, LoadSceneMode.Additive, args,
                    (info) =>
                    {
                        if (onOpen != null)
                            onOpen(info.GetSceneController<BaseSceneController>());
                    },
                    (info) =>
                    {
                        if (onClose != null)
                            onClose(info.GetSceneController<BaseSceneController>());
                    });
                newScene.SceneType = SceneType.Popup;
                newScene.OnClose += (SceneInfo obj) =>
                {
                    popupSingleCount = 0;
                    currentSinglePopup = null;
                    rootController.SetEnableCanvas(true);
                };
                currentSinglePopup = newScene;
                GameSceneManager.Instance.LoadScene(newScene, _ =>
             {
                 newScene.GetSceneController<BaseSceneController>().eventSystem.enabled = false;
                 newScene.GetSceneController<BaseSceneController>().SetSceneCanvasOrder(Root.GetSceneController<BaseSceneController>().GetTopSceneCanvasOrder() + 2);
                 Root.GetSceneController<BaseSceneController>().eventSystem.enabled = true;



             });

            }
        }
        public void CloseUniqueScene()
        {
            popupSingleCount--;
            if (popupSingleCount <= 0)
            {
                GameSceneManager.Instance.CloseScene(currentSinglePopup);
            }
        }

        public void Popup(string name, Dictionary<string, object> args = null, System.Action<BaseSceneController> onOpen = null, System.Action<BaseSceneController> onClose = null)
        {
            PopupArgs pinfo = new PopupArgs();
            pinfo.name = name;
            pinfo.args = args;
            pinfo.onOpen = onOpen;
            pinfo.onClose = onClose;
            popupQueue.Enqueue(pinfo);
            System.Action DisplayPopup = null;
            DisplayPopup = () =>
            {
                if (popupQueue.Count <= 0)
                {
                    return;
                }
                if (this.currentPopup != null)
                {
                    return;
                }
                var rootController = Root.GetSceneController<BaseSceneController>();
                PopupArgs popArgs = popupQueue.Dequeue();
                if (popArgs != null)
                {

                    rootController.SetEnableCanvas(false);
                    Root.GetSceneController<BaseSceneController>().eventSystem.enabled = false;
                    var newScene = new SceneInfo(popArgs.name, LoadSceneMode.Additive, popArgs.args,
                        (info) =>
                        {
                            if (popArgs.onOpen != null)
                                popArgs.onOpen(info.GetSceneController<BaseSceneController>());
                        },
                        (info) =>
                        {
                            if (popArgs.onClose != null)
                                popArgs.onClose(info.GetSceneController<BaseSceneController>());
                        });
                    newScene.SceneType = SceneType.Popup;
                    newScene.OnClose += (SceneInfo obj) =>
                    {
                        this.currentPopup = null;
                        rootController.SetEnableCanvas(true);
                        DisplayPopup();
                    };

                    this.currentPopup = newScene;
                    GameSceneManager.Instance.LoadScene(newScene, _ =>
                    {
                        newScene.GetSceneController<BaseSceneController>().eventSystem.enabled = false;
                        newScene.GetSceneController<BaseSceneController>().SetSceneCanvasOrder(Root.GetSceneController<BaseSceneController>().GetTopSceneCanvasOrder() + 2);
                        Root.GetSceneController<BaseSceneController>().eventSystem.enabled = true;

                    });

                }
                else
                {

                }
            };

            DisplayPopup();
        }

        #endregion

        #region Canvas

        public void SetEnableCanvas(bool bEnable)
        {
            GameObject shield = gameObject.FindAndCreateChildObjectByName(GAMEOBJECT_SHEILD, typeof(Canvas), typeof(Image));
            Canvas canvas = shield.GetComponent<Canvas>();
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            Image image = shield.GetComponent<Image>();
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.sizeDelta = Vector2.zero;
            image.rectTransform.localPosition = Vector2.zero;
            image.color = new Color(0f, 0f, 0f, 0.5f);

            GraphicRaycaster caster = shield.GetOrAddComponent<GraphicRaycaster>();
            caster.enabled = !bEnable;

            canvas.sortingOrder = GetTopSceneCanvasOrder() * 10 + 10;
            canvas.enabled = !bEnable;
        }

        public Canvas FindTopCanvas()
        {
            int sort = -1;
            Canvas ca = null;
            foreach (var can in this.canvases)
            {
                if (sort <= can.sortingOrder)
                {
                    ca = can;
                }
            }
            return ca;
        }

        public int GetTopSceneCanvasOrder()
        {
            int max = GetCurrentCanvasOrder();
            foreach (var child in childs)
            {
                var controller = child.GetSceneController<BaseSceneController>();
                if (controller != null)
                {
                    int de = child.GetSceneController<BaseSceneController>().GetTopSceneCanvasOrder();
                    if (max < de)
                    {
                        max = de;
                    }
                }
            }
            return max;
        }


        public int GetCurrentCanvasOrder()
        {
            return FindTopCanvas().sortingOrder / 10;
        }

        public void SetSceneCanvasOrder(int order)
        {
            foreach (var can in this.canvases)
            {
                can.sortingOrder = can.sortingOrder % 10 + order * 10;
            }
        }

        public void SetCanvasTop()
        {
            int current = GetCurrentCanvasOrder();
            int top = GetTopSceneCanvasOrder();
            if (current < top)
            {
                SetSceneCanvasOrder(top + 1);
            }
        }

        #endregion

        //		public T GetController<T> (SceneInfo info)  where T  : BaseSceneController
        //		{
        //			if(info == null) return null;
        //			return info.GetSceneController<T>() ; 	
        //		}
        //
        //		public T GetControllerInPerant<T> () where T : BaseSceneController
        //		{
        //			SceneInfo t = this.Parent;
        //			while (t != null) {
        //				var controller = GetController<T> (t);
        //				if (controller is T) {
        //					return controller as T;
        //				}
        //				t = t.Parent;
        //			}
        //			return null;
        //		}
        //


        protected bool IsChildSceneInfo(SceneInfo info)
        {
            Queue<SceneInfo> queue = new Queue<SceneInfo>();
            foreach (var child in this.childs)
            {
                queue.Enqueue(child);
            }
            while (queue.Count > 0)
            {
                SceneInfo target = queue.Dequeue();
                if (target.Equals(info))
                {
                    return true;
                }
                var con = target.GetSceneController<BaseSceneController>();
                if (con != null)
                {
                    foreach (var child in con.childs)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
            return false;

        }

    }
}

