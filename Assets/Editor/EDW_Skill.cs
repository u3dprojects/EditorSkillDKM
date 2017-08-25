using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// 类名 : 技能编辑器窗口Class
/// 作者 : Canyon
/// 日期 : 2017-01-05 17:10
/// 功能 : 
/// </summary>
public class EDW_Skill : EditorWindow
{
    static bool isOpenWindowView = false;

    static protected EDW_Skill vwWindow = null;

	static int leftWidth = 200;
	static int midMinWidth = 200;
	static int rightWidth = 220;

    // 窗体宽高
	static public float width = leftWidth + midMinWidth + rightWidth + 40;
    static public float height = 480;

	[MenuItem("Tools/Windows/EDSkill",false,80)]
    static void AddWindow()
    {
        if (isOpenWindowView || vwWindow != null)
            return;

        try
        {
            isOpenWindowView = true;

            // 大小不能拉伸
            // vwWindow = GetWindowWithRect<EDW_Skill>(rect, true, "SkillEditor");
            
            // 窗口，只能单独当成一个窗口,大小可以拉伸
            //vwWindow = GetWindow<EDW_Skill>(true,"SkillEditor");

            // 这个合并到其他窗口中去,大小可以拉伸
            vwWindow = GetWindow<EDW_Skill>("SkillEditor");

			Vector2 pos = new Vector2(100,100);
			Vector2 size = new Vector2(width,height);
			Rect rect = new Rect(pos,size);
            vwWindow.position = rect;
			vwWindow.minSize = size;
        }
        catch (System.Exception)
        {
            ClearWindow();
            throw;
        }
    }

    static void ClearWindow()
    {
        isOpenWindowView = false;
        vwWindow = null;
    }

    static public float Round(float org, int acc)
    {
        float pow = Mathf.Pow(10, acc);
        float temp = org * pow;
        return Mathf.RoundToInt(temp) / pow;
    }

	static void MakeApplicationPlaying(){
		if (!EditorApplication.isPlaying) {
			EditorApplication.isPlaying = true;
		}
	}

    #region  == Member Attribute ===
	bool m_isNeedApplicationPlaying = false;
	Color m_defBgColor = new Color (0.3f, 0.3f, 0.3f);
    #endregion

    #region  == EditorWindow Func ===

    void Awake()
    {
        Init();
    }

    void OnEnable()
    {
        EditorApplication.update += OnUpdate;
    }

    void OnDisable()
    {
        EditorApplication.update -= OnUpdate;
    }

    void OnGUI()
    {
		if (m_isNeedApplicationPlaying) {
			if (!EditorApplication.isPlaying) {
				GUILayout.Space (10);
				EditorGUILayout.LabelField ("描述:", "需要运行工程!!!!", EditorStyles.textArea);
				GUILayout.Space (10);
				if (GUILayout.Button ("运行当前场景Scene???")) {
					MakeApplicationPlaying ();
				}
				return;
			}
		}

		// 左
		_DrawLeft();

		// 中
		// 右
		_DrawRight();
    }

    // 在给定检视面板每秒10帧更新
    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnDestroy()
    {
		if (m_isNeedApplicationPlaying) {
			EditorApplication.isPlaying = false;
		}

        ClearWindow();
        EditorApplication.update -= OnUpdate;

        Clear();
    }

    #endregion

    #region  == Self Func ===
    void Init()
    {
		if (m_isNeedApplicationPlaying) {
			MakeApplicationPlaying ();
		}
    }

    void OnUpdate()
    {
		Messenger.Brocast (MsgConst.OnUpdate);
    }

    void Clear()
    {
		Messenger.Brocast (MsgConst.OnClear);
    }
    #endregion

	#region  == GUI Draw Func ===
	/// <summary>
	/// 创建当前行对象的位置
	/// </summary>
	Rect CreateRect(ref int nX,int nY,int nWidth,int nHeight = 20){
		Rect rect = new Rect (nX, nY, nWidth, nHeight);
		nX += nWidth + 5;
		return rect;
	}

	/// <summary>
	/// 设置下一行的开始位置
	/// </summary>
	void NextLine(ref int nX,ref int nY,int addHeight = 30,int resetX = 10){
		nX = resetX;
		nY += addHeight;
	}
	#endregion

