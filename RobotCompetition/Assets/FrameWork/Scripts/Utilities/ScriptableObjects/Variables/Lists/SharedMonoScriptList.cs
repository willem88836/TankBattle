using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Framework.ScriptableObjects.Variables
{
	[CreateAssetMenu(menuName = "Generic/Lists/MonoScript")]
	public class SharedMonoScriptList : SharedList<MonoScript> { }
}
