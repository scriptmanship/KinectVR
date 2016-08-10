/// <summary>
/// Loom: Threading functionality to accomodate Unity's particularities about main thread access to certain data.
/// 
/// 	Based on freely distributable code from http://unitygems.com/threads/
/// 
/// This code may be freely used, modified, and distributed!
/// 
/// Created: bens1984 11/10/2013
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

public class Loom : MonoBehaviour
{
	/// <summary>
	/// The max threads allowed. Best performance is around maxThreads = # of CPUs.
	/// </summary>
	public static int maxThreads = 8;
	/// <summary>
	/// The number allocated threads.
	/// </summary>
	static int numThreads;
	/// <summary>
	/// Singleton access
	/// </summary>
	private static Loom _current;
	private int _count;
	public static Loom Current
	{
		get
		{
			Initialize();
			return _current;
		}
	}
	/// <summary>
	/// Local storage of bool Application.isPlaying. Makes flag accessible from secondary threads
	/// </summary>
	public static bool isPlaying = false;
	private void OnEnable() {
		isPlaying = Application.isPlaying;
	}
	
	void Awake()
	{
		_current = this;
		initialized = true;
	}
	
	static bool initialized;
	/// <summary>
	/// Initialize the singleton for use.
	/// </summary>
	static void Initialize()
	{
		if (!initialized)
		{
		
			if(!Application.isPlaying)
				return;
			initialized = true;
			var g = new GameObject("Loom");
			_current = g.AddComponent<Loom>();
		}
			
	}
	
	private List<Action> _actions = new List<Action>();
	/// <summary>
	/// Delayed queue item.
	/// </summary>
	public struct DelayedQueueItem
	{
		public float time;
		public Action action;
	}
	private List<DelayedQueueItem> _delayed = new  List<DelayedQueueItem>();

	List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();
	/// <summary>
	/// Queues <c>Action action</c> to run on the main thread at next Update call.
	/// </summary>
	/// <param name='action'>
	/// Action to run.
	/// </param>
	public static void QueueOnMainThread(Action action)
	{
		QueueOnMainThread( action, 0f);
	}
	/// <summary>
	/// Queues <c>Action action</c> the on main thread to be run in <c>time</c> seconds.
	/// </summary>
	/// <param name='action'>
	/// Action to be queued.
	/// </param>
	/// <param name='time'>
	/// Delay time before running the action.
	/// </param>
	public static void QueueOnMainThread(Action action, float time)
	{
		if (!isPlaying) {
			action();
			return;
		}
#if UNITY_EDITOR
		try {
#endif
		if(time != 0)
		{
			lock(Current._delayed)
			{
				Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action});
			}
		}
		else
		{
			lock (Current._actions)
			{
				Current._actions.Add(action);
			}
		}
#if UNITY_EDITOR
		} catch (NullReferenceException) {
			// this gets triggered as the play mode exits and scripts/threads are transitioning to edit mode
		}
#endif
	}
	
	/// <summary>
	/// The Low Priority Queue: execute one item on main thread per update
	/// </summary>
	private List<Action> _lpq = new List<Action>();
	/// <summary>
	/// Will add an action to the low priority queue: one action is executed per frame (update call)
	/// </summary>
	/// <param name='action'>
	/// Action to queue.
	/// </param>
	public static void RunLowPriority(Action action)
	{
		lock (Current._lpq)
		{
			Current._lpq.Add(action);
		}
	}
	/// <summary>
	/// Runs <c>Action a</c> on a seperate thread. Can run up to <c>maxThreads</c> actions simultaneously. Will block unitl a free thread is available.
	/// </summary>
	/// <returns>
	/// null
	/// </returns>
	/// <param name='a'>
	/// A: the action to run.
	/// </param>
	public static Thread RunAsync(Action a)
	{
		if (!Application.isPlaying) {
			a();
			return null;
		}
		Initialize();
		while(numThreads >= maxThreads)
		{
			Thread.Sleep(1);
		}
		Interlocked.Increment(ref numThreads);
		ThreadPool.QueueUserWorkItem(RunAction, a);
		return null;
	}
	/// <summary>
	/// Runs <c>Action a</c> asyncronously on a seperate thread. If no thread is available it will yield and wait.
	/// </summary>
	/// <returns>
	/// The async coroutine.
	/// </returns>
	/// <param name='a'>
	/// A.
	/// </param>
	public static IEnumerator RunAsyncCoroutine(Action a)
	{
		if (!Application.isPlaying) {
			a();
			yield break;
		}
		Initialize();
		while(numThreads >= maxThreads)
		{
			yield return new WaitForSeconds(0.00001f);
		}
		Interlocked.Increment(ref numThreads);
		ThreadPool.QueueUserWorkItem(RunAction, a);
		yield break;
	}
	/// <summary>
	/// Runs the action now.
	/// </summary>
	/// <param name='action'>
	/// Action to run.
	/// </param>
	private static void RunAction(object action)
	{
		try
		{
			((Action)action)();
		}
		catch
		{
		}
		finally
		{
			Interlocked.Decrement(ref numThreads);
		}
			
	}
	
	
	void OnDisable()
	{
		if (_current == this)
		{
			_current = null;
		}
	}
	/// <summary>
	/// The temp actions queue
	/// </summary>
	List<Action> _currentActions = new List<Action>();
	/// <summary>
	/// The temp _current Low priority actions
	/// </summary>
	List<Action> _currentLPQ = new List<Action>();
	
	// Update is called once per frame
	void Update()
	{
		// 1st: pop off any actions that have been queued on the main thread and run them
		lock (_actions)
		{
			_currentActions.Clear();
			_currentActions.AddRange(_actions);
			_actions.Clear();
		}
		foreach(var a in _currentActions)
		{
			a();
		}
		// 2nd: check all the delayed actions and run any that are now out of time
		lock(_delayed)
		{
			_currentDelayed.Clear();
			_currentDelayed.AddRange(_delayed.Where(d=>d.time <= Time.time));
			foreach(var item in _currentDelayed)
				_delayed.Remove(item);
		}
		foreach(var delayed in _currentDelayed)
		{
			delayed.action();
		}
		// 3rd: pull actions off the low priority queue based on the current frame rate and run these.
		lock (_lpq)
		{
			_currentLPQ.Clear();
			int count = (int)Mathf.Max(1, 0.0667f / Time.deltaTime);
			while (_lpq.Count > 0 && count-- > 0) {
				_currentLPQ.Add(_lpq[0]);
				_lpq.RemoveAt(0);
			}
		}
		foreach(var a in _currentLPQ)
		{
			a();
		}
	}
}

