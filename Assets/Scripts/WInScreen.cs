using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace FacilityZero.UI
{
    public class WinScreen : MonoBehaviour
    {
        [SerializeField] private Image fadePanel;
        [SerializeField] private TMP_Text winText;
        [SerializeField] private float fadeDuration = 2f;
        [SerializeField] private float textFadeDuration = 1.5f;

        private void Start()
        {
            if (fadePanel != null)
                fadePanel.color = new Color(0, 0, 0, 0);

            if (winText != null)
                winText.color = new Color(winText.color.r, winText.color.g, winText.color.b, 0);
        }

        public void TriggerWinScreen()
        {
            StartCoroutine(FadeToWin());
        }

        private IEnumerator FadeToWin()
        {
            float timer = 0f;

            // Fade to black
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

            // Fade in "YOU WIN"
            if (winText != null)
            {
                timer = 0f;
                while (timer < textFadeDuration)
                {
                    timer += Time.deltaTime;
                    float alpha = Mathf.Clamp01(timer / textFadeDuration);
                    winText.color = new Color(winText.color.r, winText.color.g, winText.color.b, alpha);
                    yield return null;
                }

                winText.color = new Color(winText.color.r, winText.color.g, winText.color.b, 1f);
            }
        }
    }
}
