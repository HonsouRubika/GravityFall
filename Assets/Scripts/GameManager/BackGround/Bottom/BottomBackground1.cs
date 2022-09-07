using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomBackground1 : MonoBehaviour
{

    public Transform _despawnLine;
    public Transform _spawnLine;
    public Transform _rightSide;
    private Rigidbody2D _rb;
    private bool _didSpawnNextBg = false;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = new Vector2((float)-GameManager.Instance._obstacleSpeedActu * (1f / 4f), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance._isOnPause)
            Move();

        //spawn new bg
        if (!_didSpawnNextBg && _rightSide.position.x <= _spawnLine.position.x)
        {
            GameManager.Instance.SpawnBotBackground1();
            _didSpawnNextBg = true;
        }

        //depop when out of camera
        if (_despawnLine != null && _rightSide.position.x < _despawnLine.position.x)
        {
            Destroy(gameObject);
        }
    }

    public void Move()
    {
        _rb.velocity = new Vector2((float)-GameManager.Instance._obstacleSpeedActu * (1f / 4f), 0);
    }
}
