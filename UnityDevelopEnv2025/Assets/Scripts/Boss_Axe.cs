using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Boss_Axe : Enemy
{
    public GameObject bossAttackCollider;
    private Vector3 lookVector;
    private Vector3 tauntVector;
    public bool bLook;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        bodyCollider = GetComponent<Collider>();
        meshes = GetComponentsInChildren<SkinnedMeshRenderer>();

        nav.isStopped = true;
        StartCoroutine(Pattern());
    }

    private void Update()
    {
        if (bLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3(h, 0, v) * 5.0f;
            transform.LookAt(target.position + lookVector);
        }
        else
        {
            nav.SetDestination(tauntVector);
        }
    }

    IEnumerator Pattern()
    {
        yield return new WaitForSeconds(0.1f);

        int rand = Random.Range(0, 2);

        switch (rand)
        {
            case 0:
                //일반 공격 패턴 
                StartCoroutine(NormalAttack1());
                break;
            case 1:
                //점프 공격 or 돌진 공격
                StartCoroutine(JumpAttack());
                break;
                //case 2:

                //    break;
                //case 3:
                //    break;
                //case 4:
                //    break;
        }

    }

    private IEnumerator NormalAttack1()
    {
        bLook = false;
        anim.SetTrigger("Attack1");
        yield return new WaitForSeconds(2.5f);
        bLook = true;
        StartCoroutine(Pattern());
    }

    private IEnumerator JumpAttack()
    {
        tauntVector = target.position + lookVector;

        bLook = false;
        nav.isStopped = false;
        // bodyCollider.enabled = false;
        anim.SetTrigger("JumpAttack");

        yield return new WaitForSeconds(0.5f);
        //bossAttackCollider.SetActive(true);

        yield return new WaitForSeconds(0.5f);
       // bossAttackCollider.SetActive(false);

        yield return new WaitForSeconds(1.5f);
        bLook = true;
        nav.isStopped = true;
        //  bodyCollider.enabled = true;

        StartCoroutine(Pattern());
    }

}
