using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniFramework.Generic
{
	public class Pool <T> where T : class
	{
		private readonly Func<T> create;
		private readonly Action<T> kill;
		private readonly Action<T> setToSleep;
		private readonly Action<T> wakeUp;
		private readonly List<T> poolObjects;
		private readonly Func<T,bool> checkSleep;


		public int Capacity {
			get { return poolObjects.Count; }
		}

		public bool IsObjectAvailable {
			get { return FindSleepObject () != null; }
		}


		public Pool (int initialCount, Func<T> create, Action<T> kill, Action<T> setToSleep, Action<T> wakeUp, Func<T,bool> checkSleep)
		{
			this.create = create;
			this.kill = kill;
			this.setToSleep = setToSleep;
			this.wakeUp = wakeUp;
			this.checkSleep = checkSleep;
			poolObjects = new List<T> ();
			Create (initialCount);
		}


		public T GetNewObject ()
		{
			if (IsObjectAvailable) {
				var obj = FindSleepObject ();
				wakeUp (obj);
			}
			throw new InvalidOperationException ("No items in pool");
		}

		private T FindSleepObject ()
		{
			return  poolObjects.Find (o => checkSleep (o));
		}

		public void Release (T obj)
		{
			SetToSleep (obj);
		}

		public void IncCapacity (int increment)
		{
			Create (increment);
		}

		public void DecCapacity (int decrement)
		{
			int remainingObjectsCount = Mathf.Max (0, Capacity - decrement);
			poolObjects.Sort ((o1, o2) => {
				if (checkSleep (o1))
					return 1;
				return 0;
			});
			for (int i = 0; i < remainingObjectsCount; i++) {
				Kill (poolObjects [0]);
			}
		}

		private void Kill (T obj)
		{
			SetToSleep (obj); 
			poolObjects.Remove (obj);
			kill (obj);
		}

		private void Create (int count)
		{
			for (int i = 0; i < count; i++) {
				var obj = create ();
				poolObjects.Add (obj);

				SetToSleep (obj);
			}
		}

		private void SetToSleep (T obj)
		{
			setToSleep (obj);
		}

	}
}
