using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Assets.Scripts.GUI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

namespace Assets.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        private enum LevelObject
        {
            Empty,
            Brick,
            Player,
            Fire
        }

        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        public static GameManager Instance { get; private set; }

        public bool IsRunning;

        public Player Player;

        private LevelObject[,] _level;

        public int LevelWidth;
        public int LevelHeight;

        private int _playerI;
        private int _playerJ;

        private List<Brick> _bricks;

        public GameObject SpawnerParent;
        public GameObject FirePrefab;

        private int _playerPoints;

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                _level = new LevelObject[LevelHeight, LevelWidth];
                _bricks = new List<Brick>();
                DontDestroyOnLoad(gameObject);
            }
        }

        public void Start()
        {
            var (i, j) = WorldToGrid(Player.transform.position);
            _playerI = i;
            _playerJ = j;
            _playerPoints = 0;
            IsRunning = true;
            StartCoroutine(FireBrick());
        }

        public void Update()
        {
            if (_level[_playerI, _playerJ].Equals(LevelObject.Fire))
            {
                _playerPoints++;
                GUIManager.Instance.InGamePanel.ClockMeter.transform.Rotate(0, 0, -720f / 1000f);
            }
        }

        private Tuple<int, int> WorldToGrid(Vector2 v)
        {
            var iIndex = (int)((7 - ((v.y + 43) / 8)) + 1);
            var jIndex = (int)((v.x + 20) / 8) + 1;
            return new Tuple<int, int>(iIndex, jIndex);
        }

        private Tuple<int, int> GridToWorld(int i, int j)
        {
            return new Tuple<int, int>(-28 + j * 8, 20 - i * 8);
        }

        private bool IsCoordinateValid(int i, int j)
        {
            return i < LevelHeight && i >= 0 && j < LevelWidth && j >= 0;
        }

        private void SpawnFire()
        {
            while (true)
            {
                var hasSpawned = false;

                var brickIndex = Random.Range(0, _bricks.Count);

                var currentI = _bricks[brickIndex].ICoord;
                var currentJ = _bricks[brickIndex].JCoord;

                var direction = Random.Range(0, 4);

                int deltaI = 0;
                int deltaJ = 0;
                int rotX = 0;
                int rotZ = 0;

                if (direction == 0)
                {
                    deltaI = 0;
                    deltaJ = 1;
                    rotX = 0;
                    rotZ = -90;
                }
                else if (direction == 1)
                {
                    deltaI = 0;
                    deltaJ = -1;
                    rotX = 0;
                    rotZ = 90;
                }
                else if (direction == 2)
                {
                    deltaI = 1;
                    deltaJ = 0;
                    rotX = 180;
                    rotZ = 0;
                }
                else if (direction == 3)
                {
                    deltaI = -1;
                    deltaJ = 0;
                    rotX = 0;
                    rotZ = 0;
                }

                if (IsCoordinateValid(currentI + deltaI, currentJ + deltaJ) &&
                    _level[currentI + deltaI, currentJ + deltaJ].Equals(LevelObject.Empty))
                {
                    var fire = Instantiate(FirePrefab, SpawnerParent.transform);
                    var (i, j) = GridToWorld(currentI + deltaI, currentJ + deltaJ);
                    fire.transform.position = new Vector3(i, j + 1, 90);
                    fire.transform.Rotate(rotX, 0, rotZ);
                    var fireC = fire.GetComponent<Fire>();
                    fireC.Init(currentI + deltaI, currentJ + deltaJ);
                    _level[currentI + deltaI, currentJ + deltaJ] = LevelObject.Fire;
                    hasSpawned = true;
                }

                if (!hasSpawned)
                {
                    continue;
                }

                break;
            }
        }

        public void DestroyFire(int iCoord, int jCoord)
        {
            _level[iCoord, jCoord] = LevelObject.Empty;
        }

        private IEnumerator FireBrick()
        {
            yield return new WaitForSeconds(1.0f);

            SpawnFire();

            if (IsRunning)
            {
                StartCoroutine(FireBrick());
            }
        }

        public void RegisterBrick(Transform t)
        {
            var brick = t.gameObject.GetComponent<Brick>();
            var (i, j) = WorldToGrid(t.position);
            _level[i, j] = LevelObject.Brick;
            brick.ICoord = i;
            brick.JCoord = j;
            _bricks.Add(brick);
        }

        public void MovePlayer(Direction d)
        {
            bool canMove = false;

            switch (d)
            {
                case Direction.Left:
                {
                    if (_level[_playerI, _playerJ - 1].Equals(LevelObject.Empty) &&
                        (_level[_playerI + 1, _playerJ].Equals(LevelObject.Brick) || _level[_playerI - 1, _playerJ].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else 
                    if(_level[_playerI, _playerJ - 1].Equals(LevelObject.Empty) &&
                            (_level[_playerI + 1, _playerJ - 1].Equals(LevelObject.Brick) || _level[_playerI - 1, _playerJ - 1].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else if (_level[_playerI, _playerJ - 1].Equals(LevelObject.Fire))
                    {
                        canMove = true;
                    }

                    if (canMove)
                    {
                        LeanTween.moveLocalX(Player.gameObject, Player.transform.position.x - 8, 0.1f);
                        _playerJ--;
                    }
                    break;
                }
                case Direction.Right:
                {
                    if (_level[_playerI, _playerJ + 1].Equals(LevelObject.Empty) &&
                        (_level[_playerI + 1, _playerJ].Equals(LevelObject.Brick) || _level[_playerI - 1, _playerJ].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else
                    if (_level[_playerI, _playerJ + 1].Equals(LevelObject.Empty) &&
                        (_level[_playerI + 1, _playerJ + 1].Equals(LevelObject.Brick) || _level[_playerI - 1, _playerJ + 1].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else if (_level[_playerI, _playerJ + 1].Equals(LevelObject.Fire))
                    {
                        canMove = true;
                    }

                    if (canMove)
                    {
                        LeanTween.moveLocalX(Player.gameObject, Player.transform.position.x + 8, 0.1f);
                        _playerJ++;
                    }
                    break;
                }
                case Direction.Up:
                {
                    if (_level[_playerI - 1, _playerJ].Equals(LevelObject.Empty) &&
                        (_level[_playerI, _playerJ + 1].Equals(LevelObject.Brick) || _level[_playerI, _playerJ - 1].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else
                    if (_level[_playerI - 1, _playerJ].Equals(LevelObject.Empty) &&
                        (_level[_playerI - 1, _playerJ + 1].Equals(LevelObject.Brick) || _level[_playerI - 1, _playerJ - 1].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else if (_level[_playerI - 1, _playerJ].Equals(LevelObject.Fire))
                    {
                        canMove = true;
                    }

                    if (canMove)
                    {
                        LeanTween.moveLocalY(Player.gameObject, Player.transform.position.y + 8, 0.1f);
                        _playerI--;
                    }
                    break;
                }
                case Direction.Down:
                {
                    if (_level[_playerI + 1, _playerJ].Equals(LevelObject.Empty) &&
                        (_level[_playerI, _playerJ + 1].Equals(LevelObject.Brick) || _level[_playerI, _playerJ - 1].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else
                    if (_level[_playerI + 1, _playerJ].Equals(LevelObject.Empty) &&
                        (_level[_playerI + 1, _playerJ + 1].Equals(LevelObject.Brick) || _level[_playerI + 1, _playerJ - 1].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else if (_level[_playerI + 1, _playerJ].Equals(LevelObject.Fire))
                    {
                        canMove = true;
                    }

                    if (canMove)
                    {
                        LeanTween.moveLocalY(Player.gameObject, Player.transform.position.y - 8, 0.1f);
                        _playerI++;
                    }
                    break;
                }
                default:
                {
                    Debug.LogError("Invalid Direction");
                    break;
                }
            }
        }
    }
}
