using UnityEngine;
using System.Collections;
using UniFramework.Extension.Helper;


namespace UniFramework.Extension
{

	public static class AnimatorExtension
	{
		public static void Play (this Animator self, string stateName, int layer, System.Action finishCallback)
		{
			AnimatorHelper helper = self.GetComponent<AnimatorHelper> ();
			if (helper == null) {
				helper = self.gameObject.AddComponent<AnimatorHelper> ();
			}
			helper.animator = self;

			helper.Play (stateName, layer, finishCallback);
		}

		public static void SetTrigger (this Animator self, string name, System.Action finishCallback)
		{
			AnimatorHelper helper = self.GetComponent<AnimatorHelper> ();
			if (helper == null) {
				helper = self.gameObject.AddComponent<AnimatorHelper> ();
			}
			helper.animator = self;
			helper.SetTrigger (name, finishCallback);
		}

		public static float CurrentStateAnimationLength (this Animator self)
		{
			AnimatorHelper helper = self.GetComponent<AnimatorHelper> ();
			if (helper == null) {
				helper = self.gameObject.AddComponent<AnimatorHelper> ();
			}
			helper.animator = self;
			return helper.CurrentStateAnimationLength ();
		}

		public static float CurrentStateAnimationLength (this Animator self, string stateName)
		{
			AnimatorHelper helper = self.GetComponent<AnimatorHelper> ();
			if (helper == null) {
				helper = self.gameObject.AddComponent<AnimatorHelper> ();
			}
			helper.animator = self;
			return helper.CurrentStateAnimationLength (stateName);
		}

		public static bool IsActive (this Animator self)
		{
			return self.gameObject.activeInHierarchy;
		}
	}
}
