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

	static public void Invoke(string eventType)
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
		// Obtain a lock on the event table to keep this thread-safe.
		lock (eventTable)
		{
			// Create an entry for this event type if it doesn't already exist.
			if (!eventTable.ContainsKey(eventType))
			{
				eventTable.Add(eventType, null);
			}
			// Add the handler to the event.
			eventTable[eventType] = (System.Action<T>)eventTable[eventType] + handler;
		}
	}

	static public void RemoveListener<T>(string eventType, System.Action<T> handler)
	{
		// Obtain a lock on the event table to keep this thread-safe.
		lock (eventTable)
		{
			// Only take action if this event type exists.
			if (eventTable.ContainsKey(eventType))
			{
				// Remove the event handler from this event.
				eventTable[eventType] = (System.Action<T>)eventTable[eventType] - handler;

				// If there's nothing left then remove the event type from the event table.
				if (eventTable[eventType] == null)
				{
					eventTable.Remove(eventType);
				}
			}
		}
	}

	static public void Invoke<T>(string eventType, T arg1)
	{
		Delegate d;
		// Invoke the delegate only if the event type is in the dictionary.
		if (eventTable.TryGetValue(eventType, out d))
		{
			// Take a local copy to prevent a race condition if another thread
			// were to unsubscribe from this event.
			System.Action<T> callback = (System.Action<T>)d;

			// Invoke the delegate if it's not null.
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
		// Obtain a lock on the event table to keep this thread-safe.
		lock (eventTable)
		{
			// Create an entry for this event type if it doesn't already exist.
			if (!eventTable.ContainsKey(eventType))
			{
				eventTable.Add(eventType, null);
			}
			// Add the handler to the event.
			eventTable[eventType] = (System.Action<T, U>)eventTable[eventType] + handler;
		}
	}

	static public void RemoveListener<T, U>(string eventType, System.Action<T, U> handler)
	{
		// Obtain a lock on the event table to keep this thread-safe.
		lock (eventTable)
		{
			// Only take action if this event type exists.
			if (eventTable.ContainsKey(eventType))
			{
				// Remove the event handler from this event.
				eventTable[eventType] = (System.Action<T, U>)eventTable[eventType] - handler;

				// If there's nothing left then remove the event type from the event table.
				if (eventTable[eventType] == null)
				{
					eventTable.Remove(eventType);
				}
			}
		}
	}

	static public void Invoke<T, U>(string eventType, T arg1, U arg2)
	{
		Delegate d;
		// Invoke the delegate only if the event type is in the dictionary.
		if (eventTable.TryGetValue(eventType, out d))
		{
			// Take a local copy to prevent a race condition if another thread
			// were to unsubscribe from this event.
			System.Action<T, U> callback = (System.Action<T, U>)d;

			// Invoke the delegate if it's not null.
			if (callback != null)
			{
				callback(arg1, arg2);
			}
		}
	}

	static public void Brocast<T, U>(string eventType, T arg1, U arg2){
		Invoke (eventType,arg1,arg2);
	}
}