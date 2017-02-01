using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace UniFramework.TimeScale
{
	[System.Serializable]
	public sealed class TimeScaleModifierRepository : ITimeScaleModifierRepository
	{
		public ITimeScaleModifier[] TimeScaleModifierCollection {
			get {
				return timeScaleModifierCollection;
			}
		}

		[PopupDerivedClass (typeof(ITimeScaleModifier))]
		[SerializeField]
		private List<string> timsScaleModifierTypeCollection;

		private ITimeScaleModifier[] timeScaleModifierCollection;

		public TimeScaleModifierRepository ()
		{
			timsScaleModifierTypeCollection = new List<string> ();
			System.Type[] types = ReflectionUtil.FindClassWithInterface<ITimeScaleModifier> ();
			foreach (System.Type type in types) {
				timsScaleModifierTypeCollection.Add (type.ToString ());
			}
		}

		public void Awake ()
		{
			List<ITimeScaleModifier> timeScaleable = new List<ITimeScaleModifier> ();
			for (int i = 0; i < timsScaleModifierTypeCollection.Count; i++) {	
				timeScaleable.Add (System.Activator.CreateInstance (System.Type.GetType (timsScaleModifierTypeCollection [i])) as ITimeScaleModifier);
			}
			timeScaleModifierCollection = timeScaleable.ToArray ();
		}


		public void AddTimeScaleModifier<T> () where T : ITimeScaleModifier
		{
			List<ITimeScaleModifier> timeScaleable = new List<ITimeScaleModifier> (timeScaleModifierCollection);
			timeScaleable.Add (System.Activator.CreateInstance (typeof(T)) as ITimeScaleModifier);
			timeScaleModifierCollection = timeScaleable.ToArray ();

		}

		public void RemoveTimeScaleModifier<T> () where T : ITimeScaleModifier
		{
			List<ITimeScaleModifier> timeScaleable = new List<ITimeScaleModifier> (timeScaleModifierCollection);
			timeScaleable.RemoveAll (o => o is T);
			timeScaleModifierCollection = timeScaleable.ToArray ();
		}

		public void RemoveAll ()
		{
			timeScaleModifierCollection = new ITimeScaleModifier[0];	
		}

	}
}

