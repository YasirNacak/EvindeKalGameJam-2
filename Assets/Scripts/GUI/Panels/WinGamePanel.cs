using System.Collections.Generic;
using Assets.Scripts.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GUI.Panels
{
    public class WinGamePanel : GUIPanel
    {
        public GameObject NextLevelButton;
        public List<GameObject> ChefHats;

        private List<Image> _chefHatImages;

        public Text SubText;

        private RectTransform _selfRectTransform;
        private RectTransform _nextLevelButtonRectTransform;

        private float _selfRectTransformDefaultY;

        private List<string> _winTexts = new List<string>()
        {
            "delicious!",
            "yum yum!",
            "time to eat!",
            "you did it!",
            "yaaay!",
        };

        public void Awake()
        {
            IsInitialized = false;
        }

        public override void Initialize()
        {
            _selfRectTransform = GetComponent<RectTransform>();
            _nextLevelButtonRectTransform = NextLevelButton.GetComponent<RectTransform>();

            _selfRectTransformDefaultY = _selfRectTransform.anchoredPosition.y;

            _chefHatImages = new List<Image>();

            InnerObjects = new List<GameObject> { NextLevelButton };

            foreach (var chefHat in ChefHats)
            {
                _chefHatImages.Add(chefHat.GetComponent<Image>());
                InnerObjects.Add(chefHat);
            }

            IsInitialized = true;
        }

        public override void Open()
        {
            SubText.text = _winTexts[Random.Range(0, _winTexts.Count)];
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
                foreach (var chefHatImage in _chefHatImages)
                {
                    Color c = chefHatImage.color;
                    c.a = 0.5f;
                    chefHatImage.color = c;
                }
            });
        }

        public void AnimateChefHats(int count)
        {
            LeanTween.scale(gameObject, Vector3.one, 0.35f).setOnComplete(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    AnimateSingleChefHat(i);
                }
            });
        }

        private void AnimateSingleChefHat(int index)
        {
            LeanTween.scale(ChefHats[index], new Vector3(1.1f, 1.1f, 1.0f), 0.35f).setEaseOutBack().setOnComplete(() =>
            {
                LeanTween.scale(ChefHats[index], new Vector3(0.85f, 0.85f, 1.0f), 0.35f).setEaseInBack();
            });
            LeanTween.value(gameObject, 0.5f, 1.0f, 0.35f).setOnUpdate((float val) =>
            {
                Color c = _chefHatImages[index].color;
                c.a = val;
                _chefHatImages[index].color = c;
            });
        }

        public void OnNextLevelButtonClicked()
        {
            Close();
            GUIManager.Instance.ClosePanel(GUIManager.Instance.InGamePanel);
            LeanTween.scale(gameObject, Vector3.one, GUIManager.Instance.TransitionWaitTime).setOnComplete(() =>
            {
                GameManager.Instance.EndLevel(true);
            });
        }
        public void OnNextLevelButtonPointerDown()
        {
            LeanTween.scale(_nextLevelButtonRectTransform, new Vector3(0.7f, 0.7f, 1.0f), 0.1f).setEaseInBack();
        }

        public void OnNextLevelButtonPointerUp()
        {
            LeanTween.scale(_nextLevelButtonRectTransform, new Vector3(0.9f, 0.9f, 1.0f), 0.1f).setEaseInBack();
        }
    }
}
