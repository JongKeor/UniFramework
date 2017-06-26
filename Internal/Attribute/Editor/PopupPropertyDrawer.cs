using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UniFramework;


[CustomPropertyDrawer(typeof(PopupDerivedClassAttribute))]
public class PopupDerivedClassPropertyDrawer : PropertyDrawer
{
	public const string ETC = "Etc";
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
		
        PopupDerivedClassAttribute popup = attribute as PopupDerivedClassAttribute;
		
        System.Type[] types = null;
        if (popup.type.IsInterface)
        {
            types = ReflectionUtil.FindClassWithInterface(popup.type);
        }
        else
        {
            types = ReflectionUtil.FindSubClass(popup.type);
        }

        List<string> cachedTypes = new List<string>();
        cachedTypes.Add(PopupDerivedClassPropertyDrawer.ETC);
        foreach (var t in types)
        {
            cachedTypes.Add(t.ToString());
        }
        string current = property.stringValue;


        int i = 0;
        if (cachedTypes.IndexOf(current) < 0 || current == PopupDerivedClassPropertyDrawer.ETC)
        {
            Rect ret = position;
            ret.width /= 2;
            i = EditorGUI.Popup(ret,label.text , cachedTypes.IndexOf(PopupDerivedClassPropertyDrawer.ETC), cachedTypes.ToArray());
            ret.x += position.width / 2;
            if (i >= 0 && i != cachedTypes.IndexOf(PopupDerivedClassPropertyDrawer.ETC))
            {
                property.stringValue = cachedTypes[i];
            }
			else 
			{
				property.stringValue = EditorGUI.TextField(ret, property.stringValue);
			}
        }
        else
        {
            i = EditorGUI.Popup(position,label.text  ,cachedTypes.IndexOf(current), cachedTypes.ToArray());
			if (i >= 0 && i != cachedTypes.IndexOf(PopupDerivedClassPropertyDrawer.ETC))
            {
                property.stringValue = cachedTypes[i];
            }
			else 
			{
				property.stringValue = string.Empty;
			}
        }




    }

}

