#if HAND_TRACKING
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

public class HandTrackingInteractZeroGravity : HandInteract
{

    [SerializeField] private XRHandShape grab;
    [SerializeField] private Transform grabPoint;
    [SerializeField] private Velocity vel;

    private Vector3 holdPoint;
    private Vector3 playerPoint;

    public bool Holding { get; private set; }
    public bool Rotating { get; set; }

    private XRHandTrackingEvents events;
    private InteractableComponent grabbedInteractable;
    private Rigidbody rb;

    new void Awake()
    {
        base.Awake();

        events = GetComponentInParent<XRHandTrackingEvents>();
        events.jointsUpdated.AddListener(JointsUpdated);

        handTransform = grabPoint;
        rb = GetComponentInParent<Rigidbody>();

    }

    void Update()
    {
        if (Holding && !Rotating)
        {
            // Translate
            Vector3 delta = transform.parent.parent.localPosition - holdPoint;
            rb.transform.position = playerPoint - (rb.transform.forward * delta.z) - (rb.transform.right * delta.x) - (rb.transform.up * delta.y);
        }
    }

    public void JointsUpdated(XRHandJointsUpdatedEventArgs args)
    {
        if (grab.CheckConditions(args))
        {

            if (!grabbedInteractable)
            {

                if (CheckStatic())
                {
                    UpdateGrabPoints();
                    Holding = true;
                } else
                {
                    grabbedInteractable = GetNearestInteractable();
                    if (grabbedInteractable)
                    {
                        grabbedInteractable.Grabbed.Invoke(this);
                    }
                }

            }
        }
        else
        {
            if(Holding)
            {
                Holding = false;
                rb.velocity = vel.GetVelocity();
            }

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

    public bool CheckStatic()
    {
        return Physics.CheckSphere(transform.position, 0.1f, DBXRResources.Main.StaticLayerMask);
    }

    public void UpdateGrabPoints()
    {
        holdPoint = transform.parent.parent.localPosition;
        playerPoint = rb.transform.position;
    }

}
#endif