using UnityEngine;
using UnityEngine.UI;

public class UnityErrorDisplay : MonoBehaviour
{
	private static UnityErrorDisplay instance;
	private static string lastMessage;

	public Text ErrorField;


	private void Awake()
	{
		if (instance != null && instance.gameObject != null)
			Destroy(instance.gameObject);
		instance = this;

		if (lastMessage != null)
		{
			ShowError(lastMessage);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

	public void Clear()
	{
		lastMessage = null;
	}

	public static void ShowError(string message)
	{
		Debug.LogError(message);
		lastMessage = message;

		if (instance)
		{
			instance.ErrorField.text = message;
			instance.gameObject.SetActive(true);
		}
	}
}
