﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Competition
{
	/// <summary>
	///		Contains all behaviour display Tournament data.
	/// </summary>
	public class GraphTile : MonoBehaviour
	{
		public GameObject BaseTextField;
		public string BaseText;
		public float expandValue = 17.5f;

		public List<GameObject> Competitors = new List<GameObject>();

		private RectTransform Rect;


		/// <summary>
		///		Adds the data of one participant to the graph.
		/// </summary>
		public void Add(string name, int score)
		{
			if (Rect == null)
				Rect = GetComponent<RectTransform>();

			GameObject field = Instantiate(BaseTextField, transform);
			field.GetComponent<GraphText>().TextField.text = string.Format(BaseText, name, score);
			Competitors.Add(field.gameObject);

			Rect.sizeDelta += new Vector2(0, expandValue);
		}
	}
}
