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
	public static class TankBehaviourLoader
	{
		private static Type baseType { get { return typeof(TankController); } }

		/// <summary>
		///		Returns the behaviours stored in the assets folder
		///		AND the customly created behaviours. 
		/// </summary>
		public static Type[] LoadAllBehaviours(string path, int entries = 1)
		{
			List<Type> types = new List<Type>(LoadFrameworkBehaviours(entries));
			types.AddRange(LoadCustomBehaviours(path, entries));
			return types.ToArray();
		}

		/// <summary>
		///		Returns the behaviours stored in the assets folder.
		/// </summary>
		public static Type[] LoadFrameworkBehaviours(int entries = 1)
		{
			Assembly assembly = Assembly.GetAssembly(baseType);
			Type[] competitors = (assembly.GetTypes().Where(t => IsTankBehaviour(t))).ToArray();

			// This is for debugging purposes.
			Type[] multipliedCompetitors = new Type[competitors.Length * entries];
			for (int i = 0; i < multipliedCompetitors.Length; i++)
			{
				multipliedCompetitors[i] = competitors[i % competitors.Length];
			}

			Debug.LogFormat("{0} pre-made competitor behaviours loaded!", multipliedCompetitors.Length);
			return multipliedCompetitors;
		}

		/// <summary>
		///		Returns the customly created behaviours. 
		/// </summary>
		/// <param name="entries"></param>
		/// <returns></returns>
		public static Type[] LoadCustomBehaviours(string path, int entries = 1)
		{
			if (!Directory.Exists(path))
				return new Type[0];

			List<Type> types = new List<Type>();

			Utilities.ForeachFileAt(path, (FileInfo info) =>
			{
				if (info.Extension != ".cs")
					return;


				string code = File.ReadAllText(info.FullName);

				CSharpCompiler compiler = new CSharpCompiler(code);

				string name = Path.GetFileNameWithoutExtension(info.FullName);
				Type type = compiler.GetCompiledType(name);

				if (IsTankBehaviour(type))
				{
					for (int i = 0; i < entries; i++)
					{
						types.Add(type);
					}
				}
			});

			Debug.LogFormat("{0} custom competitor behaviours loaded!", types.Count);

			return types.ToArray();
		}

		/// <summary>
		///		Returns true if the provided Type inherits from TankController.
		/// </summary>
		private static bool IsTankBehaviour(Type t)
		{
			return t != baseType
			&& baseType.IsAssignableFrom(t)
			&& !typeof(IDoNotLoad).IsAssignableFrom(t);
		}
	}
}
