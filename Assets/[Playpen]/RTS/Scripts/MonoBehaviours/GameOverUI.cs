using System;
using UnityEngine;


namespace RTS
{
    

    public class GameOverUI : MonoBehaviour
    {
        private void Start()
        {
            DOTSEvents.Instance.OnHQDead += OnHQDead;
            Hide();
        }

        private void OnHQDead(object sender, EventArgs e)
        {
            Show();
            Time.timeScale = 0;
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }
        
        private void Hide()
        {
            gameObject.SetActive(false);
        }
        
    }
    
    
}
