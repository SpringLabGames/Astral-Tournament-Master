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
    public Type defenseType;

    public float drag;

    public Vector3 velocity;

    //public Vehicle vehicle;

    private GameObject board;
    public static bool changed;
    private bool done;

    public int health;
    public int maxHealth;

    public List<PowerUp> powerUps;

    public Texture status;

    public GameObject spawnBullet;

    //public Status status;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3(0, 0, 0);
        drag = 0.02f;
        powerUps = new List<PowerUp>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            //Commands();
        }
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

    /*private void Commands()
    {
        if (Input.GetKey(KeyCode.W)) //va avanti
        {
            velocity.z += getVelocity(acceleration);
        }
        else if (Input.GetKey(KeyCode.S)) //va indietro
        {
            velocity.z -= getVelocity(acceleration);
        }
        else velocity *= (1 - drag);
        if (Input.GetKey(KeyCode.A)) //gira a sinistra (destra se è in retromarcia)
        {
            float angle = -getAngle(velocity.z); 
            CmdRotate(angle);
        }
        else if (Input.GetKey(KeyCode.D))//gira a destra (sinistra se in retromarcia)
        { 
            float angle = getAngle(velocity.z);
            CmdRotate(angle);
        }
        if (velocity.z > 0) velocity.z = Mathf.Min(velocity.z, speed);
        else velocity.z = Mathf.Max(velocity.z, -speed);
        CmdMove(velocity);
    }

    private float getAngle(float velocity) //calcolo dell'angolo di rotazione
    {
        float angle = maneuverability / 10;// * Time.deltaTime;
        return angle;
    }

    private float getVelocity(float acceleration) //calcolo velocità
    {
        return acceleration * 5 * Time.deltaTime;
    }

    [Command]
    public void CmdMove(Vector3 velocity)
    {
        transform.Translate(velocity);
        //velocity *= (1 - drag);
        RpcMove(velocity);
    }

    [ClientRpc]
    public void RpcMove(Vector3 position)
    {
        transform.Translate(velocity);
        //velocity *= (1 - drag);
    }

    [Command]
    public void CmdRotate(float angle)
    {
        transform.Rotate(0, angle, 0);
        RpcRotate(angle);
    }

    [ClientRpc]
    public void RpcRotate(float angle)
    {
        if (isServer)
            transform.Rotate(0,angle,0);
    }*/


}
