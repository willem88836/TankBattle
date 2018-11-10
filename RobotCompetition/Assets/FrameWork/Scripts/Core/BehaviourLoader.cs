using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Framework.Core
{
	/// <summary>
	///		Responsible for loading custom and pre-made tankbehaviours.
	/// </summary>
	public static class BehaviourLoader
	{
		/// <summary>
		///		Returns the behaviours stored in the assets folder.
		/// </summary>
		public static Type[] LoadFrameworkBehaviours(int entries = 1)
		{
			Type baseType = typeof(TankController);
			Assembly assembly = Assembly.GetAssembly(baseType);
			Type[] competitors = (assembly.GetTypes().Where(t =>
			t != baseType
			&& baseType.IsAssignableFrom(t)
			&& !typeof(IDoNotLoad).IsAssignableFrom(t))).ToArray();

			// This is for debugging purposes.
			Type[] multipliedCompetitors = new Type[competitors.Length * entries];
			for (int i = 0; i < multipliedCompetitors.Length; i++)
			{
				multipliedCompetitors[i] = competitors[i % competitors.Length];
			}

			Debug.LogFormat("{0} competitor behaviours loaded!", multipliedCompetitors.Length);
			return multipliedCompetitors;
		}


		/// <summary>
		///		Returns the customly created behaviours. 
		/// </summary>
		/// <param name="entries"></param>
		/// <returns></returns>
		public static Type[] LoadCustomBehaviours(string path, int entries = 1)
		{
			List<Type> types = new List<Type>();

			Utilities.ForeachFileAt(path, (FileInfo info) =>
			{
				string code = File.ReadAllText(info.FullName);

				// TODO: compile stuff. 

			});

			return types.ToArray();
		}
	}
}
