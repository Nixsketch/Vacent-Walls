using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private bool isDead = false;

    public bool IsDead
    {
        get { return isDead; }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        GameManager.Instance.PlayerDied();
    }
}
