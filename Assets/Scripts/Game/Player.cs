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

            if (Input.GetKeyDown(KeyCode.A))
            {
                GameManager.Instance.MovePlayer(GameManager.Direction.Left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                GameManager.Instance.MovePlayer(GameManager.Direction.Right);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                GameManager.Instance.MovePlayer(GameManager.Direction.Up);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                GameManager.Instance.MovePlayer(GameManager.Direction.Down);
            }
        }
    }
}
