using UnityEngine;
using System.Collections;

namespace UniFramework.TouchInput
{
	public class CustomInput
	{
		public Vector3 ScreenPosition { get; set; }

		public Vector3 DeltaPosition { get; set; }

		public float DeltaTime { get; set; }

		public float Time{ get; set; }

		public bool IsDown { get; set; }

		public bool IsUp { get; set; }

		public bool IsDrag { get; set; }

		public int TouchId{ get; set; }

		public bool IsFlicking{ get; set; }


		public float LevelingTime{ get; set; }


		public Vector3 MovedDistance{ get; set; }


		public Vector3 SpeedVector {
			get {
				if (this.LevelingTime < 0.0001f)
					return Vector3.zero;
				return this.MovedDistance / this.LevelingTime; 
			}
		}


		public Vector3 LevelingOriginSpeedVector{ get; set; }


		public Vector3 AccelerationVector {
			get {
				if (this.LevelingTime < 0.0001f)
					return Vector3.zero;
				return (this.SpeedVector - this.LevelingOriginSpeedVector) / this.LevelingTime;
			}
		}

	}
}
