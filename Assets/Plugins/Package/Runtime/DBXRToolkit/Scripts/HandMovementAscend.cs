using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandMovementAscend : MonoBehaviour
{

    [SerializeField] private Transform player;
    [Space]
    [SerializeField] private InputActionProperty move;
    [Space]
    [SerializeField] private float movementSpeed;

    // Update is called once per frame
    void Update()
    {

        Vector2 input = move.action.ReadValue<Vector2>();
        Vector3 vel = movementSpeed * input.y * Vector3.up;
        player.position += vel * Time.deltaTime;

    }

}