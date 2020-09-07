using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private Global global;
    private Text health;
    private List<RawImage> powerUps;
    private RawImage status;
    public NetworkVehicle player;
    // Start is called before the first frame update
    void Start()
    {
        global = Global.Instance;
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
        player = global.player.GetComponent<NetworkVehicle>();
        if(player!=null)
        {
            health.text = player.health + "/" + player.maxHealth;
            int i = 0;
            for (i = 0; i < player.powerUps.Count; i++)
            {
                powerUps[i].texture = player.powerUps[i].GetComponent<Renderer>().sharedMaterial.mainTexture;
            }
            for (; i < 4; i++)
            {
                powerUps[i].texture = null;
            }
            status.texture = player.status;
            Color color = status.color;
            if (player.status != null)
                color.a = 1;
            else color.a = 0;
            status.color = color;
        }
    }
}
