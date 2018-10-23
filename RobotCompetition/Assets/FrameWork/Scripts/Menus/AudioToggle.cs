using UnityEngine;

namespace Framework.Menu
{
	public class AudioToggle : ComponentToggle
	{
		public AudioSource AudioSource;

		protected override void Switch(bool state)
		{
			AudioSource.mute = state;
		}
	}
}
