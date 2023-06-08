using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEnable : MonoBehaviour
{
    void Start()
    {
        // Enable the mouse cursor
        Cursor.visible = true;

        // Unlock the cursor
        Cursor.lockState = CursorLockMode.None;
    }
}

