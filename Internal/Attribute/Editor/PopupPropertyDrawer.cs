using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UniFramework;


[CustomPropertyDrawer (typeof(PopupDerivedClassAttribute))]
public class PopupDerivedClassPropertyDrawer : PropertyDrawer
{
	
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		PopupDerivedClassAttribute popup = attribute as PopupDerivedClassAttribute;

		System.Type[] types = null;
		if(popup.type.IsInterface){
			types = ReflectionUtil.FindClassWithInterface( popup.type);
		}
		else {
			types = ReflectionUtil.FindSubClass( popup.type);
		}
		 
		List<string> cachedTypes = new List<string> ();
		foreach (var t in types) {
			cachedTypes.Add (t.ToString());
		}
		string  current =  property.stringValue;
		int i =  EditorGUI.Popup(position, cachedTypes.IndexOf(current), cachedTypes.ToArray() );
		if(i  >= 0){
			property.stringValue =  cachedTypes[i];
		}

	}

}

