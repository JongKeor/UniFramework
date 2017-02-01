using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using UniFramework.Generic;
using UniFramework.Extension;

namespace UniFramework.Net
{
	public sealed class CachedAssetBundle
	{
		public CachedProperty<AssetBundle> AssetBundle;
		public int ReferencedCount;

		public CachedAssetBundle (Func<Action<AssetBundle>, Action<Exception>,AsyncTask> cacheProcess
			, System.Action<AssetBundle> onSuccess = null
			, System.Action<Exception> onFail = null)
		{
			AssetBundle = new CachedProperty<UnityEngine.AssetBundle> (cacheProcess, onSuccess, onFail);
			ReferencedCount++;
		}
	}

	public sealed class AssetBundleManager : SingletonMonoBehaviour<MonoBehaviour>
	{
		public System.Uri BaseUri {
			get {
				return baseUri;
			}
			set {
				baseUri = value;
			}
		}

		public string[] ActiveVariants {
			get {
				return activeVariants;
			}
		}

		#if UNITY_EDITOR
		public bool IsSimulate = true;
		#endif

		[SerializeField]
		private string[] activeVariants = { };

		private AssetBundleManifest manifest;
		private System.Uri baseUri;
		private Dictionary<string,CachedAssetBundle> cachedAssetBundles = new Dictionary<string, CachedAssetBundle> ();
		private Dictionary<string, string[]> cachedDependencies = new Dictionary<string, string[]> ();
		private List<KeyValuePair<Http,AsyncTaskInternal>> downloadingTask = new List<KeyValuePair<Http,AsyncTaskInternal>> ();


		public event System.Action OnStartedDownloading = delegate {};
		public event System.Action<AsyncTask,int,int> OnProgressDownloading = delegate {};
		public event System.Action OnEndedDownloading = delegate {};



		public AsyncTask LoadAssetBundleManifest (string baseUri, System.Action<AssetBundleManifest> onSuccess = null, System.Action<System.Exception> onFail = null)
		{
			#if UNITY_EDITOR
			if (IsSimulate) {
				return AsyncTaskInternal.Complete ();
			}
			#endif


			this.baseUri = new Uri (baseUri);
			string manifestAssetName = new System.IO.DirectoryInfo (this.baseUri.AbsolutePath).Name;
			AsyncTaskInternal result = new AsyncTaskInternal (manifestAssetName);

			var task = LoadAssetBundleInternal (manifestAssetName, false, (bundle) => {
				result.SetProgress (0.5f);
				var innerTask = this.StartCoroutineEx (LoadAssetCoroutine<AssetBundleManifest> (bundle, "AssetBundleManifest",
					                (manifest) => {
						result.Done ();
						this.manifest = manifest;
						if (onSuccess != null)
							onSuccess (manifest);
					}, 
					                (ex) => {
						result.Done ();
						if (onFail != null)
							onFail (ex);
					}));
				result.SetOnAbort (() => {
					innerTask.Abort ();
					if (onFail != null)
						onFail (new Exception ("Aborted"));
				});
			}
				, (ex) => {
				result.Done ();
				if (onFail != null)
					onFail (ex);
			});

			result.SetOnAbort (() => {
				task.Abort ();
			});
			return result;
		}

		public AsyncTask LoadAsset<T> (string assetBundleName, string assetName, System.Action<T> onSuccess = null, System.Action<System.Exception> onFail = null)where T : UnityEngine.Object
		{
			#if UNITY_EDITOR
			if (IsSimulate) {
				string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (assetBundleName, assetName);
				if (assetPaths.Length == 0) {
					if (onFail != null)
						onFail (new Exception ("There is no asset with name \"" + assetName + "\" in " + assetBundleName));
					return AsyncTaskInternal.Complete ();
				}
				T target = UnityEditor.AssetDatabase.LoadMainAssetAtPath (assetPaths [0]) as T;
				if (onSuccess != null)
					onSuccess (target);
				return AsyncTaskInternal.Complete ();
			}
			#endif

			AsyncTaskInternal result = new AsyncTaskInternal (assetName);
			var task = LoadAssetBundle (assetBundleName, (bundle) => {
				result.SetProgress (0.5f);
				var innerTask = this.StartCoroutineEx (LoadAssetCoroutine<T> (bundle, assetName, (asset) => {
					result.Done ();
					if (onSuccess != null)
						onSuccess (asset);
				}, (ex) => {
					result.Done ();
					if (onFail != null)
						onFail (ex);
				}));
				result.SetOnAbort (
					() => {
						innerTask.Abort ();
						if (onFail != null)
							onFail (new Exception ("Aborted"));
					}
				);

			}, (ex) => {
				result.Done ();
				if (onFail != null)
					onFail (ex);
			});
			result.SetOnAbort (() => {
				task.Abort ();
			});
			return result;
		}

