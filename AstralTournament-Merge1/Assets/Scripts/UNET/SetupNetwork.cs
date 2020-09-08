using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SetupNetwork : MonoBehaviour
{
    private Global global;
    private CustomLobby netManager;
    // Start is called before the first frame update
    void Start()
    {
        global = Global.Instance;
        GameObject prefab = global.networkVehicle;
        //prefab.active = false;
        //print(prefab);
        //prefab = Resources.Load<GameObject>("Prefabs/Empty");
        netManager = GameObject.Find("LobbyManager").GetComponent<CustomLobby>();
        netManager.gameObject.AddComponent<NetworkManagerHUD>();
        //netManager.playerPrefab = prefab;
        //ClientScene.RegisterPrefab(prefab);
        //netManager.spawnPrefabs.Insert(0, prefab);
        prefab.active = true;
        netManager.playerPrefab = prefab;
        print("PLAYERPREFAB: " + netManager.playerPrefab);
        print("ARENA: " + global.arena);
        netManager.onlineScene = global.arena;

    }

    /*private GameObject setupLocalPlayer()
    {
        Vehicle vehicle = global.GetVehicle();
        NetworkVehicle net = vehicle.createNetworkInstance();
        net.gameObject.AddComponent<SetupLocalPlayer>();
        ClientScene.RegisterPrefab(net.gameObject, NetworkHash128.Parse(net.name));
        return net.gameObject;
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }
}
