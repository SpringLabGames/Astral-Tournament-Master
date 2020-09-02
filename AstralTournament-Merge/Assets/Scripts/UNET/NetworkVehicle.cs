using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkVehicle : NetworkBehaviour
{
    public int cannon;
    public int armor;
    public int engine;
    public int wheel;

    public float drag;

    public Vector3 velocity;

    //public Vehicle vehicle;

    private GameObject board;
    public static bool changed;
    private bool done;

    public int health;
    public int maxHealth;
    public List<PowerUp> powerUps;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3(0, 0, 0);
        drag = 0.02f;
        powerUps = new List<PowerUp>();
    }

    public void AddPowerUp(PowerUp powerUp)
    {
        powerUps.Add(powerUp);
    }

    public void UsePowerUp(int index)
    {
        PowerUp powerUp = powerUps[index];
        powerUps.RemoveAt(index);
    }

    public bool IsPowerUpFull()
    {
        return powerUps.Count == 4;
    }

    


}
