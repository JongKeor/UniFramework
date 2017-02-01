using UnityEngine;
using System.Collections;
using UniFramework.Extension.Helper;

namespace UniFramework.Extension
{

	public static class MonoBehaviourGeneralExtension
	{
		public static T GetOrAddComponent<T> (this Component child) where T: Component
		{
			return child.gameObject.GetOrAddComponent<T> ();
		}

		public static Component GetOrAddComponent (this Component child, System.Type  type)
		{
			return child.gameObject.GetOrAddComponent (type);
		}
	}

	public static class MonoBehaviourInvokeExtension
	{
		public static void EInvoke (this MonoBehaviour self, float delay, System.Action callback)
		{
			EInvokeRepeating (self, string.Empty, delay, 0, callback);
		}

		public static void EInvoke (this MonoBehaviour self, string methodKey, float delay, System.Action callback)
		{
			EInvokeRepeating (self, methodKey, delay, 0, callback);
		}

		public static void EInvokeRepeating (this MonoBehaviour self, float delay, float repeatTime, System.Action callback)
		{
			EInvokeRepeating (self, string.Empty, delay, repeatTime, callback);
		}

		public static void EInvokeRepeating (this  MonoBehaviour self, string methodKey, float delay, float repeatTime, System.Action callback)
		{
			InvokeHelper helper = self.GetComponent<InvokeHelper> ();
			if (helper == null) {
				helper = self.gameObject.AddComponent<InvokeHelper> ();
			}
			if (helper.enabled == false) {
				helper.enabled = true;
			}
			helper.HelperInvokeRepeating (self, methodKey, delay, repeatTime, callback);
		}

		public static bool IsEInvoking (this MonoBehaviour self, string methodKey)
		{
			InvokeHelper helper = self.GetComponent<InvokeHelper> ();
			if (helper == null)
				return false;
			return helper.IsHelperInvoking (self, methodKey);
		}

		public static void CancelEInvoke (this MonoBehaviour self)
		{
			InvokeHelper helper = self.GetComponent<InvokeHelper> ();
			if (helper != null) {
				helper.CancelHelperInvoke (self);
			}
		}

		public static void CancelEInvoke (this MonoBehaviour self, string methodKey)
		{
			InvokeHelper helper = self.GetComponent<InvokeHelper> ();
			if (helper != null) {
				helper.CancelHelperInvoke (self, methodKey);
			}
		}
	}


	public static class MonoBehaviourCoroutineExteion{
		
		public static AsyncTask StartCoroutineEx(this MonoBehaviour self, IEnumerator iEnumerator)
		{
			AsyncTaskInternal t = new AsyncTaskInternal("coroutine");
			self.StartCoroutine(GenericRoutine(t, iEnumerator));
			return t;
		}
		private static IEnumerator GenericRoutine(AsyncTaskInternal t ,IEnumerator iEnumerator)
		{
			while(iEnumerator.MoveNext() )
			{
				if(t.IsAbort){
					yield break;
				}
				yield return null;
			}
			t.Done();
		}


	}

}


