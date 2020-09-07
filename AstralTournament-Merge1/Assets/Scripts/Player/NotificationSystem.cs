using Prototype.NetworkLobby;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;



public class NotificationSystem : MonoBehaviour
{
    public static short numNotify = MsgType.Highest + 2;

    NetworkClient client;

    [SerializeField] private TMP_Text notificationsText = null;


    public class Notification : MessageBase
    {
        public static short idMessage;
        public string content;
        public Notification()
        {

        }
    }

    private void Start()
    {
        LobbyManager lobby = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        client = lobby.client;
        if (!NetworkClient.active) { return; }
        print("register handler...");
        client.RegisterHandler(numNotify, OnNotification);


    }

    void OnNotification(NetworkMessage msg)
    {
        print("Notification Arrived!");
        Notification notificationMessage = msg.ReadMessage<Notification>();
        notificationsText.text = $"{notificationMessage.content}";
        StartCoroutine("HideMessage");
    }

    public IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(3);
        notificationsText.text = "";

    }


}
