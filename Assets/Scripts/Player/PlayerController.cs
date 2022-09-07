using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //component
    private Rigidbody2D _rbP1;
    private Rigidbody2D _rbP2;
    private BoxCollider2D _bcP1;
    private BoxCollider2D _bcP2;

    [Header("Movement Settings")]
    public float _speed = 150f;
    public float _maxSpeed = 200f;
    private Vector2 _movementInput;
    private float _movementActu = 0;
    public float _ratioAddForce = 1; //put 1 to desactivate
    public float _jumpSpeed = 20;
    public float _jumpAirIntake = 5f;
    public float _jumpFallSpeed = 1;
    public float _maxFallSpeed = 1;

    [Header("Collision Settings")]
    public GameObject _groundPositionP1;
    public GameObject _leftEdgeP1;
    public GameObject _rightEdgeP1;
    public GameObject _groundPositionP2;
    public GameObject _leftEdgeP2;
    public GameObject _rightEdgeP2;
    public float _hitboxRayLength = 1f;
    public float _groundRayLength = 1f;
    public int _nbOfRay = 5;

    [Header("Player States")]
    public JumpState _jumpStateP1 = JumpState.Falling;
    public JumpState _jumpStateP2 = JumpState.Falling;

    [Header("Player Settings")]
    public GameObject _player1;
    public GameObject _player2;
    public int _playerNumber = 0;
    public float _invulnerabilityDuration = 2f;
    private float _timeInvulnerabilityBegin;


    public enum JumpState
    {
        Grounded,
        Falling,
        Jumping
    }

    void Start()
    {
        _rbP1 = _player1.GetComponent<Rigidbody2D>();
        _rbP2 = _player2.GetComponent<Rigidbody2D>();
        _bcP1 = _player1.GetComponent<BoxCollider2D>();
        _bcP2 = _player2.GetComponent<BoxCollider2D>();

        //init var
        _timeInvulnerabilityBegin = Time.time - _invulnerabilityDuration;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    public void OnJumpP1(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ComputeJumpP1();
        }
    }

    public void OnJumpP2(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ComputeJumpP2();
        }
    }

    public void OnGravityChange(InputAction.CallbackContext context)
    {
        if (context.started && Time.time >= GameManager.Instance._lastTimeGravityUsed + GameManager.Instance._gravityUseCooldown)
        {
            //apply change to players
            GravityChange();

            //change the gravity
            if (GameManager.Instance._gravitySetting == 0)
                GameManager.Instance._gravitySetting = 1;
            else if (GameManager.Instance._gravitySetting == 1)
                GameManager.Instance._gravitySetting = 0;

            //reset timer
            GameManager.Instance._lastTimeGravityUsed = Time.time;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FallCurve();

        //MaxSpeed();

        CollisionsP1();
        CollisionsP2();
    }


    private void GravityChange()
    {
        //for spawn
        if (_rbP1 == null) _rbP1 = _player1.GetComponent<Rigidbody2D>();
        if (_rbP2 == null) _rbP2 = _player2.GetComponent<Rigidbody2D>();


        //change gravity scale
        //p1
        if (_rbP1.gravityScale == 1) _rbP1.gravityScale = -1;
        else _rbP1.gravityScale = 1;

        //p2
        if (_rbP2.gravityScale == 1) _rbP2.gravityScale = -1;
        else _rbP2.gravityScale = 1;

        //flip on Y axes
        _player1.transform.localScale = new Vector3(_player1.transform.localScale.x, -_player1.transform.localScale.y, _player1.transform.localScale.z);
        _player2.transform.localScale = new Vector3(_player2.transform.localScale.x, -_player2.transform.localScale.y, _player2.transform.localScale.z);


        //change gravity p1
        if (GameManager.Instance._gravitySetting == 0)
        {
            _rbP1.gravityScale = -1;

            //switch ground position
            if (_groundPositionP1.transform.position.y < _player1.transform.position.y)
                _groundPositionP1.transform.position = new Vector3(_groundPositionP1.transform.position.x, _player1.transform.position.y + 0.45f, _groundPositionP1.transform.position.z);
        }
        else if (GameManager.Instance._gravitySetting == 1)
        {
            _rbP1.gravityScale = 1;

            //switch ground position
            if (_groundPositionP1.transform.position.y > _player1.transform.position.y)
                _groundPositionP1.transform.position = new Vector3(_groundPositionP1.transform.position.x, _player1.transform.position.y - 0.45f, _groundPositionP1.transform.position.z);
        }

        //change gravity p2
        if (GameManager.Instance._gravitySetting == 1)
        {
            _rbP2.gravityScale = -1;

            //switch ground position
            if (_groundPositionP2.transform.position.y < _player2.transform.position.y)
                _groundPositionP2.transform.position = new Vector3(_groundPositionP2.transform.position.x, _player2.transform.position.y + 0.45f, _groundPositionP2.transform.position.z);
        }
        else if (GameManager.Instance._gravitySetting == 0)
        {
            _rbP2.gravityScale = 1;

            //switch ground position
            if (_groundPositionP2.transform.position.y > _player2.transform.position.y)
                _groundPositionP2.transform.position = new Vector3(_groundPositionP2.transform.position.x, _player2.transform.position.y - 0.45f, _groundPositionP2.transform.position.z);
        }

        //player is now in the air
        _jumpStateP1 = JumpState.Falling;
        _jumpStateP2 = JumpState.Falling;
    }

    
    private void CollisionsP1()
    {
        //gravity down
        if (GameManager.Instance._gravitySetting == 0)
        {

            //ground
            var colliderLength = _rightEdgeP1.transform.localPosition.x * _player1.transform.localScale.x * 2;
            var startPosition = _player1.transform.position.x - colliderLength / 2;

            bool didGroundCollide = false;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                if (Physics2D.Linecast(new Vector2(startPosition + (i * (colliderLength / _nbOfRay)), _groundPositionP1.transform.position.y), new Vector2(startPosition + (i * (colliderLength / _nbOfRay)), _groundPositionP1.transform.position.y - _groundRayLength), 1 << LayerMask.NameToLayer("Platform")))
                {
                    //collided to ground
                    _jumpStateP1 = JumpState.Grounded;
                    didGroundCollide = true;
                }
            }

            //TODO : la meme shaitanerie que pour SIU
            if (didGroundCollide)
            {
                //Debug.Log("non");
                _jumpStateP1 = JumpState.Grounded;
            }
            else
            {
                _jumpStateP1 = JumpState.Falling;
            }

            //obstacles
            var colliderWidth = _rightEdgeP1.transform.localPosition.y * _player1.transform.localScale.y * 2;
            var startPosition2 = _player1.transform.position.y - colliderWidth / 2;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                RaycastHit2D obstacle = Physics2D.Linecast(new Vector2(_leftEdgeP1.transform.position.x, startPosition2 + (i * (colliderWidth / _nbOfRay))), new Vector2(_leftEdgeP1.transform.position.x + _hitboxRayLength, startPosition2 + (i * (colliderWidth / _nbOfRay))), 1 << LayerMask.NameToLayer("Obstacle"));
                if (obstacle)
                {
                    //collided to obstacle

                    //Obstacles depop
                    Destroy(obstacle.collider.gameObject);

                    PlayerGotHit();
                }
            }


        }
        //gravity up
        else if (GameManager.Instance._gravitySetting == 1)
        {

            //ground
            var colliderLength = _rightEdgeP1.transform.localPosition.x * _player1.transform.localScale.x * 2;
            var startPosition = _player1.transform.position.x - colliderLength / 2;

            bool didGroundCollide = false;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                if (Physics2D.Linecast(new Vector2(startPosition + (i * (colliderLength / _nbOfRay)), _groundPositionP1.transform.position.y), new Vector2(startPosition + (i * (colliderLength / _nbOfRay)), _groundPositionP1.transform.position.y + _groundRayLength), 1 << LayerMask.NameToLayer("Platform")))
                {
                    //collided to ground
                    _jumpStateP1 = JumpState.Grounded;
                    didGroundCollide = true;
                }
            }

            //TODO : la meme shaitanerie que pour SIU
            if (didGroundCollide)
            {
                //Debug.Log("non");
                _jumpStateP1 = JumpState.Grounded;
            }
            else
            {
                _jumpStateP1 = JumpState.Falling;
            }


            //obstacles
            var colliderWidth = _rightEdgeP1.transform.localPosition.y * _player1.transform.localScale.y * 2;
            var startPosition2 = _player1.transform.position.y - colliderWidth / 2;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                RaycastHit2D obstacle = Physics2D.Linecast(new Vector2(_leftEdgeP1.transform.position.x, startPosition2 + (i * (colliderWidth / _nbOfRay))), new Vector2(_leftEdgeP1.transform.position.x + _hitboxRayLength, startPosition2 + (i * (colliderWidth / _nbOfRay))), 1 << LayerMask.NameToLayer("Obstacle"));
                if (obstacle)
                {
                    //collided to obstacle
                    Debug.Log("touched obstacle");

                    //TODO : Obstacles must depop
                    Destroy(obstacle.collider.gameObject);

                    PlayerGotHit();
                }
            }
        }
    }

    private void CollisionsP2()
    {
        //gravity down
        if (GameManager.Instance._gravitySetting == 1)
        {

            //ground
            var colliderLength = _rightEdgeP2.transform.localPosition.x * _player2.transform.localScale.x * 2;
            var startPosition = _player2.transform.position.x - colliderLength / 2;

            bool didGroundCollide = false;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                if (Physics2D.Linecast(new Vector2(startPosition + (i * (colliderLength / _nbOfRay)), _groundPositionP2.transform.position.y), new Vector2(startPosition + (i * (colliderLength / _nbOfRay)), _groundPositionP2.transform.position.y - _groundRayLength), 1 << LayerMask.NameToLayer("Platform")))
                {
                    //collided to ground
                    _jumpStateP2 = JumpState.Grounded;
                    didGroundCollide = true;
                }
            }

            //TODO : la meme shaitanerie que pour SIU
            if (didGroundCollide)
            {
                //Debug.Log("non");
                _jumpStateP2 = JumpState.Grounded;
            }
            else
            {
                _jumpStateP2 = JumpState.Falling;
            }

            //obstacles
            var colliderWidth = _rightEdgeP2.transform.localPosition.y * _player2.transform.localScale.y * 2;
            var startPosition2 = _player2.transform.position.y - colliderWidth / 2;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                RaycastHit2D obstacle = Physics2D.Linecast(new Vector2(_leftEdgeP2.transform.position.x, startPosition2 + (i * (colliderWidth / _nbOfRay))), new Vector2(_leftEdgeP2.transform.position.x + _hitboxRayLength, startPosition2 + (i * (colliderWidth / _nbOfRay))), 1 << LayerMask.NameToLayer("Obstacle"));
                if (obstacle)
                {
                    //collided to obstacle
                    Debug.Log("touched obstacle");

                    //TODO : Obstacles must depop
                    Destroy(obstacle.collider.gameObject);

                    PlayerGotHit();
                }
            }


        }
        //gravity up
        else if (GameManager.Instance._gravitySetting == 0)
        {

            //ground
            var colliderLength = _rightEdgeP2.transform.localPosition.x * _player2.transform.localScale.x * 2;
            var startPosition = _player2.transform.position.x - colliderLength / 2;

            bool didGroundCollide = false;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                if (Physics2D.Linecast(new Vector2(startPosition + (i * (colliderLength / _nbOfRay)), _groundPositionP2.transform.position.y), new Vector2(startPosition + (i * (colliderLength / _nbOfRay)), _groundPositionP2.transform.position.y + _groundRayLength), 1 << LayerMask.NameToLayer("Platform")))
                {
                    //collided to ground
                    _jumpStateP2 = JumpState.Grounded;
                    didGroundCollide = true;
                }
            }

            //TODO : la meme shaitanerie que pour SIU
            if (didGroundCollide)
            {
                //Debug.Log("non");
                _jumpStateP2 = JumpState.Grounded;
            }
            else
            {
                _jumpStateP2 = JumpState.Falling;
            }


            //obstacles
            var colliderWidth = _rightEdgeP2.transform.localPosition.y * _player2.transform.localScale.y * 2;
            var startPosition2 = _player2.transform.position.y - colliderWidth / 2;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                RaycastHit2D obstacle = Physics2D.Linecast(new Vector2(_leftEdgeP2.transform.position.x, startPosition2 + (i * (colliderWidth / _nbOfRay))), new Vector2(_leftEdgeP2.transform.position.x + _hitboxRayLength, startPosition2 + (i * (colliderWidth / _nbOfRay))), 1 << LayerMask.NameToLayer("Obstacle"));
                if (obstacle)
                {
                    //collided to obstacle

                    //Obstacles depop
                    Destroy(obstacle.collider.gameObject);

                    PlayerGotHit();
                }
            }
        }
    }

    private void PlayerGotHit()
    {
        if (Time.time >= _timeInvulnerabilityBegin + _invulnerabilityDuration)
        {
            //both player loose hp
            GameManager.Instance._playersLifeActu--;

            //invicibility time
            _timeInvulnerabilityBegin = Time.time;

            //slow obstacles
            GameManager.Instance._lastTimePlayerGotHit = Time.time;
        }
    }

    private void FallCurve()
    {
        //player 1
        if (GameManager.Instance._gravitySetting == 0)
        {
            //if (jumpState != JumpState.Grounded)
            _rbP1.velocity = new Vector2(_rbP1.velocity.x, _rbP1.velocity.y - _jumpFallSpeed * Time.fixedDeltaTime);
            if (_rbP1.velocity.y < -_maxFallSpeed)
            {
                _rbP1.velocity = new Vector2(_rbP1.velocity.x, -_maxFallSpeed);
            }
        }
        else if (GameManager.Instance._gravitySetting == 1)
        {
            //if (jumpState != JumpState.Grounded)
            _rbP1.velocity = new Vector2(_rbP1.velocity.x, _rbP1.velocity.y + _jumpFallSpeed * Time.fixedDeltaTime);
            if (_rbP1.velocity.y > _maxFallSpeed)
            {
                _rbP1.velocity = new Vector2(_rbP1.velocity.x, _maxFallSpeed);
            }
        }


        //player 2
        if (GameManager.Instance._gravitySetting == 0)
        {
            //if (jumpState != JumpState.Grounded)
            _rbP2.velocity = new Vector2(_rbP2.velocity.x, _rbP2.velocity.y + _jumpFallSpeed * Time.fixedDeltaTime);
            if (_rbP2.velocity.y > _maxFallSpeed)
            {
                _rbP2.velocity = new Vector2(_rbP2.velocity.x, _maxFallSpeed);
            }
        }
        else if (GameManager.Instance._gravitySetting == 1)
        {
            //if (_rb.gravityScale > 0) _rb.gravityScale = -_rb.gravityScale;

            //if (jumpState != JumpState.Grounded)
            _rbP2.velocity = new Vector2(_rbP2.velocity.x, _rbP2.velocity.y - _jumpFallSpeed * Time.fixedDeltaTime);
            if (_rbP2.velocity.y < -_maxFallSpeed)
            {
                _rbP2.velocity = new Vector2(_rbP2.velocity.x, -_maxFallSpeed);
            }
        }
    }

    private void ComputeJumpP1()
    {
        if (_rbP1 != null && _jumpStateP1 == JumpState.Grounded)
        {
            //player 1
            if (GameManager.Instance._gravitySetting == 0)
            {
                _rbP1.velocity = new Vector2(_rbP1.velocity.x, _jumpSpeed);
            }
            //else
            else if (GameManager.Instance._gravitySetting == 1)
            {
                _rbP1.velocity = new Vector2(_rbP1.velocity.x, -_jumpSpeed);
            }
        }

        _jumpStateP1 = JumpState.Jumping;
    }

    private void ComputeJumpP2()
    {
        if (_rbP2 != null && _jumpStateP2 == JumpState.Grounded)
        {
            //player 1
            if (GameManager.Instance._gravitySetting == 0)
            {
                _rbP2.velocity = new Vector2(_rbP2.velocity.x, -_jumpSpeed);
            }
            //else
            else if (GameManager.Instance._gravitySetting == 1)
            {
                _rbP2.velocity = new Vector2(_rbP2.velocity.x, _jumpSpeed);
            }
        }

        _jumpStateP2 = JumpState.Jumping;
    }

    public void OnDrawGizmos()
    {

        //p1
        var colliderLength = _rightEdgeP1.transform.localPosition.x * _player1.transform.localScale.x * 2;
        var startPosition = _player1.transform.position.x - colliderLength / 2;

        //ray cast
        for (int i = 0; i < _nbOfRay + 1; i++)
        {
            Gizmos.DrawLine(new Vector3(startPosition + (i * (colliderLength / _nbOfRay)), _groundPositionP1.transform.position.y, 0), new Vector3(startPosition + (i * (colliderLength / _nbOfRay)), _groundPositionP1.transform.position.y - _groundRayLength, 0));
        }

        //p2
        var colliderLength2 = _rightEdgeP2.transform.localPosition.x * _player2.transform.localScale.x * 2;
        var startPosition2 = _player2.transform.position.x - colliderLength2 / 2;

        //ray cast
        for (int i = 0; i < _nbOfRay + 1; i++)
        {
            Gizmos.DrawLine(new Vector3(startPosition2 + (i * (colliderLength2 / _nbOfRay)), _groundPositionP2.transform.position.y, 0), new Vector3(startPosition2 + (i * (colliderLength2 / _nbOfRay)), _groundPositionP2.transform.position.y + _groundRayLength, 0));
        }


        //obstacles collision

        //p1
        var colliderWidthP1 = _groundPositionP1.transform.localPosition.y * _player1.transform.localScale.y * 2;
        var startPositionObst1 = _player1.transform.position.y + colliderWidthP1 / 2;
        //var startPosition2 = _rightEdge.transform.localPosition.y;

        //ray cast
        for (int i = 0; i < _nbOfRay + 1; i++)
        {
            Gizmos.DrawLine(new Vector3(_leftEdgeP1.transform.position.x, startPositionObst1 - (i * (colliderWidthP1 / _nbOfRay)), 0), new Vector3(_leftEdgeP1.transform.position.x + _hitboxRayLength, startPositionObst1 - (i * (colliderWidthP1 / _nbOfRay)), 0));
        }

        //p2
        var colliderWidthP2 = _groundPositionP2.transform.localPosition.y * _player2.transform.localScale.y * 2;
        var startPositionObst2 = _player2.transform.position.y + colliderWidthP2 / 2;
        //var startPosition2 = _rightEdge.transform.localPosition.y;

        //ray cast
        for (int i = 0; i < _nbOfRay + 1; i++)
        {
            Gizmos.DrawLine(new Vector3(_leftEdgeP2.transform.position.x, startPositionObst2 - (i * (colliderWidthP2 / _nbOfRay)), 0), new Vector3(_leftEdgeP2.transform.position.x + _hitboxRayLength, startPositionObst2 - (i * (colliderWidthP2 / _nbOfRay)), 0));
        }


    }

}
