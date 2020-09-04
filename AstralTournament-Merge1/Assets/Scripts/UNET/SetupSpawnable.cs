using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetupSpawnable : MonoBehaviour
{
    private LobbyManager netManager;
    // Start is called before the first frame update
    void Start()
    {
        netManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        netManager.componentPrefabs= Resources.LoadAll<GameObject>("Prefabs/Components").ToList();
        netManager.powerUpPrefabs = Resources.LoadAll<GameObject>("Prefabs/Power Ups").ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
