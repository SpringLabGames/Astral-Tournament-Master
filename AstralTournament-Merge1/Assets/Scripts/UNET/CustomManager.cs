using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class CustomManager : NetworkManager
{
    public static short playerMex = MsgType.Highest + 1;

    public static int playerIndex;
    public Global global;

    public List<GameObject> powerUpPrefabs;
    public List<GameObject> componentPrefabs;

    private void Start()
    {
        playerIndex = 0;
        global = Global.Instance;
    }

    public override void OnStartServer()
    {
        print("START SERVER");
        NetworkServer.RegisterHandler(playerMex, OnResponsePrefab);
        base.OnStartServer();
    }

    private void OnResponsePrefab(NetworkMessage netMsg)
    {
        print("RESPONSE");
        PlayerMessage message = netMsg.ReadMessage<PlayerMessage>();
        //playerPrefab = spawnPrefabs[message.prefabIndex];
        //playerPrefab.active = true;
        base.OnServerAddPlayer(netMsg.conn, message.controllerId);
    }

    private void OnRequestPrefab(NetworkMessage netMsg)
    {
        print("REQUEST");
        PlayerMessage message = new PlayerMessage();
        message.controllerId = netMsg.ReadMessage<PlayerMessage>().controllerId;
        message.prefabIndex = playerIndex;
        message.cannon = global.cannon;
        message.armor = global.armor;
        message.engine = global.engine;
        message.wheel = global.wheel;
        client.Send(playerMex, message);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        print("CLIENT CONNECT");
        client.RegisterHandler(playerMex, OnRequestPrefab);
        base.OnClientConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        print("SERVER ADD PLAYER");
        PlayerMessage message = new PlayerMessage();
        message.controllerId = playerControllerId;
        message.cannon = global.cannon;
        message.armor = global.armor;
        message.engine = global.engine;
        message.wheel = global.wheel;
        NetworkServer.SendToClient(conn.connectionId, playerMex, message);
        //NetworkServer.AddPlayerForConnection(conn, playerPrefab, playerControllerId);
    }

    /*public override void OnServerReady(NetworkConnection conn)
    {
        //base.OnServerReady(conn);
        if(ClientScene.Ready(conn))
        {
            PlayerMessage message = new PlayerMessage();
            message.prefabIndex = 0;
            NetworkServer.SendToClient(conn.connectionId, playerMex, message);
        }
    }*/
}