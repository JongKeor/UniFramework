using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UniFramework;


[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
		Enum targetEnum = GetBaseProperty<Enum>(property);
		
		string propName = flagSettings.enumName;
		if (string.IsNullOrEmpty(propName))
			propName = property.name;
		
		EditorGUI.BeginProperty(position, label, property);
		Enum enumNew = EditorGUI.EnumMaskField(position, propName, targetEnum);
		property.intValue = (int) Convert.ChangeType(enumNew, targetEnum.GetType());
		EditorGUI.EndProperty();
	}
	
	static T GetBaseProperty<T>(SerializedProperty prop)
	{
		string[] separatedPaths = prop.propertyPath.Split('.');
		System.Object reflectionTarget = prop.serializedObject.targetObject as object;
		foreach (var path in separatedPaths)
		{
			FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path);
			reflectionTarget = fieldInfo.GetValue(reflectionTarget);
		}
		return (T) reflectionTarget;
	}
}
