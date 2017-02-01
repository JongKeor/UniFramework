using System;
using UnityEngine;

namespace UniFramework.Fsm
{
	public abstract class FsmComponent : MonoBehaviour
	{
		public FSMSystem fsm;
		public bool resetOnDisable = true;

		protected virtual void Awake ()
		{
			fsm = GenreateFSM ();
			fsm.Build ();
		}

		protected virtual void Start ()
		{

		}

		protected virtual void OnEnable ()
		{
			if(fsm != null)fsm.Play ();
		}

		protected virtual void OnDisable ()
		{
			if (resetOnDisable){
				if(fsm != null)fsm.Stop ();
			}
		}

		protected virtual void OnDestroy ()
		{
			
		}

		protected virtual void  Update ()
		{
		
			if (fsm.IsPlay) {
				fsm.Update ( );
			}
		}

		protected abstract  FSMSystem GenreateFSM ();
	}
}


