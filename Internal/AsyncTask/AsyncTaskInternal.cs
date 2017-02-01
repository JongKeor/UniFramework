using System;
using System.Collections;


namespace UniFramework
{	
	internal class AsyncTaskInternal : AsyncTask
	{	
		private float progress =0;
		private bool isAbort = false;
		private System.Action onAbort;

		public AsyncTaskInternal (string name , System.Action onAbort = null)
		{
			this.name = name;
			this.onAbort = onAbort;
		}

		public static AsyncTaskInternal Complete(string name = ""){
			return new AsyncTaskInternal(name) {
				progress = 1f
			};
		}

		public void Done(){
			isAbort = false;
			this.progress = 1f;
		}

		public void SetProgress(float progress){
			
			this.progress = progress;
			if(this.progress == 1f){
				Done();
			}
		}

		public void SetOnAbort(System.Action onAbort )
		{
			this.onAbort = onAbort;
		}
		#region implemented abstract members of AsyncTaskInternal
		public override void Abort ()
		{
			isAbort = true;
			if(this.onAbort  != null){
				this.onAbort(); 	
			}
		}
		public override float Progress {
			get {
				return progress;
			}
		}

		public override bool IsAbort {
			get {
				return isAbort;
			}
		}
		#endregion
	}
}