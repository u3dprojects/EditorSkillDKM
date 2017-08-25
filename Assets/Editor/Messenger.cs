// Messenger.cs v0.1 (20090925) by Rod Hyde (badlydrawnrod).
//
// This is a C# messenger (notification center) for Unity. It uses delegates
// and generics to provide type-checked messaging between event producers and
// event consumers, without the need for producers or consumers to be aware of
// each other.

// http://wiki.unity3d.com/index.php/CSharpMessenger
// Modify by Canyon
// Time : 2017-02-07

using System;
using System.Collections.Generic;


/**
 * A messenger for events that have no parameters.
 */
static public class Messenger
{
	private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

	static public void AddListener(string eventType, System.Action handler)
	{
		// Obtain a lock on the event table to keep this thread-safe.
		lock (eventTable)
		{
			// Create an entry for this event type if it doesn't already exist.
			if (!eventTable.ContainsKey(eventType))
			{
				eventTable.Add(eventType, null);
			}
			// Add the handler to the event.
			eventTable[eventType] = (System.Action)eventTable[eventType] + handler;
		}
	}

	static public void RemoveListener(string eventType, System.Action handler)
	{
		// Obtain a lock on the event table to keep this thread-safe.
		lock (eventTable)
		{
			// Only take action if this event type exists.
			if (eventTable.ContainsKey(eventType))
			{
				// Remove the event handler from this event.
				eventTable[eventType] = (System.Action)eventTable[eventType] - handler;

				// If there's nothing left then remove the event type from the event table.
				if (eventTable[eventType] == null)
				{
					eventTable.Remove(eventType);
				}
			}
		}
	}

	static void Invoke(string eventType)
	{
		Delegate d;
		// Invoke the delegate only if the event type is in the dictionary.
		if (eventTable.TryGetValue(eventType, out d))
		{
			// Take a local copy to prevent a race condition if another thread
			// were to unsubscribe from this event.
			System.Action callback = (System.Action) d;

			// Invoke the delegate if it's not null.
			if (callback != null)
			{
				callback();
			}
		}
	}

	static public void Brocast(string eventType){
		Invoke (eventType);
	}

	static public void AddListener<T>(string eventType, System.Action<T> handler)
	{
		lock (eventTable)
		{
			if (!eventTable.ContainsKey(eventType))
			{
				eventTable.Add(eventType, null);
			}
			eventTable[eventType] = (System.Action<T>)eventTable[eventType] + handler;
		}
	}

	static public void RemoveListener<T>(string eventType, System.Action<T> handler)
	{
		lock (eventTable)
		{
			if (eventTable.ContainsKey(eventType))
			{
				eventTable[eventType] = (System.Action<T>)eventTable[eventType] - handler;

				if (eventTable[eventType] == null)
				{
					eventTable.Remove(eventType);
				}
			}
		}
	}

	static void Invoke<T>(string eventType, T arg1)
	{
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			System.Action<T> callback = (System.Action<T>)d;

			if (callback != null)
			{
				callback(arg1);
			}
		}
	}

	static public void Brocast<T>(string eventType, T arg1){
		Invoke (eventType,arg1);
	}

	static public void AddListener<T, U>(string eventType, System.Action<T, U> handler)
	{
		lock (eventTable)
		{
			if (!eventTable.ContainsKey(eventType))
			{
				eventTable.Add(eventType, null);
			}
			eventTable[eventType] = (System.Action<T, U>)eventTable[eventType] + handler;
		}
	}

	static public void RemoveListener<T, U>(string eventType, System.Action<T, U> handler)
	{
		lock (eventTable)
		{
			if (eventTable.ContainsKey(eventType))
			{
				eventTable[eventType] = (System.Action<T, U>)eventTable[eventType] - handler;

				if (eventTable[eventType] == null)
				{
					eventTable.Remove(eventType);
				}
			}
		}
	}

	static void Invoke<T, U>(string eventType, T arg1, U arg2)
	{
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			System.Action<T, U> callback = (System.Action<T, U>)d;

			if (callback != null)
			{
				callback(arg1, arg2);
			}
		}
	}

	static public void Brocast<T, U>(string eventType, T arg1, U arg2){
		Invoke (eventType,arg1,arg2);
	}

	static public void AddListener<T1,T2,T3>(string eventType, System.Action<T1,T2,T3> handler)
	{
		lock (eventTable)
		{
			if (!eventTable.ContainsKey(eventType))
			{
				eventTable.Add(eventType, null);
			}
			eventTable[eventType] = (System.Action<T1,T2,T3>)eventTable[eventType] + handler;
		}
	}

	static public void RemoveListener<T1,T2,T3>(string eventType, System.Action<T1,T2,T3> handler)
	{
		lock (eventTable)
		{
			if (eventTable.ContainsKey(eventType))
			{
				eventTable[eventType] = (System.Action<T1,T2,T3>)eventTable[eventType] - handler;

				if (eventTable[eventType] == null)
				{
					eventTable.Remove(eventType);
				}
			}
		}
	}

	static public void Brocast<T1,T2,T3>(string eventType, T1 arg1, T2 arg2, T3 arg3){
		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			System.Action<T1,T2,T3> callback = (System.Action<T1,T2,T3>)d;

			if (callback != null)
			{
				callback(arg1, arg2,arg3);
			}
		}
	}
}

/// <summary>
/// 类名 : 消息 常量
/// 作者 : canyon / 龚阳辉
/// 日期 : 2017-08-16 13:57
/// 功能 : 
/// </summary>
public partial class MsgConst{
	// 暂停 (无参数)
	public const string OnPause = "Msg_OnPause";

	// 恢复 (无参数)
	public const string OnResume = "Msg_OnResume";

	// 开始 (0~N参数)
	public const string OnStart = "Msg_OnStart";

	// 更新 (无参数)
	public const string OnUpdate = "Msg_OnUpdate";

	// 结束 (无参数)
	public const string OnEnd = "Msg_OnEnd";

	// 清除 (无参数)
	public const string OnClear = "Msg_OnClear";

	// 更新(一个参数:[delta or time.realtimeSinceStartup]
	public const string OnUpdateTime1 = "Msg_OnUpdateTime1";

	// 更新(两个参数:[delta,progress or time.realtimeSinceStartup]
	public const string OnUpdateTime2 = "Msg_OnUpdateTime2";
}