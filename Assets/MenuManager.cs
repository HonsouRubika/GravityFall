using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject titleTargetPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TitleScreen()
    {
        LeanTween.move(title,titleTargetPos.transform.position, 1);
        LeanTween.rotate(title.GetComponent<RectTransform>(),90f, 1);

    }
}
