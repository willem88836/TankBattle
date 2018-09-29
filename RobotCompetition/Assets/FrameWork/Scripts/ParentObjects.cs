using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentObjects : MonoBehaviour {

	static ParentObjects _instance;

	[SerializeField] Transform _bulletParent;

	void Awake()
	{
		if (_instance != null && _instance != this)
			Destroy(_instance.gameObject);

		_instance = this;
	}

	public Transform GetBulletParent()
	{
		return _bulletParent;
	}

	public static ParentObjects Singleton()
	{
		if (_instance == null)
			Debug.LogWarning("ParentObjects singleton returned null");

		return _instance;
	}
}
