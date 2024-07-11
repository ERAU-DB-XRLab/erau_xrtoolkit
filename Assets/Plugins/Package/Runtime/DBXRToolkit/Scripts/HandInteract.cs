using UnityEngine;

[RequireComponent(typeof(Velocity))]
public class HandInteract : MonoBehaviour
{

    // The hand transform
    protected Transform handTransform;
    protected Velocity velComponent;
    private PointerInteract pointer;

    protected void Awake()
    {
        velComponent = transform.parent.GetComponentInChildren<Velocity>();
        pointer = transform.parent.parent.GetComponentInChildren<PointerInteract>();
    }

    public PointerInteract GetPointer()
    {
        return pointer;
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
