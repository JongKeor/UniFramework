using System.Collections.Generic;
using UnityEngine;
using System.Collections;
namespace UniFramework.Fsm
{

    public class FSMSystem
    {
        public MonoBehaviour Owner
        {
            get
            {
                if (owner != null)
                    return owner;
                return ownerState.Owner;
            }
        }

        public FSMState CurrentState { get { return currentState; } }

        public bool IsSubFsm
        {
            get
            {
                return ownerState != null;
            }
        }

        public bool IsPlay
        {
            get { return isPlay; }
        }

        public bool IsTransition
        {
            get
            {
                return isTransition;
            }
        }
        public FSMState OwnerState
        {
            get
            {
                return ownerState;
            }

        }
        private FSMState ownerState;
        private bool isPlay;
        private bool isTransition;
        private MonoBehaviour owner;
        private List<FSMState> states;
        private FSMState currentState;
        private FSMState preState;

        public event System.Action<FSMState, FSMState> OnChangedState;

        private Dictionary<string, FSMState> globalMap = new Dictionary<string, FSMState>();

        public FSMSystem(MonoBehaviour o)
        {
            states = new List<FSMState>();
            owner = o;
            isPlay = false;
            isTransition = false;
        }

        public FSMSystem(FSMState s)
        {
            states = new List<FSMState>();
            ownerState = s;
            isPlay = false;
            isTransition = false;
        }

        public void Build()
        {
            for (int i = 0; i < states.Count; i++)
            {
                states[i].Awake();
            }
            Reset();
        }

        public void Play(params object[] param)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            for (int i = 0; i < param.Length / 2; i++)
            {
                dic.Add((string)param[i * 2], param[i * 2 + 1]);
            }
            Play(dic);
        }

        public void Play(IDictionary paramDic)
        {
            if (states.Count > 0)
            {
                isPlay = true;
                currentState.OnEnter(paramDic);
            }
            else
            {
                isPlay = false;
            }
            isTransition = false;
        }


        public void Stop()
        {
            if (currentState != null)
            {
                currentState.OnExit();
            }
            currentState = null;
            Reset();
        }

        public void Pause()
        {
            isPlay = false;
            if (currentState != null)
            {
                currentState.OnPause();
            }
        }

        public void Resume()
        {
            isPlay = true;
            if (currentState != null)
            {
                currentState.OnResume();
            }
        }

        public void Reset()
        {
            if (states.Count > 0)
            {
                currentState = states[0];
            }
            // queueEventInfo.Clear ();
        }

        public void AddGlobalEvent(string eventName, FSMState s)
        {
            if (globalMap.ContainsKey(eventName))
            {
                globalMap[eventName] = s;
            }
            else
            {
                globalMap.Add(eventName, s);
            }
        }

        public void AddState(FSMState s, string eventName = "")
        {
            if (s == null)
            {
                Debug.LogError("FSM ERROR: Null reference is not allowed");
            }
            foreach (FSMState state in states)
            {
                if (state.Equals(s))
                {
                    Debug.LogError("FSM ERROR: Impossible to add state " + s.ToString() +
                    " because state has already been added");
                    return;
                }
            }
            s.Fsm = this;
            states.Add(s);
            if (!string.IsNullOrEmpty(eventName))
            {
                AddGlobalEvent(eventName, s);
            }
        }

        public void DeleteState(FSMState s)
        {
            foreach (FSMState state in states)
            {
                if (state.Equals(s))
                {
                    states.Remove(state);
                    return;
                }
            }
            Debug.LogError("FSM ERROR: Impossible to delete state " + s.ToString() +
            ". It was not on the list of states");
        }

        public void Update()
        {
            if (isPlay)
            {
                if (!IsTransition)
                {
                    CurrentState.OnDetermine();
                }
                if (!IsTransition)
                {
                    CurrentState.OnUpdate();
                }
            }
        }

        public bool SendEventToSub(string eventName, params object[] param)
        {
            FSMState s = currentState as FSMState;
            if (s != null)
            {
                if (s.subFsm != null)
                {
                    return s.subFsm.SendEvent(eventName, param);
                }
            }

            return false;
        }

