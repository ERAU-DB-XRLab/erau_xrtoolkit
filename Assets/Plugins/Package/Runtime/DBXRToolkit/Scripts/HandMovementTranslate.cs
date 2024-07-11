using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandMovementTranslate : MonoBehaviour
{

    [Header("Player Movement Method (Pick One)")]
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected CharacterController cc;
    
    [Space]
    [SerializeField] protected InputActionProperty move;
    [Space]
    [SerializeField] protected float movementSpeed;

    protected Transform mainCamera;

    void Awake()
    {
        mainCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 input = move.action.ReadValue<Vector2>();

        Vector3 forwardDir = mainCamera.transform.forward;
        forwardDir.y = 0;
        forwardDir.Normalize();

        Vector3 rightDir = mainCamera.transform.right;
        rightDir.y = 0;
        rightDir.Normalize();

        Vector3 vel = (input.x * rightDir) + (input.y * forwardDir);

        if(cc)
        {
            cc.Move(movementSpeed * Time.deltaTime * vel);
        } else
        if(rb)
        {
            rb.velocity = vel * movementSpeed;
        }
    }

}
