using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace FacilityZero.DeathScreen
{
    public class DeathScreen : MonoBehaviour
    {
        [SerializeField] private Image fadePanel;
        [SerializeField] private TMP_Text deathText;
        [SerializeField] private float fadeDuration = 2f;
        [SerializeField] private float textFadeDuration = 1.5f; 

        private void Start()
        {
            if (fadePanel != null)
                fadePanel.color = new Color(0, 0, 0, 0); 

            if (deathText != null)
                deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, 0); 
        }

        public void TriggerDeathScreen()
        {
            StartCoroutine(FadeToDeath());
        }

        private IEnumerator FadeToDeath()
        {
            
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Clamp01(timer / fadeDuration);
                if (fadePanel != null)
                    fadePanel.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            
            if (fadePanel != null)
                fadePanel.color = new Color(0, 0, 0, 1f);

            
            if (deathText != null)
            {
                timer = 0f;
                while (timer < textFadeDuration)
                {
                    timer += Time.deltaTime;
                    float alpha = Mathf.Clamp01(timer / textFadeDuration);
                    deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, alpha);
                    yield return null;
                }

               
                deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, 1f);
            }
        }
    }
}
