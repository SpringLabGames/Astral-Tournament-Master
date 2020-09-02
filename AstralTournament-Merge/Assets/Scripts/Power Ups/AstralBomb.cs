using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AstralBomb : ThrowablePowerUp
{
    public float forceExplosion;
    public float radiusExplosion;
    private bool explosionFlag;
    public int timer;

    private CharacterController controller;

    public override void OnThrow(Vector3 force)
    {
        print("BOMB FORCE: " + force);
        Rigidbody rigid = GetComponent<Rigidbody>();
        rigid.AddForce(force);
        explosionFlag = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        print("COLLIDED");
        NetworkVehicle net = collider.GetComponent<NetworkVehicle>();
        if (net != null)
        {
            Rigidbody rigid = GetComponent<Rigidbody>();
            Vector3 position = transform.position;
            rigid.AddExplosionForce(forceExplosion, position, radiusExplosion);
            Destroy(gameObject);
        }
        else if(collider is MeshCollider || collider is TerrainCollider)
            explosionFlag = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (explosionFlag)
        {
            print("FLAG TRUE");
            StartCoroutine("explosionCoroutine");
        }
        else print("FLAG FALSE");
    }

    private IEnumerator explosionCoroutine()
    {
        for (int i = 0; i < timer; i++)
            yield return new WaitForSeconds(1);
        Rigidbody rigid = GetComponent<Rigidbody>();
        Vector3 position = transform.position;
        rigid.AddExplosionForce(forceExplosion, position, radiusExplosion);
        Destroy(gameObject);
        
    }
}
