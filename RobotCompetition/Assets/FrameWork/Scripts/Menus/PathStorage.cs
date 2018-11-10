using Framework.ScriptableObjects.Variables;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PathStorage : MonoBehaviour
{
	public StringReference PathReference;
	public Button SaveButton;
	public InputField PathText;

	private void Awake()
	{
		PathText.text = PathReference.Value;
		SaveButton.onClick.AddListener(delegate { UpdatePath(PathText.text); });
	}

	private void OnEnable()
	{
		PathText.text = PathReference.Value;
	}

	private void UpdatePath(string path)
	{
		if (Directory.Exists(path))
		{
			PathReference.Value = path;
		}
	}
}
