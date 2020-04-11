using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUI.Panels
{
    public class HowToPlayPanel : GUIPanel
    {
        public RectTransform BackToMenuButtonRectTransform;
        private RectTransform _selfRectTransform;

        private float _selfRectTransformDefaultY;
        public void Awake()
        {
            IsInitialized = false;
        }

        public override void Initialize()
        {
            _selfRectTransform = GetComponent<RectTransform>();

            _selfRectTransformDefaultY = _selfRectTransform.anchoredPosition.y;

            InnerObjects = new List<GameObject> { };

            IsInitialized = true;
        }

        public override void Open()
        {
            LeanTween.moveY(_selfRectTransform, _selfRectTransformDefaultY + 1200, 0);
            LeanTween.scale(gameObject, Vector3.one, 0).setOnComplete(() =>
            {
                base.Open();
                LeanTween.moveY(_selfRectTransform, _selfRectTransformDefaultY, GUIManager.Instance.TransitionTime).setEaseOutSine();
            });
        }

        public override void Close()
        {
            LeanTween.moveY(_selfRectTransform, _selfRectTransformDefaultY + 1200, GUIManager.Instance.TransitionTime).setEaseInSine();
            LeanTween.scale(gameObject, Vector3.one, GUIManager.Instance.TransitionWaitTime).setOnComplete(() =>
            {
                base.Close();
                LeanTween.moveY(_selfRectTransform, _selfRectTransformDefaultY, 0);
            });
        }

        public void OnBackToMenuButtonClicked()
        {
            GUIManager.Instance.OpenPanel(GUIManager.Instance.MainMenuPanel);
        }

        public void OnBackToMenuButtonPointerDown()
        {
            LeanTween.scale(BackToMenuButtonRectTransform, new Vector3(0.8f, 0.8f, 1.0f), 0.1f).setEaseInBack();
        }

        public void OnBackToMenuButtonPointerUp()
        {
            LeanTween.scale(BackToMenuButtonRectTransform, Vector3.one, 0.1f).setEaseInBack();
        }
    }
}
