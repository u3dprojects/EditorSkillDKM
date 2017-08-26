using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;

/// <summary>
/// 类名 : 目标信息
/// 作者 : Canyon
/// 日期 : 2017-08-15 10:10
/// 功能 : 
/// </summary>
public class ED_RoleInfo {
	/// <summary>
	/// 标识
	/// </summary>
	public int m_id = 0;

	/// <summary>
	/// 名字
	/// </summary>
	public string m_name = "";

	/// <summary>
	/// 骨骼
	/// </summary>
	public string m_bodyBone = "";

	/// <summary>
	/// 蒙皮 - 身体
	/// </summary>
	public string m_skin_body = "";

	/// <summary>
	/// 蒙皮 - 头部
	/// </summary>
	public string m_skin_head = "";

	/// <summary>
	/// 武器 - 左
	/// </summary>
	public string m_weapon_left = "";

	/// <summary>
	/// 武器 - 左挂节点
	/// </summary>
	public string m_weapon_left_bonePoint = "";

	/// <summary>
	/// 武器 - 右
	/// </summary>
	public string m_weapon_right = "";

	/// <summary>
	/// 武器 - 右挂节点
	/// </summary>
	public string m_weapon_right_bonePoint = "";

	/// <summary>
	/// 技能IDS
	/// </summary>
	public List<int> m_lSkillIds = new List<int>();
}

/// <summary>
/// 类名 : 目标数据
/// 作者 : Canyon
/// 日期 : 2017-08-15 10:10
/// 功能 : 
/// </summary>
public class ED_RoleData : ScriptableObject{
	public Dictionary<int,ED_RoleInfo> m_map = new Dictionary<int, ED_RoleInfo>();
}

/// <summary>
/// 类名 : 管理目标数据
/// 作者 : Canyon
/// 日期 : 2017-08-15 10:10
/// 功能 : 
/// </summary>
public class MgrRoleDatas{
	static MgrRoleDatas _instance;
	static public MgrRoleDatas instance{
		get{
			if (_instance == null) {
				_instance = new MgrRoleDatas ();
				_instance.Init();
			}
			return _instance;
		}
	}

	// 数据
	ED_RoleData m_data;

	// 输出的列表
	List<ED_RoleInfo> m_outList = new List<ED_RoleInfo>();

	// 缓存路径
	string path = "Assets/Editor/Data/dRoleInfo.asset";

	int m_iMaxId = 1;

	public void Init(){
		FileInfo fileInfo = new FileInfo (path);
		if (fileInfo.Exists) {
			m_data = AssetDatabase.LoadAssetAtPath<ED_RoleData> (path);
		} else {
			m_data = ScriptableObject.CreateInstance<ED_RoleData> ();
		}

		foreach (var item in m_data.m_map) {
			if (item.Value.m_id > m_iMaxId)
				m_iMaxId = item.Value.m_id;
		}
	}

	public ED_RoleInfo GetInfo(int id){
		if (m_data.m_map.ContainsKey (id)) {
			return m_data.m_map [id];
		}
		return null;
	}

	public void RemoveInfo(int id){
		m_data.m_map.Remove (id);
	}

	public void RemoveInfo(ED_RoleInfo info){
		RemoveInfo(info.m_id);
	}

	public void AddInfo(ED_RoleInfo info){
		int key = info.m_id;
		if (key <= 0) {
			key = (++m_iMaxId);
		}

		bool isHas = m_data.m_map.ContainsKey (key);
		if (isHas) {
			m_data.m_map [key] = info;
		} else {
			info.m_id = key;
			m_data.m_map.Add (key, info);
		}

		if (key > m_iMaxId)
			m_iMaxId = key;
	}

	public void Save(){
		FileInfo fileInfo = new FileInfo (path);
		if (fileInfo.Exists) {
			// fileInfo.Delete ();
		}

		AssetDatabase.CreateAsset (m_data, path);
		AssetDatabase.Refresh ();
	}

	public List<ED_RoleInfo> GetAll(){
		m_outList.Clear ();
		m_outList.AddRange (m_data.m_map.Values);
		return m_outList;
	}
}