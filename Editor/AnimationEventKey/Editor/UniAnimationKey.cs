using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class UniAnimationKey: EditorWindow
{
    public enum EWindow
    {
        Menu = 0,
        Editor,
    }

    public enum EMenuPos
    {
        Left,
        Right
    }

    private static UniAnimationKey _THIS = null;

	private Vector2 m_v2MenuScrool = new Vector2(0, 0);
    private Vector2 m_v2EditorScrool = new Vector2(0, 0);

    private Dictionary<AnimationClip, bool> m_dicFoldout = new Dictionary<AnimationClip, bool>();
    private Dictionary<AnimationClip, List<AnimationEvent>> m_dicEditClip = new Dictionary<AnimationClip,List<AnimationEvent>>();
	private Dictionary<AnimationClip, List<AnimationEvent>> m_dicBackUp = new Dictionary<AnimationClip,List<AnimationEvent>>();

	// copy animation clip event
    private AnimationClip m_curCopyAniClip = null;
    private List<AnimationEvent> m_lstAniEventKeyCopy = new List<AnimationEvent>();

    // search animation clip name
    private int m_nSerachAnimationClipPopup = 0;   // default(0:All)
    private string m_serachAnimationClipString = string.Empty; // A string containing search

    private List<string> m_lstClipName = new List<string>();
	private List<string> m_lstKeyName = null;

	private string m_strEventFuntionAdd = string.Empty;
    private string m_strFindKeyEvent = string.Empty;
    private string m_strFindKeyParmater = string.Empty;

    
    private float m_fMenuWidth = 250.0f;
    private Rect m_rcMenu;
    private Rect m_rcEditor;

    [MenuItem("Window/Animation Event Key Editor")]
    public static void ShowWindow()
    {
		if (_THIS != null)
		{
			return;
		}

        _THIS = EditorWindow.GetWindow<UniAnimationKey>("AniEventKey");//, typeof(SceneView));
        _THIS.Init();
    }

    void OnDestroy()
    {
        if (null != _THIS)
        {
            _THIS.CloseWindow();
        }
        _THIS = null;
    }
	
	void Update()
	{
        if (_THIS == null)
        {
            UniAnimationKey.ShowWindow();
        }
	}
	
	void OnGUI()
	{
        UpdateWindowSize();

        BeginWindows();
        GUILayout.Window((int)EWindow.Menu, m_rcMenu, DoWindow, "Menu");
        GUILayout.Window((int)EWindow.Editor, m_rcEditor, DoWindow, "Editor");
        EndWindows();
	}

    void DoWindow(int nWindow)
    {
        EWindow eWindow = (EWindow)nWindow;

        switch (eWindow)
        {
        case EWindow.Menu:
            OnGUI_MenuNew();
            break;

        case EWindow.Editor:
            OnGUI_EventKeyEditor();
            break;
        }
    }

	public void Init()
	{
		if (m_dicFoldout == null) m_dicFoldout = new Dictionary<AnimationClip, bool>();
		m_dicFoldout.Clear();
			
		if (m_dicEditClip == null) m_dicEditClip = new Dictionary<AnimationClip, List<AnimationEvent>>();
		m_dicEditClip.Clear();
		
		if (m_dicBackUp == null) m_dicBackUp = new Dictionary<AnimationClip, List<AnimationEvent>>();
		m_dicBackUp.Clear();

		if (m_lstAniEventKeyCopy == null) m_lstAniEventKeyCopy = new List<AnimationEvent>();
		m_lstAniEventKeyCopy.Clear();

        if (m_lstClipName == null) m_lstClipName = new List<string>();
		m_lstClipName.Clear();
		
		if (m_lstKeyName == null) m_lstKeyName = new List<string>();
		m_lstKeyName.Clear();

        UpdateWindowSize();
	}

	public  void CloseWindow()
    {
        if (m_dicFoldout != null)
        {
            m_dicFoldout.Clear();
            m_dicFoldout = null;
        }

        if (m_dicEditClip != null)
        {
            var enumerator = m_dicEditClip.GetEnumerator();
            while(enumerator.MoveNext())
            {
                enumerator.Current.Value.Clear();
            }

            m_dicEditClip.Clear();
            m_dicEditClip = null;
        }

        if (m_dicBackUp != null)
        {
            var enumerator = m_dicBackUp.GetEnumerator();
            while(enumerator.MoveNext())
            {
                enumerator.Current.Value.Clear();
            }

            m_dicBackUp.Clear();
            m_dicBackUp = null;
        }

        if (m_lstAniEventKeyCopy != null)
        {
            m_lstAniEventKeyCopy.Clear();
            m_lstAniEventKeyCopy = null;
        }

        if (m_lstClipName != null)
        {
            m_lstClipName.Clear();
            m_lstClipName = null;
        }

        if (m_lstKeyName != null)
        {
            m_lstKeyName.Clear();
            m_lstKeyName = null;
        }
    }

    private void UpdateWindowSize()
    {
//        switch (m_eMenuPos)
//        {
//            case EMenuPos.Left:
//                {
                    m_rcMenu = new Rect(0.0f, 0.0f, m_fMenuWidth, this.position.height);
                    m_rcEditor = new Rect(m_rcMenu.x + m_rcMenu.width, 0.0f, this.position.width - m_rcMenu.width, this.position.height);
//                }
//                break;
//
//            case EMenuPos.Right:
//                {
//                    m_rcMenu = new Rect(this.position.width - m_fMenuWidth, 0.0f, m_fMenuWidth, this.position.height);
//                    m_rcEditor = new Rect(0.0f, 0.0f, this.position.width - m_fMenuWidth, this.position.height);
//                }
//                break;
//        }
    }

	private bool FindAnimationClip(ref Dictionary<string, AnimationClip> o_m_dicEditClip)
    {
        o_m_dicEditClip.Clear();

        Object[] objs = Selection.GetFiltered(typeof(AnimationClip), SelectionMode.Unfiltered);
        if (objs != null)
        {
            foreach (Object obj in objs)
            {
                AnimationClip clip = obj as AnimationClip;

				if (o_m_dicEditClip.ContainsKey(clip.name) == false)
				{
					o_m_dicEditClip.Add(clip.name, clip);
				}
            }
        }

        objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Unfiltered);
        if (objs != null)
        {
            foreach (Object obj in objs)
            {
                GameObject goSel = (GameObject)obj;

                Animation[] legacyAnimations = goSel.GetComponentsInChildren<Animation>(true);
                if (legacyAnimations != null)
                {
                    foreach (Animation legacyAnimation in legacyAnimations)
                    {
                        if (legacyAnimation == null)
                        {
                            continue;
                        }

						foreach (AnimationState state in legacyAnimation)
						{
							if (state == null)
							{
								continue;
							}
							
							if (o_m_dicEditClip.ContainsKey(state.clip.name) == false)
							{
								o_m_dicEditClip.Add(state.clip.name, state.clip);
							}
						}
                    }
                }


                Animator[] animators = goSel.GetComponentsInChildren<Animator>(true);
                if (animators != null)
                {
                    foreach (Animator animator in animators)
                    {
                        if (animator == null)
                        {
                            continue;
                        }

                        AnimatorOverrideController overrideController = new AnimatorOverrideController();
                        overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;

                        for (int index = 0; index < overrideController.clips.Length; index++)
                        {
                            AnimationClipPair clipPair = overrideController.clips[index];

							if (o_m_dicEditClip.ContainsKey(clipPair.originalClip.name) == false)
							{
								o_m_dicEditClip.Add(clipPair.originalClip.name, clipPair.originalClip);
							}
                        }
                    }
                }

            }
        }

        return o_m_dicEditClip.Count > 0 ? true : false;
    }

	private void AddBackUp(AnimationClip i_clip, ref List<AnimationEvent> i_lstEvent)
	{
		if (i_lstEvent == null)
		{
			return;
		}
		
		List<AnimationEvent> lstItem = new List<AnimationEvent>();
		
		if (i_lstEvent != null)
		{
			foreach (AnimationEvent item in i_lstEvent)
			{
				AnimationEvent aniEvent = UniEditorUtil.CopyAnimationEvent(item);
				if (aniEvent == null)
				{
					continue;
				}
				lstItem.Add(aniEvent);
			}
		}
		
		if (m_dicBackUp.ContainsKey(i_clip) == false)
		{
			m_dicBackUp.Add(i_clip, lstItem);
		}
		else
		{
			throw new System.Exception(string.Format("BackUpItem Overlap Key Find : {0}", i_clip.name));
		}
	}

	private int GetKeyNameIndex(string i_keyName)
	{
		if (m_lstKeyName == null)
		{
			return -1;
		}

		for (int nIndex = 0; nIndex < m_lstKeyName.Count; nIndex++)
		{
			string strName = m_lstKeyName[nIndex];
			if (string.Compare(strName, i_keyName, true) == 0)
			{
				return nIndex;
			}
		}

		return -1;
	}
	
	private void SortAniClipList()
	{
		if (m_dicEditClip == null || m_dicBackUp == null)
		{
			return;
		}
		
		List<AnimationClip> lstSort = new List<AnimationClip>();
		
		foreach (KeyValuePair<AnimationClip, List<AnimationEvent>> each in m_dicEditClip)
		{
			lstSort.Add(each.Key);
		}
		
		lstSort.Sort(delegate(AnimationClip a, AnimationClip b)
		{
			return string.Compare(a.name, b.name);
		});
		
		m_dicEditClip.Clear();
		
		foreach (AnimationClip clip in lstSort)
		{
			foreach (KeyValuePair<AnimationClip, List<AnimationEvent>> each in m_dicBackUp)
			{
				List<AnimationEvent> lstSource = each.Value;
				
				if (each.Key == clip)
				{
					List<AnimationEvent> lstDest = new List<AnimationEvent>();
					
					foreach (AnimationEvent item in lstSource)
					{
						AnimationEvent aniEvent = UniEditorUtil.CopyAnimationEvent(item);
						if (aniEvent == null)
						{
							continue;
						}
						lstDest.Add(aniEvent);
					}

					m_dicEditClip.Add(each.Key, lstDest);
				}
			}
		}
	}

    private static void SortTime(ref List<AnimationEvent> i_list)
    {
        if (i_list == null)
        {
            return;
        }

        int nCount = 0;

        int nTotalCount = i_list.Count;

        i_list.Sort(delegate(AnimationEvent a, AnimationEvent b)
        {
            string strCompare = "=";
            if (a.time > b.time)
            {
                strCompare = ">";
            }
            else if (a.time < b.time)
            {
                strCompare = "<";
            }
            else { }

            nCount++;
            UniEditorUtil.DisplayProgressBar("AniEvent Sort", string.Format("{0} {1} {2}", a.time, strCompare, b.time), nCount, nTotalCount);

            return a.time.CompareTo(b.time);
        });

        EditorUtility.ClearProgressBar();
    }
}
