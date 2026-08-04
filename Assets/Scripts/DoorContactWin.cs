using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorContactWin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Load the next scene when the player enters the trigger
            SceneManager.LoadScene("Win Screen");
        }
    }
}

