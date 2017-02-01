using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Text;
using UniFramework.Extension;

namespace UniFramework.Net
{
	public class Http
	{
		private const string CONTENTS_TYPE_JSON = "application/json";

		public Uri URI {
			get {
				return new Uri (webRequest.url);
			}
		}

		public event System.Action<Http> onSuccess;
		public event System.Action<Exception> onFail;

		private UnityWebRequest webRequest;

		public static Http Get (string uri, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Http http = new Http (UnityWebRequest.Get (uri));
			return http;
		}

		public static Http Get (string uri, Dictionary<string,string> fromfields, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Uri uriobject = new Uri (uri);
			uriobject = uriobject.ExtendQuery (fromfields);
			Http http = new Http (UnityWebRequest.Get (uriobject.AbsoluteUri), onSuccess, onFail);
			return http;
		}

		public static Http GetAssetBundle (string uri, uint crc = 0, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Http http = new Http (UnityWebRequest.GetAssetBundle (uri, crc), onSuccess, onFail);
			return http;
		}

		public static Http GetAssetBundle (string uri, uint version, uint crc, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Http http = new Http (UnityWebRequest.GetAssetBundle (uri, version, crc), onSuccess, onFail);
			return http;
		}

		public static Http GetAssetBundle (string uri, Hash128 hash, uint crc, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Http http = new Http (UnityWebRequest.GetAssetBundle (uri, hash, crc), onSuccess, onFail);
			return http;
		}

		public static Http PostJson (string uri, string jsonString, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			var webRequest = new UnityWebRequest (uri);
			UploadHandler uploader = new UploadHandlerRaw (System.Text.Encoding.UTF8.GetBytes (jsonString));
			uploader.contentType = CONTENTS_TYPE_JSON; 
			webRequest.uploadHandler = uploader;
			Http http = new Http (webRequest, onSuccess, onFail);
			return http;

		}

		public static Http Post (string uri, string postData, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Http http = new Http (UnityWebRequest.Post (uri, postData), onSuccess, onFail);
			return http;
		}

		public static Http Post (string uri, List<IMultipartFormSection> multipartFormSections, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Http http = new Http (UnityWebRequest.Post (uri, multipartFormSections), onSuccess, onFail);
			return http;
		}

		public static Http Post (string uri, Dictionary<string,string>  formfields, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Http http = new Http (UnityWebRequest.Post (uri, formfields), onSuccess, onFail);
			return http;
		}

		public static Http Put (string uri, string bodyData, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Http http = new Http (UnityWebRequest.Put (uri, bodyData), onSuccess, onFail);
			return http;
		}

		public static Http Put (string uri, byte[] bodyData, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Http http = new Http (UnityWebRequest.Put (uri, bodyData), onSuccess, onFail);
			return http;
		}

		public static Http Delete (string uri, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Http http = new Http (UnityWebRequest.Delete (uri), onSuccess, onFail);
			return http;
		}

		public static Http Head (string uri, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			Http http = new Http (UnityWebRequest.Head (uri), onSuccess, onFail);
			return http;
		}

		public Http (UnityWebRequest www, System.Action<Http> onSuccess = null, System.Action<Exception> onFail = null)
		{
			this.webRequest = www;
			this.onSuccess += onSuccess;
			this.onFail += onFail;

		}

		public AsyncTask Request (MonoBehaviour behaviour)
		{
			AsyncTaskInternal asyncResult = new AsyncTaskInternal (webRequest.url);

			behaviour.StartCoroutine (RequestCoroutine (asyncResult, onSuccess, onFail));

			return asyncResult;
		}

		private IEnumerator RequestCoroutine (AsyncTaskInternal result, System.Action<Http> onSuccess, System.Action<Exception> onFail)
		{

			AsyncOperation op = webRequest.Send ();

			while (!op.isDone) {
				if (result.IsAbort) {
					webRequest.Abort ();
				}
				result.SetProgress(op.progress);
				yield return null;

			}
			result.Done();
			if (webRequest.isError) {
				if (!string.IsNullOrEmpty (webRequest.error)) {
					if (onFail != null)
						onFail (new UnityException (webRequest.error));
				}
			} else {
				if (onSuccess != null)
					onSuccess (this);
			}
		}


		public UnityEngine.AssetBundle ToAssetBundle ()
		{
			UnityEngine.AssetBundle assetBundle = ((DownloadHandlerAssetBundle)webRequest.downloadHandler).assetBundle;
			return assetBundle;
		}

		public String ToText ()
		{
			return  webRequest.downloadHandler.text;
		}

		public T ParseJson<T> ()
		{
			return JsonUtility.FromJson<T> (ToText ());
		}

		public Texture ToTexture ()
		{
			return ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
		}

		public AudioClip ToAudioClip ()
		{
			return ((DownloadHandlerAudioClip)webRequest.downloadHandler).audioClip;
		}
	}
}

