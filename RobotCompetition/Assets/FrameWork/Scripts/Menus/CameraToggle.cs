using UnityEngine;

namespace Framework.Menu
{
	public class CameraToggle : ComponentToggle
	{
		public GameObject PrimaryCamera;
		public GameObject SecondaryCamera;

		protected override void Switch(bool state)
		{
			PrimaryCamera.SetActive(state);
			SecondaryCamera.SetActive(!state);
		}
	}
}
