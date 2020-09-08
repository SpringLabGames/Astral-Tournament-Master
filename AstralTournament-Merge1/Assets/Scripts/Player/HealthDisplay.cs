using Prototype.NetworkLobby;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    private CustomLobby lobby;
    public static event Action OnMatchWon;

    [Header("References")]
    [SerializeField] private Health health = null;
    [SerializeField] private Image healthUI = null;

    public delegate void MatchWonDelegate(NetworkInstanceId netID);
    [SyncEvent]
    public event MatchWonDelegate EventMatchWon;


    private void OnEnable()
    {
        health.EventHealthChanged += HandleHealthChanged;
        health.EventPlayerHasDied += HandlePlayerHasDied;
    }

    private void HandlePlayerHasDied(int currentHealth,  NetworkInstanceId iD)
    {
        print("A player has died!");
        print("ID PLAYER: " + iD);

        for  (int i =0; i<lobby.playersInGame.Count; i++)
        {

            GameObject player = lobby.playersInGame[i];
            if (player.GetComponent<NetworkIdentity>().netId.Equals(iD))
            {
                lobby.playersInGame.Remove(player);
                print("Player removed");

            }
                
        }

        if(lobby.playersInGame.Count == 1)
        {
            EventMatchWon?.Invoke(lobby.playersInGame[0].GetComponent<NetworkIdentity>().netId);
        }
    }

    private void OnDisable()
    {
        health.EventHealthChanged -= HandleHealthChanged;
        health.EventPlayerHasDied -= HandlePlayerHasDied;
    }
    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        healthUI.fillAmount = (float)currentHealth / maxHealth;
    }

    private void Start()
    {
        lobby = GameObject.Find("LobbyManager").GetComponent<CustomLobby>();
    }
    private void Update()
    {
        //lobby = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        lobby.playersInGame = GameObject.FindGameObjectsWithTag("Player").ToList();
    }

}
