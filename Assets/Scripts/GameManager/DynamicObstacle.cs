using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{

    public Animator _anim;
    public BoxCollider2D _bcDown;
    public BoxCollider2D _bcUp;

    void Start()
    {
        if(_anim == null)
            _anim = GetComponent<Animator>();
    }


    void Update()
    {
        if (GameManager.Instance._gravitySetting == 0)
        {
            _anim.SetBool("_GravitySetting", false);
            if(_bcDown != null) _bcDown.enabled = true;
            if (_bcUp != null) _bcUp.enabled = false;
        }
        else
        {
            _anim.SetBool("_GravitySetting", true);
            if (_bcDown != null) _bcDown.enabled = false;
            if (_bcUp != null) _bcUp.enabled = true;
        }
    }
}
