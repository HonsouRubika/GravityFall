using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private int _nbOfPlayerInScene = 0;
    public bool _isOnPause = true;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isOnPause)
        {
            SpawnObstacles();
        }
    }

    public void SpawnObstacles()
    {
        if(Time.time > lastSpawnTime + obstacleSpawnRate)
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
                if (pattern._obstacles[i, j] != (int) Pattern.ObstaclesType.empty)
                {
                    switch(pattern._obstacles[i, j])
                    {
                        case (int)Pattern.ObstaclesType.spike:
                            Instantiate(_obstaclesPrefabs[0], new Vector3(_patternOneSpawn.position.x + i * _spaceBetweenObstacles, _patternOneSpawn.position.y + j * _spaceBetweenObstacles, 0), Quaternion.identity);
                            break;

                        //add other pattern type here
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

                            //add other pattern type here
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

            //put player to appropriate spawn
            if (_nbOfPlayerInScene == 0)
            {
                playerInput.gameObject.transform.position = _playerOneSpawn.position;
            }
            else if (_nbOfPlayerInScene == 1)
            {
                playerController.SetPlayer2();
                playerInput.gameObject.transform.position = _playerTwoSpawn.position;
                
                //start game
                _isOnPause = false;
            }
        }

        _nbOfPlayerInScene++;
    }


}
