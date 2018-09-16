using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphTile : MonoBehaviour
{
	public RectTransform Rect;
	public Text BaseTextField;
	public string BaseText;
	public float expandValue = 17.5f;

	public Dictionary<string, GameObject> Competitors
		= new Dictionary<string, GameObject>();


	public void Add(string name, int score)
	{
		if (!Competitors.ContainsKey(name))
		{
			Text field = Instantiate(BaseTextField, transform);
			field.text = string.Format(BaseText, name, score);
			Competitors.Add(name, field.gameObject);

			Rect.sizeDelta += new Vector2(0, expandValue);
		}
	}

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
