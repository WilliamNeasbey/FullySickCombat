using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AISimple : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    SoundManager soundMan;
    AnimatorEventsEn animEv;
    public Transform player;
    public enum STATE
    {
        IDLE, PATROL, CHASE, MELEEATTACK, HIT
    }
    public STATE currState = STATE.IDLE;

    public List<GameObject> patrolPoints = new List<GameObject>();
    int curPatrolIndex = -1; //The point of the patrol points where the enemy goes

    private float waitTimer = 0;
    public float attackTime = 1.0f; //Time between attacks

    private bool isInvincible = false;

    float visDist = 10.0f; //Distance of vision
    float visAngle = 90.0f; //Angle of the cone vision
    float meleeDist = 1.5f; //Distance from which the enemy will attack the player

    public int maxHealth = 100; // Maximum health of the AI
    private int currentHealth; // Current health of the AI

    private bool canAttack = true;

    void Start()
    {
        currentHealth = maxHealth; // Set the initial health to maximum
    }

    public void ApplyDmg(DmgInfo dmgInfo)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            ChangeState(STATE.HIT);
            soundMan.PlaySound("Hit");

            // Deduct the damage from the AI's health
            currentHealth -= dmgInfo.dmgValue;

            // Check if the AI's health has reached zero or below
            if (currentHealth <= 0)
            {
                // AI is defeated
                Die();
            }
        }
    }

    private void Die()
    {
        // Handle AI death, such as playing death animation, disabling components, or triggering game events
        // ...

        // Delete the AI object
        Destroy(gameObject);
    }

    void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponentInChildren<Animator>();
        animEv = this.GetComponentInChildren<AnimatorEventsEn>();
        soundMan = GetComponent<SoundManager>();

        if (patrolPoints.Count != 0) // If there are patrol points
            ChangeState(STATE.PATROL);
    }

    void Update()
    {
        switch (currState)
        {
            case STATE.IDLE:
                if (CanSeePlayer())
                {
                    ChangeState(STATE.CHASE);
                }
                else if (Random.Range(0, 100) < 10)
                {
                    ChangeState(STATE.PATROL);
                }
                break;
            case STATE.PATROL:
                if (agent.remainingDistance < 1)
                {
                    if (curPatrolIndex >= patrolPoints.Count - 1)
                        curPatrolIndex = 0;
                    else
                        curPatrolIndex++;
                    agent.SetDestination(patrolPoints[curPatrolIndex].transform.position);
                }

                if (CanSeePlayer())
                {
                    ChangeState(STATE.CHASE);
                }
                break;
            case STATE.CHASE:
                agent.SetDestination(player.position);
                if (agent.hasPath)
                {
                    if (CanAttackPlayer())
                    {
                        ChangeState(STATE.MELEEATTACK);
                    }
                    else if (CanStopChase())
                    {
                        ChangeState(STATE.PATROL);
                    }
                }
                break;
            case STATE.MELEEATTACK:
                LookPlayer(2.0f);

                waitTimer += Time.deltaTime;
                if (waitTimer >= attackTime)
                {
                    if (CanAttackPlayer())
                        ChangeState(STATE.CHASE);
                    else
                        ChangeState(STATE.PATROL);
                }
                break;
            case STATE.HIT:
                waitTimer += Time.deltaTime;
                if (waitTimer < 0.5f)
                {
                    LookPlayer(5.0f);
                }
                else if (waitTimer >= 0.5f && isInvincible)
                {
                    isInvincible = false;
                    canAttack = false;
                    StartCoroutine(EnableAttackAfterSeconds(2f));
                }
                else if (waitTimer >= 1.25f)
                {
                    if (CanAttackPlayer() && canAttack)
                        ChangeState(STATE.CHASE);
                    else
                        ChangeState(STATE.PATROL);
                }
                break;
        }
    }

    private IEnumerator EnableAttackAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canAttack = true;
    }

    public bool CanSeePlayer()
    {
        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (direction.magnitude < visDist && angle < visAngle)
        {
            return true;
        }
        return false;
    }

    public bool CanAttackPlayer()
    {
        Vector3 direction = player.position - transform.position;
        if (direction.magnitude < meleeDist)
        {
            return true;
        }
        return false;
    }

    public bool CanStopChase() //Stop follow the player
    {
        Vector3 direction = player.position - transform.position;
        if (direction.magnitude > visDist)
        {
            return true;
        }
        return false;
    }

    public void ChangeState(STATE newState)
    {
        switch (currState)
        {
            case STATE.IDLE:
                anim.ResetTrigger("isIdle");
                break;
            case STATE.PATROL:
                anim.ResetTrigger("isPatrolling");
                break;
            case STATE.CHASE:
                anim.ResetTrigger("isChasing");
                break;
            case STATE.MELEEATTACK:
                anim.ResetTrigger("isMeleeAttacking");
                animEv.isAttacking = false;
                anim.GetComponent<AnimatorEventsEn>().DisableWeaponColl();
                break;
            case STATE.HIT:
                anim.ResetTrigger("isHited");
                break;
        }
        switch (newState)
        {
            case STATE.IDLE:
                anim.SetTrigger("isIdle");
                break;
            case STATE.PATROL:
                agent.speed = 2;
                agent.isStopped = false;

                float lastDist = Mathf.Infinity;
                for (int i = 0; i < patrolPoints.Count; i++)
                {
                    GameObject thisWP = patrolPoints[i];
                    float distance = Vector3.Distance(transform.position, thisWP.transform.position);
                    if (distance < lastDist)
                    {
                        curPatrolIndex = i - 1; //Because in the update it will be added one
                        lastDist = distance;
                    }
                }
                anim.SetTrigger("isPatrolling");
                break;
            case STATE.CHASE:
                agent.speed = 3;
                agent.isStopped = false;
                anim.SetTrigger("isChasing");
                break;
            case STATE.MELEEATTACK:
                anim.SetTrigger("isMeleeAttacking");
                agent.isStopped = true;
                waitTimer = 0;
                animEv.isAttacking = true;
                break;
            case STATE.HIT:
                anim.SetTrigger("isHited");
                agent.isStopped = true;
                waitTimer = 0;
                break;
        }
        currState = newState;
    }

    public void LookPlayer(float rotSpeed)
    {
        Quaternion rotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotSpeed);
    }
}