using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.GUI
{
    public class GUIPanel : MonoBehaviour
    {
        protected List<GameObject> InnerObjects;
        public bool IsInitialized { get; protected set; }

        public virtual void Initialize()
        {
            if (IsInitialized)
            {
                Debug.LogWarning("Double Initializing GUI Panel");
                return;
            }
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            foreach (var innerObject in InnerObjects)
            {
                innerObject.SetActive(true);
            }
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            foreach (var innerObject in InnerObjects)
            {
                innerObject.SetActive(false);
            }
        }
    }
}
