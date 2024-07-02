using UnityEngine;

[RequireComponent(typeof(Velocity))]
public class HandInteract : MonoBehaviour
{

    // The hand transform
    protected Transform handTransform;
    protected Velocity velComponent;

    protected void Awake()
    {
        velComponent = transform.parent.GetComponentInChildren<Velocity>();
    }
    public Vector3 GetVelocity()
    {
        return velComponent.GetVelocity();
    }

    public Transform GetHandTransform()
    {
        return handTransform;
    }

}
