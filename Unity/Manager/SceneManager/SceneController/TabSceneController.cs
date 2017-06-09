using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UniFramework.Extension;


namespace UniFramework
{
    public class TabSceneController : SceneContainerController
    {

        [SerializeField]
        protected string[] tabSceneNames;
        protected int currentSceneIdx;


        protected SceneInfo currentTapSceneInfo;


        public override void OnOpen(Dictionary<string, object> arguments)
        {
            base.OnOpen(arguments);
			currentSceneIdx = 0;
            SwitchTabScene(currentSceneIdx);
        }
        public override void OnClose()
        {

        }
        public override void OnOpenChildScene(SceneInfo info)
        {
            base.OnOpenChildScene(info);
            Debug.Log("tabScene   "  + info.SceneData.name);
            SetCanvasTop();
        }

      


        protected void SwitchTabScene(int idx)
        {
            if (currentTapSceneInfo != null && currentTapSceneInfo.Name == tabSceneNames[idx])
            {
                return;
            }
            

            SceneInfo preScene = currentTapSceneInfo;
            var info = this.childs.Find(o => o.Name == tabSceneNames[idx]);
            if (info == null)
            {
                Root.GetSceneController<BaseSceneController>().eventSystem.enabled = false;
                SceneInfo newScene = new SceneInfo(MySceneInfo, tabSceneNames[idx], LoadSceneMode.Additive);
                newScene.OnClose += (SceneInfo obj) => this.childs.Remove(newScene);
                currentTapSceneInfo = newScene;
                this.childs.Add(newScene);
                GameSceneManager.Instance.LoadScene(newScene, _=> {
                    
                    newScene.GetSceneController<BaseSceneController>().eventSystem.enabled = false;
                    Root.GetSceneController<BaseSceneController>().eventSystem.enabled = true;
                    if (preScene != null) preScene.GetSceneController<BaseSceneController>().Deactive();
                });
                

            }
            else
            {
                if (preScene != null) preScene.GetSceneController<BaseSceneController>().Deactive();
                info.GetSceneController<BaseSceneController>().Active();
                currentTapSceneInfo = info;
                return;
            }


        }

        #region implemented abstract members of SceneContainer

        public override void ContainerChildClose()
        {
            //			this.Close();
        }

        #endregion
    }
}

