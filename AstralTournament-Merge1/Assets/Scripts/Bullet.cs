using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float attack;
    public Type attackType;

    public GameObject hitPrefab;

    public int shooter;

    private void OnTriggerEnter(Collider collider)
    {
        NetworkVehicle net = collider.GetComponent<NetworkVehicle>();
        if(net!=null)
        {
            TypeManager typeManager = new TypeManager();
            NewPlayerController player = net.GetComponent<NewPlayerController>();
            int damage = Mathf.CeilToInt(player.defense - attack);//*typeManager.EffectValue(attackType,player.defenseType));
            if (damage >= 0) net.health -= damage;
        }
        Bullet bullet = collider.GetComponent<Bullet>();
        if (bullet != null) return;
        StartCoroutine("HitBullet");
    }

    private IEnumerator HitBullet()
    {
        GameObject hit = Instantiate<GameObject>(hitPrefab);
        hit.transform.position = transform.position;
        yield return new WaitForSeconds(1);
        Destroy(hit);
        Destroy(gameObject);
    }

    public void setShooter(int IDShooter)
    {

    }
}
