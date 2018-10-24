using UnityEngine;

namespace Framework.Menu
{
	public class CameraToggle : ComponentToggle
	{
		private readonly LayerMask ALL = -1;
		private readonly LayerMask NOSENSOR = 1;

		public Camera Camera;

		protected override void Switch(bool state)
		{
			Camera.cullingMask = (state) ? ALL : NOSENSOR;
		}
	}
}
