using System.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public class FadePanel : MonoBehaviour
    {
        [SerializeField]
        private float fadeSpeed;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        }

        public async Task FadeIn()
        {
            await Fade(0, 1, fadeSpeed);
        }

        public async Task FadeOut()
        {
            await Fade(1, 0, fadeSpeed);
        }

        private async Task Fade(float startValue, float endValue, float duration)
        {
            float currentTime = 0f;

            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float t = Mathf.Clamp01(currentTime / duration);
                _canvasGroup.alpha = Mathf.Lerp(startValue, endValue, t);
                await Task.Yield();
            }

            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = endValue;
        }
    }
}
