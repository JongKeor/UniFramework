using System;
using UnityEngine;
using System.Collections;
using UniFramework.Fsm;


namespace UniFramework.Fsm
{
	public interface IFSMSystem
	{
		MonoBehaviour Owner {
			get ;
		}

		bool IsSubFsm {
			get;
		}

		bool IsPlay {
			get;
		}

		bool IsTransition {
			get ;
		}

		event System.Action<IFSMState,IFSMState > OnChangedState;

		void AddGlobalEvent (string eventName, IFSMState s);

		void AddState (IFSMState s);

		void DeleteState (IFSMState s);

		bool SendEventToSub (string eventName, params object[] param);


		void Build ();

		void Play (params object[]  param);

		void Play (IDictionary paramDic);

		void Stop ();

		void Pause ();

		void Resume ();

		void Reset ();

		void Update ();


		bool SendEvent (string eventName, params object[] param);

		bool SendEvent (string eventName, IDictionary paramDic);

		void GoToPreviousState ();

		void PostSendEvent (string eventName, params object[] param);

		void PostSendEvent (string eventName, IDictionary paramDic);
	}
}



