using System;
using UnityEngine;


namespace RTS
{
    /// <summary>
    /// Controls the Game Over UI, displaying it when the HQ is destroyed.
    /// Subscribes to the OnHQDead event to show the UI and pause the game.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        /// <summary>
        /// Start is called before the first frame update.
        /// Subscribes to the OnHQDead event and hides the game over UI initially.
        /// </summary>
        private void Start()
        {
            DOTSEvents.Instance.OnHQDead += OnHQDead;
            Hide();
        }

        
        /// <summary>
        /// Called when the HQ is dead.
        /// Shows the game over UI and pauses the game by setting time scale to 0.
        /// </summary>
        private void OnHQDead(object sender, EventArgs e)
        {
            Show();
            Time.timeScale = 0;
        }

        
        /// <summary>
        /// Shows the game over UI by activating the GameObject.
        /// </summary>
        private void Show()
        {
            gameObject.SetActive(true);
        }

        
        /// <summary>
        /// Hides the game over UI by deactivating the GameObject.
        /// </summary>
        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
