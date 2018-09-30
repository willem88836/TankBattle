using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	private static Menu instance;

	public enum Scene { MenuScene, TournamentScene, VersusScene, DungeonScene};


	public static Menu Singleton()
	{
		return instance;
	}


	private void Awake()
	{
		if (instance != null && instance != this)
			Destroy(instance.gameObject);

		instance = this;

		DontDestroyOnLoad(gameObject);
	}


	public static void SwitchScene(Scene scene)
	{
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

	public void SwitchToDungeon()
	{
		SwitchScene(Scene.DungeonScene);
	}

	public void SwitchToVersus()
	{
		SwitchScene(Scene.VersusScene);
	}

}
