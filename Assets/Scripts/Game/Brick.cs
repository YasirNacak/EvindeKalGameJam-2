using System;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class Brick : MonoBehaviour
    {
        public int ICoord;
        public int JCoord;
        
        public void Start()
        {
            GameManager.Instance.RegisterBrick(transform);
        }
    }
}
