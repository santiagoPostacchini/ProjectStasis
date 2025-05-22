using UnityEngine;
using UnityEngine.UI;

namespace Interaction
{
    public class ThrowUISlider : MonoBehaviour
    {
        public static ThrowUISlider Instance;
        [SerializeField] private Image fillImage;

        private void Awake()
        {
            Instance = this;
        }


        public void SetFill(float value)
        {
            fillImage.fillAmount = value;
        }
        
    }
}
