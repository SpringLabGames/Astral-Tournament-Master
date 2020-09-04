using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float attack;
    public Type attackType;

    private void OnTriggerEnter(Collider collider)
    {
        NetworkVehicle net = collider.GetComponent<NetworkVehicle>();
        if(net!=null)
        {
            TypeManager typeManager = new TypeManager();
            PlayerController player = net.GetComponent<PlayerController>();
            int damage = Mathf.CeilToInt(player.defense - attack*typeManager.EffectValue(attackType,player.defenseType));
            if (damage >= 0) net.health -= damage;
        }
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
