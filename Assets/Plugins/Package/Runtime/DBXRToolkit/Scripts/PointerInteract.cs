using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerInteract : MonoBehaviour
{

    private HandInteract interact;
    private float pressTimer;

    void Awake()
    {
        interact = GetComponentInParent<HandInteract>();
    }

    void Update()
    {
        pressTimer += Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == DBXRResources.Main.InteractLayer)
        {
            InteractableComponent ic = other.gameObject.GetComponent<InteractableComponent>();
            if(ic)
            {
                ic.PointerEntered.Invoke(interact);
            }
        } else
        if(other.gameObject.layer == DBXRResources.Main.UILayer)
        {
            if(pressTimer > 0.35f)
            {
                pressTimer = 0;
                UIButton button = other.gameObject.GetComponent<UIButton>();
                if (button)
                {
                    button.Press();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == DBXRResources.Main.InteractLayer)
        {
            InteractableComponent ic = other.gameObject.GetComponent<InteractableComponent>();
            if(ic)
            {
                ic.PointerExited.Invoke(interact);
            }
        } else
        if (other.gameObject.layer == DBXRResources.Main.UILayer)
        {
            UIButton button = other.gameObject.GetComponent<UIButton>();
            if (button)
            {
                button.Release();
            }
        }
    }

}
