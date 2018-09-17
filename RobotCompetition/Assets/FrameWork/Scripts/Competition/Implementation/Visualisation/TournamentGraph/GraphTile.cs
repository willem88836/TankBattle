using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
	/// <summary>
	///		Contains all behaviour display Tournament data.
	/// </summary>
	public class GraphTile : MonoBehaviour
	{
		public GameObject BaseTextField;
		public string BaseText;
		public float expandValue = 17.5f;

		public Dictionary<string, GameObject> Competitors
			= new Dictionary<string, GameObject>();


		private RectTransform Rect;

		
		/// <summary>
		///		Adds the data of one participant to the graph.
		/// </summary>
		public void Add(string name, int score)
		{
			if (Rect == null)
				Rect = GetComponent<RectTransform>();

			if (!Competitors.ContainsKey(name))
			{
				GameObject field = Instantiate(BaseTextField, transform);
				field.GetComponent<GraphText>().TextField.text = string.Format(BaseText, name, score);
				Competitors.Add(name, field.gameObject);

				Rect.sizeDelta += new Vector2(0, expandValue);
			}
		}

		/// <summary>
		///		Removes the data of one participant from the graph.
		/// </summary>
		public void Remove(string name)
		{
			if (Competitors.ContainsKey(name))
			{
				Destroy(Competitors[name]);
				Competitors.Remove(name);
				Rect.sizeDelta -= new Vector2(0, expandValue);
			}
		}
	}
}
