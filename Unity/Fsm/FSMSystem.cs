using System.Collections.Generic;
using UnityEngine;
using System.Collections;
namespace UniFramework.Fsm
{
	
	public class FSMSystem : IFSMSystem
	{
		public MonoBehaviour Owner {
			get {
				if (owner != null)
					return owner;
				return ownerState.Owner;
			}
		}

		public IFSMState CurrentState { get { return currentState; } }

		public bool IsSubFsm {
			get {
				return ownerState != null;
			}
		}

		public bool IsPlay {
			get { return isPlay; }
		}

		public bool IsTransition {
			get {
				return isTransition;
			}
		}
		public IFSMState OwnerState {
			get {
				return ownerState;
			}

		}
		private IFSMState ownerState;
		private bool isPlay;
		private bool isTransition;
		private MonoBehaviour owner;
		private List<IFSMState> states;
		private IFSMState currentState;
		private IFSMState preState;

		public event System.Action<IFSMState,IFSMState > OnChangedState;

		private Dictionary<string, IFSMState> globalMap = new Dictionary<string, IFSMState> ();

		public FSMSystem (MonoBehaviour o)
		{
			states = new List<IFSMState> ();
			owner = o;
			isPlay = false;
			isTransition = false;
		}

		public FSMSystem (IFSMState s)
		{
			states = new List<IFSMState> ();
			ownerState = s;
			isPlay = false;
			isTransition = false;
		}

		public void Build ()
		{
			for (int i = 0; i < states.Count; i++) {
				states [i].Awake ();
			}
			Reset ();
		}

		public void Play (params object[]  param)
		{
			Dictionary<string, object> dic = new Dictionary<string,object> ();

			for (int i = 0; i < param.Length / 2; i++) {
				dic.Add ((string)param [i * 2], param [i * 2 + 1]);
			}
			Play (dic);
		}

		public void Play (IDictionary paramDic)
		{
			if (states.Count > 0) {
				isPlay = true;
				currentState.OnEnter (paramDic);
			} else {
				isPlay = false;
			}
			isTransition = false;
		}


		public void Stop ()
		{
			if (currentState != null) {
				currentState.OnExit ();
			}
			currentState = null;
			Reset ();
		}

		public void Pause ()
		{
			isPlay = false;
		}

		public void Resume ()
		{
			isPlay = true;
		}

		public void Reset ()
		{
			if (states.Count > 0) {
				currentState = states [0];
			}
			queueEventInfo.Clear ();
		}

		public void AddGlobalEvent (string eventName, IFSMState s)
		{
			if (globalMap.ContainsKey (eventName)) {
				globalMap [eventName] = s;
			} else {
				globalMap.Add (eventName, s);
			}
		}

		public void AddState (IFSMState s)
		{
			if (s == null) {
				Debug.LogError ("FSM ERROR: Null reference is not allowed");
			}
			foreach (FSMState state in states) {
				if (state.Equals (s)) {
					Debug.LogError ("FSM ERROR: Impossible to add state " + s.ToString () +
					" because state has already been added");
					return;
				}
			}
			if(s is FSMState){
				((FSMState)s).Fsm = this;
			}
			states.Add (s);
		}



		public void DeleteState (IFSMState s)
		{
			foreach (IFSMState state in states) {
				if (state.Equals (s)) {
					states.Remove (state);
					return;
				}
			}
			Debug.LogError ("FSM ERROR: Impossible to delete state " + s.ToString () +
			". It was not on the list of states");
		}

		public void Update ()
		{
			if (isPlay) {
				if (!IsTransition) {
					CurrentState.OnPreUpdate ();
					CurrentState.OnUpdate ();
				}
			}
		}

		public bool SendEventToSub (string eventName, params object[] param)
		{
			FSMState s = currentState as FSMState;
			if(s != null){
				if (s.subFsm != null) {
					return s.subFsm.SendEvent (eventName, param);
				}
			}

			return false;
		}

		public bool SendEvent (string eventName, params object[] param)
		{
			Dictionary<string, object> dic = new Dictionary<string,object> ();

			for (int i = 0; i < param.Length / 2; i++) {
				dic.Add ((string)param [i * 2], param [i * 2 + 1]);
			}
			return SendEvent (eventName, dic);

		}

		public bool SendEvent (string eventName, IDictionary paramDic)
		{
		
			if (!isPlay)
				return false;
			IFSMState s = null; 
			if (isTransition == true) {
				Debug.Log ("Post " + eventName);
				PostSendEvent (eventName, paramDic);
				return false;
			}

			if (globalMap.ContainsKey (eventName)) {
				s = globalMap [eventName];
			} else {
				s = currentState.GetOutputState (eventName) as FSMState;
			}
			foreach (FSMState state in states) {
				if (state.Equals (s)) {
					SwitchState (state, paramDic);
					while (queueEventInfo.Count != 0) {
						EventInfo info = queueEventInfo.Dequeue ();
						SendEvent (info.eventName, info.paramDic);
					}
					return true;
				}
			}
			return false;
		}

		public void GoToPreviousState ()
		{
			if (preState == null) {
				Debug.LogWarning ("No Pre State");
				return;
			}
			SwitchState (preState, null);
		}

		private void SwitchState (IFSMState to, IDictionary paramDic)
		{
			isTransition = true;
			currentState.OnExit ();
			preState = currentState;
			currentState = to;
			if (OnChangedState != null) {
				OnChangedState (preState, currentState);
			}
			isTransition = false;
			currentState.OnEnter (paramDic);

		}

		public void PostSendEvent (string eventName, params object[] param)
		{
			Dictionary<string, object> dic = new Dictionary<string,object> ();

			for (int i = 0; i < param.Length / 2; i++) {
				dic.Add ((string)param [i * 2], param [i * 2 + 1]);
			}
			queueEventInfo.Enqueue (new EventInfo (eventName, dic));
		}

		public void PostSendEvent (string eventName, IDictionary paramDic)
		{
			queueEventInfo.Enqueue (new EventInfo (eventName, paramDic));
		}

		public IFSMState FindFSMState (System.Type type)
		{
			return  states.Find (o => o.GetType () == type);
		}

		public struct EventInfo
		{
			public string eventName;
			public IDictionary paramDic;

			public EventInfo (string e, IDictionary dic)
			{
				eventName = e;
				paramDic = dic;
			}
			
		}

		public Queue <EventInfo> queueEventInfo = new Queue<EventInfo> ();


	}
}