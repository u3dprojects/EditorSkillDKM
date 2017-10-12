using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// 类名 : 创建缓存角色窗口Class
/// 作者 : Canyon
/// 日期 : 2017-08-15 16:50
/// 功能 : 
/// </summary>
public class EDW_AddRoleInfo : EditorWindow
{
    static bool isOpenWindowView = false;

    static protected EDW_AddRoleInfo vwWindow = null;

    // 窗体宽高
	static public float width = 480;
    static public float height = 360;

	static public EDW_AddRoleInfo AddWindow()
    {
        if (isOpenWindowView || vwWindow != null)
			return vwWindow;

        try
        {
			isOpenWindowView = true;
            vwWindow = GetWindow<EDW_AddRoleInfo>("添加RoleInfo");

			Vector2 pos = new Vector2(300,300);
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

		return vwWindow;
    }

    static void ClearWindow()
    {
        isOpenWindowView = false;
        vwWindow = null;
    }

    #region  == Member Attribute ===
	ED_RoleInfo m_data;

	int m_id = 0;

	bool isChanged = false;
    #endregion

    #region  == EditorWindow Func ===

    void Awake()
    {
        Init();
    }

    void OnGUI()
    {
		if (m_data == null)
			return;
		
		_DrawView ();
    }

    // 在给定检视面板每秒10帧更新
    void OnInspectorUpdate()
    {
        Repaint();
    }

	void OnLostFocus(){
		this.Focus ();
	}

    void OnDestroy()
    {
        ClearWindow();
        Clear();
		MgrRoleDatas.instance.Save ();
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

    #region  == Self Func ===
    void Init()
    {
    }

    void Clear()
    {
		Messenger.Brocast (MsgConst.OnClear);
    }

	ED_RoleInfo GetByID(int id){
		return MgrRoleDatas.instance.GetInfo (id);
	}

	void SaveOrAdd(){
		m_data.m_id = m_id;
		MgrRoleDatas.instance.AddInfo (m_data);
	}

	public void Init(int id){
		m_id = id;
		m_data = GetByID (m_id);
		if (m_data == null) {
			m_data = new ED_RoleInfo ();
			m_data.m_id = m_id;
		}
	}

	public void LoadObj(int id,ref GameObject gobj,string name){
		ED_RoleInfo info = GetByID (id);
		if (info == null) {
			EditorUtility.DisplayDialog ("提示", "请选择有效的角色信息", "确定");
			return;
		}

		if (gobj != null)
			GameObject.Destroy (gobj);

		gobj = new GameObject (name);

		// GameObject gobjBody;
		Dictionary<string,Transform> boneMap = GetBones (gobj);
	}

	public Dictionary<string,Transform> GetBones(GameObject gobj,Dictionary<string,Transform> dic = null){
		if (dic == null) {
			dic = new Dictionary<string, Transform> ();
		}
		dic.Clear ();

		Transform[] trsfArrs = gobj.GetComponentsInChildren<Transform> (true);
		int lens = trsfArrs.Length;
		Transform trsf;
		for (int i = 0; i < lens; i++) {
			trsf = trsfArrs [i];
			if (!dic.ContainsKey (trsf.name)) {
				dic.Add (trsf.name, trsf);
			}
		}
		return dic;
	}

	public void ReBindBones(GameObject gobj,Dictionary<string,Transform> dic){
		SkinnedMeshRenderer[] skinArrs = gobj.GetComponentsInChildren<SkinnedMeshRenderer> (true);
		int lens = skinArrs.Length;
		SkinnedMeshRenderer skin;
		List<Transform> trsfBones = new List<Transform> ();
		string keyBone = "";
		for (int i = 0; i < lens; i++) {
			skin = skinArrs [i];
			if (skin.rootBone != null) {
				keyBone = skin.rootBone.name;
				if (dic.ContainsKey (keyBone)) {
					skin.rootBone = dic [keyBone];
				}
			}
			if (skin.bones != null) {
				trsfBones.Clear ();
				int lensBone = skin.bones.Length;
				for (int j = 0; j < lensBone; j++) {
					keyBone = skin.bones[i].name;
					if (dic.ContainsKey (keyBone)) {
						trsfBones.Add(dic [keyBone]);
					}
				}
				skin.bones = trsfBones.ToArray ();
			}
		}
	}

    #endregion

	void _DrawView(){
		int curX = 10;
		int curY = 10;
		int nWidth = (int)position.width;

		GUI.Label(CreateRect (ref curX, curY, 30), "ID:");
		m_id = EditorGUI.IntField (CreateRect (ref curX, curY, 80), m_id);

		GUI.Label(CreateRect (ref curX, curY, 30), "名称:");
		string _name = m_data.m_name;
		m_data.m_name = EditorGUI.TextField (CreateRect (ref curX, curY, 80), m_data.m_name);
		if (m_data.m_name != _name)
			isChanged = true;

		if (GUI.Button (CreateRect (ref curX, curY, 60), "读取")) {
			if (isChanged) {
				if (EditorUtility.DisplayDialog ("Tips", "是否保存已对当前对象进行的修改?", "确定", "取消")) {
					SaveOrAdd ();
				}
				isChanged = false;
			}

			Init (m_id);
		}

		if (GUI.Button (CreateRect (ref curX, curY, 60), "保存")) {
			isChanged = false;
			SaveOrAdd ();
		}

		NextLine (ref curX, ref curY, 30);
		if (GUI.Button (CreateRect (ref curX, curY, 80), "选择骨骼")) {
			isChanged = GetResPath ("选择骨骼", ref m_data.m_bodyBone);
		}
		GUI.Label(CreateRect (ref curX, curY, nWidth - 85), m_data.m_bodyBone);

		NextLine (ref curX, ref curY, 30);
		if (GUI.Button (CreateRect (ref curX, curY, 80), "身体蒙皮")) {
			isChanged = GetResPath ("身体蒙皮", ref m_data.m_skin_body);
		}
		GUI.Label(CreateRect (ref curX, curY, nWidth - 85), m_data.m_skin_body);

		NextLine (ref curX, ref curY, 30);
		if (GUI.Button (CreateRect (ref curX, curY, 80), "头部蒙皮")) {
			isChanged = GetResPath ("头部蒙皮", ref m_data.m_skin_head);
		}
		GUI.Label(CreateRect (ref curX, curY, nWidth - 85), m_data.m_skin_head);

		NextLine (ref curX, ref curY, 30);
		if (GUI.Button (CreateRect (ref curX, curY, 80), "武器(左)")) {
			isChanged = GetResPath ("武器(左)", ref m_data.m_weapon_left);
		}
		GUI.Label(CreateRect (ref curX, curY, nWidth - 85), m_data.m_weapon_left);

		NextLine (ref curX, ref curY, 30);
		if (GUI.Button (CreateRect (ref curX, curY, 80), "武器(左)挂点")) {
		}

		string oldPoint = m_data.m_weapon_left_bonePoint;
		m_data.m_weapon_left_bonePoint = EditorGUI.TextArea(CreateRect (ref curX, curY, nWidth - 85), m_data.m_weapon_left_bonePoint);
		if (!oldPoint.Equals (m_data.m_weapon_left_bonePoint)) {
			isChanged = true;
		}

		NextLine (ref curX, ref curY, 30);
		if (GUI.Button (CreateRect (ref curX, curY, 80), "武器(右)")) {
			isChanged = GetResPath ("武器(右)", ref m_data.m_weapon_right);
		}
		GUI.Label(CreateRect (ref curX, curY, nWidth - 85), m_data.m_weapon_right);

		NextLine (ref curX, ref curY, 30);
		if (GUI.Button (CreateRect (ref curX, curY, 80), "武器(右)挂点")) {
		}

		oldPoint = m_data.m_weapon_right_bonePoint;
		m_data.m_weapon_right_bonePoint = EditorGUI.TextArea(CreateRect (ref curX, curY, nWidth - 85), m_data.m_weapon_right_bonePoint);
		if (!oldPoint.Equals (m_data.m_weapon_right_bonePoint)) {
			isChanged = true;
		}
	}

	bool GetResPath(string title,ref string value){
		string str = EditorUtility.OpenFilePanel (title, "", "");
		if (!string.IsNullOrEmpty (str)) {
			str = str.Replace ("\\", "/");
			string fst = "Build";
			int index = str.IndexOf (fst);
			if (index >= 0) {
				str = str.Substring (index + fst.Length);
				value = str.Substring (str.IndexOf ("/") + 1);
				return true;
			}
		}
		return false;
	}
}
