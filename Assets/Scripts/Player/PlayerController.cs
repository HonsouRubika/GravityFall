using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //component
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private BoxCollider2D _bc;

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

    //wall slide
    [Header("WallSlide Settings")]
    public float _timeBeforeWallSlide = 1f;
    public float _timeBeforeWallSlideStart;

    [Header("Collision Settings")]
    public GameObject _groundPosition;
    public GameObject _leftEdge;
    public GameObject _rightEdge;
    public float _rayLength = 1f;
    public int _nbOfRay = 5;

    [Header("Player States")]
    private JumpState _jumpState = JumpState.Falling;

    [Header("Player Settings")]
    public int _playerNumber = 0;
    public int _gravitySetting = 0; //down = 0, up = 1
    public float _gravityUseCooldown = 2f;
    private float _lastTimeGravityUsed;


    public enum JumpState
    {
        Grounded,
        Falling,
        Jumping
    }

    /// <summary>
    /// TODO List :
    ///     - Collision (Sol & Coté platforme => le perso dois glisser
    /// </summary>

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _bc = GetComponent<BoxCollider2D>();

        //init var
        _lastTimeGravityUsed = Time.time - _gravityUseCooldown;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ComputeJump();
        }
    }

    public void OnGravityChange(InputAction.CallbackContext context)
    {
        if (context.started && Time.time >= _lastTimeGravityUsed + _gravityUseCooldown)
        {
            GravityChange();

            //reset timer
            _lastTimeGravityUsed = Time.time;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move();

        FallCurve();

        MaxSpeed();

        Collisions();
    }

    /*private void Move()
    {
        if (_movementInput.x < -0.3)
        {
            //AddForce
            if (_movementActu <= -1)
            {
                _movementActu = -1;
                _rb.velocity = new Vector2(_movementActu * _speed * Time.fixedDeltaTime, _rb.velocity.y);
            }
            else if (_movementActu >= 1)
            {
                _movementActu = 0;
                _rb.velocity = new Vector2(_movementActu * _speed * Time.fixedDeltaTime, _rb.velocity.y);
            }
            else
            {
                //player is turning
                _movementActu -= _ratioAddForce * Time.fixedDeltaTime;
                _rb.velocity = new Vector2(_movementActu * _speed * Time.fixedDeltaTime, _rb.velocity.y);
                //Debug.Log("is turning");
            }

            //turn sprite
            if (!_sr.flipX)
            {
                //right is default
                _sr.flipX = true;
            }
        }
        else if (_movementInput.x > 0.3)
        {
            //AddForce
            if (_movementActu >= 1)
            {
                _movementActu = 1;
                _rb.velocity = new Vector2(_movementActu * _speed * Time.fixedDeltaTime, _rb.velocity.y);
            }
            else if (_movementActu <= -1)
            {
                _movementActu = 0;
                _rb.velocity = new Vector2(_movementActu * _speed * Time.fixedDeltaTime, _rb.velocity.y);
            }
            else
            {
                //player is turning
                _movementActu += _ratioAddForce * Time.fixedDeltaTime;
                _rb.velocity = new Vector2(_movementActu * _speed * Time.fixedDeltaTime, _rb.velocity.y);
                //Debug.Log("is turning");
            }

            //turn sprite
            if (_sr.flipX)
            {
                //right is default
                _sr.flipX = false;
            }
        }
        else if (_movementInput.x == 0)
        {
            //if no input

            //slow player
            _movementActu = 0;


            //if( dans l'air)
            //nullifies in X axis
            if (_rb.velocity.x > 0)
            {
                _rb.velocity = new Vector2(_rb.velocity.x - (_speed * Time.fixedDeltaTime), _rb.velocity.y);
                if (_rb.velocity.x < 0) _rb.velocity = new Vector2(0, _rb.velocity.y);
            }
            else if (_rb.velocity.x < 0)
            {
                _rb.velocity = new Vector2(_rb.velocity.x + (_speed * Time.fixedDeltaTime), _rb.velocity.y);
                if (_rb.velocity.x > 0) _rb.velocity = new Vector2(0, _rb.velocity.y);
            }

        }

    }*/

    public void SetPlayer2()
    {
        _playerNumber = 1;
        GravityChange();
    }

    private void GravityChange()
    {
        //for spawn
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();


        //change gravity
        if (_gravitySetting == 0)
        {
            _rb.gravityScale = -1;
            _gravitySetting = 1;

            //switch ground position
            if (_groundPosition.transform.position.y < transform.position.y)
                _groundPosition.transform.position = new Vector3(_groundPosition.transform.position.x, transform.position.y + 0.45f, _groundPosition.transform.position.z);
        }
        else if (_gravitySetting == 1)
        {
            _rb.gravityScale = 1;
            _gravitySetting = 0;

            //switch ground position
            if (_groundPosition.transform.position.y > transform.position.y)
                _groundPosition.transform.position = new Vector3(_groundPosition.transform.position.x, transform.position.y - 0.45f, _groundPosition.transform.position.z);
        }

        //player is now in the air
        _jumpState = JumpState.Falling;
    }

    private void Collisions()
    {
        //gravity down
        if (_gravitySetting == 0)
        {

            //ground
            var colliderLength = _rightEdge.transform.localPosition.x * transform.localScale.x * 2;
            var startPosition = transform.position.x - colliderLength / 2;

            bool didGroundCollide = false;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                if (Physics2D.Linecast(new Vector2(startPosition + (i * (colliderLength / _nbOfRay)), _groundPosition.transform.position.y), new Vector2(startPosition + (i * (colliderLength / _nbOfRay)), _groundPosition.transform.position.y - _rayLength), 1 << LayerMask.NameToLayer("Platform")))
                {
                    //collided to ground
                    _jumpState = JumpState.Grounded;
                    didGroundCollide = true;
                }
            }

            //TODO : la meme shaitanerie que pour SIU
            if (!didGroundCollide)
            {
                //Debug.Log("non");
                _jumpState = JumpState.Grounded;
            }

            //obstacles
            var colliderWidth = _rightEdge.transform.localPosition.y * transform.localScale.y * 2;
            var startPosition2 = transform.position.y - colliderWidth / 2;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                if (Physics2D.Linecast(new Vector2(_rightEdge.transform.position.x, startPosition2 - (i * (colliderWidth / _nbOfRay))), new Vector2(_rightEdge.transform.position.x + _rayLength, startPosition2 - (i * (colliderWidth / _nbOfRay))), 1 << LayerMask.NameToLayer("Obstacle")))
                {
                    //collided to obstacle
                    Debug.Log("touched obstacle");
                }
            }


        }
        //gravity up
        else if (_gravitySetting == 1)
        {

            //ground
            var colliderLength = _rightEdge.transform.localPosition.x * transform.localScale.x * 2;
            var startPosition = transform.position.x - colliderLength / 2;

            bool didGroundCollide = false;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                if (Physics2D.Linecast(new Vector2(startPosition + (i * (colliderLength / _nbOfRay)), _groundPosition.transform.position.y), new Vector2(startPosition + (i * (colliderLength / _nbOfRay)), _groundPosition.transform.position.y + _rayLength), 1 << LayerMask.NameToLayer("Platform")))
                {
                    //collided to ground
                    _jumpState = JumpState.Grounded;
                    didGroundCollide = true;
                }
            }

            //TODO : la meme shaitanerie que pour SIU
            if (!didGroundCollide)
            {
                //Debug.Log("non");
                _jumpState = JumpState.Grounded;
            }


            //obstacles
            var colliderWidth = _rightEdge.transform.localPosition.y * transform.localScale.y * 2;
            var startPosition2 = transform.position.y - colliderWidth / 2;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                if (Physics2D.Linecast(new Vector2(_rightEdge.transform.position.x, startPosition2 + (i * (colliderWidth / _nbOfRay))), new Vector2(_rightEdge.transform.position.x + _rayLength, startPosition2 + (i * (colliderWidth / _nbOfRay))), 1 << LayerMask.NameToLayer("Obstacle")))
                {
                    //collided to obstacle
                    Debug.Log("touched obstacle");

                    //TODO : Obstacles must depop

                    PlayerGotHit();
                }
            }
        }

    }

    private void PlayerGotHit()
    {
        //both player loose hp


        //invicibility time

        //players slows down
    }

    /*private void WallSlide()
    {

        _movementActu = 0;

        if (_rb.velocity.x > 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x - (_speed * Time.fixedDeltaTime), _rb.velocity.y);
            if (_rb.velocity.x < 0) _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
        else if (_rb.velocity.x < 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x + (_speed * Time.fixedDeltaTime), _rb.velocity.y);
            if (_rb.velocity.x > 0) _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
    }*/

    private void FallCurve()
    {
        //player 1
        if (_gravitySetting == 0)
        {
            //if (jumpState != JumpState.Grounded)
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y - _jumpFallSpeed * Time.fixedDeltaTime);
            if (_rb.velocity.y < -_maxFallSpeed)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, -_maxFallSpeed);
            }
        }
        //player 2
        else if (_gravitySetting == 1)
        {
            if (_rb.gravityScale > 0) _rb.gravityScale = -_rb.gravityScale;

            //if (jumpState != JumpState.Grounded)
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y + _jumpFallSpeed * Time.fixedDeltaTime);
            if (_rb.velocity.y > _maxFallSpeed)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _maxFallSpeed);
            }
        }


    }

    private void MaxSpeed()
    {
        if (_rb.velocity.x > 0 && _rb.velocity.x > _maxSpeed)
        {
            _rb.velocity = new Vector2(_maxSpeed, _rb.velocity.y);
        }

        else if (_rb.velocity.x < 0 && _rb.velocity.x < -_maxSpeed)
        {
            _rb.velocity = new Vector2(-_maxSpeed, _rb.velocity.y);
        }
    }

    private void ComputeJump()
    {
        if (_rb != null)
        {
            //player 1
            if (_gravitySetting == 0)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _jumpSpeed);
            }
            //else
            else if (_gravitySetting == 1)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, -_jumpSpeed);
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (_gravitySetting == 0)
        {
            var colliderLength = _rightEdge.transform.localPosition.x * transform.localScale.x * 2;
            var startPosition = transform.position.x - colliderLength / 2;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                Gizmos.DrawLine(new Vector3(startPosition + (i * (colliderLength / _nbOfRay)), _groundPosition.transform.position.y, 0), new Vector3(startPosition + (i * (colliderLength / _nbOfRay)), _groundPosition.transform.position.y - _rayLength, 0));
            }
        }
        else if (_gravitySetting == 1)
        {
            var colliderLength = _rightEdge.transform.localPosition.x * transform.localScale.x * 2;
            var startPosition = transform.position.x - colliderLength / 2;

            //ray cast
            for (int i = 0; i < _nbOfRay + 1; i++)
            {
                Gizmos.DrawLine(new Vector3(startPosition + (i * (colliderLength / _nbOfRay)), _groundPosition.transform.position.y, 0), new Vector3(startPosition + (i * (colliderLength / _nbOfRay)), _groundPosition.transform.position.y + _rayLength, 0));
            }
        }

        //obstacles collision
        var colliderWidth = _groundPosition.transform.localPosition.y * transform.localScale.y * 2;
        var startPosition2 = transform.position.y + colliderWidth / 2;
        //var startPosition2 = _rightEdge.transform.localPosition.y;

        //ray cast
        for (int i = 0; i < _nbOfRay + 1; i++)
        {
            Gizmos.DrawLine(new Vector3(_rightEdge.transform.position.x, startPosition2 - (i * (colliderWidth / _nbOfRay)), 0), new Vector3(_rightEdge.transform.position.x + _rayLength, startPosition2 - (i * (colliderWidth / _nbOfRay)), 0));
        }


    }

}
