using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonTester : MonoBehaviour
{
    public float Speed = 1f;
    public InputActionReference MoveAction;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var movement = MoveAction.action.ReadValue<Vector2>();

        transform.Translate(0f, movement.y * Speed * Time.deltaTime, 0f, Space.World);
    }

}
