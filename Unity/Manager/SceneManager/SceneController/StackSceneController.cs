﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


namespace UniFramework
{
    public class StackSceneController : SceneContainerController
    {
        public string rootSceneName;


        public override void OnOpen(Dictionary<string, object> arguments)
        {
            Push(rootSceneName);
        }

        public override void OnClose()
        {

        }

        public override void OnOpenChildScene(SceneInfo info)
        {
            Debug.Log("stackScene    "  + info.SceneData.name);
            SetCanvasTop();
        }

        public void Push(string rootSceneName ,Dictionary<string, object> args = null)
        {
         
            SceneInfo preScene = childs.Count != 0 ? childs[childs.Count - 1] : null;
            SceneInfo newScene = new SceneInfo(MySceneInfo, rootSceneName, LoadSceneMode.Additive ,args);
            
            Root.GetSceneController<BaseSceneController>().eventSystem.enabled = false;
            newScene.OnClose += (SceneInfo obj) =>
            {
                int removeIndex = this.childs.IndexOf(obj);
                if (removeIndex >= 0)
                {
                    this.childs.RemoveAt(removeIndex);
                    var delList = this.childs.GetRange(removeIndex, childs.Count - removeIndex);
                    foreach (var del in delList)
                    {
                        this.childs.Remove(del);
                    }

                    foreach (var del in delList)
                    {
                        GameSceneManager.Instance.CloseScene(del);
                    }
                    if (childs.Count != 0)
                    {
                        childs[childs.Count - 1].GetSceneController<BaseSceneController>().Active();
                    }
                }
            };

            this.childs.Add(newScene);
            GameSceneManager.Instance.LoadScene(newScene, _ =>
            {
                newScene.GetSceneController<BaseSceneController>().eventSystem.enabled = false;
                Root.GetSceneController<BaseSceneController>().eventSystem.enabled = true;
                
                if (preScene != null)
                    preScene.GetSceneController<BaseSceneController>().Deactive();
            });

        }

        #region implemented abstract members of SceneContainer

        public override void ContainerChildClose()
        {
            Pop();
        }

        #endregion

        public void Pop()
        {

            if (childs.Count > 1)
            {
                GameSceneManager.Instance.CloseScene(childs[childs.Count - 1]);
            }
        }






    }
}

