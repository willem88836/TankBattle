using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
	public static class Utilities
	{

		#region Shuffle

		/// <summary>
		///		Returns a shuffled version of the provided list.
		/// </summary>
		public static List<T> Shuffle<T>(List<T> list)
		{
			List<T> shuffledList = new List<T>();
			for (int i = list.Count; i > 0; i--)
			{
				int index = UnityEngine.Random.Range(0, i);
				shuffledList.Add(list[index]);
				list.RemoveAt(index);
			}
			return shuffledList;
		}
		/// <summary>
		///		Returns a shuffled version of the provided range.
		/// </summary>
		public static T[] Shuffle<T>(T[] range)
		{
			List<T> list = new List<T>(range);
			list = Shuffle(list);
			return list.ToArray();
		}

		#endregion	

		#region SafeInvokes

		// Ty 8D
		public static void SafeInvoke(this Action a)
		{
			if (a != null)
			{ a.Invoke(); }
		}
		public static void SafeInvoke<T>(this Action<T> a, T t)
		{
			if (a != null)
			{ a.Invoke(t); }
		}
		public static void SafeInvoke<T0, T1>(this Action<T0, T1> a, T0 t0, T1 t1)
		{
			if (a != null)
			{ a.Invoke(t0, t1); }
		}
		public static void SafeInvoke<T0, T1, T2>(this Action<T0, T1, T2> a, T0 t0, T1 t1, T2 t2)
		{
			if (a != null)
			{ a.Invoke(t0, t1, t2); }
		}
		public static void SafeInvoke<T0, T1, T2, T3>(this Action<T0, T1, T2, T3> a, T0 t0, T1 t1, T2 t2, T3 t3)
		{
			if (a != null)
			{ a.Invoke(t0, t1, t2, t3); }
		}

		public static TReturn SafeInvoke<TReturn>(this Func<TReturn> a)
		{
			return a == null ? default(TReturn) : a.Invoke();
		}
		public static TReturn SafeInvoke<T0, TReturn>(this Func<T0, TReturn> a, T0 t0)
		{
			return a == null ? default(TReturn) : a.Invoke(t0);
		}
		public static TReturn SafeInvoke<T0, T1, TReturn>(this Func<T0, T1, TReturn> a, T0 t0, T1 t1)
		{
			return a == null ? default(TReturn) : a.Invoke(t0, t1);
		}
		public static TReturn SafeInvoke<T0, T1, T2, TReturn>(this Func<T0, T1, T2, TReturn> a, T0 t0, T1 t1, T2 t2)
		{
			return a == null ? default(TReturn) : a.Invoke(t0, t1, t2);
		}

		#endregion

		#region Foreach

		/// <summary>
		///		Iterates through all children.
		/// </summary>
		public static void Foreach(this Transform transform, Action<Transform> action)
		{
			var count = transform.childCount;
			for (var i = 0; i < count; i++)
			{
				var child = transform.GetChild(i);

				action(child);
				Foreach(child, action);
			}
		}

		/// <summary>
		///		Iterates through all children in reversed order.
		/// </summary>
		public static void ReversedForeach(this Transform transform, Action<Transform> action)
		{
			var count = transform.childCount;
			for (var i = count - 1; i >= 0; i--)
			{
				var child = transform.GetChild(i);

				ReversedForeach(child, action);
				action(child);
			}
		}

		#endregion
	}
}
