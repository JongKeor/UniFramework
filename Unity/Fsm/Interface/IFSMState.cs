using System;
using UnityEngine;
using System.Collections;

namespace UniFramework.Fsm
{
	public interface IFSMState
	{
		MonoBehaviour Owner {
			get;
		}

		void AddEvent (string trans, IFSMState s);

		void DeleteEvent (string trans);

		IFSMState GetOutputState (string trans);

		void Awake ();

		void OnEnter (IDictionary paramDic);

		void OnExit ();

		void OnPreUpdate ();

		void OnUpdate ();
	}
}
