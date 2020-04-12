using UnityEngine;

namespace Assets.Scripts.Game
{
    public class Player : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                GameManager.Instance.Move(GameManager.Direction.Left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                GameManager.Instance.Move(GameManager.Direction.Right);
            }
        }
    }
}
