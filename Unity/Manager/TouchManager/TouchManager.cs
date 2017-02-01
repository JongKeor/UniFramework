using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniFramework.Extension;
using UniFramework.Generic;

namespace UniFramework.TouchInput
{
	public class TouchManager : SingletonMonoBehaviour<MonoBehaviour>
	{
		private List<AbstractGestureDetector> detectors = new List<AbstractGestureDetector> ();
		private CustomInput lastInput = null;
		public bool DebugMode = false;

		public T GetGesture<T> () where T : AbstractGestureDetector
		{
			return  detectors.Find (o => o is T) as T;
		}

		private static bool IsTouchPlatform { 
			get {
				switch (Application.platform) {
				case RuntimePlatform.Android:
				case RuntimePlatform.IPhonePlayer:
					return true;
				default:
					return false;
				}
			}
		}

		private UnityEngine.Touch? CurrentTouch {
			get {
				if (Input.touchCount <= 0) {
					return null;
				}
				if (this.lastInput == null) {
					foreach (var touch in Input.touches) {
						if (touch.phase == TouchPhase.Began) {
							return touch;
						}
					}
				} else {
					foreach (var touch in Input.touches) {
						if (touch.fingerId == this.lastInput.TouchId) {
							return touch;
						}
					}
				}
				return null;
			}
		}

		private CustomInput InputOfTouch {
			get {
				CustomInput input = new CustomInput ();
				input.Time = Time.time;

				Touch? touch = this.CurrentTouch;

				if (!touch.HasValue) {
					return input;
				}
				input.ScreenPosition = touch.Value.position;
				input.DeltaPosition = touch.Value.deltaPosition;
				input.TouchId = touch.Value.fingerId;
				if (this.lastInput != null) {
					input.DeltaTime = Time.time - this.lastInput.Time;
				}
				switch (touch.Value.phase) {
				case TouchPhase.Began:
					input.IsDown = true;
					break;
				case TouchPhase.Moved:
				case TouchPhase.Stationary:
					input.IsDrag = true;
					break;
				case TouchPhase.Ended:
				case TouchPhase.Canceled:
					input.IsUp = true;
					break;
				}
				return input;
			}
		}

		private  CustomInput InputOfMouse {
			get {
				CustomInput input = new CustomInput ();            
				input.ScreenPosition = Input.mousePosition;
				input.Time = Time.time;
				if (this.lastInput != null) {
					input.DeltaPosition = Input.mousePosition - lastInput.ScreenPosition;
					input.DeltaTime = Time.time - this.lastInput.Time;
				}
				if (Input.GetMouseButtonDown (0)) {
					input.IsDown = true;
					input.DeltaPosition = new Vector3 ();
				} else if (Input.GetMouseButtonUp (0)) {
					input.IsUp = true;
				} else if (Input.GetMouseButton (0)) {
					input.IsDrag = true;
				}
				return input;
			}
		}

		private void Reset ()
		{
			System.Type[] types = ReflectionUtil.FindSubClass<AbstractGestureDetector> ();
			foreach (System.Type type in types) {
				gameObject.GetOrAddComponent (type);
			}
		}

		private void Awake ()
		{	
			DontDestroyOnLoad (gameObject);
			detectors.AddRange (this.GetComponentsInChildren<AbstractGestureDetector> ()); 
		}

		private void Update ()
		{
			CustomInput currentInput = IsTouchPlatform ? InputOfTouch : InputOfMouse;

			for (int i = 0; i < this.detectors.Count; i++) {
				this.detectors [i].Enqueue (currentInput);
			}

			this.lastInput = currentInput;
		}
	}
}
  