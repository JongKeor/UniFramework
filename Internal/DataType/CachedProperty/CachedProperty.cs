using System;
using System.Collections;
using UnityEngine;
using UniFramework.Extension;


namespace UniFramework.Generic
{
	public enum CachedState
	{
		NotCached,
		Cached,
		Caching,
		Fail,
	}

	public class CachedProperty<T>
	{
		public T Value {
			get {
				if (State == CachedState.Cached)
					return t;
				else {
					return default(T);
				}
			}
		}
		public CachedState State {
			get {
				return state;
			}
		}
		public AsyncTask AsyncResult {
			get {
				return asyncResult;
			}
		}
		private T t;
		private CachedState state;
		private System.Exception exception;
		private AsyncTask asyncResult;

		public CachedProperty()
		{
			this.state = CachedState.NotCached;
		}
		public CachedProperty ( Func<Action<T>, Action<Exception>,AsyncTask> cacheProcess , System.Action<T> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Cache( cacheProcess,onSuccess , onFail);
		}
		public AsyncTask Cache( Func<Action<T>, Action<Exception>,AsyncTask> cacheProcess, System.Action<T> onSuccess = null, System.Action<Exception> onFail = null){
			state = CachedState.Caching;
			asyncResult = cacheProcess(
				value =>{ 
					Cached (value);
					if(onSuccess != null){
						onSuccess(value);
					}
				},
				ex => {
					Failed(ex);
					if(onFail != null){
						onFail(ex);
					}
				}
			);
			return asyncResult;
		}
		public void Abort(){
			asyncResult.Abort();
		}

		private void Failed (System.Exception exception)
		{
			this.state = CachedState.Fail;
			this.exception = exception;
		}

		private void Cached (T value)
		{
			this.state = CachedState.Cached;
			this.t = value;
		}

		public AsyncTask WaitForValueCreated(MonoBehaviour behaviour,  Action<T> onSuccess , Action<Exception> onFail )
		{
			if(State == CachedState.NotCached){
				onFail(new Exception("Cache is not started yet"));
				return AsyncTaskInternal.Complete();
			}
			return behaviour.StartCoroutineEx(WaitForValueCreated(onSuccess ,onFail));
		}

		private IEnumerator WaitForValueCreated(Action<T> onSuccess , Action<Exception> onFail  )
		{
			if(State == CachedState.Caching){
				yield return asyncResult;
			}
			if(State == CachedState.Cached){
				onSuccess(t);
			}
			else if(State == CachedState.Fail){
				onFail(this.exception);
			}

		}

	}
}