		public AsyncTask LoadScene (string assetBundleName, string levelName, System.Action<string> onSuccess = null, System.Action<System.Exception> onFail = null)
		{
			var task = LoadAssetBundle (assetBundleName, (bundle) => {
				string[] paths = bundle.GetAllScenePaths ();
				string path = System.Array.Find<string> (paths, o => System.IO.Path.GetFileNameWithoutExtension (o).Equals (levelName));
				if (onSuccess != null)
					onSuccess (path);
			}, (ex) => {
				if (onFail != null)
					onFail (ex);
			});
			return task;
		}

		public AsyncTask LoadAssetBundle (string assetBundleName, System.Action<UnityEngine.AssetBundle> onSuccess = null, System.Action<System.Exception> onFail = null)
		{
			assetBundleName = RemapVariantName (assetBundleName);
			string[] dependencies = null;
			if (!this.cachedDependencies.TryGetValue (assetBundleName, out dependencies)) {
				dependencies = CachingDependencies (assetBundleName);
				this.cachedDependencies.Add (assetBundleName, dependencies);
			}
			AsyncTaskInternal result = new AsyncTaskInternal ("LoadAssetBundle " + assetBundleName);

			var task = this.StartCoroutineEx (LoadAssetBundleAll (dependencies, 
				           () => {
					result.SetProgress (0.5f);
					var innerTask = LoadAssetBundleInternal (assetBundleName, false, 
						                (bundle) => {
							result.Done ();
							if (onSuccess != null)
								onSuccess (bundle);
						}, 
						                (ex) => {
							result.Done ();
							if (onFail != null)
								onFail (ex);
						});
					result.SetOnAbort (() => {
						innerTask.Abort ();
					});
				},
				           (ex) => {
					result.Done ();
					if (onFail != null)
						onFail (ex);
				}));

			result.SetOnAbort (
				() => {
					task.Abort ();
					if (onFail != null)
						onFail (new Exception ("Aborted"));
					
				}
			);

			return result;
		}

		private AsyncTask LoadAssetBundleInternal (string assetBundleName, bool isManifest, System.Action<UnityEngine.AssetBundle> onSuccess = null, System.Action<System.Exception> onFail = null)
		{
			CachedAssetBundle ret = null;
			if (cachedAssetBundles.TryGetValue (assetBundleName, out ret)) {
				if (ret.AssetBundle.State == CachedState.Cached) {
					onSuccess (ret.AssetBundle.Value);
					return AsyncTaskInternal.Complete ();
				} else {
					var task = ret.AssetBundle.WaitForValueCreated (
						           this,
						           onSuccess,
						           onFail
					           );
					return task;
				}
			} else {
				CachedAssetBundle cachedAssetBundle = new CachedAssetBundle (
					                                      (s, f) => {
						string assetbundleUri = System.IO.Path.Combine (BaseUri.AbsoluteUri, assetBundleName);
						AsyncTaskInternal downloading = new AsyncTaskInternal (assetbundleUri);
						Http http = null;
						if (!isManifest) {
							http = Http.GetAssetBundle (assetbundleUri, manifest.GetAssetBundleHash (assetBundleName), 0,
								(www) => {
									AssetBundle assetBundle = www.ToAssetBundle ();
									s (assetBundle);
								},
								f
							);
						} else {
							http = Http.GetAssetBundle (assetbundleUri, 0, (www) => {
								AssetBundle assetBundle = www.ToAssetBundle ();
								s (assetBundle);
							}, f);
						}
						if (downloadingTask.Count == 0) {
							StartCoroutine (LoadAssetBundleQueue ());
						}
						downloadingTask.Add (new KeyValuePair<Http, AsyncTaskInternal> (http, downloading));
						return downloading;
					}, 
					                                      onSuccess,
					                                      (ex) => {
						cachedAssetBundles.Remove (assetBundleName);
					}
				                                      );
				cachedAssetBundles.Add (assetBundleName, cachedAssetBundle);
				return cachedAssetBundle.AssetBundle.AsyncResult;
			}
		}

