using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UniFramework.Extension;



namespace UniFramework
{
	public enum MessageBoxType
	{
		Close,
		YesNo,
	}

	public enum MessageBoxResult
	{
		Yes,
		No,
	}


	public class MessageBox : BaseSceneController
	{
		public const string PARAM_MESSAGE = "message";
		public const string PARAM_MESSAGE_TYPE = "messageType";

		private const string MESSAGE_TEXT = "_MessageText";
		private const string CLOSE_BUTTON = "_CloseButton";
		private const string YES_BUTTON = "_YesButton";
		private const string NO_BUTTON = "_NoButton";


		public static void ShowMessageBox ( string message, System.Action<MessageBoxResult> finish)
		{
			ShowMessageBox ( MessageBoxType.Close, message, finish);
		}

		public static void ShowYesNoMessageBox ( string message, System.Action<MessageBoxResult> finish)
		{
			ShowMessageBox ( MessageBoxType.YesNo, message, finish);
		}

		public static void ShowMessageBox ( MessageBoxType type, string message, System.Action<MessageBoxResult> finish)
		{
			Dictionary<string,object> dic = new Dictionary<string, object> ();
			dic.Add (MessageBox.PARAM_MESSAGE, message);
			dic.Add (MessageBox.PARAM_MESSAGE_TYPE, type);
			GameSceneManager.Instance.Root.GetSceneController<BaseSceneController>().Popup (ApplicationMeta.messageBoxScene, dic, null, (ctrl) => {
				MessageBox msg = ctrl as MessageBox;
				if (finish != null)
					finish (msg.result);
			});
		}


		private MessageBoxType type;
		private MessageBoxResult result;
		private Button closeButton;
		private Button yesButton;
		private Button noButton;
		private Text messageText;


		public override void OnLoad ()
		{
			base.OnLoad();
			result = MessageBoxResult.Yes;
			messageText = canvases[0].gameObject.FindChildObjectByName (MESSAGE_TEXT).GetComponent<Text> ();
			closeButton = canvases[0].gameObject.FindChildObjectByName (CLOSE_BUTTON).GetComponent<Button> ();
			yesButton = canvases[0].gameObject.FindChildObjectByName (YES_BUTTON).GetComponent<Button> ();
			noButton = canvases[0].gameObject.FindChildObjectByName (NO_BUTTON).GetComponent<Button> ();

		}
		public override void OnOpen (Dictionary<string, object> arguments)
		{
			base.OnOpen (arguments);

			messageText.text = arguments [PARAM_MESSAGE].ToString ();
			type = (MessageBoxType)arguments [PARAM_MESSAGE_TYPE];


			noButton.onClick.AddListener (() => {
				result = MessageBoxResult.No;
				Close ();

			});
			yesButton.onClick.AddListener (() => {
				result = MessageBoxResult.Yes;
				Close ();
			});
			closeButton.onClick.AddListener (() => {
				result = MessageBoxResult.No;
				Close ();
			});
			if (type == MessageBoxType.Close) {
				yesButton.gameObject.SetActive (false);
				noButton.gameObject.SetActive (false);
			} else if (type == MessageBoxType.YesNo) {
				closeButton.gameObject.SetActive (false);
			}
		}

	}
}
