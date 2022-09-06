using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public float _obstacleSpeed = 5f;
    public Transform _despawnLine;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(-_obstacleSpeed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(_despawnLine != null && transform.position.x < _despawnLine.position.x)
        {
            Destroy(gameObject);
        }
    }
}
