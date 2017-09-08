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

	#region 摄像机数据
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


	Queue<EDT_CGCamera> m_queue = new Queue<EDT_CGCamera>();

	EDT_CGCamera begData,nextData;

	public float m_angleHorizontal = 0;
	public float m_angleVertical = 0;
	public float m_distance = 0;
	public float m_offsetY = 0;
	public float m_fov = 60;

	bool isInit = false;

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

		Calc (m_trsfCamera, Vector3.forward);
	}

	public void OnUpdate(float timeSecond){
		if (!isInit)
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

		Calc (m_trsfCamera, Vector3.forward);
	}

	/// <summary>
	/// 核心计算方法
	/// </summary>
	public void Calc(Transform trsfCamera,Vector3 cameraTargetPos){

		float fPitch = m_angleVertical * Mathf.PI / 180f;
		float fYaw = m_angleHorizontal * Mathf.PI / 180f;

		Vector3 vPos = cameraTargetPos;
		vPos.x += m_distance * Mathf.Sin (fPitch) * Mathf.Cos (fYaw);
		vPos.y += m_distance * Mathf.Cos (fPitch) + m_offsetY;
		vPos.z += m_distance * Mathf.Sin (fPitch) * Mathf.Sin (fYaw);

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