        public bool SendEvent(string eventName, params object[] param)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            for (int i = 0; i < param.Length / 2; i++)
            {
                dic.Add((string)param[i * 2], param[i * 2 + 1]);
            }
            return SendEvent(eventName, dic);

        }

        public bool SendEvent(string eventName, IDictionary paramDic)
        {

            if (!isPlay)
                return false;
            FSMState s = null;
            if (isTransition == true)
            {
                Debug.Log("Post " + eventName);
                // PostSendEvent (eventName, paramDic);
                return false;
            }

            if (globalMap.ContainsKey(eventName))
            {
                s = globalMap[eventName];
            }
            else
            {
                s = currentState.GetOutputState(eventName) as FSMState;
            }
            foreach (FSMState state in states)
            {
                if (state.Equals(s))
                {
                    if (this.currentState != s || s.allowSelfTransition)
                    {
                        SwitchState(state, paramDic);
                    }
                    // while (queueEventInfo.Count != 0) {
                    // 	EventInfo info = queueEventInfo.Dequeue ();
                    // 	SendEvent (info.eventName, info.paramDic);
                    // }
                    return true;
                }
            }
            return false;
        }

        public void GoToPreviousState()
        {
            if (preState == null)
            {
                Debug.LogWarning("No Pre State");
                return;
            }
            SwitchState(preState, null);
        }

        private void SwitchState(FSMState to, IDictionary paramDic)
        {
            isTransition = true;
            currentState.OnExit();
            preState = currentState;
            currentState = to;
            if (OnChangedState != null)
            {
                OnChangedState(preState, currentState);
            }
            isTransition = false;
            currentState.OnEnter(paramDic);

        }

        // public void PostSendEvent (string eventName, params object[] param)
        // {
        // 	Dictionary<string, object> dic = new Dictionary<string,object> ();

        // 	for (int i = 0; i < param.Length / 2; i++) {
        // 		dic.Add ((string)param [i * 2], param [i * 2 + 1]);
        // 	}
        // 	queueEventInfo.Enqueue (new EventInfo (eventName, dic));
        // }

        // public void PostSendEvent (string eventName, IDictionary paramDic)
        // {
        // 	queueEventInfo.Enqueue (new EventInfo (eventName, paramDic));
        // }

        public FSMState FindFSMState(System.Type type)
        {
            return states.Find(o => o.GetType() == type);
        }

        // public struct EventInfo
        // {
        // 	public string eventName;
        // 	public IDictionary paramDic;

        // 	public EventInfo (string e, IDictionary dic)
        // 	{
        // 		eventName = e;
        // 		paramDic = dic;
        // 	}

        // }

        // public Queue <EventInfo> queueEventInfo = new Queue<EventInfo> ();
    }


    public abstract class FSMState
    {
        public FSMSystem Fsm;

        public MonoBehaviour Owner
        {
            get
            {
                return Fsm.Owner;
            }
        }

        public FSMSystem subFsm;
        public bool allowSelfTransition = false;
        protected Dictionary<string, FSMState> map = new Dictionary<string, FSMState>();

        public FSMState()
        {

        }
        public FSMState(bool allowSelfTransition)
        {
            this.allowSelfTransition = allowSelfTransition;
        }
        public bool SendEvent(string eventName, params object[] param)
        {
			return this.Fsm.SendEvent(eventName,param);
        }

        public void AddEvent(string trans, FSMState s)
        {
            if (map.ContainsKey(trans))
            {
                Debug.LogError("FSMState ERROR: State " + this.ToString() + " already has transition " + trans.ToString() +
                    "Impossible to assign to another state");
                return;
            }
            map.Add(trans, s);
        }

        public void DeleteEvent(string trans)
        {
            if (map.ContainsKey(trans))
            {
                map.Remove(trans);
                return;
            }
            Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + this.ToString() +
                " was not on the state's transition list");
        }

        public FSMState GetOutputState(string trans)
        {
            if (map.ContainsKey(trans))
            {
                return map[trans];
            }
            return null;
        }

        public virtual void Awake()
        {
            if (subFsm != null)
            {
                subFsm.Build();
            }
        }

        public virtual void OnEnter(IDictionary paramDic)
        {
            if (subFsm != null)
            {
                subFsm.Play();
            }
        }

        public virtual void OnExit()
        {
            if (subFsm != null)
            {
                subFsm.Stop();
            }
        }

        public virtual void OnPause()
        {

        }
        public virtual void OnResume()
        {
        }

        public virtual void OnDetermine()
        {

        }
        public virtual void OnUpdate()
        {
            if (subFsm != null)
            {
                subFsm.Update();
            }
        }
    }
}