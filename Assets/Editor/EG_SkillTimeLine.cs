using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 类名 : 时间线绘制
/// 作者 : Canyon
/// 日期 : 2017-08-15 15:10
/// 功能 : 
/// </summary>
public class EG_SkillTimeLine{
	// 初始数据
	int nInitX = 0;
	int nInitY = 0;
	int nWidth = 0;
	int nHeight = 0;

	// 背景颜色
	Color m_bgColor = new Color(0.3f,0.3f,0.3f,1);

	// 是否显示格子帧数
	bool showTimeNum = true;

	// 总帧数
	int allNumFrames = EDT_Line.invFrameRate * 10;

	// n 帧 / 格子
	int oneCellFrameCount = 1;

	// 每帧的宽度
	float oneFrameWidth = 0.65f;

	// 格子的宽度
	float cellWidth = 0;

	// 格子数量
	int numCell = 0;

	// 滚动内容的宽度
	float contentWidth = 0;

	// 滚动值
	Vector2 v2ScrollPos = Vector2.zero;

	// 默认线的颜色
	Color m_lineColor = new Color(0.2f,0.2f,0.2f,1);

	// 线高
	int lineHeight = 0;

	// 线的开始位置
	int lineStartX = 0;
	int lineStartY = 0;

	// 开始播放了动作
	bool isStartAct = false;

	// 选中对象
	long m_nSelect2Draging = -1;

	public EG_SkillTimeLine(){
	}

	~ EG_SkillTimeLine(){
	}

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

	/// <summary>
	/// 绘制视图
	/// </summary>
	public void DrawView(ref int curX,ref int curY,int width,int height){
		nInitX = curX;
		nInitY = curY;
		nWidth = width;
		nHeight = height;

		EditorGUI.DrawRect (CreateRect (ref curX, curY, nWidth, nHeight), m_bgColor);
		NextLine (ref curX, ref curY, 5, nInitX);

		GUI.Label(CreateRect(ref curX,curY,width - 5),"技能-时间线:");
		NextLine (ref curX, ref curY, 20, nInitX);

		_DrawTimeLine (ref curX, ref curY);
	}

	void _DrawTimeLine(ref int curX,ref int curY){
		NextLine (ref curX, ref curY, 0, nInitX + 5);
		EditorGUI.DrawRect (CreateRect (ref curX, curY, nWidth - 6,2), Color.black);

		NextLine (ref curX, ref curY, 5, nInitX + 10);
		showTimeNum = GUI.Toggle (CreateRect (ref curX, curY, 90), showTimeNum, "显示时间坐标");

		GUI.Label (CreateRect (ref curX, curY, 90), string.Format ("时长({0}帧/秒):", EDT_Line.invFrameRate));
		allNumFrames = EditorGUI.IntField (CreateRect (ref curX, curY, 60), allNumFrames);
		allNumFrames = Mathf.Max (allNumFrames, 10);

		if (Event.current.type == EventType.ScrollWheel) {
			// 滚动鼠标滑轮
			oneFrameWidth += -1 * oneFrameWidth * Event.current.delta.y / 20;
		}

		cellWidth = oneCellFrameCount * oneFrameWidth;
		numCell = Mathf.CeilToInt ((float)allNumFrames / oneCellFrameCount);
		contentWidth = numCell * cellWidth + 10;

		if (contentWidth < nWidth - 50) {
			cellWidth = (nWidth - 50) / (float)numCell;
		} else {
			cellWidth = Mathf.Max(cellWidth,20f);
			cellWidth = Mathf.Min(cellWidth,100f);
		}
		oneFrameWidth = cellWidth / oneCellFrameCount;

		NextLine (ref curX, ref curY, 25, nInitX + 10);
		GUI.Label (CreateRect (ref curX, curY, 50), "当前帧:");
		int _preFrame = EDT_Line.curFrame;
		EDT_Line.curFrame = EditorGUI.IntField (CreateRect (ref curX, curY, 50), EDT_Line.curFrame);
		EDT_Line.curFrame = Mathf.Min (EDT_Line.curFrame, allNumFrames);
		if (_preFrame != EDT_Line.curFrame) {
			// 通知滚动列表移动到当前帧对象去
			// Messenger.Brocast(MsgConst.CurrFrame2TimeLineScrollPos);
		}

		GUI.Label (CreateRect (ref curX, curY, 60), "帧数/格子:");
		oneCellFrameCount = EditorGUI.IntField (CreateRect (ref curX, curY, 40),oneCellFrameCount);
		oneCellFrameCount = Mathf.Max (oneCellFrameCount, 1);
		oneCellFrameCount = Mathf.Min (oneCellFrameCount, allNumFrames);

		NextLine (ref curX, ref curY, 25, nInitX);

		int desY = curY;
		NextLine (ref curX, ref desY, 30, nInitX);
		GUI.Label (CreateRect (ref curX, desY, 40), "事件");

		NextLine (ref curX, ref curY, 0, nInitX + 40);

		int scrollH = nInitY + nHeight - curY - 1;
		lineHeight = scrollH - 20;
		Rect scrollRect = new Rect (curX, curY, nWidth - 40, scrollH);
		Rect contentRect = new Rect (curX, curY, contentWidth, lineHeight);

		if (isStartAct)
			CalcScrollPos ();
		
		v2ScrollPos = GUI.BeginScrollView (scrollRect, v2ScrollPos, contentRect, false, false);
		lineStartX = curX + 5;
		lineStartY = curY;

		EditorGUI.DrawRect (new Rect(lineStartX,lineStartY + 20,contentWidth,2), m_lineColor);
		EditorGUI.DrawRect (new Rect(lineStartX,lineStartY + lineHeight - 10,contentWidth,2), m_lineColor);

		int curCellFrame = 0;
		float curCellX = 0;
		string strCellFame = "";
		for (int i = 0; i <= numCell; i++) {
			curCellFrame = i * oneCellFrameCount;
			curCellX = lineStartX + curCellFrame * oneFrameWidth;
			if (i > 0) {
				_DrawOneCell (new Rect (curCellX - cellWidth + 2, lineStartY + 22, cellWidth - 4, lineHeight - 32), curCellFrame);
			}

			if (showTimeNum) {
				strCellFame = curCellFrame.ToString();
				GUI.Label(new Rect(curCellX - 4 * strCellFame.Length,lineStartY,50,30),strCellFame);
			}
			EditorGUI.DrawRect (new Rect (curCellX, lineStartY + 15, 2, lineHeight), m_lineColor);
		}

		if(isStartAct){
			// EDT_Line.curFrame = EDT_Line.ToFrame();
		}
		_DrawCurTimeLine();

		GUI.EndScrollView ();
	}

