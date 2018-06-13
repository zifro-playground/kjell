using UnityEngine;
using UnityEngine.UI;

namespace Kjell
{
	public class Container : MonoBehaviour
	{
		public RectTransform TextTransform;
		public LayoutElement LayoutElement;

		void Update()
		{
			UpdateWidth();
		}

		public void UpdateWidth()
		{
			if (TextTransform.rect.width >= 275)
				LayoutElement.preferredWidth = 300;
			else if (TextTransform.rect.width < 250)
				LayoutElement.preferredWidth = -1;
		}
	}
}
