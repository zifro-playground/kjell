using UnityEngine;
using UnityEngine.UI;

namespace Kjell
{
	public class InputLabel : MonoBehaviour
	{
		public Text Text;
        
        private void Start()
        {
            if (Text.text == "")
            {
                Destroy(gameObject);
            }
        }
	}
}
