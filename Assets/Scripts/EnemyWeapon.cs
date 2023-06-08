using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
  //  public int dmgValue = 132; // Damage of the weapon
  //  public Color dmgColor = Color.cyan; // Color of the text with the damage value

    private BoxCollider coll; // Collider of the weapon

    void Awake()
    {
        coll = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerChunLi playerChunLi = other.GetComponent<PlayerChunLi>();
            if (playerChunLi != null)
            {
                playerChunLi.TakeDamage(10); // Deduct 10 health from Chun Li
            }
        }
    }

    public void EnableColliders() // Called from the AnimatorEvent script
    {
        coll.enabled = true;
    }

    public void DisableColliders() // Called from the AnimatorEvent script
    {
        coll.enabled = false;
    }
}
