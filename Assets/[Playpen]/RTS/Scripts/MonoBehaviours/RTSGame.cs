using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Singleton class that serves as a central point for RTS game settings.
    /// </summary>
    public class RTSGame : MonoBehaviour
    {
        #region Public Constants

        /// <summary> Layer for units. </summary>
        public const int UNITS_LAYER = 10;
        public const int BUILDINGS_LAYER = 11;

        
        public UnitSetScriptableObject units;

        #endregion

        
        #region Static Class Variables

        /// <summary>Singleton instance.</summary>
        public static RTSGame Instance { get; private set; }
        
        #endregion


        #region MonoBehaviour Callbacks
        
        /// <summary>
        /// Stores reference to the singleton instance of RTSGame.
        /// If an instance already exists, the new one is destroyed to enforce the singleton pattern.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        
        #endregion
        
        
    }
}
