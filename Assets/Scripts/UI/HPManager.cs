using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPManager : MonoBehaviour
{
    //Singleton
    public static HPManager Instance;

    int _hp = 3;
    public GameObject _hp3;
    public GameObject _hp2;
    public GameObject _hp1;

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
        _hp = 3;

        _hp3.SetActive(true);
        _hp2.SetActive(false);
        _hp1.SetActive(false);
    }

    public void LooseHp()
    {
        _hp--;

        switch (_hp)
        {
            case 3:
                _hp3.SetActive(true);
                _hp2.SetActive(false);
                _hp1.SetActive(false);
                break;
            case 2:
                _hp3.SetActive(false);
                _hp2.SetActive(true);
                _hp1.SetActive(false);
                break;
            case 1:
                _hp3.SetActive(false);
                _hp2.SetActive(false);
                _hp1.SetActive(true);
                break;
        }

    }


}
