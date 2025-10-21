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
        [SerializeField] private GameObject retryPanel; // panel with buttons
        [SerializeField] private float fadeDuration = 2f;
        [SerializeField] private float textFadeDuration = 1.5f;
        [SerializeField] private float panelFadeDuration = 1.5f; // new speed for panel fade

        private CanvasGroup retryCanvasGroup;

        private void Start()
        {
            if (fadePanel != null)
                fadePanel.color = new Color(0, 0, 0, 0); // fully transparent at start

            if (deathText != null)
                deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, 0); // hidden

            if (retryPanel != null)
            {
                retryPanel.SetActive(true); // keep active so buttons work
                retryCanvasGroup = retryPanel.GetComponent<CanvasGroup>();
                if (retryCanvasGroup == null)
                    retryCanvasGroup = retryPanel.AddComponent<CanvasGroup>();

                retryCanvasGroup.alpha = 0; // start invisible
                retryCanvasGroup.interactable = true; // buttons functional immediately
                retryCanvasGroup.blocksRaycasts = true; // buttons clickable
            }
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

            if (fadePanel != null)
                fadePanel.color = new Color(0, 0, 0, 1f);

            // Step 2: Fade in "YOU DIED" text
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

            // Optional pause to read "YOU DIED"
            yield return new WaitForSeconds(1f);

            // Step 3: Fade out "YOU DIED" text
            if (deathText != null)
            {
                timer = 0f;
                Color originalColor = deathText.color;
                while (timer < textFadeDuration)
                {
                    timer += Time.deltaTime;
                    float alpha = Mathf.Clamp01(1f - (timer / textFadeDuration));
                    deathText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                    yield return null;
                }
                deathText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            }

            // Step 4: Fade in retry panel
            if (retryCanvasGroup != null)
            {
                Time.timeScale = 0f; // pause game
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                timer = 0f;
                while (timer < panelFadeDuration)
                {
                    timer += Time.unscaledDeltaTime; // use unscaled time because timeScale = 0
                    float alpha = Mathf.Clamp01(timer / panelFadeDuration);
                    retryCanvasGroup.alpha = alpha;
                    yield return null;
                }
                retryCanvasGroup.alpha = 1f; // fully visible
            }
        }
    }
}
