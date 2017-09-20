using UnityEngine;
using System.Collections;
using UnityEditor;
using System.ComponentModel;

/// <summary>
/// 类名 : SceneGUI中绘制
/// 作者 : Canyon
/// 日期 : 2017-08-19 09:50
/// 功能 : 
/// </summary>
public class EG_HandlesHelper
{
	public enum AreaType{
		[Description("无")]
		None = 0,

		[Description("圆形")]
		Circle = 1,

		[Description("弧形")]
		Arc = 2,

		[Description("矩形")]
		Rectangle = 3,
	}

	static Color GetColor(float r,float g,float b,float a = 1){
		r = r > 1 ? r / 255 : r;
		g = g > 1 ? g / 255 : g;
		b = b > 1 ? b / 255 : b;
		a = a > 1 ? a / 255 : a;
		return new Color (r, g, b, a);
	}

	static Color _lineMove,_lineCaster,_scope,_areaColor;

	static Color m_colLineMove{
		get{
			if (_lineMove.a == 0) { 
				_lineMove = GetColor (99, 15, 189);
			}
			return _lineMove;
		}
	}

	static Color m_colLineCaster{
		get{
			if (_lineCaster.a == 0) { 
				_lineCaster = GetColor (255, 10, 212);
			}
			return _lineCaster;
		}
	}

	static Color m_colScope{
		get{
			if (_scope.a == 0) { 
				_scope = GetColor (5, 215, 212,0.02f);
			}
			return _scope;
		}
	}

	static Color m_colArea{
		get{
			if (_areaColor.a == 0) {
				_areaColor = GetColor (215,10 , 16,0.1f);
			}
			return _areaColor;
		}
	}

	/// <summary>
	/// 绘制
	/// </summary>
	/// <param name="disMove">位移距离</param>
	/// <param name="disCaster">释放距离</param>
	/// <param name="v3Start">V3 start.</param>
	static public void Draw (ref float disMove,ref float disCaster, Vector3 v3Start,Vector3 v3Dir, params object[] parms)
	{
		Handles.BeginGUI ();

		Color _def = Handles.color;

		v3Dir.y = v3Start.y;
		v3Dir.Normalize ();

		string val = string.Format ("中心");
		Handles.Label (v3Start, val);

		Vector3 v3NewPos = Vector3.zero;

		Vector3 v3Target = v3Start + v3Dir * disMove;
		Handles.color = m_colLineMove;
		Handles.DrawLine (v3Start, v3Target);
		v3NewPos = Handles.FreeMoveHandle (v3Target, Quaternion.identity, HandleUtility.GetHandleSize (v3Target) * 0.1f, Vector3.zero, Handles.CircleCap);
		v3NewPos.y = v3Target.y;
		Handles.Label (v3Target, string.Format("位移距离:{0:F}", disMove));

		if (!v3NewPos.Equals (v3Target)) {
			Vector3 dir = v3NewPos - v3Start;
			dir.y = v3Dir.y;

			float dot = Vector3.Dot (dir, v3Dir);
			if (dot > 0) {
				disMove = Vector3.Distance (v3NewPos, v3Start);
			} else {
				disMove = 0;
			}
			Messenger.Brocast (MsgConst.OnRepantEditorWindow);
		}

		Vector3 v3End = v3Start + v3Dir * (disCaster + disMove);
		Handles.color = m_colLineCaster;
		Handles.DrawLine (v3Target, v3End);
		v3NewPos = Handles.FreeMoveHandle (v3End, Quaternion.identity, HandleUtility.GetHandleSize (v3End) * 0.1f, Vector3.zero, Handles.CircleCap);
		v3NewPos.y = v3End.y;
		Handles.Label (v3End, string.Format("释放距离:{0:F}", disCaster));

		if (!v3NewPos.Equals (v3End)) {
			Vector3 dir = v3NewPos - v3Target;
			dir.y = v3Dir.y;

			float dot = Vector3.Dot (dir, v3Dir);
			if (dot > 0) {
				disCaster = Vector3.Distance (v3NewPos, v3Target);
			} else {
				disCaster = 0;
			}
			Messenger.Brocast (MsgConst.OnRepantEditorWindow);
		}

		// 搜索范围
		float range = disMove + disCaster; 
		Handles.color = m_colScope;
		Handles.DrawSolidDisc(v3Start,Vector3.up,range);

		Vector3 nwEnd = RotateY (v3End, -90);
		Handles.color = Color.red;
		Handles.DrawLine (v3Start, nwEnd);
		Handles.Label (nwEnd, string.Format("搜索范围:{0:F}", range));
		v3NewPos = Handles.FreeMoveHandle (nwEnd, Quaternion.identity, HandleUtility.GetHandleSize (nwEnd) * 0.1f, Vector3.zero, Handles.CircleCap);
		v3NewPos.y = nwEnd.y;
		if (!v3NewPos.Equals (nwEnd)) {
			Vector3 dir = v3NewPos - v3Start;
			dir.y = v3Dir.y;

			Vector3 nDir = RotateY (v3Dir, -90);
			nDir.y = v3Dir.y;

			float dot = Vector3.Dot (dir, nDir);
			if (dot > 0) {
				float dis = Vector3.Distance (v3NewPos, v3Start);
				disMove = Mathf.Min (dis, disMove);
				disCaster = dis - disMove;
			} else {
				disMove = 0;
				disCaster = 0;
			}
			Messenger.Brocast (MsgConst.OnRepantEditorWindow);
		}

		DrawArea (v3Target, v3Dir,disCaster, parms);

		Handles.color = _def;
		Handles.EndGUI ();
	}

