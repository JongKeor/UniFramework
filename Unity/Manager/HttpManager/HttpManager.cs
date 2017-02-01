using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Generic;

namespace UniFramework.Net
{
	public class HttpManager : SingletonMonoBehaviour<HttpManager>
	{
		public List<AsyncTask> downloadingTask = new List<AsyncTask> ();

		public event System.Action OnStartedDownloading = delegate {};
		public event System.Action OnEndedDownloading = delegate {};


		public AsyncTask Request (Http http, System.Action<Http> onSuccess, System.Action<System.Exception>  onFail)
		{

			var asyncResult = http.Request (GlobalMonoBehaviour.Instance);
			http.onSuccess += (www) => {
				downloadingTask.Remove (asyncResult);
				if (downloadingTask.Count == 0)
					OnEndedDownloading ();
				if (onSuccess != null)
					onSuccess (www);
			};
			http.onFail += (ex) => {
				downloadingTask.Remove (asyncResult);
				if (downloadingTask.Count == 0)
					OnEndedDownloading ();
				if (onFail != null)
					onFail (ex);
			};

			downloadingTask.Add (asyncResult);
			if (downloadingTask.Count == 1) {
				OnStartedDownloading ();
			}
			return asyncResult;
		}

		public AsyncTask PostJson<T> (string uri, T json, System.Action<Http> onSuccess, System.Action<System.Exception>  onFail)
		{
			return PostJson (uri, JsonUtility.ToJson (json), onSuccess, onFail);
		}

		public AsyncTask PostJson (string uri, string json, System.Action<Http> onSuccess, System.Action<System.Exception>  onFail)
		{
			return Request (Http.PostJson (uri, json), onSuccess, onFail);
		}

		public AsyncTask Post (string uri, Dictionary<string,string>  formfields, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
		
			return Request (Http.Post (uri, formfields), onSuccess, onFail);
		}

		public AsyncTask Get (string uri, System.Action<Http> onSuccess, System.Action<System.Exception>  onFail)
		{
			return Request (Http.Get (uri), onSuccess, onFail);
		}

		public AsyncTask Get (string uri, Dictionary<string,string> query, System.Action<Http> onSuccess, System.Action<System.Exception>  onFail)
		{
		
			return Request (Http.Get (uri, query), onSuccess, onFail);
		}

	}
}


