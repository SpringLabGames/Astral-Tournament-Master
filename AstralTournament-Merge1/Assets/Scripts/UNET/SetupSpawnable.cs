using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetupSpawnable : MonoBehaviour
{
    private CustomManager netManager;
    // Start is called before the first frame update
    void Start()
    {
        netManager = GameObject.Find("NetworkManager").GetComponent<CustomManager>();
        netManager.componentPrefabs= Resources.LoadAll<GameObject>("Prefabs/Components").ToList();
        netManager.powerUpPrefabs = Resources.LoadAll<GameObject>("Prefabs/Power Ups").ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
