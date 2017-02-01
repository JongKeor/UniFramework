using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace UniFramework.TouchInput
{
	public class FlickEventArgs
	{
		public enum Direction4
		{
			None = -1,
			Up = 0,
			Left = 1,
			Down = 2,
			Right = 3
		}

		public CustomInput StartInput{ get; private set; }

		public CustomInput EndInput{ get; private set; }

		public Vector3 MovedDistance{ get { return EndInput.ScreenPosition - StartInput.ScreenPosition; } }

		public float ElapsedTime{ get { return EndInput.Time - StartInput.Time; } }

		public float Speed {
			get {
				if (this.ElapsedTime < 0.0001f)
					return 0;
				return this.MovedDistance.magnitude / this.ElapsedTime;
			}
		}

		public Direction4 Direction{ get { return VectorDirection4 (this.MovedDistance.x, this.MovedDistance.y); } }



		public FlickEventArgs (CustomInput startInput, CustomInput endInput)
		{
			this.StartInput = startInput;
			this.EndInput = endInput;
		}

		private static Direction4 VectorDirection4 (float x, float y)
		{
			if (y == 0 && x == 0)
				return Direction4.None;
			if (y >= Mathf.Abs (x))
				return Direction4.Up;
			if (y <= -Mathf.Abs (x))
				return Direction4.Down;
			if (x < -Mathf.Abs (y))
				return Direction4.Left;
			if (x > Mathf.Abs (y))
				return Direction4.Right;
			return Direction4.None;
		}
	}


	public class FlickDetector : AbstractGestureDetector
	{
		[Range (1f, 60f)]
		public int LevelingFrameCount = 5;
		[Range (1f, 100000f)]
		public float DetectAcceleration = 1000f;
		[Range (1f, 10000f)]
		public float DefeatSpeed = 100f;
		[Range (0f, 1000f)]
		public float MinFlickDistance = 0f;

		public event System.Action<FlickEventArgs> OnFlickComplete;
		public event System.Action<FlickEventArgs> OnFlickStart;



		public bool ContinuousDetect = true;

		private  List<CustomInput> pastInputs = new List<CustomInput> ();

		public CustomInput FlickStartInput = null;
		private bool IsDetected = false;

		public override void Enqueue (CustomInput currentInput)
		{
			if (!(currentInput.IsDown || currentInput.IsDrag || currentInput.IsUp))
				return;

			this.pastInputs.Add (currentInput);

			if (this.pastInputs.Count == 1) {
				//First Input
				currentInput.MovedDistance = Vector3.zero;
				currentInput.LevelingTime = 0;
				currentInput.LevelingOriginSpeedVector = Vector3.zero;
			} else {
				//currentInputからLevelingFrame数だけ古いフレームのInput
				CustomInput levelingOriginInput = this.pastInputs [0];
				currentInput.MovedDistance = currentInput.ScreenPosition - levelingOriginInput.ScreenPosition;
				currentInput.LevelingTime = currentInput.Time - levelingOriginInput.Time;// this.LevelingFrameCount;
				currentInput.LevelingOriginSpeedVector = levelingOriginInput.SpeedVector;

				//フリック開始＆継続判定
				var lastInput = this.pastInputs [this.pastInputs.Count - 2];
				if (lastInput.IsFlicking) {
					//継続判定
					if (currentInput.SpeedVector.magnitude > this.DefeatSpeed) {
						currentInput.IsFlicking = true;
					} else {
						//フリック中止
						this.FlickStartInput = null;

						currentInput.IsFlicking = false;
						this.FlickStartInput = null;
					}
				} else {
					//フリック開始判定
					if (currentInput.AccelerationVector.magnitude > this.DetectAcceleration) {
						if (currentInput.SpeedVector.magnitude > 0.0001f) {
							if (!this.ContinuousDetect && this.IsDetected) {
								//指を離すまで再検知しない
							} else {
								currentInput.IsFlicking = true;
								this.FlickStartInput = currentInput;
								this.IsDetected = true;
								if (OnFlickStart != null)
									OnFlickStart (new FlickEventArgs (levelingOriginInput, currentInput));
							}
						}
					}
				}

				//フリック完了判定
				if (currentInput.IsFlicking && currentInput.IsUp) {

					Vector3 flickDistance = currentInput.ScreenPosition - this.FlickStartInput.ScreenPosition;
					if (flickDistance.magnitude > this.MinFlickDistance) {

						//フリック成立
						if (OnFlickComplete != null)
							OnFlickComplete (new FlickEventArgs (levelingOriginInput, currentInput));
						//TouchManager.Instance.OnFlickComplete (new FlickEventArgs (this.FlickStartInput, currentInput));

						currentInput.IsFlicking = false;
						this.FlickStartInput = null;

					}
				}

				//指が離れた
				if (currentInput.IsUp) {
					this.IsDetected = false;
					this.pastInputs.Clear ();
				}
			}

			while (this.pastInputs.Count > this.LevelingFrameCount) {
				this.pastInputs.RemoveAt (0);
			}

		}
	}
}
