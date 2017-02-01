using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniFramework.Generic;

namespace UniFramework.TimeScale
{
	public sealed class TimeScaleManager : SingletonMonoBehaviour<TimeScaleManager>,ISerializationCallbackReceiver
	{
		public LayerTime Default {
			get {
				return layerTimeList [0];
			}
		}

		public TimeScaleModifierRepository TimeScaleRepository {
			get {
				return timeScaleRepository;
			}
		}

		[SerializeField]
		private TimeScaleModifierRepository timeScaleRepository;
		[SerializeField]
		private List<LayerTime> layerTimeList;

		private void Reset ()
		{
			layerTimeList = new List<LayerTime> ();
			layerTimeList.Add (new LayerTime ());

		}

		private void Awake ()
		{
			timeScaleRepository.Awake ();
		}

		public void AddLayerTime (LayerMask mask)
		{
			LayerTime layerTime = new LayerTime ();
			layerTime.mask = mask;
			layerTime.SetITimeScaleable (TimeScaleRepository);
			layerTimeList.Add (layerTime);
		}

		public void RemoveLayer (LayerMask mask)
		{
			layerTimeList.RemoveAll (o => o.mask == mask);
		}


		public float DeltaTime (GameObject t)
		{
			return  FindLayerTime (t.layer).CurrentDeltaTime;
		}

		public float TimeScale (GameObject t)
		{
			return  FindLayerTime (t.layer).CurrentTimeScale;
		}

		private LayerTime FindLayerTime (int layer)
		{
			return layerTimeList.Find (o => o.mask == (o.mask | 1 << layer));
		}

		public void OnBeforeSerialize ()
		{
			for (int i = 0; i < layerTimeList.Count; i++) {
				layerTimeList [i].SetITimeScaleable (TimeScaleRepository);
			}
		}

		public void OnAfterDeserialize ()
		{

		}
	}
}