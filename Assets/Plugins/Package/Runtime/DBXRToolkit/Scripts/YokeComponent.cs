using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class YokeComponent : InteractableComponent
{

    public UnityEvent<Vector2> GrabUpdate;

    [SerializeField] private Transform yoke, clampPosLeft, clampPosRight;

    [Space] // Local position of the "lever" object when it is fully extended and fully pressed
    [SerializeField] private float outValue;
    [SerializeField] private float inValue;
    private HandInteract primaryHand, secondaryHand;
    private Vector2 currentValue;

    private Transform primaryClampPos, secondaryClampPos;
    private Quaternion desiredRot;

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();
        // Set default yoke position on awake
        float value = yoke.localPosition.z;
        currentValue.x = Mathf.Clamp(value, outValue, inValue);
    }

    void Update()
    {
        if (primaryHand)
        {

            Vector3 localPos = transform.InverseTransformPoint(primaryHand.GetHandTransform().parent.position);
            float value = localPos.z;
            currentValue.x = Mathf.Clamp(value, outValue, inValue);

            yoke.localPosition = new Vector3(0, 0, currentValue.x);

            if (!secondaryHand)
            {
                Vector3 dir = primaryHand.GetHandTransform().parent.forward;
                desiredRot = Quaternion.LookRotation(transform.forward, dir);
            } else
            {
                Vector3 primaryDir = primaryHand.GetHandTransform().parent.forward;
                Vector3 secondaryDir = secondaryHand.GetHandTransform().parent.forward;
                float primaryY = transform.InverseTransformPoint(primaryHand.transform.position).y;
                float secondaryY = transform.InverseTransformPoint(secondaryHand.transform.position).y;
                float diff = Mathf.Abs(primaryY - secondaryY);
                desiredRot = Quaternion.LookRotation(transform.forward, diff < 0.2f ? primaryDir : primaryY > secondaryY ? primaryDir : secondaryDir);
                //desiredRot = Quaternion.LookRotation(transform.forward, primaryDir + secondaryDir);
            }

            yoke.transform.rotation = Quaternion.Lerp(yoke.transform.rotation, desiredRot, .7f);
            currentValue.y = yoke.transform.localEulerAngles.z;
            if(currentValue.y > 180)
            {
                currentValue.y -= 360;
            }

            GrabUpdate.Invoke(GetValue());

        } else
        {
            // Move back to zero position
            if(yoke.localPosition != Vector3.zero || yoke.localEulerAngles != Vector3.zero)
            {
                yoke.SetLocalPositionAndRotation(Vector3.MoveTowards(yoke.localPosition, Vector3.zero, 1.5f * Time.deltaTime),
                                 Quaternion.RotateTowards(yoke.localRotation, Quaternion.Euler(Vector3.zero), 30f * Time.deltaTime));
                GrabUpdate.Invoke(GetValue());

                currentValue.x = yoke.transform.localPosition.z;
                currentValue.y = yoke.transform.localEulerAngles.z;
                if (currentValue.y > 180)
                {
                    currentValue.y -= 360;
                }

            }

        }

    }

    public override void Grab(HandInteract interact)
    {

        if(primaryHand == null)
        {
            primaryHand = interact;
            float leftDist = Vector3.SqrMagnitude(clampPosLeft.position - interact.GetHandTransform().position);
            float rightDist = Vector3.SqrMagnitude(clampPosRight.position - interact.GetHandTransform().position);
            if(leftDist < rightDist)
            {
                if (interact is HandControllerInteract)
                {
                    ((HandControllerInteract)interact).SetHandOverride(clampPosLeft);
                }
                primaryClampPos = clampPosLeft;
            } else 
            {
                if (interact is HandControllerInteract)
                {
                    ((HandControllerInteract)interact).SetHandOverride(clampPosRight);
                }
                primaryClampPos = clampPosRight;
            }
        } else 
        {
            secondaryHand = interact;
            if(primaryClampPos == clampPosLeft)
            {
                if (interact is HandControllerInteract)
                {
                    ((HandControllerInteract)interact).SetHandOverride(clampPosRight);
                }
                secondaryClampPos = clampPosRight;
            } else 
            {
                if (interact is HandControllerInteract)
                {
                    ((HandControllerInteract)interact).SetHandOverride(clampPosLeft);
                }
                secondaryClampPos = clampPosLeft;
            }
        }

        held = true;

    }

    public override void Drop(HandInteract interact)
    {
        if(secondaryHand == null)
        {
            primaryHand = null;
            primaryClampPos = null;
            if (interact is HandControllerInteract)
            {
                ((HandControllerInteract)interact).SetHandOverride(null);
            }
            held = false;
        } else 
        {
            if(interact == primaryHand)
            {

                if (interact is HandControllerInteract)
                {
                    ((HandControllerInteract)interact).SetHandOverride(null);
                }

                primaryHand = secondaryHand;
                primaryClampPos = secondaryClampPos;

                if (primaryHand is HandControllerInteract)
                {
                    ((HandControllerInteract)primaryHand).SetHandOverride(primaryClampPos);
                }

                secondaryHand = null;
                secondaryClampPos = null;

            } else 
            {
                if (secondaryHand is HandControllerInteract)
                {
                    ((HandControllerInteract)secondaryHand).SetHandOverride(null);
                }
                secondaryHand = null;
                secondaryClampPos = null;
            }
        }
    }

    public Vector2 GetValue()
    {
        float translationValue = (currentValue.x - outValue) / (inValue - outValue);
        float rotationValue = currentValue.y;
        return new Vector2(translationValue, rotationValue);
    }
}