using UnityEngine;
using UnityEngine.UI;

namespace Framework.Competition
{
	public class CompetitionInterface : MonoBehaviour
	{
		public GameObject menuScreen;
		public CompetitionManager Manager;
		public string WinnerText;
		public Text WinnerTextField;

		// Use this for initialization
		private void Awake()
		{
			Manager.OnGameFinish += (System.Type winner) => { GameFinished(winner); };
			WinnerTextField.gameObject.SetActive(false);
		}

		private void GameFinished(System.Type winner)
		{
			menuScreen.SetActive(true);
			WinnerTextField.text = string.Format(WinnerText, winner.ToString());
			WinnerTextField.gameObject.SetActive(true);
		}
	}
}
