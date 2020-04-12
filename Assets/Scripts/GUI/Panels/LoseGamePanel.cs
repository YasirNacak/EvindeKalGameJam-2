using System.Collections.Generic;
using Assets.Scripts.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GUI.Panels
{
    public class LoseGamePanel : GUIPanel
    {
        public enum LoseReason
        {
            Burn,
            Spices,
            Cook
        }

        public GameObject RetryButton;

        public Text SubText;

        private RectTransform _selfRectTransform;
        private RectTransform _retryButtonRectTransform;

        private float _selfRectTransformDefaultY;

        private List<string> _burnLoseTexts = new List<string>()
        {
            "ouch!",
            "burning!!",
            "i'm burning!",
            "it smells burnt",
            "better luck next time"
        };

        private List<string> _spicesLoseTexts = new List<string>()
        {
            "need more spices :(",
            "spice me up",
            "where is pepper?!",
            "salt and pepper please",
            "spice spice spiceee"
        };

        private List<string> _cookLoseTexts = new List<string>()
        {
            "i should've been cooked more",
            "where is fire??",
            "cook cook cook",
            "cook me up",
            "me need fire"
        };

        public void Awake()
        {
            IsInitialized = false;
        }

        public override void Initialize()
        {
            _selfRectTransform = GetComponent<RectTransform>();
            _retryButtonRectTransform = RetryButton.GetComponent<RectTransform>();

            _selfRectTransformDefaultY = _selfRectTransform.anchoredPosition.y;

            InnerObjects = new List<GameObject> { RetryButton };

            IsInitialized = true;
        }

        public override void Open()
        {
            LeanTween.moveY(_selfRectTransform, _selfRectTransformDefaultY + 1200, 0);
            LeanTween.scale(gameObject, Vector3.one, 0).setOnComplete(() =>
            {
                GUIManager.Instance.OpenDarkTint(true);
                base.Open();
                LeanTween.moveY(_selfRectTransform, _selfRectTransformDefaultY, GUIManager.Instance.TransitionTime).setEaseOutSine();
            });
        }

        public override void Close()
        {
            LeanTween.moveY(_selfRectTransform, _selfRectTransformDefaultY + 1200, GUIManager.Instance.TransitionTime).setEaseInSine();
            GUIManager.Instance.CloseDarkTint(true);
            LeanTween.scale(gameObject, Vector3.one, GUIManager.Instance.TransitionWaitTime).setOnComplete(() =>
            {
                base.Close();
                LeanTween.moveY(_selfRectTransform, _selfRectTransformDefaultY, 0);
            });
        }

        public void SetSubText(LoseReason r)
        {
            switch (r)
            {
                case LoseReason.Burn:
                {
                    SubText.text = _burnLoseTexts[Random.Range(0, _burnLoseTexts.Count)];
                    break;
                }
                case LoseReason.Spices:
                {
                    SubText.text = _spicesLoseTexts[Random.Range(0, _spicesLoseTexts.Count)];
                    break;
                }
                case LoseReason.Cook:
                {
                    SubText.text = _cookLoseTexts[Random.Range(0, _cookLoseTexts.Count)];
                    break;
                }
            }
        }

        public void OnRetryButtonClicked()
        {
            Close();
            GUIManager.Instance.ClosePanel(GUIManager.Instance.InGamePanel);
            GameManager.Instance.EndLevel(false);
        }
        public void OnRetryButtonPointerDown()
        {
            LeanTween.scale(_retryButtonRectTransform, new Vector3(0.7f, 0.7f, 1.0f), 0.1f).setEaseInBack();
        }

        public void OnRetryButtonPointerUp()
        {
            LeanTween.scale(_retryButtonRectTransform, new Vector3(0.9f, 0.9f, 1.0f), 0.1f).setEaseInBack();
        }
    }
}
