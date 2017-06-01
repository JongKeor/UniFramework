using System;
using System.Collections;
using UnityEngine;

namespace UniFramework
{
	

	public abstract class  AsyncTask : CustomYieldInstruction
	{	
		public bool IsDone {
			get {
				if(IsAbort) return true;
				return Progress == 1f;
			}
		}

		

		public override bool keepWaiting {
			get {
				 return IsDone == false;
			}
		}

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

