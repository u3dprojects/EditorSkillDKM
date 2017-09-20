using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// 类名 : 技能编辑器窗口Class
/// 作者 : Canyon
/// 日期 : 2017-08-15 09:50
/// 功能 : 
/// </summary>
public class EDW_Skill : EditorWindow
{
    static bool isOpenWindowView = false;

    static protected EDW_Skill vwWindow = null;

	static int leftWidth = 200;
	static int midMinWidth = 300;
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
		SceneView.onSceneGUIDelegate += OnSceneGUI;
		Messenger.AddListener (MsgConst.OnRepantEditorWindow, CallFuncRepaint);
    }

    void OnDisable()
    {
        EditorApplication.update -= OnUpdate;
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
		Messenger.RemoveListener (MsgConst.OnRepantEditorWindow, CallFuncRepaint);
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
		_DrawMiddle();

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

	float disCaster = 10;
	float disMove = 5;
	void OnSceneGUI(SceneView sceneView){
		Vector3 start = Vector3.zero;
		start.y = 0;
		Vector3 dir = Vector3.forward;

		EG_HandlesHelper.AreaType areaType = EG_HandlesHelper.AreaType.Arc;
		float otherVal = 60;
		if(areaType == EG_HandlesHelper.AreaType.Rectangle){
			otherVal = 5;
		}

		EG_HandlesHelper.Draw (ref disMove, ref disCaster, start,dir,areaType,otherVal);
	}

	void CallFuncRepaint(){
		OnInspectorUpdate ();
	}
    #endregion

	#region  == GUI Draw Func ===
	/// <summary>
	/// 创建当前行对象的位置
	/// </summary>
	static public Rect CreateRect(ref int nX,int nY,int nWidth,int nHeight = 20){
		Rect rect = new Rect (nX, nY, nWidth, nHeight);
		nX += nWidth + 5;
		return rect;
	}

	/// <summary>
	/// 设置下一行的开始位置
	/// </summary>
	static public void NextLine(ref int nX,ref int nY,int addHeight = 30,int resetX = 10){
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
		int scrollHeight = (height - curY - 40);

		v2LeftScroll = GUI.BeginScrollView (CreateRect (ref curX, curY, leftWidth, scrollHeight), v2LeftScroll, new Rect (10, curY, leftWidth - 20, contentHeight),false,false);
		NextLine (ref curX, ref curY, 0,20);
		Rect cellRect,cellBtnDel;
		string strTitle = "";
		for (int i = 0; i < lens; i++) {
			cellRect = new Rect(curX,curY,leftWidth - 30,30);
			strTitle = string.Format("Role{0:000}",i);
			EditorGUI.DrawRect (cellRect, (lastSelectId == m_cacheDatas [i]) ? m_lfCellSelColor : m_lfCellDefColor);

			cellBtnDel = new Rect (curX + leftWidth - 30 - 42, curY, 40, 28);
			if (GUI.Button (cellBtnDel, "X")) {
				Debug.Log ("Delet " + strTitle);
			}

			if (Event.current.type == EventType.MouseUp) {
				Vector2 mousePos = Event.current.mousePosition;
				if (!cellBtnDel.Contains(mousePos) && cellRect.Contains (mousePos)) {
					if (lastSelectId == m_cacheDatas [i]) {
						OpenChangeRoleInfo (lastSelectId);
					}

					lastSelectId = m_cacheDatas [i];
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
		EDW_AddRoleInfo.AddWindow ().Init (id);
	}
	#endregion

	#region == 绘制中间 ==
	int m_midBtnWidth = 75;
	Vector2 v2MidBtnScroll = Vector2.zero;
	int lastSelectSkillId = -1;
	EG_SkillTimeLine timeLine = new EG_SkillTimeLine ();

	void _DrawMiddle(){
		int width = (int)(position.width - leftWidth - rightWidth - 40);
		int initX = leftWidth + 20;
		int initY = 10;
		int height = (int)(position.height - initY - 10);

		int curX = initX;
		int curY = initY;
		// EditorGUI.DrawRect (CreateRect (ref curX, curY, width, height), Color.gray);

		NextLine (ref curX, ref curY, 5, initX + 2);
		GUI.Label(CreateRect(ref curX,curY,width - 5),"拥有技能列表:");

		// 测试数据
		List<int> m_cacheDatas = new List<int> () {
			1101,1102,1103,1104,1105,1106,1107,1108,1109,1110,
			2101,2102,2103,2104,2105,2106,2107,2108,2109,2110,
			3101,3102,3103,3104,3105,3106,3107,3108,3109,3110,
		};

		int lens = m_cacheDatas.Count;

		int colunm = Mathf.FloorToInt ((float)width / m_midBtnWidth);
		int row = Mathf.CeilToInt ((float)lens / colunm);

		NextLine (ref curX, ref curY, 20, initX);
		Rect scrollRect = new Rect (curX, curY, width, 150);
		Rect contentRect = new Rect (curX, curY, width - 20, row * 30);

		v2MidBtnScroll = GUI.BeginScrollView (scrollRect, v2MidBtnScroll,contentRect,false,false);

		int curBtnX = curX;
		int curBtnY = curY;
		Rect cellRect;
		int skillID = -1;
		for (int i = 0; i < lens; i++) {
			if (i > 0 && i % colunm == 0) {
				NextLine (ref curBtnX, ref curBtnY, 30,initX);
			}

			skillID = m_cacheDatas [i];
			cellRect = CreateRect(ref curBtnX,curBtnY,70,25);
			if (lastSelectSkillId == skillID) {
				cellRect.y -= 2;
				cellRect.height += 5;
				EditorGUI.DrawRect (cellRect, Color.magenta);
				cellRect.y += 2;
				cellRect.height -= 5;
			}

			if(GUI.Button(cellRect,GetSkillName(skillID))){
				lastSelectSkillId = skillID;
				Debug.Log ("Click " + skillID);
			}
		}
		GUI.EndScrollView ();

		// 绘制时间线
		NextLine (ref curX, ref curY, 152, initX);
		timeLine.DrawView (ref curX, ref curY, width, height + initY - curY);
	}

	string GetSkillName(int skillID){
		return skillID.ToString ();
	}
	#endregion

	#region == 绘制右边 ==
	int m_rtBarIndex = 0;
	string[] m_rtBarTitles = { "技能表", "CGCamera", "bar3", "bar4" };
	EG_SkillCGCamera m_cgCamera = new EG_SkillCGCamera();

	void _DrawRight(){
		int initX = (int)(position.width - rightWidth - 10);
		int initY = 10;
		int height = (int)(position.height - initY - 10);

		int curX = initX;
		int curY = initY;
		EditorGUI.DrawRect (CreateRect (ref curX, curY, rightWidth, height), m_defBgColor);

		NextLine (ref curX, ref curY, 0, initX);
		m_rtBarIndex = GUI.Toolbar (CreateRect (ref curX, curY, rightWidth, 30),m_rtBarIndex,m_rtBarTitles);

		NextLine (ref curX, ref curY, 32, initX);
		switch (m_rtBarIndex) {
		case 1:
			m_cgCamera.DrawView (ref curX, ref curY, rightWidth, height);
			break;
		default:
			break;
		}
	}
	#endregion
}

public partial class MsgConst{
	public const string OnRepantEditorWindow = "Msg_OnRepantEditorWindow";
}