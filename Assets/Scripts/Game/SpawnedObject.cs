using System.Collections;
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
            GameManager.Instance.DestroyLevelObject(_iCoord, _jCoord);
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
