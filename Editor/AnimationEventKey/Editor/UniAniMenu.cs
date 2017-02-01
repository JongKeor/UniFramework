
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class UniAnimationKey
{
    private void OnGUI_MenuNew()
    {
        if (m_dicEditClip == null)
        {
            return;
        }

        m_v2MenuScrool = EditorGUILayout.BeginScrollView(m_v2MenuScrool);

        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        MenuFind();
        MenuClearAll();

		MenuRevert();
		MenuApply();
        EditorGUILayout.Space();
        MenuSearchAnimationClip();
        EditorGUILayout.Space();
        MenuSearchEventFunction();
        EditorGUILayout.Space();
        MenuEventKeyAdd();

        EditorGUILayout.EndScrollView();
    }

  

    private void MenuFind()
    {
        GUI.color = Color.green;

        if (GUILayout.Button("FIND") == true)
        {
            m_dicFoldout.Clear();
            m_dicEditClip.Clear();
            m_dicBackUp.Clear();
            m_lstKeyName.Clear();

            Dictionary<string, AnimationClip> dicTempClip = new Dictionary<string, AnimationClip>();

            if (FindAnimationClip(ref dicTempClip))
            {
                foreach (KeyValuePair<string, AnimationClip> _keyValue in dicTempClip)
                {
                    AnimationClip clip = _keyValue.Value;

                    UnityEngine.AnimationEvent[] anievents = AnimationUtility.GetAnimationEvents(clip);

                    m_dicFoldout.Add(clip, true);

                    List<AnimationEvent> lstEvent = new List<AnimationEvent>();

                    if (anievents != null)
                    {
                        lstEvent.AddRange(anievents);
                    }

                    if (m_dicEditClip.ContainsKey(clip))
                    {
                        new System.Exception();
                    }
                    else
                    {
                        m_dicEditClip.Add(clip, lstEvent);
                        AddBackUp(clip, ref lstEvent);

                        UniEditorUtil.DisplayProgressBar("AniEvent Find", clip, m_dicEditClip.Count, dicTempClip.Count);
                    }
                }
                EditorUtility.ClearProgressBar();
            }

            SortAniClipList();
        }

        GUI.color = Color.white;
    }
	private void MenuApply()
	{
		GUI.color = Color.red;
		bool isChange = false;
		foreach (KeyValuePair<AnimationClip, List<AnimationEvent>> _keyValue in m_dicEditClip)
		{
			if(!IsSame(_keyValue.Key ,_keyValue.Value)){
				isChange = true;
				break;
			}
		}

		GUI.enabled = isChange;
		if (GUILayout.Button("APPLY") == false)
		{
			GUI.color = Color.white;
			GUI.enabled = true;
			return;
		}
		GUI.enabled = true;
		foreach (KeyValuePair<AnimationClip, List<AnimationEvent>> _keyValue in m_dicEditClip)
		{

			if(IsSame(_keyValue.Key ,_keyValue.Value  )) continue;
			
			string destPath = AssetDatabase.GetAssetPath (_keyValue.Key);
			ModelImporter destModelImporter = ModelImporter.GetAtPath (destPath) as ModelImporter;
			if (destModelImporter != null) {
				ModelImporterClipAnimation[] clips = destModelImporter.defaultClipAnimations;
				AnimationEvent[] evts = _keyValue.Value.ToArray();
				foreach(var evt in  evts)
				{
					evt.time = evt.time /_keyValue.Key.length;
				}
				foreach (var clip   in  clips) {
					clip.events = evts;
				}
				destModelImporter.clipAnimations = clips;
				destModelImporter.SaveAndReimport ();
			}
		}
		if (null != m_dicFoldout) m_dicFoldout.Clear();
		if (null != m_dicEditClip) m_dicEditClip.Clear();
		if (null != m_dicBackUp) m_dicBackUp.Clear();
		if (null != m_lstKeyName) m_lstKeyName.Clear();
		GUI.color = Color.white;
	}

	private bool IsSame(AnimationClip clip , List<AnimationEvent> destEvents)
	{
		AnimationEvent[]  sourceEvents=  AnimationUtility.GetAnimationEvents(clip);

		if(sourceEvents.Length != destEvents.Count) return false;
		for(int i = 0 ; i < sourceEvents.Length ; i++)
		{
			if(sourceEvents[i].floatParameter != destEvents[i].floatParameter
				||sourceEvents[i].intParameter != destEvents[i].intParameter
				||sourceEvents[i].stringParameter != destEvents[i].stringParameter
				||sourceEvents[i].objectReferenceParameter != destEvents[i].objectReferenceParameter
				||sourceEvents[i].time != destEvents[i].time
				||sourceEvents[i].functionName != destEvents[i].functionName
			) return false;
		}
		return true;
	}



    private void MenuRevert()
    {
        if (GUILayout.Button("REVERT") == false)
        {
            return;
        }

        if (m_dicBackUp != null)
        {
            foreach (KeyValuePair<AnimationClip, List<AnimationEvent>> _keyValue in m_dicBackUp)
            {
                AnimationClip clip = _keyValue.Key;
                List<AnimationEvent> lstItem = _keyValue.Value;

                List<AnimationEvent> lstSource = new List<AnimationEvent>();

                foreach (AnimationEvent item in lstItem)
                {
                    AnimationEvent aniEvent = UniEditorUtil.CopyAnimationEvent(item);
                    if (aniEvent == null)
                    {
                        continue;
                    }
                    lstSource.Add(aniEvent);
                }

                if (clip != null && lstSource != null)
                {
                    if (m_dicEditClip.ContainsKey(clip))
                    {
                        List<AnimationEvent> lstDest = m_dicEditClip[clip];

                        lstDest.Clear();
                        lstDest.AddRange(lstSource.ToArray());

//                        AnimationUtility.SetAnimationEvents(clip, lstDest.ToArray());
                    }
                }

                lstSource.Clear();
            }
        }
    }

    private void MenuClearAll()
    {
        if (GUILayout.Button("CLEAR") == false)
        {
            return;
        }

        if (null != m_dicFoldout) m_dicFoldout.Clear();
        if (null != m_dicEditClip) m_dicEditClip.Clear();
        if (null != m_dicBackUp) m_dicBackUp.Clear();
        if (null != m_lstKeyName) m_lstKeyName.Clear();
    }

    private void MenuSearchAnimationClip()
    {
        EditorGUILayout.LabelField("Search AnimationClip", UniEditorUtil.CreateGUIStyle(11, new Color(0.8f, 0.8f, 0.8f, 1.0f), FontStyle.Bold));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Filter", GUILayout.Width(70.0f));
        if (m_lstClipName != null)
        {
            m_lstClipName.Clear();
            m_lstClipName.Add("ALL");

            foreach (KeyValuePair<AnimationClip, List<AnimationEvent>> _keyValue in m_dicEditClip)
            {
                m_lstClipName.Add(_keyValue.Key.name);
            }

            m_nSerachAnimationClipPopup = EditorGUILayout.Popup(m_nSerachAnimationClipPopup, m_lstClipName.ToArray(), EditorStyles.popup);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("String", GUILayout.Width(70.0f));
        m_serachAnimationClipString = EditorGUILayout.TextField(m_serachAnimationClipString, (GUIStyle)"SearchTextField");
        EditorGUILayout.EndHorizontal();
    }

    private void MenuSearchEventFunction()
    {
        EditorGUILayout.LabelField("Search Function", UniEditorUtil.CreateGUIStyle(11, new Color(0.8f, 0.8f, 0.8f, 1.0f), FontStyle.Bold));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Function", GUILayout.Width(70.0f));
        m_strFindKeyEvent = EditorGUILayout.TextField(m_strFindKeyEvent, (GUIStyle)"SearchTextField");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Parmater", GUILayout.Width(70.0f));
        m_strFindKeyParmater = EditorGUILayout.TextField(m_strFindKeyParmater, (GUIStyle)"SearchTextField");
        EditorGUILayout.EndHorizontal();
    }

    private void MenuEventKeyAdd()
    {
        EditorGUILayout.LabelField("New Function", UniEditorUtil.CreateGUIStyle(11, new Color(0.8f, 0.8f, 0.8f, 1.0f), FontStyle.Bold));

        EditorGUILayout.BeginHorizontal();

        m_strEventFuntionAdd = EditorGUILayout.TextField(m_strEventFuntionAdd);

        if (GUILayout.Button("ADD"))
        {
            if (string.IsNullOrEmpty(m_strEventFuntionAdd) == false)
            {
                if (m_lstKeyName != null)
                {
                    if (m_lstKeyName.Contains(m_strEventFuntionAdd) == false)
                    {
                        m_lstKeyName.Add(m_strEventFuntionAdd);
                        m_strEventFuntionAdd = string.Empty;
                    }
                }
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}