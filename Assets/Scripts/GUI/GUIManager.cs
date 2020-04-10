using System.Collections.Generic;
using Assets.Scripts.GUI.Panels;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GUI
{
    public class GUIManager : MonoBehaviour
    {
        public static GUIManager Instance { get; private set; }

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        [SerializeField] public readonly float TransitionTime = 0.3f;
        [SerializeField] public readonly float TransitionWaitTime = 0.4f;
        
        [SerializeField] private Image DarkTint;
        private Color DarkTintActiveColor;
        private Color DarkTintInactiveColor;
        
        public List<GUIPanel> ActivePanels;

        public MainMenuPanel MainMenuPanel;

        public void Start()
        {
            DarkTintActiveColor = new Color(0, 0, 0, 0.6f);
            DarkTintInactiveColor = new Color(0, 0, 0, 0);
            MainMenuPanel.Initialize();
        }

        public void OpenPanel(GUIPanel panel)
        {
            if (!panel.IsInitialized)
            {
                panel.Initialize();
            }

            foreach (var activePanel in ActivePanels)
            {
                activePanel.Close();
            }

            ActivePanels.Clear();

            LeanTween.move(gameObject, gameObject.transform.position, TransitionWaitTime).setOnComplete(
                () =>
                {
                    ActivePanels.Add(panel);
                    panel.Open();
                });
        }

        public void ClosePanel(GUIPanel panel)
        {
            ActivePanels.Remove(panel);
            panel.Close();
        }

        public void OpenPanelOnTop(GUIPanel panel)
        {
            ActivePanels.Add(panel);
            panel.Open();
        }

        public void OpenDarkTint(bool isGradually)
        {
            if (isGradually)
            {
                DarkTint.color = DarkTintInactiveColor;
                DarkTint.gameObject.SetActive(true);
                LeanTween.value(gameObject, 0, 0.6f, TransitionTime).setOnUpdate((float val) =>
                {
                    Color c = DarkTint.color;
                    c.a = val;
                    DarkTint.color = c;
                });
            }
            else
            {
                DarkTint.color = DarkTintActiveColor;
                DarkTint.gameObject.SetActive(true);
            }
        }

        public void CloseDarkTint(bool isGradually)
        {
            if (isGradually)
            {
                LeanTween.value(gameObject, 0.6f, 0.0f, TransitionTime).setOnUpdate((float val) =>
                {
                    Color c = DarkTint.color;
                    c.a = val;
                    DarkTint.color = c;
                }).setOnComplete(() =>
                {
                    DarkTint.gameObject.SetActive(false);
                });
            }
            else
            {
                DarkTint.gameObject.SetActive(false);
                DarkTint.color = DarkTintInactiveColor;
            }
        }

        public void Update()
        {
            // TODO(yasir): remove
            if (Input.GetKeyDown(KeyCode.F1))
            {
                OpenPanel(MainMenuPanel);
            }
        }
    }
}
