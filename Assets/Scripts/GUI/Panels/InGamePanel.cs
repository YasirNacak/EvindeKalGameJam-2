using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GUI.Panels
{
    public class InGamePanel : GUIPanel
    {
        public GameObject ClockBase;
        public GameObject ClockMeter;
        public GameObject SpicesBase;
        public GameObject SpicesMeter;
        public GameObject SpicesMeterDark;
        public GameObject Countdown;
        public GameObject CookedText;
        public GameObject DontBurnText;

        private RectTransform _clockBaseRectTransform;
        private RectTransform _clockMeterRectTransform;
        private RectTransform _spicesBaseRectTransform;
        private RectTransform _spicesMeterRectTransform;
        private RectTransform _spicesMeterDarkRectTransform;
        private RectTransform _countdownRectTransform;
        private RectTransform _cookedTextRectTransform;
        private RectTransform _dontBurnTextRectTransform;

        private float _clockBaseDefaultY;
        private float _clockMeterDefaultY;
        private float _spicesBaseDefaultY;
        private float _spicesMeterDefaultY;
        private float _spicesMeterDarkDefaultY;
        private float _countdownDefaultY;

        public Color CountdownDefaultColor;
        public Color CountdownCriticalColor;

        [HideInInspector] public Image SpicesMeterImage;
        [HideInInspector] public Text CountdownText;

        private bool _isTimerRunning;

        public void Awake()
        {
            IsInitialized = false;
        }

        public override void Initialize()
        {
            SpicesMeterImage = SpicesMeter.GetComponent<Image>();
            CountdownText = Countdown.GetComponent<Text>();
            InnerObjects = new List<GameObject> { ClockBase, ClockMeter, SpicesBase, SpicesMeter, Countdown };

            _clockBaseRectTransform = ClockBase.GetComponent<RectTransform>();
            _clockMeterRectTransform = ClockMeter.GetComponent<RectTransform>();
            _spicesBaseRectTransform = SpicesBase.GetComponent<RectTransform>();
            _spicesMeterRectTransform = SpicesMeter.GetComponent<RectTransform>();
            _spicesMeterDarkRectTransform = SpicesMeterDark.GetComponent<RectTransform>();
            _cookedTextRectTransform = CookedText.GetComponent<RectTransform>();
            _dontBurnTextRectTransform = DontBurnText.GetComponent<RectTransform>();
            _countdownRectTransform = Countdown.GetComponent<RectTransform>();

            _clockBaseDefaultY = _clockBaseRectTransform.anchoredPosition.y;
            _clockMeterDefaultY = _clockMeterRectTransform.anchoredPosition.y;
            _spicesBaseDefaultY = _spicesBaseRectTransform.anchoredPosition.y;
            _spicesMeterDefaultY = _spicesMeterRectTransform.anchoredPosition.y;
            _spicesMeterDarkDefaultY = _spicesMeterDarkRectTransform.anchoredPosition.y;
            _countdownDefaultY = _countdownRectTransform.anchoredPosition.y;

            InnerObjects = new List<GameObject>() { ClockBase, ClockMeter, SpicesBase, SpicesMeter, Countdown, CookedText, DontBurnText };

            _isTimerRunning = false;
            IsInitialized = true;
        }

        public override void Open()
        {
            LeanTween.moveY(_clockBaseRectTransform, _clockBaseDefaultY + 600, 0);
            LeanTween.moveY(_clockMeterRectTransform, _clockMeterDefaultY + 600, 0);
            LeanTween.moveY(_spicesBaseRectTransform, _spicesBaseDefaultY + 600, 0);
            LeanTween.moveY(_spicesMeterRectTransform, _spicesMeterDefaultY + 600, 0);
            LeanTween.moveY(_spicesMeterDarkRectTransform, _spicesMeterDarkDefaultY + 600, 0);
            LeanTween.moveY(_countdownRectTransform, _countdownDefaultY + 600, 0);
            LeanTween.move(gameObject, gameObject.transform.position, 0).setOnComplete(() =>
            {
                gameObject.SetActive(true);
                LeanTween.moveY(_clockBaseRectTransform, _clockBaseDefaultY, GUIManager.Instance.TransitionTime).setEaseOutSine();
                LeanTween.moveY(_clockMeterRectTransform, _clockMeterDefaultY, GUIManager.Instance.TransitionTime).setEaseOutSine();
                LeanTween.moveY(_spicesBaseRectTransform, _spicesBaseDefaultY, GUIManager.Instance.TransitionTime).setEaseOutSine();
                LeanTween.moveY(_spicesMeterRectTransform, _spicesMeterDefaultY, GUIManager.Instance.TransitionTime).setEaseOutSine();
                LeanTween.moveY(_spicesMeterDarkRectTransform, _spicesMeterDarkDefaultY, GUIManager.Instance.TransitionTime).setEaseOutSine();
                LeanTween.moveY(_countdownRectTransform, _countdownDefaultY, GUIManager.Instance.TransitionTime).setEaseOutSine();
            });
        }

        public override void Close()
        {
            LeanTween.moveY(_clockBaseRectTransform, _clockBaseDefaultY + 600, GUIManager.Instance.TransitionTime).setEaseInSine();
            LeanTween.moveY(_clockMeterRectTransform, _clockMeterDefaultY + 600, GUIManager.Instance.TransitionTime).setEaseInSine();
            LeanTween.moveY(_spicesBaseRectTransform, _spicesBaseDefaultY + 600, GUIManager.Instance.TransitionTime).setEaseInSine();
            LeanTween.moveY(_spicesMeterRectTransform, _spicesMeterDefaultY + 600, GUIManager.Instance.TransitionTime).setEaseInSine();
            LeanTween.moveY(_spicesMeterDarkRectTransform, _spicesMeterDarkDefaultY + 600, GUIManager.Instance.TransitionTime).setEaseInSine();
            LeanTween.moveY(_countdownRectTransform, _countdownDefaultY + 600, GUIManager.Instance.TransitionTime).setEaseInSine();
            LeanTween.move(gameObject, gameObject.transform.position, GUIManager.Instance.TransitionWaitTime).setOnComplete(() =>
            {
                gameObject.SetActive(false);
                LeanTween.moveY(_clockBaseRectTransform, _clockBaseDefaultY, 0);
                LeanTween.moveY(_clockMeterRectTransform, _clockMeterDefaultY, 0);
                LeanTween.moveY(_spicesBaseRectTransform, _spicesBaseDefaultY, 0);
                LeanTween.moveY(_spicesMeterRectTransform, _spicesMeterDefaultY, 0);
                LeanTween.moveY(_spicesMeterDarkRectTransform, _spicesMeterDarkDefaultY, 0);
                LeanTween.moveY(_countdownRectTransform, _countdownDefaultY, 0);
            });
        }

        public void StartTimerAnimation()
        {
            _isTimerRunning = true;
            PulsateCountdownUp();
        }

        public void StopTimerAnimation()
        {
            _isTimerRunning = false;
            LeanTween.scale(_countdownRectTransform, Vector3.one, 0.15f).setEaseInSine();
        }

        private void PulsateCountdownUp()
        {
            if (!_isTimerRunning) return;
            LeanTween.scale(_countdownRectTransform, new Vector3(1.1f, 1.1f, 1.0f), 0.35f).setEaseOutSine().setOnComplete(PulsateCountdownDown);
        }

        private void PulsateCountdownDown()
        {
            if (!_isTimerRunning) return;
            LeanTween.scale(_countdownRectTransform, new Vector3(1.0f, 1.0f, 1.0f), 0.35f).setEaseOutSine().setOnComplete(PulsateCountdownUp);
        }

        public void SpawnCookedText()
        {
            LeanTween.scale(_cookedTextRectTransform, Vector3.zero, 0.0f);
            LeanTween.scale(_dontBurnTextRectTransform, Vector3.zero, 0.0f);
            LeanTween.scale(gameObject, Vector3.one, 0.0f).setOnComplete(() =>
            {
                CookedText.SetActive(true);
                DontBurnText.SetActive(true);
                LeanTween.scale(_cookedTextRectTransform, Vector3.one, 0.05f).setEaseOutSine();
                LeanTween.scale(_dontBurnTextRectTransform, Vector3.one, 0.05f).setEaseOutSine();
                LeanTween.scale(gameObject, Vector3.one, 1f).setOnComplete(() =>
                {
                    LeanTween.scale(_cookedTextRectTransform, Vector3.zero, 0.15f).setEaseInSine();
                    LeanTween.scale(_dontBurnTextRectTransform, Vector3.zero, 0.15f).setEaseInSine();
                    LeanTween.scale(gameObject, Vector3.one, 0.15f).setOnComplete(() =>
                    {
                        CookedText.SetActive(false);
                        DontBurnText.SetActive(false);
                        LeanTween.scale(_cookedTextRectTransform, Vector3.one, 0.0f);
                        LeanTween.scale(_dontBurnTextRectTransform, Vector3.one, 0.0f);
                    });
                });
            });
        }
    }
}
