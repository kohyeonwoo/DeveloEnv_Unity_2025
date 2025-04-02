using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public enum EnemyType { Basic, Dash, LongDistance, Boss }

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;

    public Rigidbody rigid;
    public Animator anim;
    public SkinnedMeshRenderer[] meshes;
    public NavMeshAgent nav;
    public Collider bodyCollider;

    public int maxHealth;
    public int currentHealth;
    public int attackPoint;

    public bool bChase;
    public bool bAttack;

    public Transform target;
    public GameObject enemyAttackCollision;
    public GameObject particleEffect;
    public GameObject bullet;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        bodyCollider = GetComponent<Collider>();
        meshes = GetComponentsInChildren<SkinnedMeshRenderer>();

        currentHealth = maxHealth;

        if (enemyType != EnemyType.Boss)
        {
            Invoke("ChaseStart", 1.0f);
        }


    }

    private void Update()
    {
        ChaseTarget();
    }

    private void FixedUpdate()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        Targeting();
        FreezeVelocity();
    }

    public void ChaseStart()
    {
        bChase = true;
        anim.SetBool("bMove", true);
    }

    public void ChaseTarget()
    {
        if (nav.enabled && enemyType != EnemyType.Boss)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !bChase;
        }
    }

    protected void FreezeVelocity()
    {
        if (bChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    protected void Targeting()
    {
        float targetRadius = 0.0f;
        float targetRange = 0.0f;

        if (enemyType != EnemyType.Boss)
        {
            switch (enemyType)
            {
                case EnemyType.Basic:
                    targetRadius = 1.5f;
                    targetRange = 3.0f;
                    break;
                case EnemyType.Dash:
                    targetRadius = 0.5f;
                    targetRange = 12.0f;
                    break;
                case EnemyType.LongDistance:
                    targetRadius = 0.5f;
                    targetRange = 25.0f;
                    break;
            }

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                targetRadius,
                transform.forward,
                targetRange,
                LayerMask.GetMask("Player"));

            if (rayHits.Length > 0 && !bAttack)
            {
                StartCoroutine(Attack());
            }
        }


    }

    public void ActiveEnemyAttackCollision()
    {
        enemyAttackCollision.SetActive(true);
    }

    public void DeActiveEnemyAttackCollision()
    {
        enemyAttackCollision.SetActive(false);
    }

    private IEnumerator Attack()
    {

        bChase = false;
        bAttack = true;
        anim.SetBool("bAttack", true);

        switch (enemyType)
        {
            case EnemyType.Basic:

                yield return new WaitForSeconds(2.0f);

                break;

            case EnemyType.Dash:

                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(this.transform.forward * 20, ForceMode.Impulse);

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;

                yield return new WaitForSeconds(2.0f);

                break;

            case EnemyType.LongDistance:

                yield return new WaitForSeconds(0.5f);
                //총알 생성 부분 

                break;
        }



        bChase = true;
        bAttack = false;
        anim.SetBool("bAttack", false);
    }

    IEnumerator ChangeColor()
    {

        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.3f);

        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.white;
        }
    }

    public void Damage(int Damage)
    {
        currentHealth -= Damage;
        StartCoroutine(ChangeColor());
        GameObject obj = Instantiate(particleEffect, transform.position, Quaternion.identity);
        Destroy(obj, 2.0f);

        if (currentHealth <= 0 && enemyType != EnemyType.Boss)
        {
            Dead();
        }
    }

    private void Dead()
    {
        bChase = false;
        nav.enabled = false;
        anim.SetTrigger("Die");
        Invoke("EraseBody", 2.0f);
    }

    private void EraseBody()
    {
        this.gameObject.SetActive(false);
    }

}
