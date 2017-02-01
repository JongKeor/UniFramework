using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class UniAnimationKey
{
    private void OnGUI_EventKeyEditor()
    {
        m_v2EditorScrool = EditorGUILayout.BeginScrollView(m_v2EditorScrool);

        AniClipCopyView();
        EditorAniClipList();

        EditorGUILayout.EndScrollView();
    }

    private void AniClipCopyView()
    {
        EditorGUILayout.BeginHorizontal();

        if (m_lstAniEventKeyCopy != null && m_lstAniEventKeyCopy.Count > 0)
        {
            System.IO.FileInfo fileInfo;
            UniEditorUtil.GetFileInfo(out fileInfo, m_curCopyAniClip);

            string strMsg = string.Format("[EVENT_KEY_COPY] AniName({0}) EventKey Count({1})", fileInfo.Name, m_lstAniEventKeyCopy.Count);
            EditorGUILayout.LabelField(strMsg, UniEditorUtil.CreateGUIStyle(12, Color.green, FontStyle.Bold));

            if (GUILayout.Button("CLEAR", GUILayout.Width(50)))
            {
                m_lstAniEventKeyCopy.Clear();
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void EditorAniClipList()
    {
        if (m_dicEditClip == null)
        {
            return;
        }

        foreach (KeyValuePair<AnimationClip, List<AnimationEvent>> _keyValue in m_dicEditClip)
        {
            AnimationClip clip = _keyValue.Key;
            List<AnimationEvent> lstEvent = _keyValue.Value;

            // serach string animation clip
            if (m_lstClipName != null &&
                m_lstClipName.Count > 0 &&
                m_nSerachAnimationClipPopup > -1)
            {
                string strPopUpName = m_lstClipName[Mathf.Clamp(m_nSerachAnimationClipPopup, 0, m_lstClipName.Count - 1)];
                if (UniEditorUtil.IsCompareStr("ALL", strPopUpName) == false)
                {
                    if (UniEditorUtil.IsCompareStr(clip.name, strPopUpName, false) == false)
                    {
                        continue;
                    }
                }
            }

            // serach string animation clip
            if (string.IsNullOrEmpty(m_serachAnimationClipString) == false)
            {
                if (UniEditorUtil.IsCompareIncludeStr(clip.name, m_serachAnimationClipString, true) == false)
                {
                    continue;
                }
            }

            System.IO.FileInfo fileInfo;
            UniEditorUtil.GetFileInfo(out fileInfo, clip);

            EditorGUILayout.Space();
			if(!IsSame(clip,lstEvent)){
				GUI.color = Color.yellow;
			}

            m_dicFoldout[clip] = EditorGUILayout.Foldout(m_dicFoldout[clip], fileInfo.Name);


			GUI.color = Color.white;

            if (m_dicFoldout[clip] == false)
            {
                continue;
            }

            int nChangeCount = 0;

            EditorGUILayout.BeginHorizontal();

            float fMaxFrame = clip.length * clip.frameRate;
            string strInfo = string.Format("Frame({0}) Time({1}) Sample({2}) Path({3})", fMaxFrame, clip.length, clip.frameRate, fileInfo.Directory);
            EditorGUILayout.HelpBox(strInfo, MessageType.None);

            float fBtnWidth = 80.0f;

            if (GUILayout.Button("Trace", GUILayout.Width(fBtnWidth)))
            {
                //Selection.activeObject = clip;
                ProjectWindowUtil.ShowCreatedAsset(clip);
            }

            if (GUILayout.Button("Revert", GUILayout.Width(fBtnWidth)))
            {
                if (m_dicBackUp.ContainsKey(clip))
                {
                    List<AnimationEvent> lstItem = m_dicBackUp[clip];
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

                    lstEvent.Clear();
                    lstEvent.AddRange(lstSource.ToArray());

                    lstSource.Clear();

                    nChangeCount++;
                }
            }

            if (lstEvent != null && lstEvent.Count > 0)
            {
                if (GUILayout.Button("Replace", GUILayout.Width(fBtnWidth)))
                {
                    SortTime(ref lstEvent);
                    nChangeCount++;
                }

                if (GUILayout.Button("Copy", GUILayout.Width(fBtnWidth)))
                {
                    m_curCopyAniClip = clip;

                    m_lstAniEventKeyCopy.Clear();

                    foreach (AnimationEvent item in lstEvent)
                    {
                        AnimationEvent aniEvent = UniEditorUtil.CopyAnimationEvent(item);
                        if (aniEvent == null)
                        {
                            continue;
                        }

                        m_lstAniEventKeyCopy.Add(aniEvent);
                    }
                }
            }

            if (m_lstAniEventKeyCopy != null && m_lstAniEventKeyCopy.Count > 0)
            {
                if (GUILayout.Button("Paste", GUILayout.Width(fBtnWidth)))
                {
					foreach (AnimationEvent item in m_lstAniEventKeyCopy)
					{
						AnimationEvent aniEvent = UniEditorUtil.CopyAnimationEvent(item);
						if (aniEvent == null)
						{
							continue;
						}

						lstEvent.Add(aniEvent);
					}
                    
                    SortTime(ref lstEvent);
                    nChangeCount++;
                }
            }

            if (GUILayout.Button("＋", GUILayout.Width(20)))
            {
                AnimationEvent aniEvent = new AnimationEvent();
                aniEvent.time = clip.length;
                aniEvent.functionName = "NEW_EVENT";
                lstEvent.Add(aniEvent);
                nChangeCount++;
            }

            EditorAniClipEventList(lstEvent, clip, ref nChangeCount, fMaxFrame);

            if (nChangeCount > 0)
            {
//                AnimationUtility.SetAnimationEvents(clip, lstEvent.ToArray());
            }
        }
    }

    private void EditorAniClipEventList(List<AnimationEvent> lstEvent, AnimationClip clip, ref int nChangeCount, float fMaxFrame)
    {
        EditorGUILayout.EndHorizontal();

		AnimationEvent[] original =  AnimationUtility.GetAnimationEvents(clip);


        if (lstEvent != null && lstEvent.Count > 0)
        {
            GUIStyle titleStyle = UniEditorUtil.CreateGUIStyle(10, new Color(0.8f, 0.8f, 0.8f), FontStyle.Bold);


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Frame", titleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("KeyEvent", titleStyle, GUILayout.Width(100));
            EditorGUILayout.LabelField("Float", titleStyle, GUILayout.Width(50));
            EditorGUILayout.LabelField("Int", titleStyle, GUILayout.Width(50));
            EditorGUILayout.LabelField("String", titleStyle, GUILayout.Width(100));
			EditorGUILayout.LabelField("Object", titleStyle, GUILayout.Width(150));
            EditorGUILayout.LabelField("", titleStyle, GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();

            AnimationEvent delEvent = null;

            for (int index = 0; index < lstEvent.Count; index++)
            {

				AnimationEvent originAniEvent = null;
				if(lstEvent.Count  != original.Length){
					GUI.color = Color.cyan;
				}
				else {
					originAniEvent = original[index];
				}

                AnimationEvent aniEvent = lstEvent[index];
				if (aniEvent == null)
                {
					GUI.color = Color.white;
                    continue;
                }

				string strFunName = aniEvent.functionName;
                float paramFloat = aniEvent.floatParameter;
                int paramInt = aniEvent.intParameter;
                string paramString = aniEvent.stringParameter;
				Object paramObject = aniEvent.objectReferenceParameter;

                if (m_lstKeyName.Contains(aniEvent.functionName) == false)
                {
                    m_lstKeyName.Add(aniEvent.functionName);
                }

                // serach event function
                if (string.IsNullOrEmpty(m_strFindKeyEvent) == false)
                {
                    if (UniEditorUtil.IsCompareIncludeStr(aniEvent.functionName, m_strFindKeyEvent, true) == false)
                    {
						GUI.color = Color.white;
                        continue;
                    }
                }

                // serach event function parmater
                if (string.IsNullOrEmpty(m_strFindKeyParmater) == false)
                {
                    int nSearch = 0;

                    string floatParameter = aniEvent.floatParameter.ToString();
                    string intParameter = aniEvent.intParameter.ToString();
                    string stringParameter = aniEvent.stringParameter;
					string objectParameter = aniEvent.objectReferenceParameter.ToString();

                    if (string.IsNullOrEmpty(floatParameter) == false)
                    {
                        if (UniEditorUtil.IsCompareIncludeStr(floatParameter, m_strFindKeyParmater, true) == true)
                        {
                            nSearch++;
                        }
                    }

                    if (string.IsNullOrEmpty(intParameter) == false)
                    {
                        if (UniEditorUtil.IsCompareIncludeStr(intParameter, m_strFindKeyParmater, true) == true)
                        {
                            nSearch++;
                        }
                    }

                    if (string.IsNullOrEmpty(stringParameter) == false)
                    {
                        if (UniEditorUtil.IsCompareIncludeStr(stringParameter, m_strFindKeyParmater, true) == true)
                        {
                            nSearch++;
                        }
                    }
					if(string.IsNullOrEmpty(objectParameter) == false)
					{
						if (UniEditorUtil.IsCompareIncludeStr(objectParameter, m_strFindKeyParmater, true) == true)
						{
							nSearch++;
						}
					}
                    if (nSearch <= 0)
                    {
						GUI.color = Color.white;
                        continue;
                    }
                }

                EditorGUILayout.BeginHorizontal();

    
				if(GUI.color == Color.cyan)
				{
				}
				else if(originAniEvent != null && originAniEvent.time != aniEvent.time){
					GUI.color = Color.yellow;
				}
				else {
					GUI.color = Color.white;
				}
                float fOldFrame = (aniEvent.time * clip.frameRate);
                float fNewFrame = EditorGUILayout.Slider(fOldFrame, 0.0f, fMaxFrame, GUILayout.ExpandWidth(true));

                fNewFrame = Mathf.Clamp(fNewFrame, 0.0f, fMaxFrame);
                if (float.Equals(fOldFrame, fNewFrame) == false)
                {
                    aniEvent.time = fNewFrame / clip.frameRate;
                    nChangeCount++;
                }
				if(GUI.color == Color.cyan)
				{
				}
				else if(originAniEvent != null && originAniEvent.functionName != aniEvent.functionName){
					GUI.color = Color.yellow;
				}
				else {
					GUI.color = Color.white;
				}
                int nSelectName = GetKeyNameIndex(aniEvent.functionName);
                int nNewSelectName = EditorGUILayout.Popup(nSelectName, m_lstKeyName.ToArray(), GUILayout.Width(100));

                aniEvent.functionName = m_lstKeyName[nNewSelectName];
                nChangeCount = aniEvent.functionName != strFunName ? nChangeCount + 1 : nChangeCount;

				if(GUI.color == Color.cyan)
				{
				}
				else if( originAniEvent != null &&originAniEvent.floatParameter != aniEvent.floatParameter){
					GUI.color = Color.yellow;
				}
				else {
					GUI.color = Color.white;
				}

                aniEvent.floatParameter = EditorGUILayout.FloatField(aniEvent.floatParameter, GUILayout.Width(50));
                nChangeCount = aniEvent.floatParameter != paramFloat ? nChangeCount + 1 : nChangeCount;
				if(GUI.color == Color.cyan)
				{
				}
				else if(originAniEvent != null &&originAniEvent.intParameter != aniEvent.intParameter){
					GUI.color = Color.yellow;
				}
				else {
					GUI.color = Color.white;
				}

                aniEvent.intParameter = EditorGUILayout.IntField(aniEvent.intParameter, GUILayout.Width(50));
                nChangeCount = aniEvent.intParameter != paramInt ? nChangeCount + 1 : nChangeCount;
				if(GUI.color == Color.cyan)
				{
				}
				else if(originAniEvent != null &&originAniEvent.stringParameter != aniEvent.stringParameter){
					GUI.color = Color.yellow;
				}
				else {
					GUI.color = Color.white;
				}
                aniEvent.stringParameter = EditorGUILayout.TextField(aniEvent.stringParameter, GUILayout.Width(100));
                nChangeCount = aniEvent.stringParameter != paramString ? nChangeCount + 1 : nChangeCount;

				if(GUI.color == Color.cyan)
				{
				}
				else if(originAniEvent != null &&originAniEvent.objectReferenceParameter != aniEvent.objectReferenceParameter){
					GUI.color = Color.yellow;
				}
				else {
					GUI.color = Color.white;
				}
				aniEvent.objectReferenceParameter = EditorGUILayout.ObjectField(aniEvent.objectReferenceParameter,typeof(Object) ,false, GUILayout.Width(150));
				nChangeCount = aniEvent.objectReferenceParameter != paramObject ? nChangeCount + 1 : nChangeCount;

                if (GUILayout.Button("－", GUILayout.Width(20)))
                {
                    delEvent = aniEvent;
                }
				GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }

            if (delEvent != null)
            {
                lstEvent.Remove(delEvent);
                nChangeCount++;
            }

		
        }
    }

    private bool IsSearchAniEvent(AnimationEvent aniEvent)
    {
        int nSearch = 0;

        // serach event function
        if (string.IsNullOrEmpty(m_strFindKeyEvent) == true)
        {
            nSearch++;
        }
        else
        {
            if (UniEditorUtil.IsCompareIncludeStr(aniEvent.functionName, m_strFindKeyEvent, true) == true)
            {
                nSearch++;
            }
        }

        // serach event function parmater
        if (string.IsNullOrEmpty(m_strFindKeyParmater) == true)
        {
            nSearch++;
        }
        else
        {
            float fFloat = 0.0f;
            float.TryParse(m_strFindKeyParmater, out fFloat);
            if (true == float.Equals(fFloat, aniEvent.floatParameter))
            {
                nSearch++;
            }

            int nInt = 0;
            int.TryParse(m_strFindKeyParmater, out nInt);
            if (true == int.Equals(nInt, aniEvent.intParameter))
            {
                nSearch++;
            }

            if (UniEditorUtil.IsCompareIncludeStr(m_strFindKeyParmater, aniEvent.stringParameter, true) == true)
            {
                nSearch++;
            }
        }

        return (nSearch > 0);
    }
}