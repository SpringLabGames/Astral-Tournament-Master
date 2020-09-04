﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AimingSystem : NetworkBehaviour
{
    public Transform tower;
    public Transform cannon;
    //public float towerSpeed;
    public float cannonSpeed;
    public GameObject crossHairGO;

    //private float towerAngle;
    private float cannonAngle;
    //private Vector3 target;

    private void Start()
    {
        crossHairGO = GameObject.Find("crosshair");
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            rotateTower();
            rotateCannon();
            moveCrosshair();
        }
    }

    void rotateTower()
    {

        //towerAngle += Input.GetAxis("Mouse X") * towerSpeed * -Time.deltaTime;
        //tower.localRotation = Quaternion.AngleAxis(-towerAngle, Vector3.up);

        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (ground.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            tower.LookAt(new Vector3(pointToLook.x, tower.position.y, pointToLook.z));
        }

    }

    void rotateCannon()
    {
        
        cannonAngle += Input.GetAxis("Mouse Y") * cannonSpeed * -Time.deltaTime;
        cannonAngle = Mathf.Clamp(cannonAngle, 80f, 100f);
        cannon.localRotation = Quaternion.AngleAxis(cannonAngle, Vector3.right);

    }

    void moveCrosshair()
    {
        
        Vector3 chPos = Input.mousePosition;
        chPos.x = Mathf.Clamp(chPos.x, 0, Screen.width);
        chPos.y = Mathf.Clamp(chPos.y, 0, Screen.height);
        crossHairGO.GetComponent<RawImage>().transform.position = chPos;

    }

}
