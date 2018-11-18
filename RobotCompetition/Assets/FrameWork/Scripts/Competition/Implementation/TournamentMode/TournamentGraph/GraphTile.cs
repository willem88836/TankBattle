using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Competition.Graph
{
	/// <summary>
	///		Contains all behaviour display Tournament data.
	/// </summary>
	public class GraphTile : MonoBehaviour
	{
		public GameObject BaseTextField;
		public string BaseText;

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
			string text = string.Format(BaseText, name, score);
			field.GetComponent<GraphText>().TextField.text = text;
			field.name = "GraphText_" + text;
			Competitors.Add(field.gameObject);
		}
	}
}
