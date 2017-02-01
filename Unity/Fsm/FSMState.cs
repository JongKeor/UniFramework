using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace UniFramework.Fsm
{
	public abstract class FSMState : IFSMState
	{
		public IFSMSystem Fsm;

		public MonoBehaviour Owner {
			get {
				return Fsm.Owner;
			}
		}

		public IFSMSystem subFsm;
		protected Dictionary<string, IFSMState> map = new Dictionary<string, IFSMState> ();

		public void AddEvent (string trans, IFSMState s)
		{
			if (map.ContainsKey (trans)) {
				Debug.LogError ("FSMState ERROR: State " + this.ToString () + " already has transition " + trans.ToString () +
					"Impossible to assign to another state");
				return;
			}
			map.Add (trans, s);
		}

		public void DeleteEvent (string trans)
		{
			if (map.ContainsKey (trans)) {
				map.Remove (trans);
				return;
			}
			Debug.LogError ("FSMState ERROR: Transition " + trans.ToString () + " passed to " + this.ToString () +
				" was not on the state's transition list");
		}

		public IFSMState GetOutputState (string trans)
		{
			if (map.ContainsKey (trans)) {
				return map [trans];
			}
			return null;
		}

		public virtual void Awake ()
		{
			if (subFsm != null) {
				subFsm.Build ();
			}
		}

		public virtual void OnEnter (IDictionary paramDic)
		{
			if (subFsm != null) {
				subFsm.Play ();
			}
		}

		public virtual void OnExit ()
		{ 
			if (subFsm != null) {
				subFsm.Stop ();
			}
		}

		public virtual void OnPreUpdate ()
		{

		}
		public virtual void OnUpdate ()
		{
			if (subFsm != null) {
				subFsm.Update ();
			}
		}
	}

}

