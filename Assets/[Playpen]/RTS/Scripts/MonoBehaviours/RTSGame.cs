using UnityEngine;


namespace RTS
{

    public class RTSGame : MonoBehaviour
    {
        public const int UNITS_LAYER = 10;

        public static RTSGame Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }


    }
}
