using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: This class will eventually be rewritten where UIButtons are Interactable components

[RequireComponent(typeof(LineRenderer))]
public class HandControllerLaser : MonoBehaviour
{

    [SerializeField] private InputActionProperty interact;
    [SerializeField] private Transform laserOrigin;

    private InteractableComponent currentInteractable;
    private UIButton currentUI;

    // Line
    private LineRenderer line;
    private bool interacting;
    private Vector3 hitPoint;

    private float interaction;

    // Start is called before the first frame update
    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(new Ray(laserOrigin.position, laserOrigin.forward), out hit, Mathf.Infinity, DBXRResources.Main.InteractLayerMask | DBXRResources.Main.UILayerMask, QueryTriggerInteraction.Collide))
        {

            if (hit.transform.gameObject.layer == DBXRResources.Main.InteractLayer)
            {

                InteractableComponent ic = hit.transform.gameObject.GetComponent<InteractableComponent>();
                if(ic && currentInteractable != ic)
                {

                    ResetIC();

                    currentInteractable = ic;
                    currentInteractable.RayEntered.Invoke(null);

                } else
                {
                    ResetIC();
                }

                ResetUI();

            }
            else
            if (hit.transform.gameObject.layer == DBXRResources.Main.UILayer)
            {
                UIButton button = hit.transform.gameObject.GetComponent<UIButton>();
                if(button && currentUI != button)
                {

                    ResetUI();
                    currentUI = button;
                
                }

                ResetIC();

            }

            hitPoint = hit.point;

        } else
        {
            hitPoint = laserOrigin.position + (laserOrigin.forward * 999);
            ResetUI();
            ResetIC();
        }

        interaction = interact.action.ReadValue<float>();
        if(interaction > 0)
        {
            line.positionCount = 2;
            line.SetPositions(new Vector3[] { laserOrigin.position, hitPoint });
            
            if(interaction > 0.85f && !interacting)
            {
                interacting = true;
                if(currentUI)
                {
                    currentUI.Press();
                } else
                if(currentInteractable)
                {
                    currentInteractable.RayInteractStarted.Invoke(null);
                }
            } else
            {
                if(interacting && interaction < 0.85f)
                {
                    if (currentUI)
                    {
                        currentUI.Release();
                    }
                    else
                    if (currentInteractable)
                    {
                        currentInteractable.RayInteractStopped.Invoke(null);
                    }
                    interacting = false;
                }

            }

        } else
        {
            if(interacting && currentUI)
            {
                currentUI.Release();
            } else
            if (interacting && currentInteractable)
            {
                currentInteractable.RayInteractStopped.Invoke(null);
            }
            interacting = false;
            line.positionCount = 0;
        }

    }

    public void ResetIC()
    {
        if(currentInteractable)
        {
            currentInteractable.RayExited.Invoke(null);
            currentInteractable = null;
        }
    }

    public void ResetUI()
    {
        if(currentUI)
        {
            if(interacting)
            {
                currentUI.Release();
            }
            currentUI = null;
        }
    }

}
