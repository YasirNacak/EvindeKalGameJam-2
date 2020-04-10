using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUI.Panels
{
    public class InGamePanel : GUIPanel
    {
        public GameObject ClockBase;

        public GameObject ClockMeter;

        public void Awake()
        {
            IsInitialized = false;
        }

        public override void Initialize()
        {
            base.Initialize();

            InnerObjects = new List<GameObject> { ClockBase, ClockMeter };

            IsInitialized = true;
        }
    }
}
