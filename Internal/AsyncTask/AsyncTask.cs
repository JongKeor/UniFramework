using System;
using System.Collections;
using UnityEngine;

namespace UniFramework
{
	internal interface IAsyncTask :IEnumerator
	{
		bool IsDone {
			get ;
		}


		float Progress {
			get;
		}

		string Name {
			get;
		}

		bool IsAbort {
			get ;
		}

		void Abort ();


	}

	public abstract class  AsyncTask : CustomYieldInstruction
	{	
		public bool IsDone {
			get {
				if(IsAbort) return true;
				return Progress == 1f;
			}
		}

		#region implemented abstract members of CustomYieldInstruction

		public override bool keepWaiting {
			get {
				 return IsDone == false;
			}
		}

		#endregion

		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}

		protected string name;



		public abstract float Progress {
			get;
		}
		public abstract bool IsAbort {
			get ;
		}

		public abstract void Abort ();

	
	}

}

