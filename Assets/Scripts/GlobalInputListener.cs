using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GlobalInputListener : MonoBehaviour
{
    public InputActionAsset InputActions;

    // Start is called before the first frame update
    void Start()
    {
        InputActionMap map = InputActions.FindActionMap("XRI LeftHand");
        InputAction openMenu = map.FindAction("Open Menu");
        openMenu.performed += OnOpenMenu;
    }

    private void OnOpenMenu(InputAction.CallbackContext obj)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
