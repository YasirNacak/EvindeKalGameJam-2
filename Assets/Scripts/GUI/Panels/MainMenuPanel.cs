using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUI.Panels
{
    public class MainMenuPanel : GUIPanel
    {
        public GameObject TitleText;

        private RectTransform TitleTextRectTransform;

        private float TitleTextDefaultY;

        public void Awake()
        {
            IsInitialized = false;
        }

        public override void Initialize()
        {
            base.Initialize();

            TitleTextRectTransform = TitleText.GetComponent<RectTransform>();

            TitleTextDefaultY = TitleTextRectTransform.anchoredPosition.y;

            InnerObjects = new List<GameObject> { TitleText };

            IsInitialized = true;
        }

        public override void Open()
        {
            LeanTween.moveY(TitleTextRectTransform, TitleTextDefaultY+ 700, 0);
            LeanTween.move(gameObject, gameObject.transform.position, 0).setOnComplete(() =>
            {
                GUIManager.Instance.OpenDarkTint(true);
                base.Open();
                LeanTween.moveY(TitleTextRectTransform, TitleTextDefaultY, GUIManager.Instance.TransitionTime).setEaseOutSine();
            });
        }

        public override void Close()
        {
            LeanTween.moveY(TitleTextRectTransform, TitleTextDefaultY + 700, GUIManager.Instance.TransitionTime).setEaseInSine();
            GUIManager.Instance.CloseDarkTint(true);
            LeanTween.move(gameObject, gameObject.transform.position, GUIManager.Instance.TransitionWaitTime).setOnComplete(() =>
            {
                base.Close();
                LeanTween.moveY(TitleTextRectTransform, TitleTextDefaultY, 0);
            });
        }

        public void OnPlayButtonClicked()
        {
            /*
            Close();
            GUIManager.Instance.OpenPanel(GUIManager.Instance.InGamePanel);
            */
        }
    }
}
