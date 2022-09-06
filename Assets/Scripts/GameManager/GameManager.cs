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

    [Header("Obstacles")]
    public Transform _obstaclesOneSpawn;
    public Transform _obstaclesTwoSpawn;
    public Transform _despawnLine;
    public GameObject[] _obstaclesPrefabs;

    //game settings
    private float lastSpawnTime=0;
    public float obstacleSpawnRate = 2f;

    // Start is called before the first frame update
    void Start()
    {
        _isOnPause = true;
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

            //spawn obstacle j1
            GameObject obst = Instantiate(_obstaclesPrefabs[0], _obstaclesOneSpawn);
            obst.GetComponent<Obstacle>()._despawnLine = _despawnLine;

            //spawn obstacle j2
            obst = Instantiate(_obstaclesPrefabs[0], _obstaclesTwoSpawn);
            obst.GetComponent<Obstacle>()._despawnLine = _despawnLine;
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
