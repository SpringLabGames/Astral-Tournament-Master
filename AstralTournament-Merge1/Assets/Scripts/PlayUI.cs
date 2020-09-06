using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayUI : MonoBehaviour
{
    private Text health;
    private List<RawImage> powerUps;
    private RawImage status;
    public NetworkVehicle player;
    // Start is called before the first frame update
    void Start()
    {
        //NetworkVehicle net = GameObject.Find("LocalVehicle").GetComponent<NetworkVehicle>();
        //global = Global.Instance;
        powerUps = new List<RawImage>();
        health = GameObject.Find("Canvas/HP").GetComponent<Text>();
        //health.text = player.health + "/" + player.maxHealth;
        for (int i = 0; i < 4; i++)
        {
            powerUps.Add(GameObject.Find("Canvas/Power Ups/Power Up " + i).GetComponent<RawImage>());
            Color color = powerUps[i].color;
            //color.a = 0;
            powerUps[i].color = color;
        }
        status = GameObject.Find("Canvas/Status").GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        NetworkVehicle net = GameObject.Find("LocalVehicle").GetComponent<NetworkVehicle>();
        if (net != null && net.isLocalPlayer)
        {
            health.text=net.health+"/"+net.maxHealth;
            int i = 0;
            for (i = 0; i < net.powerUps.Count; i++)
            {
                powerUps[i].texture = net.powerUps[i].GetComponent<Renderer>().sharedMaterial.mainTexture;
            }
            for (; i < 4; i++)
            {
                powerUps[i].texture = null;
            }
            status.texture = net.status;
            Color color = status.color;
            if (net.status != null)
                color.a = 1;
            else color.a = 0;
            status.color = color;
        }
    }
}
