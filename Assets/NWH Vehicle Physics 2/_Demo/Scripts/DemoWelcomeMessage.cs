using UnityEngine;
using UnityEngine.UI;

namespace NWH.VehiclePhysics2
{
    public class DemoWelcomeMessage : MonoBehaviour
    {
        public Button closeButton;

        void Start()
        {
            if (!Application.isEditor)
            {
                gameObject.SetActive(true);
            }

            closeButton.onClick.AddListener(Close);   
        }

        void Close()
        {
            gameObject.SetActive(false);
        }
    }
}