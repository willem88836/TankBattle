using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	public enum Scene { MenuScene, TournamentScene, VersusScene_Primitive, DungeonScene, FreeForAllScene};


	public void SwitchScene(Scene scene)
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
		SwitchScene(Scene.VersusScene_Primitive);
	}
}
