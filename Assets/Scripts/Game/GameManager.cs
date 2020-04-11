using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
            Fire,
            Spice
        }

        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        public static GameManager Instance { get; private set; }

        public bool HasGameStarted;

        public Player Player;

        private int _currentLevel;
        public List<Level> Levels;
        private LevelObject[,] _levelObjects;

        public int LevelWidth;
        public int LevelHeight;

        private int _playerI;
        private int _playerJ;

        private List<Brick> _bricks;
        private List<SpawnedObject> _spawnedObjects;

        public GameObject SpawnerParent;
        public GameObject FirePrefab;
        public List<GameObject> SpicePrefabs;

        private int _cookedPoints;
        private int _spicePoints;

        private float _levelTimer;

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                _levelObjects = new LevelObject[LevelHeight, LevelWidth];
                _bricks = new List<Brick>();
                _spawnedObjects = new List<SpawnedObject>();
                _currentLevel = 0;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void Start()
        {
            ResetGame();
            GUIManager.Instance.OpenPanel(GUIManager.Instance.MainMenuPanel);
        }

        public void ResetGame()
        {
            _cookedPoints = 0;
            _spicePoints = 0;
            _levelTimer = 90.0f;
            HasGameStarted = false;
            _spawnedObjects = new List<SpawnedObject>();
            Player.gameObject.SetActive(false);
            GUIManager.Instance.InGamePanel.ClockMeter.transform.eulerAngles = new Vector3(0, 0, 90.75f);
            GUIManager.Instance.InGamePanel.SpicesMeter.GetComponent<Image>().fillAmount = 0.0f;
        }

        public void StartGame()
        {
            var level = Levels[_currentLevel];
            
            LeanTween.move(gameObject, gameObject.transform.position, GUIManager.Instance.TransitionWaitTime).setOnComplete(() =>
            {
                LeanTween.moveY(level.gameObject, -100, 0.0f).setOnComplete(() =>
                {
                    level.gameObject.SetActive(true);
                    level.LevelSprite.SetActive(true);
                    LeanTween.moveY(level.gameObject, 1, 0.2f).setEaseOutSine().setOnComplete(() =>
                    {
                        level.Bricks.SetActive(true);
                        var (i, j) = WorldToGrid(Player.transform.position);
                        _playerI = i;
                        _playerJ = j;
                        LeanTween.moveY(Player.gameObject, -53.0f, 0.0f).setOnComplete(() =>
                        {
                            Player.gameObject.SetActive(true);
                            LeanTween.moveY(Player.gameObject, -43.0f, 0.25f).setEaseOutSine();
                            _cookedPoints = 0;
                            _spicePoints = 0;
                            HasGameStarted = true;
                        });
                        
                        StartCoroutine(SpawnLevelObjectPeriodically(LevelObject.Fire, 1.5f));
                        StartCoroutine(SpawnLevelObjectPeriodically(LevelObject.Spice, 2.5f));
                    });
                });
            });
        }

        public void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                StartGame();
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                ResetGame();
            }

            if (!HasGameStarted) return;

            _levelTimer -= Time.deltaTime;

            GUIManager.Instance.InGamePanel.CountdownText.text = SecondsToClockString(_levelTimer);

            // todo: check if timer has finished

            if (_levelObjects[_playerI, _playerJ].Equals(LevelObject.Fire))
            {
                _cookedPoints++;
                GUIManager.Instance.InGamePanel.ClockMeter.transform.Rotate(0, 0, -720f / 1000f);
            }
            else if (_levelObjects[_playerI, _playerJ].Equals(LevelObject.Spice))
            {
                _spicePoints++;
                GUIManager.Instance.InGamePanel.SpicesMeterImage.fillAmount = _spicePoints / 500.0f;
            }
        }

        private string SecondsToClockString(float s)
        {
            var sRounded = Mathf.Round(s);
            var m = (int)(sRounded / 60.0f);
            var remS = (int) sRounded - (m * 60);
            var minStr = $"{m:D2}";
            var secStr = $"{remS:D2}";
            return minStr + ":" + secStr;
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

        private void SpawnLevelObject(LevelObject objectType)
        {
            while (true)
            {
                var hasSpawned = false;

                var brickIndex = Random.Range(0, _bricks.Count);

                var currentI = _bricks[brickIndex].ICoord;
                var currentJ = _bricks[brickIndex].JCoord;

                var direction = Random.Range(0, 4);

                int deltaI = 0, deltaJ = 0, rotX = 0, rotZ = 0;

                if (direction == 0)
                {
                    deltaI = 0; deltaJ = 1; rotX = 0; rotZ = -90;
                }
                else if (direction == 1)
                {
                    deltaI = 0; deltaJ = -1; rotX = 0; rotZ = 90;
                }
                else if (direction == 2)
                {
                    deltaI = 1; deltaJ = 0; rotX = 180; rotZ = 0;
                }
                else if (direction == 3)
                {
                    deltaI = -1; deltaJ = 0; rotX = 0; rotZ = 0;
                }

                if (IsCoordinateValid(currentI + deltaI, currentJ + deltaJ) &&
                    _levelObjects[currentI + deltaI, currentJ + deltaJ].Equals(LevelObject.Empty))
                {
                    GameObject prefab = null;
                    float aliveSeconds = 0.0f;
                    switch (objectType)
                    {
                        case LevelObject.Fire:
                        {
                            prefab = FirePrefab;
                            aliveSeconds = 3.5f;
                            _levelObjects[currentI + deltaI, currentJ + deltaJ] = LevelObject.Fire;
                            break;
                        }
                        case LevelObject.Spice:
                        {
                            var spiceIndex = Random.Range(0, SpicePrefabs.Count);
                            prefab = SpicePrefabs[spiceIndex];
                            aliveSeconds = 2.0f;
                            _levelObjects[currentI + deltaI, currentJ + deltaJ] = LevelObject.Spice;
                            break;
                        }
                        default:
                        {
                            Debug.LogError("Unknown object to spawn.");
                            break;
                        }
                    }

                    if (prefab != null)
                    {
                        var obj = Instantiate(prefab, SpawnerParent.transform);
                        var (i, j) = GridToWorld(currentI + deltaI, currentJ + deltaJ);
                        obj.transform.position = new Vector3(i, j + 1, 90);
                        obj.transform.Rotate(rotX, 0, rotZ);
                        var sObj = obj.GetComponent<SpawnedObject>();
                        sObj.Initialize(currentI + deltaI, currentJ + deltaJ, aliveSeconds);
                        _spawnedObjects.Add(sObj);
                        hasSpawned = true;
                    }
                }

                if (!hasSpawned)
                {
                    continue;
                }

                break;
            }
        }

        public void DestroyLevelObject(int iCoord, int jCoord)
        {
            _levelObjects[iCoord, jCoord] = LevelObject.Empty;
        }

        private IEnumerator SpawnLevelObjectPeriodically(LevelObject lo, float period)
        {
            var noisyPeriod = Random.Range(period - 0.5f, period + 0.5f);
            yield return new WaitForSeconds(noisyPeriod);

            if (HasGameStarted)
            {
                SpawnLevelObject(lo);
                StartCoroutine(SpawnLevelObjectPeriodically(lo, period));
            }
        }

        public void RegisterBrick(Transform t)
        {
            var brick = t.gameObject.GetComponent<Brick>();
            var (i, j) = WorldToGrid(t.position);
            _levelObjects[i, j] = LevelObject.Brick;
            brick.ICoord = i;
            brick.JCoord = j;
            _bricks.Add(brick);
        }

        public void MovePlayer(Direction d)
        {
            if (!HasGameStarted)
            {
                return;
            }

            bool canMove = false;

            switch (d)
            {
                case Direction.Left:
                {
                    if (_levelObjects[_playerI, _playerJ - 1].Equals(LevelObject.Empty) &&
                        (_levelObjects[_playerI + 1, _playerJ].Equals(LevelObject.Brick) || _levelObjects[_playerI - 1, _playerJ].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else 
                    if(_levelObjects[_playerI, _playerJ - 1].Equals(LevelObject.Empty) &&
                            (_levelObjects[_playerI + 1, _playerJ - 1].Equals(LevelObject.Brick) || _levelObjects[_playerI - 1, _playerJ - 1].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else if (_levelObjects[_playerI, _playerJ - 1].Equals(LevelObject.Fire))
                    {
                        canMove = true;
                    }
                    else if (_levelObjects[_playerI, _playerJ - 1].Equals(LevelObject.Spice))
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
                    if (_levelObjects[_playerI, _playerJ + 1].Equals(LevelObject.Empty) &&
                        (_levelObjects[_playerI + 1, _playerJ].Equals(LevelObject.Brick) || _levelObjects[_playerI - 1, _playerJ].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else
                    if (_levelObjects[_playerI, _playerJ + 1].Equals(LevelObject.Empty) &&
                        (_levelObjects[_playerI + 1, _playerJ + 1].Equals(LevelObject.Brick) || _levelObjects[_playerI - 1, _playerJ + 1].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else if (_levelObjects[_playerI, _playerJ + 1].Equals(LevelObject.Fire))
                    {
                        canMove = true;
                    }
                    else if (_levelObjects[_playerI, _playerJ + 1].Equals(LevelObject.Spice))
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
                    if (_levelObjects[_playerI - 1, _playerJ].Equals(LevelObject.Empty) &&
                        (_levelObjects[_playerI, _playerJ + 1].Equals(LevelObject.Brick) || _levelObjects[_playerI, _playerJ - 1].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else
                    if (_levelObjects[_playerI - 1, _playerJ].Equals(LevelObject.Empty) &&
                        (_levelObjects[_playerI - 1, _playerJ + 1].Equals(LevelObject.Brick) || _levelObjects[_playerI - 1, _playerJ - 1].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else if (_levelObjects[_playerI - 1, _playerJ].Equals(LevelObject.Fire))
                    {
                        canMove = true;
                    }
                    else if (_levelObjects[_playerI - 1, _playerJ].Equals(LevelObject.Spice))
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
                    if (_levelObjects[_playerI + 1, _playerJ].Equals(LevelObject.Empty) &&
                        (_levelObjects[_playerI, _playerJ + 1].Equals(LevelObject.Brick) || _levelObjects[_playerI, _playerJ - 1].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else
                    if (_levelObjects[_playerI + 1, _playerJ].Equals(LevelObject.Empty) &&
                        (_levelObjects[_playerI + 1, _playerJ + 1].Equals(LevelObject.Brick) || _levelObjects[_playerI + 1, _playerJ - 1].Equals(LevelObject.Brick)))
                    {
                        canMove = true;
                    }
                    else if (_levelObjects[_playerI + 1, _playerJ].Equals(LevelObject.Fire))
                    {
                        canMove = true;
                    }
                    else if (_levelObjects[_playerI + 1, _playerJ].Equals(LevelObject.Spice))
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
