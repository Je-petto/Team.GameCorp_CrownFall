using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MathcingButton : MonoBehaviour
{
    private bool isPressed;

    private Image buttonImg;

    [SerializeField] Sprite nonPressed;
    [SerializeField] Sprite onPressed;

    void Start()
    {
        isPressed = false;
        buttonImg.sprite = nonPressed;
    }

    public void OnClickMatchingButton()
    {
        if (isPressed)
        {
            buttonImg.sprite = nonPressed;
        }
        else
        {
            buttonImg.sprite = onPressed;
        }
    }
}
