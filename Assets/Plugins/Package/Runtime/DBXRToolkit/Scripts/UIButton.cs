using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class UIButton : MonoBehaviour
{


    public UnityEvent OnClicked;

    [SerializeField] protected TMP_Text buttonText;
    [SerializeField] private bool toggle;
    [Space]
    [Header("Color Info")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color selectedColor;

    private bool pressed;

    public void Press()
    {

        if(!toggle)
        {
            pressed = true;
        } else
        {
            pressed = !pressed;
        }
        UpdateColor();

        if(pressed)
            OnClicked.Invoke();

    }

    public void Release()
    {
        if(!toggle)
        {
            pressed = false;
            UpdateColor();
        }
    }

    void OnDisable()
    {
        pressed = false;
        UpdateColor();
    }

    public void UpdateColor()
    {
        if(buttonText)
        buttonText.color = pressed ? selectedColor : normalColor;
    }

}