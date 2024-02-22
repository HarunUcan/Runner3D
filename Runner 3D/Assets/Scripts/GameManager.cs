using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Game Manager is null");

            return _instance;
        }
    }

    private Queue<GameObject> _GameAreas = new Queue<GameObject>();
    [SerializeField] private GameObject _platformPrefab;
    [SerializeField] private int _numOfGamePlatforms;
    private GameObject _lastObjOfQueue;

    [SerializeField] private GameObject _goldPrefab;
    private Queue<GameObject> _GoldPool = new Queue<GameObject>();

    [SerializeField] private List<ObstacleType> _Obstacles = new List<ObstacleType>();
    private List<GameObject> _ObstaclePool = new List<GameObject>();

    private float _score = 0;
    private int _highScore = 0;

    private bool isHighScoreStyleActive = false;
    void Awake()
    {
        _instance = this;

        JsonSaveLoad jsonSaveLoad = new JsonSaveLoad();
        jsonSaveLoad.LoadData();

        _highScore = PlayerStats.highScore;

        ObstacleFactory();
        GoldFactory();
        GamePlatformBuilder();
    }

    void Update()
    {
        IncreaseScore();
        if (_score > _highScore && !isHighScoreStyleActive)
        {
            isHighScoreStyleActive = true;
            UIManager.Instance.ChangeScoreStyle();
        }
    }

    void GamePlatformBuilder()
    {
        for (int i = 0; i < _numOfGamePlatforms; i++)
        {
            GameObject gameArea = Instantiate(_platformPrefab, new Vector3(-3.7f, 0f, i * 7.5f), Quaternion.identity);

            gameArea.GetComponent<MeshRenderer>().material.color = i % 2 == 0 ? Color.black : Color.red;

            _GameAreas.Enqueue(gameArea);

            if (i == _numOfGamePlatforms - 1)
            {
                _lastObjOfQueue = gameArea;
            }

            if (i > 1) // first 2 platforms are empty
            {
                for (int j = -1; j < 2; j++)
                {
                    int randObstacle = Random.Range(0, _Obstacles.Count * 3);
                    if (randObstacle < _Obstacles.Count)
                    {
                        Obstacles obstacleType = _Obstacles[randObstacle].obstacleType;

                        GameObject obstacle = _ObstaclePool.First(x => x.GetComponent<Obstacle>().ObstacleType == obstacleType);

                        if (obstacle.GetComponent<Obstacle>().CanBeGoldOnBottom)
                        {
                            int randNum = Random.Range(0, 3); // 1/3 chance to spawn gold
                            if (randNum == 0)
                            {
                                GameObject golds = _GoldPool.Dequeue();
                                golds.SetActive(true);
                                golds.transform.position = new Vector3(j * 2.5f, 0.5f, gameArea.transform.position.z + 3.75f);
                                golds.transform.SetParent(gameArea.transform);
                            }
                        }
                        else if (obstacle.GetComponent<Obstacle>().CanBeGoldOnTop)
                        {
                            int randNum = Random.Range(0, 3); // 1/3 chance to spawn gold
                            if (randNum == 0)
                            {
                                GameObject golds = _GoldPool.Dequeue();
                                golds.SetActive(true);
                                golds.transform.position = new Vector3(j * 2.5f, 2f, gameArea.transform.position.z + 3.75f);
                                golds.transform.SetParent(gameArea.transform);
                            }
                        }

                        _ObstaclePool.Remove(obstacle);
                        obstacle.SetActive(true);
                        obstacle.transform.position = new Vector3(j * 2.5f, obstacle.transform.position.y, gameArea.transform.position.z + 3.75f);
                        obstacle.transform.SetParent(gameArea.transform);
                    }
                    else
                    {
                        int randNum = Random.Range(0, 3);
                        if (randNum == 0)
                        {
                            GameObject golds = _GoldPool.Dequeue();
                            golds.SetActive(true);
                            golds.transform.position = new Vector3(j * 2.5f, 0.5f, gameArea.transform.position.z + 3.75f);
                            golds.transform.SetParent(gameArea.transform);
                        }
                    }
                }

            }

            /************************************************************************/

        }
    }

    private void ObstacleFactory()
    {
        for (int i = 0; i < _Obstacles.Count; i++)
        {
            int maxObstacleCount = _numOfGamePlatforms * 3 / _Obstacles[i].obstacleSize;

            for (int j = 0; j < maxObstacleCount; j++)
            {
                GameObject obstacle = Instantiate(_Obstacles[i].obstaclePrefab);
                var obstacleFeatures = obstacle.GetComponent<Obstacle>();

                obstacleFeatures.ObstacleType = _Obstacles[i].obstacleType;
                obstacleFeatures.CanBeGoldOnTop = _Obstacles[i].canBeGoldOnTop;
                obstacleFeatures.CanBeGoldOnBottom = _Obstacles[i].canBeGoldOnBottom;

                obstacle.SetActive(false);
                _ObstaclePool.Add(obstacle);
            }
        }
    }

    private void GoldFactory()
    {
        for (int i = 0; i < _numOfGamePlatforms * 3; i++)
        {
            GameObject gold = Instantiate(_goldPrefab);
            gold.SetActive(false);
            _GoldPool.Enqueue(gold);
        }
    }

    public void ReplaceGamePlatform()
    {
        GameObject gameArea = _GameAreas.Dequeue();
        gameArea.transform.position = _lastObjOfQueue.transform.position + new Vector3(0, 0, 7.5f);
        _GameAreas.Enqueue(gameArea);
        _lastObjOfQueue = gameArea;

        ChangeOnPlatform(gameArea);
    }

    void ChangeOnPlatform(GameObject gameArea)
    {
        for (int j = -1; j < 2; j++)
        {
            int randObstacle = Random.Range(0, _Obstacles.Count * 3);
            if (randObstacle < _Obstacles.Count)
            {
                Obstacles obstacleType = _Obstacles[randObstacle].obstacleType;

                try
                {
                    GameObject obstacle = _ObstaclePool.First(x => x.GetComponent<Obstacle>().ObstacleType == obstacleType);

                    if (obstacle.GetComponent<Obstacle>().CanBeGoldOnBottom)
                    {
                        int randNum = Random.Range(0, 3); // 1/3 chance to spawn gold
                        if (randNum == 0)
                        {
                            GameObject golds = _GoldPool.Dequeue();
                            golds.SetActive(true);
                            golds.transform.position = new Vector3(j * 2.5f, 0.5f, gameArea.transform.position.z + 3.75f);
                            golds.transform.SetParent(gameArea.transform);
                        }
                    }
                    else if (obstacle.GetComponent<Obstacle>().CanBeGoldOnTop)
                    {
                        int randNum = Random.Range(0, 3); // 1/3 chance to spawn gold
                        if (randNum == 0)
                        {
                            GameObject golds = _GoldPool.Dequeue();
                            golds.SetActive(true);
                            golds.transform.position = new Vector3(j * 2.5f, 2f, gameArea.transform.position.z + 3.75f);
                            golds.transform.SetParent(gameArea.transform);
                        }
                    }

                    _ObstaclePool.Remove(obstacle);
                    obstacle.SetActive(true);
                    obstacle.transform.position = new Vector3(j * 2.5f, obstacle.transform.position.y, gameArea.transform.position.z + 3.75f);
                    obstacle.transform.SetParent(gameArea.transform);
                }
                catch (System.Exception)
                {
                    Debug.LogError(obstacleType.ToString() + " is not in the pool");
                }
            }
            else
            {
                int randNum = Random.Range(0, 3); // 1/3 chance to spawn gold
                if (randNum == 0)
                {
                    GameObject golds = _GoldPool.Dequeue();
                    golds.SetActive(true);
                    golds.transform.position = new Vector3(j * 2.5f, 0.5f, gameArea.transform.position.z + 3.75f);
                    golds.transform.SetParent(gameArea.transform);
                }
            }
        }
    }

    public void AddObstacleToPool(GameObject obstacle)
    {
        _ObstaclePool.Add(obstacle);
    }

    public void AddGoldToPool(GameObject gold)
    {
        _GoldPool.Enqueue(gold);
    }
    public void IncreaseScore()
    {
        _score += Time.deltaTime * 10f;
        UIManager.Instance.UpdateScore((int)_score);
    }
    public void IncreaseScore(int score) // for gold
    {
        _score += score;
        UIManager.Instance.UpdateScore((int)_score);
    }

    public void GameOver()
    {
        UIManager.Instance.ShowGameOverMenu();
        if (_score > PlayerStats.highScore)
        {
            PlayerStats.highScore = (int)_score;
        }
        JsonSaveLoad jsonSaveLoad = new JsonSaveLoad();
        jsonSaveLoad.SaveData();

        Time.timeScale = 0;
    }
}
