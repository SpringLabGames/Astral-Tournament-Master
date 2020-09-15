using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Boss : MonoBehaviour
{
    public int health = 1000;
    public bool attacked;
    public int kickDamage = 20;
    public int spearDamage = 30;
    public float spearAttackRange = 0.1f;
    public float kickAttackRange = 0.1f;
    public Transform spearTip;
    public Transform foot;

    public LayerMask layerMask;

    private Transform target;

    private bool m_Started;

    // Start is called before the first frame update
    void Start()
    {
        //transform.up = handToFollow.forward;
        m_Started = true;
    }

    public void Spear_Attack()
    {
        Vector3 scaleBox = new Vector3(2f, .3f, .2f);

        Collider[] hitColliders = Physics.OverlapBox(spearTip.position, scaleBox, Quaternion.identity, layerMask);
        foreach (Collider col in hitColliders)
        {
            if (col.tag == "Player")
            {
                //col.GetComponent<Health>().CmdDealDamageWithAmount(attackDamage);
                Debug.Log("attacked " + col.name);
            }
        }
    }

    public void Kick_Attack()
    {
        Vector3 scaleBox = new Vector3(.5f, .2f, .1f);

        Collider[] hitColliders = Physics.OverlapBox(foot.position, scaleBox, Quaternion.identity, layerMask);
        foreach (Collider col in hitColliders)
        {
            if (col.tag == "Player")
            {
                //col.GetComponent<Health>().CmdDealDamageWithAmount(attackDamage);
                Debug.Log("attacked " + col.name);
            }
        }
    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Vector3 scaleBox1 = new Vector3(2f, .3f, .2f);
        Vector3 scaleBox2 = new Vector3(.5f, .2f, .1f);

        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (m_Started)
        {
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(spearTip.position, scaleBox1);
            Gizmos.DrawWireCube(foot.position, scaleBox2);
        }
    }

    public void takeDamage(int amount)
    {
        health -= amount;
        if(health <= 0)
        {
            die();
        }
    }

    private void die()
    {
        Destroy(gameObject);
        Debug.Log("Boss died");
    }

    public void moveTo(GameObject target)
    {
        GetComponent<NavMeshAgent>().destination = target.transform.position;
    }

    public void moveToTarget(ChaseData data)
    {
        
        StartCoroutine("Chase",data);
    }

    public void moveToFruit()
    {
        StopCoroutine("Chase");

    }

    private IEnumerator Chase(ChaseData data)
    {
        while (true)
        {
            if(target!=null)
            {
                data.walk.target_player = GameObject.FindGameObjectWithTag("Player");
                GetComponent<NavMeshAgent>().destination = data.target.transform.position;
                yield return new WaitForSeconds(5f);
            }
        }
    }

}

public class ChaseData
{
    public Boss_walk walk;
    public Transform target;

    public ChaseData(Boss_walk walk_ref, Transform trans)
    {
        walk = walk_ref;
        target = trans;
    }
}