		private IEnumerator LoadAssetBundleQueue ()
		{
			yield return null;
			OnStartedDownloading ();
			int compelete = 0;
			while (downloadingTask.Count != 0) {
				AsyncTask result = downloadingTask [0].Key.Request (this);
				OnProgressDownloading (result, compelete, compelete + downloadingTask.Count);
				while (!result.IsDone) {
					if (downloadingTask [0].Value.IsAbort) {
						result.Abort ();	
					} else {
						downloadingTask [0].Value.SetProgress (result.Progress);
					}
					yield return null;
				}
				downloadingTask [0].Value.Done ();
				downloadingTask.RemoveAt (0);
				compelete++;
			}
			OnEndedDownloading ();
		}

		private IEnumerator LoadAssetCoroutine<T> (UnityEngine.AssetBundle bundle, string assetName, System.Action<T> onSuccess, System.Action<System.Exception > onFail) where T : UnityEngine.Object
		{
			AssetBundleRequest request = bundle.LoadAssetAsync<T> (assetName);
			yield return request;
			T obj = request.asset as T;
			if (obj != null) {
				if (onSuccess != null)
					onSuccess (obj);
			} else {
				if (onFail != null)
					onFail (new Exception ("There is no asset with name \"" + assetName + "\" in " + bundle.name));
			}
		}

		private IEnumerator LoadAssetBundleAll (string[] assetBundleNames, System.Action onSuccess = null, System.Action<System.Exception> onFail = null)
		{
			for (int i = 0; i < assetBundleNames.Length; i++) {
				
				Exception exception = null;
				AsyncTask task = LoadAssetBundleInternal (assetBundleNames [i], false,
					                 (bundle) => {
				
					}
					, 
					                 (ex) => {
						exception = new Exception ("Fail to Download AssetBundle ", ex);
					}
				                 );
				yield return task;
				if (exception != null) {
					if (onFail != null)
						onFail (exception);
				}
			}
			if (onSuccess != null)
				onSuccess ();
		}

		public void UnloadAssetBundle (string assetBundleName)
		{
			assetBundleName = RemapVariantName (assetBundleName);
			#if UNITY_EDITOR
			if (IsSimulate) {

				return;
			}
			#endif
			UnloadAssetBundleInternal (assetBundleName);
			UnloadDependencies (assetBundleName);
		}

		private void UnloadDependencies (string assetBundleName)
		{	
			string[] dependencies = null;
			if (!cachedDependencies.TryGetValue (assetBundleName, out dependencies))
				return;
			foreach (var dependency in dependencies) {
				UnloadAssetBundleInternal (dependency);
			}

			cachedDependencies.Remove (assetBundleName);
		}

		private void UnloadAssetBundleInternal (string assetBundleName)
		{	
			CachedAssetBundle ret = null;
			if (cachedAssetBundles.TryGetValue (assetBundleName, out ret)) {
				if (ret.AssetBundle.State == CachedState.Cached) {
					ret.ReferencedCount--;
					if (ret.ReferencedCount == 0) {
						ret.AssetBundle.Value.Unload (true);
					}
				} else if (ret.AssetBundle.State == CachedState.Caching) {
					ret.ReferencedCount--;
					if (ret.ReferencedCount == 0) {
						ret.AssetBundle.Abort ();
					}
				}
				if (ret.ReferencedCount == 0) {
					cachedAssetBundles.Remove (assetBundleName);
				}
			}
		}

		private string[] CachingDependencies (string assetBundleName)
		{
			if (manifest == null) {
				Debug.LogError ("There is no manifest on " + this.BaseUri.AbsoluteUri);
				return null;
			}
			string[] dependencies = manifest.GetAllDependencies (assetBundleName);
			if (dependencies.Length == 0)
				return null;

			for (int i = 0; i < dependencies.Length; i++)
				dependencies [i] = RemapVariantName (dependencies [i]);

			return dependencies;
		}

		private string RemapVariantName (string assetBundleName)
		{
			string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant ();
			string[] split = assetBundleName.Split ('.');
			int bestFit = int.MaxValue;
			int bestFitIndex = -1;
			for (int i = 0; i < bundlesWithVariant.Length; i++) {
				string[] curSplit = bundlesWithVariant [i].Split ('.');
				if (curSplit [0] != split [0])
					continue;

				int found = System.Array.IndexOf (this.ActiveVariants, curSplit [1]);
				if (found == -1)
					found = int.MaxValue - 1;

				if (found < bestFit) {
					bestFit = found;
					bestFitIndex = i;
				}
			}

			if (bestFit == int.MaxValue - 1) {
				Debug.LogWarning ("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant [bestFitIndex]);
			}

			if (bestFitIndex != -1) {
				return bundlesWithVariant [bestFitIndex];
			} else {
				return assetBundleName;
			}
		}
	}
}
