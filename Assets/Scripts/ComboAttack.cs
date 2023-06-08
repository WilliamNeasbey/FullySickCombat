using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAttack : MonoBehaviour
{
    private Animator anim;
    private PlayerController playerCont;
    private int comboCount; // Tracks the current combo count

    void Awake()
    {
        playerCont = GetComponent<PlayerController>();
        anim = GetComponentInChildren<Animator>();
        comboCount = 0;
    }

    void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            if (playerCont.canMove)
            {
                if (playerCont.charCont.isGrounded && !playerCont.IsDashing())
                {
                    if (comboCount == 0)
                    {
                        anim.Play("Attack1");
                        comboCount++;
                    }
                    else if (comboCount == 1)
                    {
                        anim.Play("Attack2");
                        comboCount++;
                    }
                    else if (comboCount == 2)
                    {
                        anim.Play("Attack3");
                        comboCount = 0; // Reset the combo count
                    }

                    playerCont.soundMan.PlaySound("Attack");
                }
            }
        }
    }
}
