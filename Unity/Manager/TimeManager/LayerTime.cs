using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UniFramework.TimeScale
{
	public interface ITimeScaleModifierRepository
	{
		ITimeScaleModifier[] TimeScaleModifierCollection {
			get;
		}

		void AddTimeScaleModifier<T> () where T : ITimeScaleModifier;

		void RemoveTimeScaleModifier<T> () where T : ITimeScaleModifier;

		void RemoveAll ();
	}


	[System.Serializable]
	public class LayerTime
	{
		public LayerMask mask = -1;

		public float CurrentTimeScale {
			get {
				return LocalTimeScale;
			}
		}

		public float CurrentDeltaTime {
			get {
				return CurrentTimeScale * Time.deltaTime;
			}
		}

		public float  LocalTimeScale {
			get {
				return localTimeScale;
			}

			set {
				if (value != localTimeScale) {
					localTimeScale = value;
					Notify ();
					if (CurrentTimeScale != 0)
						previousTimeModifer = CurrentTimeScale;
				}
			}
		}

		private float localTimeScale = 1f;
		private float previousTimeModifer = 1f;
		private ITimeScaleModifierRepository timeScaleableRepository;

		public void SetITimeScaleable (ITimeScaleModifierRepository repository)
		{
			timeScaleableRepository = repository;
		}

		private float GetNewValueFromPercent (float v)
		{
			float thisOriginalSpeed;
			if (previousTimeModifer != 0f)
				thisOriginalSpeed = (1f / previousTimeModifer) * v;
			else
				thisOriginalSpeed = v;

			return (thisOriginalSpeed) * CurrentTimeScale;
		}

		protected void FindTimeScaleableComponent (GameObject t)
		{
			for (int i = 0; i < timeScaleableRepository.TimeScaleModifierCollection.Length; i++) {
				timeScaleableRepository.TimeScaleModifierCollection [i].ApplyTimeScale (t, CurrentTimeScale);
			}
		}

		protected void Notify ()
		{
			GameObject[] objects = GameObject.FindObjectsOfType<GameObject> ();
			for (int i = 0; i < objects.Length; i++) {
				if (mask == (mask | 1 << objects [i].layer)) {
					FindTimeScaleableComponent (objects [i]);
				}
			}

		}
	}
}