using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Movement : NetworkBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public float angle;
    public float drag;
    public float speedLimit;

    private CharacterController controller;
    private Vector3 velocity;
    private Quaternion rotation;
    void Start()
    {
        velocity = new Vector3(0, 0, 0);
        rotation = Quaternion.Euler(0, 0, 0);
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        if (Input.GetKey(KeyCode.W))
        {
            velocity.x = getSpeed();
            
            //acc += 0.1f;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            velocity.x = -getSpeed();
            
            //acc -= 0.1f;
        }
        
        if (Input.GetKey(KeyCode.A) && velocity.normalized.x!=0)
        {
            transform.Rotate(0,-angle/10,0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0,angle/10,0);
        }
        if (Input.GetKey(KeyCode.M))
        {
            Application.Quit();
            print("Exit");
        }
        //if(acc!=0)
        //   acc = (acc >= 0) ? Mathf.Min(acc - 0.1f, 1) : Mathf.Max(acc + 0.1f, -1);
        //print(acc);
        /*if (!controller.isGrounded) velocity.y *= gravity();
        else
        {
            velocity.y = 7;
        }*/
        if (velocity.x>0) velocity.x = Mathf.Min(velocity.x, speedLimit);
        else velocity.x= Mathf.Max(velocity.x, -speedLimit);
        velocity *=drag;
        transform.Translate(velocity);
        //transform.Rotate(rotation.eulerAngles);


    }

    private float gravity()
    {
        return 9.81f * Time.deltaTime;
    }

    private float getSpeed()
    {
        return speed/10;// *10 * Time.deltaTime;
    }

    private float getAngle()
    {
        return angle * Time.deltaTime;
    }
}
