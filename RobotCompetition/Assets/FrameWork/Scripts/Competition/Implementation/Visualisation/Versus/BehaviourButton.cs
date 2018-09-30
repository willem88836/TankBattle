using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BehaviourButton : MonoBehaviour {

	[Header("Visuals")]
	[SerializeField] Button _button;
	[SerializeField] Image _image;
	[SerializeField] Text _text;
	
	[Header("Colors")]
	[SerializeField] Color _selectedColor;
	[SerializeField] Color _deselectedColor;

	Type _attachedBehaviour;

	public void SetBehaviour(Type behaviour)
	{
		_attachedBehaviour = behaviour;
		_text.text = behaviour.ToString();
	}

	public Type GetBehaviour()
	{
		return _attachedBehaviour;
	}

	public void OnSelected()
	{
		_image.color = _selectedColor;
	}

	public void OnDeselected()
	{
		_image.color = _deselectedColor;
	}

	public Button.ButtonClickedEvent GetOnClick()
	{
		return _button.onClick;
	}
}
