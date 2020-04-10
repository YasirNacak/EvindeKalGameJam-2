using UnityEngine;

namespace Assets.Scripts.Game
{
    public class Player : MonoBehaviour
    {
        void Start()
        {
        
        }

        void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                // move left
                GameManager.Instance.MovePlayer(GameManager.Direction.Left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                // move right
                GameManager.Instance.MovePlayer(GameManager.Direction.Right);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                // move up
                GameManager.Instance.MovePlayer(GameManager.Direction.Up);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                // move down
                GameManager.Instance.MovePlayer(GameManager.Direction.Down);
            }
        }
    }
}
