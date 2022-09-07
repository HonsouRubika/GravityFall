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
        }

        //handles obstacle movement
        ObstacleSpeedRate();
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
            int selectedPattern = Random.Range(0, _patterns.Count);
            InstantiatePattern(_patterns[selectedPattern]);

        }
    }

    public void ObstacleSpeedRate()
    {
        //slow then come slowly back to normal when player get's hit
        if (_lastTimePlayerGotHit != 0 && Time.time < _lastTimePlayerGotHit + _timeBeforeNormalObstacleSpeed)
        {
            _obstacleSpeedActu = _obstacleSlowSpeed;
        }
        else if (_obstacleSpeedActu <_obstacleSpeed)
        {
            //slowly go back to normal
            _obstacleSpeedActu += _speedComeBackRate * Time.deltaTime;
        }
    }

    public void CreatePatterns()
    {
        //pattern example
        //see explanation in "Pattern" script
        Pattern example = new Pattern(10, 5);

        example.AddObstacle(2, 1, Pattern.ObstaclesType.spike);
        example.AddObstacle(3, 2, Pattern.ObstaclesType.spike);
        example.AddObstacle(6, 2, Pattern.ObstaclesType.spike);
        example.AddObstacle(7, 3, Pattern.ObstaclesType.spike);
        _patterns.Add(example);


        //write your other pattern here
        //...

    }

    public void InstantiatePattern(Pattern pattern)
    {
        //same pattern for p1 and p2

        //for p1
        for (int i = 0; i < pattern._length; i++)
        {
            for (int j = 0; j < pattern._width; j++)
            {
                if (pattern._obstacles[i, j] != (int)Pattern.ObstaclesType.empty)
                {
                    switch (pattern._obstacles[i, j])
                    {
                        case (int)Pattern.ObstaclesType.spike:
                            Instantiate(_obstaclesPrefabs[0], new Vector3(_patternOneSpawn.position.x + i * _spaceBetweenObstacles, _patternOneSpawn.position.y + j * _spaceBetweenObstacles, 0), Quaternion.identity);
                            break;

                            //add other objects type here
                    }
                }
            }
        }

        //for p2
        for (int i = 0; i < pattern._length; i++)
        {
            for (int j = 0; j < pattern._width; j++)
            {
                if (pattern._obstacles[i, j] != (int)Pattern.ObstaclesType.empty)
                {
                    switch (pattern._obstacles[i, j])
                    {
                        case (int)Pattern.ObstaclesType.spike:
                            Instantiate(_obstaclesPrefabs[0], new Vector3(_patternTwoSpawn.position.x + i * _spaceBetweenObstacles, _patternTwoSpawn.position.y + j * _spaceBetweenObstacles, 0), Quaternion.identity);
                            break;

                            //add other objects type here
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
