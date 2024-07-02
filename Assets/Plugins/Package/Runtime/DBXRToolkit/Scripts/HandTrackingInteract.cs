#if HAND_TRACKING
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

public class HandTrackingInteract : HandInteract
{

    [SerializeField] private XRHandShape grab;
    [SerializeField] private Transform grabPoint;

    private XRHandTrackingEvents events;
    private InteractableComponent grabbedInteractable;

    new void Awake()
    {
        base.Awake();

        events = GetComponentInParent<XRHandTrackingEvents>();
        events.jointsUpdated.AddListener(JointsUpdated);

        handTransform = grabPoint;
    }

    public void JointsUpdated(XRHandJointsUpdatedEventArgs args)
    {
        if (grab.CheckConditions(args))
        {
            if (!grabbedInteractable)
            {
                grabbedInteractable = GetNearestInteractable();
                if(grabbedInteractable)
                {
                    grabPoint.gameObject.SetActive(false);
                    grabbedInteractable.Grabbed.Invoke(this);
                }
            }
        }
        else
        {
            grabPoint.gameObject.SetActive(true);
            if (grabbedInteractable)
            {
                grabbedInteractable.Dropped.Invoke(this);
                grabbedInteractable = null;
            }
        }
    }

    public InteractableComponent GetNearestInteractable()
    {

        Collider[] col = Physics.OverlapSphere(transform.position, DBXRResources.Main.InteractRadius, DBXRResources.Main.InteractLayerMask);

        if (col.Length > 0)
        {
            float closestDist = Mathf.Infinity;
            InteractableComponent closest = null;

            foreach (Collider c in col)
            {

                InteractableComponent ic = c.GetComponentInParent<InteractableComponent>();

                if (ic == null || !ic.IsGrabbable() || (ic.IsHeld() && ic is not TwoHandedInteractableComponent))
                    continue;

                float dist = Vector3.SqrMagnitude(c.transform.position - transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = ic;
                }
            }

            return closest;

        }
        else
        {
            return null;
        }

    }

}
#endif