	#region == 绘制左边 ==
	int lastSelectId = 0;
	Vector2 v2LeftScroll = Vector2.zero;

	Color m_lfCellDefColor = new Color (0.2f, 0.2f, 0.2f);
	Color m_lfCellSelColor = new Color (0.4f, 0.4f, 0.4f);

	void _DrawLeft(){
		int curX = 10;
		int curY = 10;
		int height = (int)(position.height - curY - 10);
		EditorGUI.DrawRect (CreateRect (ref curX, curY, leftWidth, height), m_defBgColor);

		NextLine (ref curX, ref curY, 5);
		GUI.Label (CreateRect (ref curX, curY, leftWidth - 20, 25), "选择对象:");

		NextLine (ref curX, ref curY, 20,15);
		int wHalfBtn = Mathf.FloorToInt ((leftWidth - 20) * 0.5f);
		if (GUI.Button (CreateRect (ref curX, curY, wHalfBtn), "设置编辑对象")) {
			// 编辑地方数据缓存机制可以写xml，或者写json，或者读写txt文本
		}
		if (GUI.Button (CreateRect (ref curX, curY, wHalfBtn), "设置目标对象")) {
		}

		NextLine (ref curX, ref curY, 30);

		// 测试数据
		List<int> m_cacheDatas = new List<int> () {
			1,2,3,4,5,6,7,8,9,10
		};
		int lens = m_cacheDatas.Count;
		int contentHeight = lens * 40;

		v2LeftScroll = GUI.BeginScrollView (CreateRect (ref curX, curY, leftWidth, (height - curY - 40)), v2LeftScroll, new Rect (10, curY, leftWidth - 20, contentHeight),false,false);
		NextLine (ref curX, ref curY, 0,20);
		Rect cellRect,cellBtnDel;
		string strTitle = "";
		for (int i = 0; i < lens; i++) {
			cellRect = new Rect(curX,curY,leftWidth - 30,30);
			strTitle = string.Format("cell{0:000}",i);
			EditorGUI.DrawRect (cellRect, (lastSelectId == m_cacheDatas [i]) ? m_lfCellSelColor : m_lfCellDefColor);

			cellBtnDel = new Rect (curX + leftWidth - 30 - 42, curY, 40, 28);
			if (GUI.Button (cellBtnDel, "X")) {
				Debug.Log ("Delet " + strTitle);
			}

			if (Event.current.type == EventType.MouseUp) {
				Vector2 mousePos = Event.current.mousePosition;
				if (!cellBtnDel.Contains(mousePos) && cellRect.Contains (mousePos)) {
					lastSelectId = m_cacheDatas [i];
					OpenChangeRoleInfo (lastSelectId);
					Debug.Log ("Click " + strTitle);
				}
			}

			cellRect.x += 8;
			cellRect.width -= 8;
			cellRect.y += 6;
			cellRect.height -= 6;
			GUI.Label (cellRect, strTitle);

			NextLine (ref curX, ref curY, 40,20);
		}
		GUI.EndScrollView ();

		int botY = (int)(height - 40 + 20 * 0.5);
		if (botY > curY) {
			botY = curY;
		}
		if (GUI.Button (CreateRect (ref curX, botY, (leftWidth - 20),30), "新增目标对象")) {
			OpenChangeRoleInfo (0);
		}
	}

	void OpenChangeRoleInfo(int id){
	}
	#endregion

	#region == 绘制右边 ==
	int m_rtBarIndex = 0;
	string[] m_rtBarTitles = { "bar1", "bar2", "bar3", "bar4" };

	void _DrawRight(){
		int initX = (int)(position.width - rightWidth - 10);
		int initY = 10;
		int height = (int)(position.height - initY - 10);

		int curX = initX;
		int curY = initY;
		EditorGUI.DrawRect (CreateRect (ref curX, curY, rightWidth, height), m_defBgColor);

		NextLine (ref curX, ref curY, 0, initX);
		m_rtBarIndex = GUI.Toolbar (CreateRect (ref curX, curY, rightWidth, 30),m_rtBarIndex,m_rtBarTitles);
	}
	#endregion
}
