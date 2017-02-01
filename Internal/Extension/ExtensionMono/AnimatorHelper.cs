using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UniFramework.Extension.Helper
{
	public class AnimatorHelper : MonoBehaviour
	{
		public const int MAIN_LAYER = 0;
		public Dictionary< string,IEnumerator> coroutineList = new Dictionary< string,IEnumerator> ();
		public Animator animator;
		//		public event System.Action<AnimatorStateInfo,AnimatorStateInfo> OnChangedState;
		public AnimatorStateInfo currentAnimator;


		public 	bool IsTriggerAnimation = false;
		public 	bool IsChanged = false;
		IEnumerator setTrigger;
		//		void Update()
		//		{
		//			AnimatorStateInfo cur;
		//
		//			if(animator.IsInTransition(MAIN_LAYER)){
		//				cur =  animator.GetNextAnimatorStateInfo(MAIN_LAYER);
		//
		//			}
		//			else {
		//				cur =  animator.GetCurrentAnimatorStateInfo(MAIN_LAYER);
		//
		//			}
		//			if(currentAnimator.fullPathHash != cur.fullPathHash){
		//				if(OnChangedState != null){
		//					OnChangedState(currentAnimator ,cur);
		//				}
		//				currentAnimator = cur;
		//			}
		//		}

		public float CurrentStateAnimationLength ()
		{
			if (IsTriggerAnimation) {
				if (IsChanged) {
					if (animator.IsInTransition (MAIN_LAYER)) {
						AnimatorStateInfo info = animator.GetNextAnimatorStateInfo (MAIN_LAYER);
						return info.normalizedTime;
					} else {
						AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo (MAIN_LAYER);
						return info.normalizedTime;
					}
				}
			} else {
				if (animator.IsInTransition (MAIN_LAYER)) {
					AnimatorStateInfo info = animator.GetNextAnimatorStateInfo (MAIN_LAYER);
					return info.normalizedTime;
				} else {
					AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo (MAIN_LAYER);
					return info.normalizedTime;
				}
			}
			return 0;
		}

		public float CurrentStateAnimationLength (string stateName)
		{
			AnimatorStateInfo currentStateInfo = new AnimatorStateInfo ();
			if (animator.IsInTransition (MAIN_LAYER)) {
				currentStateInfo = animator.GetNextAnimatorStateInfo (MAIN_LAYER);
			} else {
				currentStateInfo = animator.GetCurrentAnimatorStateInfo (MAIN_LAYER);
			}
			if (currentStateInfo.IsName (stateName)) {
				return currentStateInfo.normalizedTime;
			} else {
				return 0;
			}
		}


		public void Play (string stateName, int  layer, System.Action finishCallback)
		{
			if (!animator.HasState (layer, Animator.StringToHash (stateName))) {
				finishCallback ();
				return;
			}
			if (setTrigger != null) {
				StopCoroutine (setTrigger);
			}


			animator.Play (stateName, 0, 0);
			setTrigger = CheckCrossFadeEnd (stateName, finishCallback);
			StartCoroutine (setTrigger);
		}

		private IEnumerator CheckCrossFadeEnd (string stateName, System.Action finishCallback)
		{
			AnimatorStateInfo currentStateInfo = new AnimatorStateInfo ();
			while (true) {
				if (animator.IsInTransition (MAIN_LAYER)) {
					currentStateInfo = animator.GetNextAnimatorStateInfo (MAIN_LAYER);
				} else {
					currentStateInfo = animator.GetCurrentAnimatorStateInfo (MAIN_LAYER);
				}
				if (currentStateInfo.IsName (stateName) && currentStateInfo.normalizedTime >= 1f) {
					finishCallback ();
					break;
				}
				yield return null;

			}
		}


		public void SetTrigger (string name, System.Action finishCallback)
		{
			if (setTrigger != null) {
				StopCoroutine (setTrigger);
			}
			setTrigger = CheckEnd (name, finishCallback);
			StartCoroutine (setTrigger);
		}


		private IEnumerator  CheckEnd (string name, System.Action finishCallback)
		{
			IsTriggerAnimation = true;
			IsChanged = false;
			AnimatorStateInfo preStateInfo = new AnimatorStateInfo ();
	
			while (true) {
				if (!animator.IsInTransition (MAIN_LAYER)) { 
					preStateInfo = animator.GetCurrentAnimatorStateInfo (MAIN_LAYER);
					break;
				}
				yield return null;
			}
			animator.SetTrigger (name);
			AnimatorStateInfo currentStateInfo = new AnimatorStateInfo ();
			while (true) {
				yield return null;
				if (!IsChanged) {
					if (animator.IsInTransition (MAIN_LAYER)) {
						currentStateInfo = animator.GetNextAnimatorStateInfo (MAIN_LAYER);
					} else {
						currentStateInfo = animator.GetCurrentAnimatorStateInfo (MAIN_LAYER);
					}
					if (preStateInfo.fullPathHash != currentStateInfo.fullPathHash) {
						IsChanged = true;
					}
				}
				if (IsChanged) {
					if (!animator.IsInTransition (MAIN_LAYER)) {
						AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo (MAIN_LAYER);
						if (info.fullPathHash != currentStateInfo.fullPathHash
						   || info.normalizedTime >= 1f) {
							IsTriggerAnimation = false;
							IsChanged = false;
							finishCallback ();
							break;
						}
					}
				}
			}
		}



	}
}