	void _DrawOneCell(Rect rect,int endFrame){
		if (!isStartAct) {
			if (Event.current.type == EventType.MouseUp) {
				Vector2 mousePosition = Event.current.mousePosition;
				if (rect.Contains (mousePosition)) {
					float dX = mousePosition.x - rect.x;
					int curStart = endFrame - oneCellFrameCount;
					EDT_Line.curFrame = curStart + Mathf.CeilToInt (dX * oneCellFrameCount / cellWidth);
				}
			}
		}
		EditorGUI.DrawRect (rect, m_bgColor);
	}

	void _DrawCurTimeLine(){
		float lineWidth = oneFrameWidth * 0.5f;
		float curX = lineStartX + EDT_Line.curFrame * oneFrameWidth - lineWidth;
		Rect rect = new Rect(curX,lineStartY + 20,1,lineHeight - 30);
		EditorGUI.DrawRect (rect, Color.red);
	}

	void _DrawLineEntity<T>(int row,Color col,List<T> list,string title = "",System.Action callChange = null) where T : EDT_Line{
		if (list == null) {
			return;
		}

		int lens = list.Count;
		if (lens <= 0)
			return;

		int curX = lineStartX;
		int curY = lineStartY + 30 + row * 25;

		T entity;
		Rect rect;

		float cellX = 0;
		float lineWidth = oneFrameWidth * 0.5f;
		int curFrame = 0;
		
		for (int i = 0; i < lens; i++) {
			entity = list [i];
			cellX = curX + entity.m_frame * oneFrameWidth - lineWidth * 1.35f;
			rect = new Rect (cellX, curY, lineWidth, 10);
			curFrame = entity.m_frame + _DrawDragBar (rect, col, entity.uniqueID, title);
			if (curFrame != entity.m_frame) {
				entity.SetFrame (curFrame);
				if (callChange != null) {
					callChange ();
				}
			}
		}
	}

	int _DrawDragBar(Rect rect,Color color,long uniqueID,string tip = ""){ 
		int ret = 0;
		if (Event.current.type == EventType.MouseUp) {
			m_nSelect2Draging = -1;
		}

		if (!isStartAct) {
			if (Event.current.type == EventType.MouseDown) {
				Vector2 p = Event.current.mousePosition;
				if (rect.Contains (p)) {
					m_nSelect2Draging = uniqueID;
				}
			}
		}

		Color drawColor = new Color (color.r, color.g, color.b, color.a);

		if (m_nSelect2Draging == uniqueID) {
			drawColor *= 0.9f;

			if (Event.current.type == EventType.MouseDrag) {
				Vector2 mousePosition = Event.current.mousePosition;
				float dX = mousePosition.x - rect.x;
				if(dX != 0){
					float dXAbs = Mathf.Abs (dX);
					float halfFrameWidth = oneFrameWidth * 0.5f;
					if (dXAbs > halfFrameWidth) {
						ret = Mathf.CeilToInt (dXAbs / oneFrameWidth);
						if (dX < 0)
							ret *= -1;
					}
				}
			}
		}

		EditorGUI.DrawRect (rect, drawColor);

		if (!string.IsNullOrEmpty (tip)) {
			GUI.Label (new Rect (rect.x, rect.y - 5, 100, 30), tip);
		}

		return ret;
	}

	/// <summary>
	/// 计算滚动时间线的滚动位置
	/// </summary>
	void CalcScrollPos(){
		v2ScrollPos.x = (EDT_Line.curFrame - 1) * (contentWidth - nWidth + 20) / numCell;
	}
}
