using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Menu
{
	public class Menu : MonoBehaviour
	{
		public static Menu Instance;

		public enum Scene { MenuScene, TournamentScene, VersusScene, DungeonScene, FreeForAllScene };

		public Scene Current { get; private set; } = Scene.MenuScene;

		public void Awake()
		{
			if (Instance != null)
			{
				this.Current = Instance.Current;
				Destroy(Instance.gameObject);
			}

			Instance = this;
			DontDestroyOnLoad(this);
		}


		public void ExitGame()
		{
			Application.Quit();
		}


		public void SwitchScene(Scene scene)
		{
			Current = scene;
			SceneManager.LoadScene(scene.ToString());
		}


		public void SwitchToMenu()
		{
			SwitchScene(Scene.MenuScene);
		}

		public void SwitchToTournament()
		{
			SwitchScene(Scene.TournamentScene);
		}

		public void SwitchToFreeForAll()
		{
			SwitchScene(Scene.FreeForAllScene);
		}

		public void SwitchToDungeon()
		{
			SwitchScene(Scene.DungeonScene);
		}

		public void SwitchToVersus()
		{
			SwitchScene(Scene.VersusScene);
		}
	}
}
