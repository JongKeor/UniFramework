using System;
using UnityEditor;

namespace UniFramework
{
	[InitializeOnLoad]
	public class ScriptOrderEditor
	{
		static ScriptOrderEditor ()
		{
			foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts()) {
				if (monoScript.GetClass() != null) {
					foreach (var a in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(ScriptOrderAttribute))) {
						var currentOrder = MonoImporter.GetExecutionOrder(monoScript);
						var newOrder = ((ScriptOrderAttribute)a).order;
						if (currentOrder != newOrder)
							MonoImporter.SetExecutionOrder(monoScript, newOrder);
					}
				}
			}
		}
	}
}

