using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 类名 : 时间线 - CGCamera 镜头动画
/// 作者 : Canyon
/// 日期 : 2017-08-18 11:30
/// 功能 : 
/// </summary>
public class EG_SkillCGCamera{

	public EG_SkillCGCamera(){
		Messenger.AddListener(MsgConst.OnUpdate, OnUpdate);
	}

	~EG_SkillCGCamera(){
		Messenger.RemoveListener(MsgConst.OnUpdate, OnUpdate);
	}

	#region == 摄像机数据 ==
	Transform _trsfCamera;
	Camera m_camera;

	Vector3 m_v3DefCameraAngle = Vector3.zero;
	Vector3 m_v3DefCameraPos = Vector3.zero;
	float m_fDefCeameraFieldOfView = 60;

	Transform m_trsfCamera{
		get{
			if (_trsfCamera == null) {
				GameObject tmp = GameObject.FindGameObjectWithTag ("MainCamera");
				if (tmp == null) {
					tmp = new GameObject ("Main Camera", typeof(Camera));
					tmp.tag = "MainCamera";
				}
				_trsfCamera = tmp.transform;

				m_v3DefCameraAngle = _trsfCamera.eulerAngles;
				m_v3DefCameraPos = _trsfCamera.position;

				m_camera = _trsfCamera.GetComponent<Camera> ();
				m_fDefCeameraFieldOfView = m_camera.fieldOfView;
			}
			return _trsfCamera;
		}
	}
	#endregion

	#region === 绘制 ===
	Rect CreateRect(ref int nX,int nY,int nWidth,int nHeight = 20){
		Rect rect = new Rect (nX, nY, nWidth, nHeight);
		nX += nWidth + 5;
		return rect;
	}

	void NextLine(ref int nX,ref int nY,int addHeight = 30,int resetX = 10){
		nX = resetX;
		nY += addHeight;
	}

	// 初始数据
	int nInitX = 0;
	// int nInitY = 0;
	int nWidth = 0;
	// int nHeight = 0;

	List<EDT_CGCamera> m_list = new List<EDT_CGCamera> ();

	SortEDT_Line<EDT_CGCamera> m_sort = new SortEDT_Line<EDT_CGCamera> ();

	bool isSetCGCamera = false;

	bool isRunning = false;

	EDT_CGCamera GetCGCameraByFrame(int frame){
		int lens = m_list.Count;
		for (int i = 0; i < lens; i++) {
			if (m_list [i].m_frame == frame)
				return m_list [i];
		}
		return null;
	}

	public void DrawView(ref int curX,ref int curY,int width,int height){
		nInitX = curX;
		// nInitY = curY;
		nWidth = width;
		// nHeight = height;

		NextLine (ref curX, ref curY, 0, nInitX + 2);
		GUI.Label (CreateRect (ref curX, curY, 60), "当前帧:");
		int _curFame = EDT_Line.curFrame;
		EDT_Line.curFrame = EditorGUI.IntField (CreateRect (ref curX, curY, 45), EDT_Line.curFrame);
		if (_curFame != EDT_Line.curFrame) {
			// 滚动到当前帧
			// Messenger.Brocast(MsgConst.CurrFrame2TimeLineScrollPos);
		}

		if (GUI.Button (CreateRect (ref curX, curY, 90), "查询")) {
			EDT_CGCamera data = GetCGCameraByFrame (EDT_Line.curFrame);
			if (data != null) {
				m_angleHorizontal = data.m_angleHorizontal;
				m_angleVertical = data.m_angleVertical;
				m_distance = data.m_distance;
				m_offsetY = data.m_offsetY;
				m_fov = data.m_fov;
			}
		}

		NextLine (ref curX, ref curY, 30, nInitX + 2);
		GUI.Label (CreateRect (ref curX, curY, 60), "变换镜头:");
		isSetCGCamera = EditorGUI.Toggle (CreateRect (ref curX, curY, 150), isSetCGCamera);
		if (isSetCGCamera) {
			Calc (m_trsfCamera, cameraTargetPos);
		}

		NextLine (ref curX, ref curY, 30, nInitX + 2);
		GUI.Label (CreateRect (ref curX, curY, 60), "目标对象:");
		m_trsfCameraTarget = EditorGUI.ObjectField (CreateRect (ref curX, curY, 150),m_trsfCameraTarget,typeof(Transform),true) as Transform;

		NextLine (ref curX, ref curY, 30, nInitX + 2);
		GUI.Label (CreateRect (ref curX, curY, 60), "距离:");
		m_distance = EditorGUI.Slider (CreateRect (ref curX, curY, 150), m_distance, 0.01f, 30);

		NextLine (ref curX, ref curY, 30, nInitX + 2);
		GUI.Label (CreateRect (ref curX, curY, 60), "视野:");
		m_fov = EditorGUI.Slider (CreateRect (ref curX, curY, 150), m_fov, 1f, 179);

		NextLine (ref curX, ref curY, 30, nInitX + 2);
		GUI.Label (CreateRect (ref curX, curY, 60), "偏移Y:");
		m_offsetY = EditorGUI.Slider (CreateRect (ref curX, curY, 150), m_offsetY, 0.1f, 50);

		NextLine (ref curX, ref curY, 30, nInitX + 2);
		GUI.Label (CreateRect (ref curX, curY, 60), "水平角度:");
		m_angleHorizontal = EditorGUI.Slider (CreateRect (ref curX, curY, 150), m_angleHorizontal, 0, 360);

		NextLine (ref curX, ref curY, 30, nInitX + 2);
		GUI.Label (CreateRect (ref curX, curY, 60), "垂直角度:");
		m_angleVertical = EditorGUI.Slider (CreateRect (ref curX, curY, 150), m_angleVertical, 0, 360);

		NextLine (ref curX, ref curY, 30, nInitX + 2);
		if (GUI.Button (CreateRect (ref curX, curY, nWidth / 2 - 6), "保存")) {
			ChangeList (true);
		}
		if (GUI.Button (CreateRect (ref curX, curY, nWidth / 2 - 6), "删除")) {
			ChangeList (false);
		}

		NextLine (ref curX, ref curY, 30, nInitX + 2);
		if (GUI.Button (CreateRect (ref curX, curY, nWidth / 2 - 6), "运行动画")) {
			PreInit ();
			isRunning = true;
		}
		if (GUI.Button (CreateRect (ref curX, curY, nWidth / 2 - 6), "清空动画")) {
			m_list.Clear ();
			m_queue.Clear ();
			isRunning = false;
		}
	}

