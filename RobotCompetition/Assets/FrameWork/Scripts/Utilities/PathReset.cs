using UnityEngine;
using Framework.ScriptableObjects.Variables;

namespace Framework
{
	public class PathReset : MonoBehaviour
	{
		public  static PathReset instance;

		public StringReference BehaviourPath;

		private void Awake()
		{
			if (instance != null)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;
			DontDestroyOnLoad(gameObject);

			// Set default path.
			string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), "Tank_Behaviours");
			if (!System.IO.Directory.Exists(path))
				System.IO.Directory.CreateDirectory(path);
			BehaviourPath.Value = path;
		}
	}
}
