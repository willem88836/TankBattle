using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFramerate : MonoBehaviour {

	[SerializeField] int _targetFramerate = 60;

	void Awake()
	{
		Application.targetFrameRate = _targetFramerate;
	}
}
