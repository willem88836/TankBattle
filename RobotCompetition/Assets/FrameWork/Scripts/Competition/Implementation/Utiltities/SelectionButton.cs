using Framework.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Competition
{
	public class SelectionButton : MonoBehaviour
	{
		public Text NameBox;
		public Image Image;
		public Color selectedColor;
		public Color unselectedColor;


		public Action OnClickAction;
		[NonSerialized] public int BoxIndex;
		[NonSerialized] public Type Behaviour;

		public void OnClick()
		{
			OnClickAction.SafeInvoke();
		}

		public void Select()
		{
			Image.color = selectedColor;
		}

		public void Deselect()
		{
			Image.color = unselectedColor;
		}
	}
}
