using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class Level : MonoBehaviour
    {
        public GameObject Bricks;
        public GameObject LevelSprite;
        public float TimeLimit;
        public List<MovementStyle> Movement;
    }
}
