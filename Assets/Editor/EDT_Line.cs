using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 类名 : 时间线 Time Line
/// 作者 : Canyon
/// 日期 : 2017-08-15 10:10
/// 功能 : 时间帧数对象
/// </summary>
public class EDT_Line {
	/// <summary>
	/// 自增ID计数
	/// </summary>
	static long AutoAddIdCount = 0;

	/// <summary>
	/// 时间帧率(n帧/秒)
	/// </summary>
	static public int invFrameRate = 30;

	/// <summary>
	/// 当前帧数
	/// </summary>
	static public int curFrame = 1;

	/// <summary>
	/// 时间 转为 帧数
	/// </summary>
	static public int ToFrame(float second){
		return Mathf.CeilToInt (second * invFrameRate);
	}

	/// <summary>
	/// 帧数 转为 时间
	/// </summary>
	static public float ToTimeSecond(int frame){
		return (float)frame / invFrameRate;
	}

	/// <summary>
	/// 唯一标识
	/// </summary>
	public long uniqueID = (++AutoAddIdCount);

	/// <summary>
	/// 帧数
	/// </summary>
	public int m_frame = 1;

	/// <summary>
	/// 时间
	/// </summary>
	public float m_time;

	/// <summary>
	/// 设置帧数
	/// </summary>
	public void SetFrame(int frame){
		this.m_frame = frame;
		this.m_time = ToTimeSecond (m_frame);
	}

	/// <summary>
	/// 设置时间
	/// </summary>
	public void SetTime(float second){
		SetFrame (ToFrame (second));
	}

	/// <summary>
	/// 转为字符串
	/// </summary>
	public virtual string ToStrVal(){
		return "";
	}

	public EDT_Line(){
	}

	public EDT_Line(int frame){
		SetFrame (frame);
	}

	public EDT_Line(float second){
		SetTime (second);
	}
}

/// <summary>
/// 类名 : 时间线 Time Line 排序
/// 作者 : Canyon
/// 日期 : 2017-08-15 10:30
/// 功能 : 
/// </summary>
public class SortEDT_Line<T> : IComparer<T> where T:EDT_Line{

	public int Compare (T x, T y)
	{
		return Compare ((EDT_Line)x, (EDT_Line)y);
	}

	public int Compare (EDT_Line x, EDT_Line y)
	{
		if (x.m_frame > y.m_frame)
			return 1;

		if (x.m_frame < y.m_frame)
			return -1;

		if (x.uniqueID > y.uniqueID)
			return 1;

		if (x.uniqueID < y.uniqueID)
			return -1;

		return 0;
	}
}

/// <summary>
/// 类名 : 时间线 - CGCamera动画
/// 作者 : Canyon
/// 日期 : 2017-08-18 10:30
/// 功能 : 
/// </summary>
public class EDT_CGCamera : EDT_Line{
	/// <summary>
	/// 水平旋转角度
	/// </summary>
	public float m_angleHorizontal = 0;

	/// <summary>
	/// 垂直旋转角度
	/// </summary>
	public float m_angleVertical = 0;

	/// <summary>
	/// 距离
	/// </summary>
	public float m_distance = 0;

	/// <summary>
	/// 视角Y的偏移量
	/// </summary>
	public float m_offsetY = 0;

	/// <summary>
	/// 摄像机视野大小
	/// </summary>
	public float m_fov = 60;

	/// <summary>
	/// 核心计算方法
	/// </summary>
	public void Calc(Transform trsfCamera,Vector3 cameraTargetPos){

		float fPitch = m_angleVertical * Mathf.Deg2Rad;
		float fYaw = m_angleHorizontal * Mathf.Deg2Rad;

		Vector3 vPos = cameraTargetPos;
		vPos.x += m_distance * Mathf.Sin (fPitch) * Mathf.Cos (fYaw);
		vPos.y += m_distance * Mathf.Cos (fPitch) + m_offsetY;
		vPos.z += m_distance * Mathf.Sin (fPitch) * Mathf.Sin (fYaw);

		trsfCamera.position = vPos;

		cameraTargetPos.y += m_offsetY;
		trsfCamera.LookAt (cameraTargetPos);
	}

	/// <summary>
	/// 核心计算方法 - 正确的三角函数算法
	/// </summary>
	public void CalcTriangle(Transform trsfCamera,Vector3 cameraTargetPos){

		float fPitch = m_angleVertical * Mathf.Deg2Rad;
		float fYaw = m_angleHorizontal * Mathf.Deg2Rad;

		Vector3 vPos = cameraTargetPos;
		vPos.x += m_distance * Mathf.Cos (fPitch) * Mathf.Cos (fYaw);
		vPos.y += m_distance * Mathf.Sin (fPitch) + m_offsetY;
		vPos.z += m_distance * Mathf.Cos (fPitch) * Mathf.Sin (fYaw);

		trsfCamera.position = vPos;

		cameraTargetPos.y += m_offsetY;
		trsfCamera.LookAt (cameraTargetPos);
	}

	public EDT_CGCamera(){
	}

	public EDT_CGCamera(int frame,float angleX,float angleY,float distance,float offsetY,float fov):base(frame){
		this.m_angleHorizontal = angleX;
		this.m_angleVertical = angleY;
		this.m_distance = distance;
		this.m_offsetY = offsetY;
		this.m_fov = fov;
	}
}