	/// <summary>
	/// 绘制搜索范围
	/// </summary>
	/// <param name="v3Start">开始点</param>
	/// <param name="v3Dir">结束点</param>
	/// <param name="parms">第一个标识,</param>
	static void DrawArea(Vector3 v3Start,Vector3 v3Dir,float range,params object[] parms){
		if (parms == null)
			return;
		int lens = parms.Length;
		if (lens <= 0 || range <= 0)
			return;

		Vector3 upDir = Vector3.up;

		AreaType _emType = (AreaType)System.Enum.Parse (typeof(AreaType),parms [0].ToString (), true);

		float val = 0;
		if (parms.Length >= 2) {
			val = (float)parms [1];
		}

		Handles.color = m_colArea;

		Vector3 pos = Vector3.zero;
		Vector3 v3NewPos = Vector3.zero;
		Quaternion quaternion = Quaternion.AngleAxis (0, upDir);
		switch (_emType) {
		case AreaType.Circle:
			Handles.DrawSolidDisc(v3Start,upDir,range);
			break;
		case AreaType.Arc:
			float hfAngle = -1 * val * 0.5f;
			pos = Quaternion.AngleAxis (hfAngle, upDir) * v3Dir;
			pos.Normalize ();
			Handles.DrawSolidArc (v3Start, upDir, pos, val, range);

			Handles.color = Color.cyan;
			pos = v3Start + pos * range;
			Handles.Label (pos, string.Format ("半径:{0:F},夹度:{1:F}", range, val));
			v3NewPos = Handles.FreeMoveHandle (pos, Quaternion.identity, HandleUtility.GetHandleSize (pos) * 0.1f, Vector3.zero, Handles.CircleCap);

			pos = RotateByY (v3Dir, hfAngle * -1);
			pos.Normalize ();
			pos = v3Start + pos * range;
			Handles.Label (pos, string.Format ("半径:{0:F},夹度:{1:F}", range, val));
			v3NewPos = Handles.FreeMoveHandle (pos, Quaternion.identity, HandleUtility.GetHandleSize (pos) * 0.1f, Vector3.zero, Handles.CircleCap);
			float a = Mathf.Abs (Mathf.Acos (Vector3.Dot (v3Dir, v3NewPos - v3Start)) * Mathf.Rad2Deg);
			parms[1] = a * 2;
			break;
		case AreaType.Rectangle:
			float hfw = val * 0.5f;
			float hfl = range * 0.5f;
			float hfr = Mathf.Sqrt(Mathf.Pow(range,2)+Mathf.Pow(val,2)) * 0.5f;
			pos = v3Start;

			Vector3 pos01 = new Vector3(pos.x - hfw,0,pos.z - hfl);
			Vector3 pos1 = quaternion * pos01.normalized * hfr;
			pos1.y = pos.y;

			Vector3 pos02 = new Vector3(pos.x - hfw,0,pos.z + hfl);
			Vector3 pos2 = quaternion * pos02.normalized * hfr;
			pos2.y = pos.y;

			Vector3 pos03 = new Vector3(pos.x + hfw,0,pos.z + hfl);
			Vector3 pos3 = quaternion * pos03.normalized * hfr;
			pos3.y = pos.y;

			Vector3 pos04 = new Vector3(pos.x + hfw,0,pos.z - hfl);
			Vector3 pos4 = quaternion * pos04.normalized * hfr;
			pos4.y = pos.y;

			Vector3[] verts = new Vector3[] { 
				pos1,pos2,pos3,pos4
			};
			Handles.DrawSolidRectangleWithOutline(verts,m_colArea, new Color( 0, 0, 0, 1 ));
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// 绕y轴旋转 - 矩阵旋转
	/// 向量 A [1,1,1] -> 绕y旋转角度θ(希腊字母)得矩阵M
	/// -> [cosθ,0,sinθ]
	/// -> [0,1,0]
	/// -> [-sinθ,0,cosθ]
	/// 将值转为新的向量B = AM
	/// -> [cosθ * x + 0 * y + (-sinθ) * z,0 *x + 1 * y + 0 * z,sinθ * x + 0 * y + cosθ * z]
	/// -> B = [cosθ - sinθ,1,sinθ + cosθ]
	/// </summary>
	/// <param name="src">对象</param>
	/// <param name="angle">角度,非弧度</param>
	static Vector3 RotateY(Vector3 src,float angle){
		float radian = angle * Mathf.Deg2Rad;
		float sin = Mathf.Sin (radian);
		float cos = Mathf.Cos (radian);

		Vector3 target = Vector3.zero;
		target.x = src.x *cos - sin * src.z;
		target.y = src.y;
		target.z = src.x * sin + cos * src.z;
		return target;
	}

	/// <summary>
	/// unity 自带的算法
	/// RotateY + RotateByY = Vector3.zero,在参数相同的情况下
	/// </summary>
	/// <returns>The by y.</returns>
	/// <param name="src">对象</param>
	/// <param name="angle">角度,非弧度</param>
	static Vector3 RotateByY(Vector3 src,float angle){
		// Quaternion.AngleAxis(angle,Vector3.up) == Quaternion.Euler(0,angle,0);
		return Quaternion.Euler(0,angle,0) * src;
	}

	static float Dir2Radian(Vector3 dir){
		dir.y = 0;
		dir.Normalize ();
		float acos = Mathf.Acos (dir.x);
		float asin = Mathf.Asin (dir.z);
		float radian = 0;
		if (asin > 0) {
			radian = acos;
		} else {
			if (acos >= 0) {
				radian = -acos;
			} else {
				radian = acos - Mathf.PI / 2;
			}
		}
		if (radian < 0f)
			radian += Mathf.PI * 2;

		return radian;
	}

	static float Dir2RadianBy(Vector3 dir){
		dir.y = 0;
		dir.Normalize ();
		return Vector3.Angle (dir, Vector3.forward);
	}
}
