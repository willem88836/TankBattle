using UnityEngine;
using UnityEngine.UI;

namespace Framework.Competition
{
	public class CompetitionInterface : MonoBehaviour
	{
		const string NOWINNERNAME = "Nobody";

		public GameObject MenuScreen;
		public GameObject IntermissionScreen;
		public CompetitionManager Manager;
		public string WinnerText;
		public Text WinnerTextField;

		// Use this for initialization
		private void Awake()
		{
			Manager.OnGameFinish += (System.Type winner) => { GameFinished(winner); };
			Manager.OnIntermission += (System.Type winner) => { IntermissionStarted(winner); };
			WinnerTextField.gameObject.SetActive(false);
		}

		private void GameFinished(System.Type winner)
		{
			IntermissionScreen.SetActive(false);
			MenuScreen.SetActive(true);
			string winnerName = winner == null ? NOWINNERNAME : winner.ToString();
			WinnerTextField.text = string.Format(WinnerText, winnerName);
			WinnerTextField.gameObject.SetActive(true);
		}

		private void IntermissionStarted(System.Type winner)
		{
			MenuScreen.SetActive(false);
			IntermissionScreen.SetActive(true);
			WinnerTextField.text = string.Format(WinnerText, winner.ToString());
			WinnerTextField.gameObject.SetActive(true);
		}
	}
}
