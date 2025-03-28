using System.Collections;
using Game.Data;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingScreenUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tipText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameTips tipsCollection;
        private int _currentTipIndex;
        private Coroutine _tipsCoroutine;

        private void Start() => Show();

        private void Show()
        {
            canvasGroup.alpha = 1;
            _tipsCoroutine = StartCoroutine(CycleTipsCoroutine());
        }

        private void Hide()
        {
            canvasGroup.alpha = 0;
            StopCoroutine(_tipsCoroutine);
        }

        public void UpdateProgress(float progress) => progressSlider.value = progress;

        private IEnumerator CycleTipsCoroutine()
        {
            const float TipDisplayTime = 5f;
            while (true)
            {
                tipText.text = tipsCollection.tips[_currentTipIndex];

                // awaits the entire duration of the animation
                var sequence = LSequence.Create()
                    .Append(LMotion.Create(Color.clear, Color.white, 1).BindToColor(tipText))
                    .AppendInterval(TipDisplayTime)
                    .Append(LMotion.Create(Color.white, Color.clear, 1).BindToColor(tipText));

                yield return sequence.Run().ToYieldInstruction();

                // cycle through tip collection
                _currentTipIndex = (_currentTipIndex + 1) % tipsCollection.tips.Length;
            }
        }
    }
}
