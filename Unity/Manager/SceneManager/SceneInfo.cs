using System;
using UniFramework.Generic;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace UniFramework
{
	public enum SceneType
	{
		Screen,
		Popup
	}

	public interface ISceneController
	{

		void OnLoad ();

		bool IsReady ();

		void OnOpen (Dictionary<string,object> args);

		void OnClose ();

		void OnActive ();

		void OnDeactive ();

		void OnOpenChildScene (SceneInfo info);
		void OnCloseChildScene (SceneInfo info);
	}

	public class SceneInfo
	{
		public SceneInfo Root {
			get {
				if(Parent == null) return this;
				else return Parent;
			}
		}
		public SceneInfo Parent{
			get {
				return parent;
			}
		}

		public string Name {
			get {
				return sceneName;
			}
		}
		public LoadSceneMode Mode {
			get {
				return loadMode;
			}
		}
		public bool IsLoaded {
			get {
				if(asycOperation == null){
					return SceneData.Value.isLoaded;
				}
				else {
					return asycOperation.isDone;
				}
			}
		}
		public CachedProperty<Scene> SceneData = new CachedProperty<Scene> ();
		public SceneType SceneType = SceneType.Screen;
		public Dictionary<string,object>  Arguments {
			get {
				return arguments;
			}
		}
		public event System.Action<SceneInfo> OnOpen = delegate {
			
		};
		public event System.Action<SceneInfo> OnClose = delegate {
			
		};

		private ISceneController iSceneController ;

		private string sceneName;
		private LoadSceneMode loadMode;
		private Dictionary<string,object> arguments;
		private GameObject[] roots;
		private SceneInfo parent;
		private AsyncOperation asycOperation ;
		public SceneInfo(SceneInfo parent , string sceneName, LoadSceneMode mode, Dictionary<string,object> arguments = null, System.Action<SceneInfo> onOpen = null, System.Action<SceneInfo> opClose = null)
		{
			this.parent = parent;
			this.loadMode = mode;
			this.sceneName = sceneName;
			this.arguments = arguments;
			this.OnOpen += onOpen;
			this.OnClose += opClose; 

		}
		public SceneInfo (string sceneName, LoadSceneMode mode, Dictionary<string,object> arguments = null, System.Action<SceneInfo> onOpen = null, System.Action<SceneInfo> onClose = null)
			: this(null,sceneName,mode,arguments,onOpen,onClose)
		{
			
		}

		public SceneInfo (Scene scene)
		{
			this.sceneName = scene.name;
			this.loadMode = LoadSceneMode.Single;
			SceneData = new CachedProperty<Scene> (
				(s, f) => {
					s (scene);
					return AsyncTaskInternal.Complete ();
				}
			);
		}

		public void SetAsyncOperaction(AsyncOperation op ){
			this.asycOperation = op;
		}

		public bool IsReady(){
			if(iSceneController == null) return  false;
			return iSceneController.IsReady();

		}
		public void InvokeLoad(){
			iSceneController.OnLoad();
		}

		public void InvokeOpen(){
			OnOpen(this);
			iSceneController.OnOpen (Arguments);
			iSceneController.OnActive ();
			NotifyOnOpenChild(this);
		}
		public void InvokeClose(){
			
			OnClose(this);
			NotifyOnCloseChild(this);
		}
		protected void NotifyOnOpenChild (SceneInfo scene)
		{
			if (Parent != null) {
				Parent.NotifyOnOpenChild (scene);
				if(Parent.iSceneController != null)
					Parent.iSceneController.OnOpenChildScene(scene);

			}
		}

		protected void NotifyOnCloseChild (SceneInfo scene)
		{
			if (Parent != null) {
				Parent.NotifyOnCloseChild (scene);
				if(Parent.iSceneController != null)
					Parent.iSceneController.OnCloseChildScene(scene);
			}
		}

		public void ActiveScene(){
			
			EnableScene ();
			iSceneController.OnActive();
		}
		public void DeactiveScene(){
			iSceneController.OnDeactive();
			DisableScene ();
		}


		public void EnableScene ()
		{
			var roots = SceneData.Value.GetRootGameObjects ();
			foreach (var root  in roots) {
				root.SetActive (true);
			}
		}

		public void DisableScene ()
		{
			var roots = SceneData.Value.GetRootGameObjects ();
			foreach (var root  in roots) {
				root.SetActive (false);
			}
		}

		public T GetSceneController<T>() where T: class,ISceneController
		{
			return GetSceneController() as T;
		}

		public ISceneController GetSceneController ()
		{
			return iSceneController;
		}
		public T GetControllerInParant<T> () where T : class,ISceneController
		{
			SceneInfo t = this.Parent;
			while (t != null) {
				var controller = t.GetSceneController<T> ();
				if (controller is T) {
					return controller as T;
				}
				t = t.Parent;
			}
			return null;
		}	

		public void RegisterSceneController (ISceneController sceneController)
		{
			iSceneController = sceneController;
		}

		public void UnRegisterSceneController ()
		{
			iSceneController.OnDeactive ();
			iSceneController.OnClose ();	

		}


	}


}

