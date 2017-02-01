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


		public override void OnLoad()
		{
			base.OnLoad();
			currentSceneIdx = 0;
			SwitchTabScene (currentSceneIdx);
			isReadyToOpen = false;
		}
		public override void OnClose ()
		{
			
		}
		public override void OnOpenChildScene (SceneInfo info)
		{
			base.OnOpenChildScene (info);
			SetCanvasTop ();
		}

//		void GameSceneManager_Instance_onSceneLoad (SceneInfo obj)
//		{
//			if(IsChildSceneInfo(obj)){
//				SetCanvasTop();
//			}
//		}


		protected void SwitchTabScene (int idx)
		{
			if (currentTapSceneInfo != null &&currentTapSceneInfo.Name ==  tabSceneNames[idx]) {
				return ;
			}
			if (currentTapSceneInfo != null && !currentTapSceneInfo.IsLoaded) {
				return ;
			}

			SceneInfo preScene = currentTapSceneInfo;
			var info = this.childs.Find (o => o.Name == tabSceneNames [idx]);
			if (info == null) {
				Root.GetSceneController<BaseSceneController>().eventSystem.enabled = false;
				SceneInfo newScene = new SceneInfo (MySceneInfo,tabSceneNames [idx], LoadSceneMode.Additive);
				newScene.OnClose += (SceneInfo obj) => this.childs.Remove(newScene);
				currentTapSceneInfo = newScene;
				this.childs.Add(newScene);
				GameSceneManager.Instance.LoadSceneAsync (newScene,()=> {
					Root.GetSceneController<BaseSceneController>().eventSystem.enabled = true;		
					if(preScene!= null) preScene.GetSceneController<BaseSceneController> ().Deactive();
					isReadyToOpen = true;

				}, (ex)=>{
					Root.GetSceneController<BaseSceneController>().eventSystem.enabled = true;		
				} 
					
				);
			} else {
				if(preScene!= null) preScene.GetSceneController<BaseSceneController> ().Deactive();
				info.GetSceneController<BaseSceneController> ().Active();
				currentTapSceneInfo = info;
				return ;
			}


		}

		#region implemented abstract members of SceneContainer

		public override void ContainerChildClose ()
		{
//			this.Close();
		}

		#endregion
	}
}

