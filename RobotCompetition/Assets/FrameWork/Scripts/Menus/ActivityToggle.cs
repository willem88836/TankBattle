using UnityEngine;

public class ActivityToggle : MonoBehaviour
{
	public void Toggle()
	{
		gameObject.SetActive(!gameObject.activeSelf);
	}
}
