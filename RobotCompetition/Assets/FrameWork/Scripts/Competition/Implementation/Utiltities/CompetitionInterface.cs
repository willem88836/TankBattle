using UnityEngine;
using UnityEngine.UI;

namespace Framework.Competition
{
	public class CompetitionInterface : MonoBehaviour
	{
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
			MenuScreen.SetActive(true);
			WinnerTextField.text = string.Format(WinnerText, winner.ToString());
			WinnerTextField.gameObject.SetActive(true);
		}

		private void IntermissionStarted(System.Type winner)
		{
			IntermissionScreen.SetActive(true);
			WinnerTextField.text = string.Format(WinnerText, winner.ToString());
			WinnerTextField.gameObject.SetActive(true);
		}
	}
}
