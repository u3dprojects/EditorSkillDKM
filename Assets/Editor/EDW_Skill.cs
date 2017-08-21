using UnityEngine;
using System.Collections;
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
	static public float width = leftWidth + midMinWidth + rightWidth + 30;
    static public float height = 480;

    [MenuItem("Tools/Windows/EDSkill")]
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
            OnClearSWindow();
            throw;
        }
    }

    static void OnClearSWindow()
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

	static void CheckApplicationPlaying(){
		if (!EditorApplication.isPlaying) {
			EditorApplication.isPlaying = true;
		}
	}

    #region  == Member Attribute ===

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
        
    }

    // 在给定检视面板每秒10帧更新
    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnDestroy()
    {
        OnClearSWindow();
        EditorApplication.update -= OnUpdate;

        Clear();
    }

    #endregion

    #region  == Self Func ===

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

    void Init()
    {
    }

    void OnUpdate()
    {
    }

    void Clear()
    {
    }
    #endregion
}
