using Framework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Competition.Graph
{
	public class GraphLine : MonoBehaviour
	{
		public RectTransform Origin;
		public RectTransform Destination;
		public RectOffset Padding;

		[Space]
		[SerializeField] private RectTransform Top;
		[SerializeField] private RectTransform Body;
		[SerializeField] private RectTransform Bottom;


		private void Update()
		{
			if (Origin && Destination)
				UpdateDrawing();
		}

		public void UpdateDrawing()
		{
			bool flipHor = Destination.position.x < Origin.position.x;
			bool flipVer = Destination.position.y > Origin.position.y;


			float left = flipHor ? Destination.position.x : Origin.position.x;
			float right = flipHor ? Origin.position.x : Destination.position.x;
			float top = flipVer ? Destination.position.y : Origin.position.y;
			float bottom = flipVer ? Origin.position.y : Destination.position.y;


			float width = Mathf.Abs(right - left) / 2;
			float height = Mathf.Abs(bottom - top);

			Top.sizeDelta = new Vector2(width, Top.sizeDelta.y);
			Bottom.sizeDelta = new Vector2(width, Bottom.sizeDelta.y);
			Body.sizeDelta = new Vector2(Body.sizeDelta.x, height);

			if (flipVer)
			{
				Top.position = new Vector3(flipHor ? left : Body.position.x, Top.position.z);
				Bottom.position = new Vector3(flipHor ? Body.position.x : right, Bottom.position.z);
			}
			else
			{
				Top.position = new Vector3(flipHor ? Body.position.x : left, top, Top.position.z);
				Bottom.position = new Vector3(flipHor ? Body.position.x : right, bottom, Bottom.position.z);
			}

			Body.position = (Origin.position + Destination.position) / 2;
		}
	}
}
