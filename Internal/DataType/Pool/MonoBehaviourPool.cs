using System;
using UnityEngine;

namespace UniFramework.Generic
{
	public class MonoBehaviourPool<T> where T : MonoBehaviour
	{
		private readonly Pool<T> pool;


		public bool IsObjectAvailable {
			get {
				return pool.IsObjectAvailable;
			}
		}


		public MonoBehaviourPool (T prefab, int initialCount, Action<T> setToSleep, Action<T> wakeUp, Func<T,bool> checkSleep)
		{
			pool = new Pool<T> (
				initialCount, 
				() => (T)UnityEngine.Object.Instantiate (prefab), 
				UnityEngine.Object.Destroy, 
				setToSleep,
				wakeUp,
				checkSleep);
		}


		public T GetNewObject ()
		{
			return pool.GetNewObject ();
		}


		public void ReleaseObject (T obj)
		{
			pool.Release (obj);
		}

		public void IncCapacity (int increment)
		{
			pool.IncCapacity (increment);
		}


		public void DecCapacity (int decrement)
		{
			pool.DecCapacity (decrement);
		}
	}
}