	void ChangeList(bool isAdd){
		int _curFrame = EDT_Line.curFrame;
		EDT_CGCamera val = GetCGCameraByFrame (_curFrame);
		if (isAdd) {
			if (val == null) {
				val = new EDT_CGCamera ();
				m_list.Add (val);
			}
			val.SetFrame (_curFrame);
			val.m_angleHorizontal = m_angleHorizontal;
			val.m_angleVertical = m_angleVertical;
			val.m_distance = m_distance;
			val.m_offsetY = m_offsetY;
			val.m_fov = m_fov;
		} else {
			if (val != null) {
				m_list.Remove (val);
			}
		}
	}
	#endregion

	// 摄像机目标对象
	Transform m_trsfCameraTarget;

	Queue<EDT_CGCamera> m_queue = new Queue<EDT_CGCamera>();

	EDT_CGCamera begData,nextData;

	public float m_angleHorizontal = 60;
	public float m_angleVertical = 30;
	public float m_distance = 1;
	public float m_offsetY = 0.5f;
	public float m_fov = 60;

	bool isInit = false;

	EN_Time m_time = new EN_Time();

	Vector3 cameraTargetPos{
		get{
			if (m_trsfCameraTarget) {
				return m_trsfCameraTarget.position;
			}
			return Vector3.forward;
		}
	}

	public void PreInit(){
		m_list.Sort (m_sort);
		m_queue.Clear ();
		for (int i = 0; i < m_list.Count; i++) {
			m_queue.Enqueue (m_list [i]);
		}
		m_time.Reset ();

		Init ();
	}

	public void Init(){
		if (m_queue.Count <= 1)
			return;

		isInit = true;

		begData = m_queue.Dequeue ();
		nextData = m_queue.Dequeue ();

		m_angleHorizontal = begData.m_angleHorizontal;
		m_angleVertical = begData.m_angleVertical;
		m_distance = begData.m_distance;
		m_offsetY = begData.m_offsetY;
		m_fov = begData.m_fov;

		Calc (m_trsfCamera, cameraTargetPos);
	}

	void OnUpdate(){
		if (!isInit || !isRunning)
			return;
		m_time.OnUpdateTime ();
		OnUpdate (m_time.progressTime);
	}

	void OnUpdate(float timeSecond){
		if (!isInit || !isRunning)
			return;
		
		int _curFrame = EDT_CGCamera.ToFrame (timeSecond);
		if (_curFrame > nextData.m_frame) {
			if (m_queue.Count > 0) {
				begData = nextData;
				nextData = m_queue.Dequeue ();
			} else {
				return;
			}
		}

		float lerpTime = (float)(_curFrame - begData.m_frame) / (nextData.m_frame - begData.m_frame);
		lerpTime = Mathf.Max (lerpTime, 0);

		m_angleHorizontal = Mathf.Lerp(begData.m_angleHorizontal,nextData.m_angleHorizontal,lerpTime);
		m_angleVertical = Mathf.Lerp(begData.m_angleVertical,nextData.m_angleVertical,lerpTime);
		m_distance = Mathf.Lerp(begData.m_distance,nextData.m_distance,lerpTime);
		m_offsetY = Mathf.Lerp(begData.m_offsetY,nextData.m_offsetY,lerpTime);
		m_fov = Mathf.Lerp(begData.m_fov,nextData.m_fov,lerpTime);

		Calc (m_trsfCamera, cameraTargetPos);
	}

	/// <summary>
	/// 核心计算方法
	/// </summary>
	public void Calc(Transform trsfCamera,Vector3 cameraTargetPos){

		float fPitch = m_angleVertical * Mathf.Deg2Rad;
		float fYaw = m_angleHorizontal * Mathf.Deg2Rad;

		Vector3 vPos = cameraTargetPos;
		vPos.x += m_distance * Mathf.Cos (fPitch) * Mathf.Cos (fYaw);
		vPos.y += m_distance * Mathf.Sin (fPitch) + m_offsetY;
		vPos.z += m_distance * Mathf.Cos (fPitch) * Mathf.Sin (fYaw);

		trsfCamera.position = vPos;

		cameraTargetPos.y += m_offsetY;
		trsfCamera.LookAt (cameraTargetPos);

		m_camera.fieldOfView = m_fov;
	}

	/// <summary>
	/// 还原摄像头
	/// </summary>
	public void ReBackCamera(){
		m_trsfCamera.eulerAngles = m_v3DefCameraAngle;
		m_trsfCamera.position = m_v3DefCameraPos;
		m_camera.fieldOfView = m_fDefCeameraFieldOfView;
	}
}
