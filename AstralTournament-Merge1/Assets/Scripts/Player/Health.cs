using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{

    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damagePerPress = 10;
   
    [SyncVar]
    private int currentHealth;

    //defining the event to use
    public delegate void HealthChangedDelegate(int currentHealth, int maxHealth);
    //Use the event
    [SyncEvent]
    public event HealthChangedDelegate EventHealthChanged;

   
    public delegate void hasDiedDelegate(int currentHealth, NetworkInstanceId netID );
    [SyncEvent]
    public event hasDiedDelegate EventPlayerHasDied;

    #region Server

    [Server]
    private void setHealth(int value)
    {
        currentHealth = value;
        EventHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth == 0) EventPlayerHasDied?.Invoke(currentHealth, GetComponent<NetworkIdentity>().netId);
    }


    public override void OnStartServer()
    {
        setHealth(maxHealth);
    }


    [Command]
    public void CmdDealDamage()
    {
        setHealth(Mathf.Max(currentHealth - damagePerPress, 0));
    }

    [Command]
    public void CmdDealDamageWithAmount(int damageAmount)
    {
        setHealth(Mathf.Max(currentHealth - damageAmount, 0));
    }
    #endregion


    #region Client

    [ClientCallback]
    private void Update()
    {
        if (!isLocalPlayer) return;
        if (!Input.GetKeyDown(KeyCode.H)) return;

        CmdDealDamage();
    }
    #endregion

}


