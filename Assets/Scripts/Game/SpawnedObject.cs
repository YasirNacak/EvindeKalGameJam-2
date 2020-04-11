using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class SpawnedObject : MonoBehaviour
    {
        private int _iCoord;
        private int _jCoord;
        private float _aliveSeconds;

        public void Initialize(int iCoord, int jCoord, float aliveSeconds)
        {
            _iCoord = iCoord;
            _jCoord = jCoord;
            _aliveSeconds = aliveSeconds;
            StartCoroutine(Burn());
        }

        private IEnumerator Burn()
        {
            yield return new WaitForSeconds(_aliveSeconds);
            DestroyObject();
        }

        public void DestroyObject()
        {
            GameManager.Instance.DestroyLevelObject(_iCoord, _jCoord);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
