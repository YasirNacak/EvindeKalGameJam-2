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
        public GameObject Countdown;

        [HideInInspector] public Image SpicesMeterImage;

        [HideInInspector] public Text CountdownText;

        public void Awake()
        {
            IsInitialized = false;
        }

        public override void Initialize()
        {
            SpicesMeterImage = SpicesMeter.GetComponent<Image>();

            CountdownText = Countdown.GetComponent<Text>();

            InnerObjects = new List<GameObject> { ClockBase, ClockMeter, SpicesBase, SpicesMeter, Countdown };

            IsInitialized = true;
        }
    }
}
