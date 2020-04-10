using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class Fire : MonoBehaviour
    {
        private int _iCoord;
        private int _jCoord;
        public void Init(int iCoord, int jCoord)
        {
            _iCoord = iCoord;
            _jCoord = jCoord;
            StartCoroutine(Burn());
        }

        private IEnumerator Burn()
        {
            yield return new WaitForSeconds(2.5f);
            GameManager.Instance.DestroyFire(_iCoord, _jCoord);
            Destroy(gameObject);
        }
    }
}
