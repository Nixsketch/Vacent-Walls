using UnityEngine;

public class MouseVisible : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true; // Make the mouse cursor visible
        Cursor.lockState = CursorLockMode.None; // Unlock the mouse cursor
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
