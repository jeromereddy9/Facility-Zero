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
        [SerializeField] private float textFadeDuration = 1.5f; // separate speed for text

        private void Start()
        {
            if (fadePanel != null)
                fadePanel.color = new Color(0, 0, 0, 0); // fully transparent at start

            if (deathText != null)
                deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, 0); // hidden text
        }

        public void TriggerDeathScreen()
        {
            StartCoroutine(FadeToDeath());
        }

        private IEnumerator FadeToDeath()
        {
            // Step 1: Fade screen to black
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Clamp01(timer / fadeDuration);
                if (fadePanel != null)
                    fadePanel.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            // ?? Force final state to full black
            if (fadePanel != null)
                fadePanel.color = new Color(0, 0, 0, 1f);

            // Step 2: Fade in "YOU DIED" text after screen is black
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

                // ?? Force final state to fully visible
                deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, 1f);
            }
        }
    }
}
