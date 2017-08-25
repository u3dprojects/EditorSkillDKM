using UnityEngine;
using System.Collections;

/// <summary>
/// 类名 : 时间
/// 作者 : Canyon
/// 日期 : 2017-01-05 17:10
/// 功能 : 
/// </summary>
public class EN_Time  {
	/// <summary>
	/// 当前时间
	/// </summary>
	float cur_time =0;

	/// <summary>
	/// 上一次更新时间
	/// </summary>
	float last_time = 0;

	/// <summary>
	/// 帧率时间
	/// </summary>
	float delta_time = 0;

	/// <summary>
	/// 进程时间
	/// </summary>
	float progress_time = 0;

	/// <summary>
	/// 暂停
	/// </summary>
	bool isPause = false;

	/// <summary>
	/// 暂停总时长
	/// </summary>
	float all_pause_time = 0;

	/// <summary>
	/// 暂停
	/// </summary>
	public void DoPause(){
		isPause = true;
		last_time = Time.realtimeSinceStartup;
	}

	/// <summary>
	/// 恢复
	/// </summary>
	public void DoResume(){
		isPause = false;
		cur_time = Time.realtimeSinceStartup;
		all_pause_time += cur_time - last_time;
		last_time = cur_time;
	}

	/// <summary>
	/// 更新
	/// </summary>
	public void OnUpdateTime(){
		if (isPause)
			return;
		
		cur_time = Time.realtimeSinceStartup;
		if (last_time <= 0.001f) {
			delta_time = 0;
		} else {
			delta_time = cur_time - last_time;
		}

		last_time = cur_time;
		progress_time += delta_time;
	}

	/// <summary>
	/// 重置
	/// </summary>
	public void Reset(){
		cur_time = 0;
		last_time = 0;
		delta_time = 0;
		progress_time = 0;
		all_pause_time = 0;
	}

	/// <summary>
	/// 帧率时间间隔
	/// </summary>
	public float deltaTime{
		get{
			return delta_time;
		}
	}

	/// <summary>
	/// 更新总时长(不包涵暂停时间)
	/// </summary>
	public float progressTime{
		get{
			return progress_time;
		}
	}

	/// <summary>
	/// 暂停时间
	/// </summary>
	public float allPauseTime{
		get{
			return all_pause_time;
		}
	}
}
