using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerZeroGravity : Player
{

    [SerializeField] private HandControllerInteractZeroGravity leftHandInteract, rightHandInteract;
    [SerializeField] private Transform leftHandTracked, rightHandTracked;

    private Camera mainCamera;
    private Vector3 leftStart, rightStart;
    private Vector3 vectorStart, axis;

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        if(leftHandInteract.Holding && rightHandInteract.Holding)
        {
            
            if(!leftHandInteract.Rotating)
            {
                leftHandInteract.Rotating = true;
                rightHandInteract.Rotating = true;
                leftStart = leftHandTracked.localPosition;
                rightStart = rightHandTracked.localPosition;
                axis = mainCamera.transform.forward;
                vectorStart = leftStart - rightStart;
            }

            Vector3 newVector = leftHandTracked.localPosition - rightHandTracked.localPosition;
            transform.RotateAround(mainCamera.transform.position, axis, Vector3.SignedAngle(newVector, vectorStart, axis));
            vectorStart = newVector;

        } else
        {
            if (leftHandInteract.Rotating)
            {
                leftHandInteract.Rotating = false;
                rightHandInteract.Rotating = false;
                leftHandInteract.UpdateGrabPoints();
                rightHandInteract.UpdateGrabPoints();
            }
        }

    }
}
