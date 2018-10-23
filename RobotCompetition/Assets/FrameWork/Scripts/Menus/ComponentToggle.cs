using UnityEngine;
using UnityEngine.UI;

namespace Framework.Menu
{
	public abstract class ComponentToggle : MonoBehaviour
	{
		public Toggle Toggle;
		public bool Inverted;

		public void Start()
		{
			Toggle.onValueChanged.AddListener((bool state) => { Switch(state == Inverted); });
			Switch(Toggle.isOn == Inverted);
		}

		protected abstract void Switch(bool state);
	}
}
