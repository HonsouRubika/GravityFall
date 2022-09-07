using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public Transform _despawnLine;
    public Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = new Vector2(-GameManager.Instance._obstacleSpeedActu, 0);
    }

    // Update is called once per frame
    void Update()
    {

        Move();

        //depop when out of camera
        if(_despawnLine != null && transform.position.x < _despawnLine.position.x)
        {
            Destroy(gameObject);
        }
    }

    public void Move()
    {
        _rb.velocity = new Vector2(-GameManager.Instance._obstacleSpeedActu, 0);
    }
}
