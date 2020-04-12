using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GUI;
using Assets.Scripts.GUI.Panels;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
        private bool _isWinning;

        private float _levelTimer;

        private int _playerMovementIndex;

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void Start()
        {
            _currentLevel = 0;
            _bricks = new List<Brick>();
            _levelObjects = new LevelObject[LevelHeight, LevelWidth];
            ResetGame();
            GUIManager.Instance.OpenPanel(GUIManager.Instance.MainMenuPanel);
        }

        private void StopGame()
        {
            HasGameStarted = false;
            GUIManager.Instance.InGamePanel.StopTimerAnimation();
        }

        public void ResetGame()
        {
            HasGameStarted = false;
            _playerMovementIndex = 0;
            _cookedPoints = 0;
            _spicePoints = 0;
            _levelTimer = Levels[_currentLevel].TimeLimit;
            _spawnedObjects = new List<SpawnedObject>();
            _spawnedObjects = new List<SpawnedObject>();
            Player.gameObject.SetActive(false);
            LeanTween.rotate(Player.gameObject, Vector3.zero, 0.0f);
            GUIManager.Instance.InGamePanel.ClockMeter.transform.eulerAngles = new Vector3(0, 0, 90.75f);
            GUIManager.Instance.InGamePanel.SpicesMeter.GetComponent<Image>().fillAmount = 0.0f;
            GUIManager.Instance.InGamePanel.Countdown.GetComponent<Text>().color = GUIManager.Instance.InGamePanel.CountdownDefaultColor;
        }

        public void EndLevel(bool isAdvanced)
        {
            var level = Levels[_currentLevel];
            foreach (var spawnedObject in _spawnedObjects)
            {
                spawnedObject.DestroyObject();
            }
            LeanTween.scale(Player.gameObject, Vector3.zero, 0.25f).setEaseInSine();
            LeanTween.move(gameObject, gameObject.transform.position, 0.2f).setOnComplete(() =>
            {
                LeanTween.moveY(level.gameObject, -150, 0.25f).setEaseInSine().setOnComplete(() =>
                {
                    GUIManager.Instance.OpenPanel(GUIManager.Instance.InGamePanel);
                    if (isAdvanced)
                    {
                        if (_currentLevel < Levels.Count - 1)
                        {
                            _bricks = new List<Brick>();
                            _levelObjects = new LevelObject[LevelHeight, LevelWidth];
                            _currentLevel++;
                        }
                        else
                        {
                            print("max level reached!");
                        }
                    }
                    ResetGame();
                    StartGame();
                });
            });
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
                        Player.transform.position = new Vector3(-20f, -43f, 90f);
                        var (i, j) = WorldToGrid(Player.transform.position);
                        _playerI = i;
                        _playerJ = j;
                        LeanTween.moveY(Player.gameObject, -53.0f, 0.0f).setOnComplete(() =>
                        {
                            Player.gameObject.SetActive(true);
                            Player.transform.localScale = Vector3.one;
                            LeanTween.moveY(Player.gameObject, -43.0f, 0.25f).setEaseOutSine();
                            _cookedPoints = 0;
                            _spicePoints = 0;
                            HasGameStarted = true;
                            _isWinning = false;
                            GUIManager.Instance.InGamePanel.StartTimerAnimation();
                        });
                        
                        StartCoroutine(SpawnLevelObjectPeriodically(LevelObject.Fire, 1.5f));
                        StartCoroutine(SpawnLevelObjectPeriodically(LevelObject.Spice, 2.5f));
                    });
                });
            });
        }

        public void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                GUIManager.Instance.OpenPanel(GUIManager.Instance.WinGamePanel);
                LeanTween.scale(gameObject, Vector3.one, GUIManager.Instance.TransitionTime * 2.0f).setOnComplete(() =>
                {
                    GUIManager.Instance.WinGamePanel.AnimateChefHats(2);
                });
            }

            if (!HasGameStarted) return;

            _levelTimer -= Time.deltaTime;

            GUIManager.Instance.InGamePanel.CountdownText.text = SecondsToClockString(_levelTimer);

            if (_levelTimer <= 10.0f)
            {
                GUIManager.Instance.InGamePanel.CountdownText.color = GUIManager.Instance.InGamePanel.CountdownCriticalColor;
            }

            if (_levelTimer <= 0.1f)
            {
                StopGame();

                if (_isWinning)
                {
                    GUIManager.Instance.OpenPanelOnTop(GUIManager.Instance.WinGamePanel);
                    LeanTween.scale(gameObject, Vector3.one, GUIManager.Instance.TransitionTime * 2.0f).setOnComplete(() =>
                    {
                        GUIManager.Instance.WinGamePanel.AnimateChefHats(_spicePoints / 200);
                    });
                    print("you win");
                }
                else
                {
                    if (_spicePoints < 200)
                    {
                        GUIManager.Instance.LoseGamePanel.SetSubText(LoseGamePanel.LoseReason.Spices);
                    }
                    else
                    {
                        GUIManager.Instance.LoseGamePanel.SetSubText(LoseGamePanel.LoseReason.Cook);
                    }
                    GUIManager.Instance.OpenPanelOnTop(GUIManager.Instance.LoseGamePanel);
                    print("you lose");
                }
            }

            if (_levelObjects[_playerI, _playerJ].Equals(LevelObject.Fire))
            {
                _cookedPoints++;

                if (_cookedPoints >= 330)
                {
                    StopGame();
                    print("you lose");
                    _isWinning = false;
                    GUIManager.Instance.LoseGamePanel.SetSubText(LoseGamePanel.LoseReason.Burn);
                    GUIManager.Instance.OpenPanelOnTop(GUIManager.Instance.LoseGamePanel);
                }
                GUIManager.Instance.InGamePanel.ClockMeter.transform.Rotate(0, 0, -720f / 1000f);
            }
            else if (_spicePoints < 600 && _levelObjects[_playerI, _playerJ].Equals(LevelObject.Spice))
            {
                _spicePoints++;
                GUIManager.Instance.InGamePanel.SpicesMeterImage.fillAmount = _spicePoints / 600.0f;
            }

            if (_cookedPoints >= 250 && _spicePoints >= 200 && !_isWinning)
            {
                _isWinning = true;
                _levelTimer = 10;
                GUIManager.Instance.InGamePanel.SpawnCookedText();
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
                            aliveSeconds = 3.0f;
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

        public void Move(Direction d)
        {
            
            var playerPos = Player.transform.position;

            if (d.Equals(Direction.Right))
            {
                var ind = _playerMovementIndex;
                if (ind == Levels[_currentLevel].Movement.Count)
                {
                    ind = 0;
                }
                var ms = Levels[_currentLevel].Movement[ind];
                LeanTween.rotate(Player.gameObject, new Vector3(0, 0, ms.ZRotation), 0.1f).setEaseOutSine();
                LeanTween.move(Player.gameObject, new Vector3(playerPos.x + ms.IMovement * 8, playerPos.y + ms.JMovement * 8, playerPos.z), 0.1f).setEaseOutSine();
                _playerMovementIndex++;
                if (_playerMovementIndex >= Levels[_currentLevel].Movement.Count)
                {
                    _playerMovementIndex = 0;
                }

                _playerJ += ms.IMovement;
                _playerI -= ms.JMovement;
            }
            else if (d.Equals(Direction.Left))
            {
                int ind1 = _playerMovementIndex - 1;
                if (ind1 == -1)
                {
                    ind1 = Levels[_currentLevel].Movement.Count - 1;
                }

                int ind2 = _playerMovementIndex - 2;
                if (ind2 == -1)
                {
                    ind2 = Levels[_currentLevel].Movement.Count - 1;
                }
                if (ind2 == -2)
                {
                    ind2 = Levels[_currentLevel].Movement.Count - 2;
                }
                var ms = Levels[_currentLevel].Movement[ind1];
                var msRot = Levels[_currentLevel].Movement[ind2];
                LeanTween.rotate(Player.gameObject, new Vector3(0, 0, msRot.ZRotation), 0.1f).setEaseOutSine();
                LeanTween.move(Player.gameObject, new Vector3(playerPos.x + (-ms.IMovement) * 8, playerPos.y + (-ms.JMovement) * 8, playerPos.z), 0.1f).setEaseOutSine();
                _playerMovementIndex--;
                if (_playerMovementIndex == -1)
                {
                    _playerMovementIndex = Levels[_currentLevel].Movement.Count - 1;
                }

                _playerJ += -ms.IMovement;
                _playerI -= -ms.JMovement;
            }
        }
    }
}
