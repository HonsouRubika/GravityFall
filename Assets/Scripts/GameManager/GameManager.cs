using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager Instance;

    [Header("Player Spawns")]
    public Transform _playerOneSpawn;
    public Transform _playerTwoSpawn;

    /* DEPRECIATED
    [Header("Obstacles")]
    public Transform _obstaclesOneSpawn;
    public Transform _obstaclesTwoSpawn;
    public Transform _despawnLine;
    public GameObject[] _obstaclesPrefabs;
    */

    //game settings
    private float lastSpawnTime = 0;
    public float obstacleSpawnRate = 2f;

    [Header("Pattern Settings")]
    public float _spaceBetweenObstacles = 1f;
    public Transform _patternOneSpawn;
    public Transform _patternTwoSpawn;
    public GameObject[] _obstaclesPrefabs;
    public Transform _despawnLine;
    private List<Pattern> _patterns;

    [Header("Other")]
    public bool _isOnPause = true;
    public int _playersLifeActu;
    public int _playersLifeStart = 3;
    public int _gravitySetting = 0; //down = 0, up = 1
    public float _gravityUseCooldown = 2f;
    public float _lastTimeGravityUsed;

    //obstacles
    public float _lastTimePlayerGotHit;
    public float _obstacleSpeed = 5f;
    public float _obstacleSpeedActu = 5f;
    public float _obstacleSlowSpeed = 2f;
    public float _speedComeBackRate = 10f;
    public float _timeBeforeNormalObstacleSpeed = 2f;

    //UI
    public GameObject _pause;
    public GameObject _inGame;
    public GameObject _gameOver;

    [Header("Background Settings")]
    //ignore last layer (doesnt move, nor get's replaced)
    //1st layer
    public GameObject _background1Prefab;
    public List<GameObject> _backgrounds1;
    //2nd layer
    public GameObject _background2Prefab;
    public List<GameObject> _backgrounds2;
    //3rd layer
    public GameObject _background3Prefab;
    public List<GameObject> _backgrounds3;
    //4 layer
    public GameObject _background4Prefab;
    public List<GameObject> _backgrounds4;
    //5 layer
    public GameObject _background5Prefab;
    public List<GameObject> _backgrounds5;
    //6layer
    public GameObject _background6Prefab;
    public List<GameObject> _backgrounds6;
    //7 layer
    public GameObject _background7Prefab;
    public List<GameObject> _backgrounds7;

    //bot
    //1 layer
    public GameObject _botBackground1Prefab;
    public List<GameObject> _botBackgrounds1;
    //2 layer
    public GameObject _botBackground2Prefab;
    public List<GameObject> _botBackgrounds2;
    //3 layer
    public GameObject _botBackground3Prefab;
    public List<GameObject> _botBackgrounds3;
    //4 layer
    public GameObject _botBackground4Prefab;
    public List<GameObject> _botBackgrounds4;

    void Awake()
    {
        #region Make Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        //game start on pause
        _isOnPause = true;

        //intitilise dynamic array
        _patterns = new List<Pattern>();
        //_backgrounds1 = new List<GameObject>();

        //add patterns in the pattern the dynamic array
        CreatePatterns();

        //init random seed based on actual time
        Random.InitState(System.DateTime.Now.Millisecond);

        //reset players life count
        _playersLifeActu = _playersLifeStart;

        //init var
        _lastTimeGravityUsed = Time.time - _gravityUseCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isOnPause)
        {
            SpawnObstacles();
            _pause.SetActive(false);
        }
        else
        {
            _pause.SetActive(true);
        }

        //hp
        if (_playersLifeActu <= 0)
        {
            _gameOver.SetActive(true);
            _obstacleSpeedActu = 0;
        }
        else
        {
            //handles obstacle movement
            ObstacleSpeedRate();
        }
    }

    public void SpawnBackground1()
    {
        //pop new background
        GameObject newBg = Instantiate(_background1Prefab, new Vector3(_patternOneSpawn.position.x + 50, 1, 0), Quaternion.identity);
        newBg.GetComponent<BackGround1>()._despawnLine = _despawnLine;
        newBg.GetComponent<BackGround1>()._spawnLine = _patternOneSpawn;
        _backgrounds1.Add(newBg);

        newBg.transform.position = new Vector3(_backgrounds1[_backgrounds1.Count - 2].transform.position.x + newBg.GetComponent<SpriteRenderer>().bounds.size.x, newBg.transform.position.y, newBg.transform.position.z);
    }

    public void SpawnBackground2()
    {
        //pop new background
        GameObject newBg = Instantiate(_background2Prefab, new Vector3(_patternOneSpawn.position.x + 50, 1, 0), Quaternion.identity);
        newBg.GetComponent<BackGround2>()._despawnLine = _despawnLine;
        newBg.GetComponent<BackGround2>()._spawnLine = _patternOneSpawn;
        _backgrounds2.Add(newBg);

        newBg.transform.position = new Vector3(_backgrounds2[_backgrounds2.Count - 2].transform.position.x + newBg.GetComponent<SpriteRenderer>().bounds.size.x, newBg.transform.position.y, newBg.transform.position.z);
    }

    public void SpawnBackground3()
    {
        //pop new background
        GameObject newBg = Instantiate(_background3Prefab, new Vector3(_patternOneSpawn.position.x + 50, 1, 0), Quaternion.identity);
        newBg.GetComponent<BackGround3>()._despawnLine = _despawnLine;
        newBg.GetComponent<BackGround3>()._spawnLine = _patternOneSpawn;
        _backgrounds3.Add(newBg);

        newBg.transform.position = new Vector3(_backgrounds3[_backgrounds3.Count - 2].transform.position.x + newBg.GetComponent<SpriteRenderer>().bounds.size.x, newBg.transform.position.y, newBg.transform.position.z);
    }

    public void SpawnBackground4()
    {
        //pop new background
        GameObject newBg = Instantiate(_background4Prefab, new Vector3(_patternOneSpawn.position.x + 50, 1, 0), Quaternion.identity);
        newBg.GetComponent<BackGround4>()._despawnLine = _despawnLine;
        newBg.GetComponent<BackGround4>()._spawnLine = _patternOneSpawn;
        _backgrounds4.Add(newBg);

        newBg.transform.position = new Vector3(_backgrounds4[_backgrounds4.Count - 2].transform.position.x + newBg.GetComponent<SpriteRenderer>().bounds.size.x, newBg.transform.position.y, newBg.transform.position.z);
    }

    public void SpawnBackground5()
    {
        //pop new background
        GameObject newBg = Instantiate(_background5Prefab, new Vector3(_patternOneSpawn.position.x + 50, 1, 0), Quaternion.identity);
        newBg.GetComponent<BackGround5>()._despawnLine = _despawnLine;
        newBg.GetComponent<BackGround5>()._spawnLine = _patternOneSpawn;
        _backgrounds5.Add(newBg);

        newBg.transform.position = new Vector3(_backgrounds5[_backgrounds5.Count - 2].transform.position.x + newBg.GetComponent<SpriteRenderer>().bounds.size.x, newBg.transform.position.y, newBg.transform.position.z);
    }

    public void SpawnBackground6()
    {
        //pop new background
        GameObject newBg = Instantiate(_background6Prefab, new Vector3(_patternOneSpawn.position.x + 50, 1, 0), Quaternion.identity);
        newBg.GetComponent<BackGround6>()._despawnLine = _despawnLine;
        newBg.GetComponent<BackGround6>()._spawnLine = _patternOneSpawn;
        _backgrounds6.Add(newBg);

        newBg.transform.position = new Vector3(_backgrounds6[_backgrounds6.Count - 2].transform.position.x + newBg.GetComponent<SpriteRenderer>().bounds.size.x, newBg.transform.position.y, newBg.transform.position.z);
    }

    public void SpawnBackground7()
    {
        //pop new background
        GameObject newBg = Instantiate(_background7Prefab, new Vector3(_patternOneSpawn.position.x + 50, 1, 0), Quaternion.identity);
        newBg.GetComponent<BackGround7>()._despawnLine = _despawnLine;
        newBg.GetComponent<BackGround7>()._spawnLine = _patternOneSpawn;
        _backgrounds7.Add(newBg);

        newBg.transform.position = new Vector3(_backgrounds7[_backgrounds7.Count - 2].transform.position.x + newBg.GetComponent<SpriteRenderer>().bounds.size.x, newBg.transform.position.y, newBg.transform.position.z);
    }

    public void SpawnBotBackground1()
    {
        //pop new background
        GameObject newBg = Instantiate(_botBackground1Prefab, new Vector3(_patternOneSpawn.position.x + 50, _botBackground1Prefab.transform.position.y, 0), Quaternion.identity);
        newBg.GetComponent<BottomBackground1>()._despawnLine = _despawnLine;
        newBg.GetComponent<BottomBackground1>()._spawnLine = _patternOneSpawn;
        _botBackgrounds1.Add(newBg);

        newBg.transform.position = new Vector3(_botBackgrounds1[_botBackgrounds1.Count - 2].transform.position.x + newBg.GetComponent<SpriteRenderer>().bounds.size.x, newBg.transform.position.y, newBg.transform.position.z);
    }

    public void SpawnBotBackground2()
    {
        //pop new background
        GameObject newBg = Instantiate(_botBackground2Prefab, new Vector3(_patternOneSpawn.position.x + 50, _botBackground2Prefab.transform.position.y, 0), Quaternion.identity);
        newBg.GetComponent<BottomBackground2>()._despawnLine = _despawnLine;
        newBg.GetComponent<BottomBackground2>()._spawnLine = _patternOneSpawn;
        _botBackgrounds2.Add(newBg);

        newBg.transform.position = new Vector3(_botBackgrounds2[_botBackgrounds2.Count - 2].transform.position.x + newBg.GetComponent<SpriteRenderer>().bounds.size.x, newBg.transform.position.y, newBg.transform.position.z);
    }

    public void SpawnBotBackground3()
    {
        //pop new background
        GameObject newBg = Instantiate(_botBackground3Prefab, new Vector3(_patternOneSpawn.position.x + 50, _botBackground3Prefab.transform.position.y, 0), Quaternion.identity);
        newBg.GetComponent<BottomBackground3>()._despawnLine = _despawnLine;
        newBg.GetComponent<BottomBackground3>()._spawnLine = _patternOneSpawn;
        _botBackgrounds3.Add(newBg);

        newBg.transform.position = new Vector3(_botBackgrounds3[_botBackgrounds3.Count - 2].transform.position.x + newBg.GetComponent<SpriteRenderer>().bounds.size.x, newBg.transform.position.y, newBg.transform.position.z);
    }

    public void SpawnBotBackground4()
    {
        //pop new background
        GameObject newBg = Instantiate(_botBackground4Prefab, new Vector3(_patternOneSpawn.position.x + 50, _botBackground4Prefab.transform.position.y, 0), Quaternion.identity);
        newBg.GetComponent<BottomBackground4>()._despawnLine = _despawnLine;
        newBg.GetComponent<BottomBackground4>()._spawnLine = _patternOneSpawn;
        _botBackgrounds4.Add(newBg);

        newBg.transform.position = new Vector3(_botBackgrounds4[_botBackgrounds4.Count - 2].transform.position.x + newBg.GetComponent<SpriteRenderer>().bounds.size.x, newBg.transform.position.y, newBg.transform.position.z);
    }

    public void SpawnObstacles()
    {
        if (Time.time > lastSpawnTime + obstacleSpawnRate + (obstacleSpawnRate * (_obstacleSpeed - _obstacleSpeedActu)))
        {
            //reset timer
            lastSpawnTime = Time.time;

            /***** TEST ******
            //spawn obstacle j1
            GameObject obst = Instantiate(_obstaclesPrefabs[0], _obstaclesOneSpawn);
            obst.GetComponent<Obstacle>()._despawnLine = _despawnLine;

            //spawn obstacle j2
            obst = Instantiate(_obstaclesPrefabs[0], _obstaclesTwoSpawn);
            obst.GetComponent<Obstacle>()._despawnLine = _despawnLine;
            */

            //procedural generation of pattern
            int selectedPattern1 = Random.Range(0, _patterns.Count);
            int selectedPattern2 = Random.Range(0, _patterns.Count);
            while (selectedPattern1 == selectedPattern2)
            {
                selectedPattern2 = Random.Range(0, _patterns.Count);
            }

            InstantiatePattern(_patterns[selectedPattern1], _patterns[selectedPattern2]);

        }
    }

    public void ObstacleSpeedRate()
    {
        //slow then come slowly back to normal when player get's hit
        if (_lastTimePlayerGotHit != 0 && Time.time < _lastTimePlayerGotHit + _timeBeforeNormalObstacleSpeed)
        {
            _obstacleSpeedActu = _obstacleSlowSpeed;
        }
        else if (_obstacleSpeedActu < _obstacleSpeed)
        {
            //slowly go back to normal
            _obstacleSpeedActu += _speedComeBackRate * Time.deltaTime;
        }
    }

    public void CreatePatterns()
    {
        //pattern example
        //see explanation in "Pattern" script
        /*
        Pattern example = new Pattern(10, 5);

        example.AddObstacle(2, 1, Pattern.ObstaclesType.babredWires);
        example.AddObstacle(3, 2, Pattern.ObstaclesType.babredWires);
        example.AddObstacle(6, 2, Pattern.ObstaclesType.babredWires);
        example.AddObstacle(7, 3, Pattern.ObstaclesType.babredWires);
        _patterns.Add(example);


        //write your other pattern here
        //...
        Pattern pat1 = new Pattern(10, 5);
        pat1.AddObstacle(1, 0, Pattern.ObstaclesType.elecCables);
        pat1.AddObstacle(7, 3, Pattern.ObstaclesType.elecCables);
        pat1.AddObstacle(4, 1, Pattern.ObstaclesType.elecCables);
        _patterns.Add(pat1);


        Pattern pat2 = new Pattern(10, 5);
        pat2.AddObstacle(2, 0, Pattern.ObstaclesType.elecCables);
        pat2.AddObstacle(3, 0, Pattern.ObstaclesType.elecCables);
        pat2.AddObstacle(6, 1, Pattern.ObstaclesType.elecCables);
        pat2.AddObstacle(4, 3, Pattern.ObstaclesType.elecCables);
        _patterns.Add(pat2);
        */

        Pattern p1 = new Pattern(10, 5);
        p1.AddObstacle(3, 3, Pattern.ObstaclesType.elecCables);
        p1.AddObstacle(4, 3, Pattern.ObstaclesType.elecCables);
        p1.AddObstacle(6, 3, Pattern.ObstaclesType.elecCables);
        _patterns.Add(p1);


        Pattern p2 = new Pattern(10, 5);
        p2.AddObstacle(2, 3, Pattern.ObstaclesType.elecCables);
        p2.AddObstacle(5, 3, Pattern.ObstaclesType.elecCables);

        Pattern p3 = new Pattern(10, 5);
        p3.AddObstacle(2, 3, Pattern.ObstaclesType.elecCables);
        p3.AddObstacle(5, 3, Pattern.ObstaclesType.elecCables);
        p3.AddObstacle(7, 3, Pattern.ObstaclesType.elecCables);
        _patterns.Add(p3);

        Pattern p4 = new Pattern(10, 5);
        p4.AddObstacle(4, 3, Pattern.ObstaclesType.elecCables);
        p4.AddObstacle(6, 3, Pattern.ObstaclesType.elecCables);
        p4.AddObstacle(1, 3, Pattern.ObstaclesType.elecCables);
        _patterns.Add(p4);

        //p3.AddObstacle(3, 0)

    }

    public void InstantiatePattern(Pattern p1, Pattern p2)
    {
        //same pattern for p1 and p2

        //for p1
        for (int i = 0; i < p1._length; i++)
        {
            for (int j = 0; j < p1._width; j++)
            {
                if (p1._obstacles[i, j] != (int)Pattern.ObstaclesType.empty)
                {
                    switch (p1._obstacles[i, j])
                    {
                        case 1:
                            GameObject obst = Instantiate(_obstaclesPrefabs[0], new Vector3(_patternOneSpawn.position.x + i * _spaceBetweenObstacles, _patternOneSpawn.position.y + j * _spaceBetweenObstacles, 0), Quaternion.identity);
                            obst.GetComponent<Obstacle>()._despawnLine = _despawnLine;
                            break;

                        //add other objects type here
                        case 2:
                            GameObject obst2 = Instantiate(_obstaclesPrefabs[1], new Vector3(_patternOneSpawn.position.x + i * _spaceBetweenObstacles, _patternOneSpawn.position.y + j * _spaceBetweenObstacles, 0), Quaternion.identity);
                            obst2.GetComponent<Obstacle>()._despawnLine = _despawnLine;
                            break;

                    }
                }
            }
        }

        //for p2
        for (int i = 0; i < p2._length; i++)
        {
            for (int j = 0; j < p2._width; j++)
            {
                if (p2._obstacles[i, j] != (int)Pattern.ObstaclesType.empty)
                {
                    switch (p2._obstacles[i, j])
                    {

                        case 1:
                            GameObject obst3 = Instantiate(_obstaclesPrefabs[2], new Vector3(_patternTwoSpawn.position.x + i * _spaceBetweenObstacles, _patternTwoSpawn.position.y + j * _spaceBetweenObstacles, 0), Quaternion.identity);
                            obst3.GetComponent<Obstacle>()._despawnLine = _despawnLine;
                            break;

                        case 2:
                            GameObject obst4 = Instantiate(_obstaclesPrefabs[3], new Vector3(_patternTwoSpawn.position.x + i * _spaceBetweenObstacles, _patternTwoSpawn.position.y + j * _spaceBetweenObstacles, 0), Quaternion.identity);
                            obst4.GetComponent<Obstacle>()._despawnLine = _despawnLine;
                            break;
                    }
                }
            }
        }
    }


    public void OnPlayerJoin(PlayerInput playerInput)
    {
        PlayerController playerController = playerInput.gameObject.GetComponent<PlayerController>();

        if (playerController != null)
        {

            //TODO : spawn just one and place both pawn

            //put players to appropriate spawn
            playerController._player1.transform.position = _playerOneSpawn.position;
            playerController._player2.transform.position = _playerTwoSpawn.position;

            //start game
            _isOnPause = false;
        }
    }


}
