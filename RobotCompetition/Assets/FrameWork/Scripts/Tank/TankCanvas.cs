using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TankCanvas
{
	[SerializeField] Renderer _renderer;

	[SerializeField] int[] _materialIndexes;

	public void ChangeColor(Color color)
	{
		for (int i = 0; i < _materialIndexes.Length; i++)
		{
			int currentIndex = _materialIndexes[i];

			if (currentIndex > _renderer.materials.Length)
				continue;

			_renderer.materials[currentIndex].color = color;
		}
	}
}
