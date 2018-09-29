using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
	public class BattleManager : MonoBehaviour {
	
		static BattleManager _instance;
	
		[Header("BattleManager")]
		[SerializeField] Transform _bulletParent;
		[SerializeField] int _targetFramerate = 60;
	
		protected virtual void Awake()
		{
			if (_instance != null && _instance != this)
				Destroy(_instance.gameObject);
	
			_instance = this;
		}
	
		public Transform GetBulletParent()
		{
			return _bulletParent;
		}
	
		public static BattleManager Singleton()
		{
			if (_instance == null)
				Debug.LogWarning("BattleManager singleton returned null");
	
			return _instance;
		}
	}
}
