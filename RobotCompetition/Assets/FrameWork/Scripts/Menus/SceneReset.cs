using UnityEngine;
using Framework.Core;

namespace Framework.Menu
{
	public class SceneReset : MonoBehaviour
	{
		/// <summary>
		///		Reloads the scene AND destroys the BattleManager.
		/// </summary>
		public void HardReset()
		{
			Debug.Log("hard reset");
			BattleManager instance = BattleManager.Singleton();
			Destroy(instance.gameObject);

			Menu menu = Menu.Instance;
			menu.SwitchScene(menu.Current);
		}

		/// <summary>
		///		Reloads the scene WITHOUT destroying the BattleManager.
		/// </summary>
		public void SoftReset()
		{
			Debug.Log("soft reset reset");
			Menu menu = Menu.Instance;
			menu.SwitchScene(menu.Current);
		}
	}
}
