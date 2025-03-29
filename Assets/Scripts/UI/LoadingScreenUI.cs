using System;
using System.Collections;
using Game.Data;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LoadingScreenUI : PanelBase
    {
        [SerializeField] private Slider progressSlider;

        [Header("Tips Display")]
        [SerializeField] private GameTips tipsCollection;
        [SerializeField] private CanvasGroup tipTextCanvasGroup;
        [SerializeField] private TextMeshProUGUI tipText;

        private int _currentTipIndex;
        private Coroutine _tipsCoroutine;

        private void Start() => Show();

        public override void Show()
        {
            base.Show();
            _tipsCoroutine = StartCoroutine(CycleTipsCoroutine());
        }

        public override void Hide()
        {
            base.Hide();
            StopCoroutine(_tipsCoroutine);
        }

        public void UpdateProgress(float progress)
        {
            Debug.Log($"Progress: {progress*100}%");
            progressSlider.value = progress;
        }

        private IEnumerator CycleTipsCoroutine()
        {
            const float TipDisplayTime = 5f;
            while (true)
            {
                tipText.text = tipsCollection.tips[_currentTipIndex];

                // awaits the entire duration of the animation
                var sequence = LSequence.Create()
                    .Append(LMotion.Create(0f, 1f, 1).BindToAlpha(tipTextCanvasGroup))
                    .AppendInterval(TipDisplayTime)
                    .Append(LMotion.Create(1f, 0f, 1).BindToAlpha(tipTextCanvasGroup));

                yield return sequence.Run().ToYieldInstruction();

                // cycle through tip collection
                _currentTipIndex = (_currentTipIndex + 1) % tipsCollection.tips.Length;
            }
        }
    }
}
