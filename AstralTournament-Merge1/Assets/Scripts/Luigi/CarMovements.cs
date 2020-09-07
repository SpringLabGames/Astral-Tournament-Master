﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(Rigidbody))]
public class CarMovements :  NetworkBehaviour
{
    public InputManager im;
    public List<WheelCollider> wheels;
    public List<WheelCollider> steeringWheels;
    public List<GameObject> meshes;
    public float torqueCoefficient = 200000f;
    public float maxTurn = 20f;
    public float brakeStrength;
    public Transform CM;
    public Rigidbody rb;

    private Global globalRef;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            //CM = transform;
            im = GetComponent<InputManager>();
            rb = GetComponent<Rigidbody>();
            globalRef = Global.Instance;
            globalRef.player = gameObject;
        }
        /*if(CM)
        {
            rb.centerOfMass = CM.position;
        }*/
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            foreach (WheelCollider wheel in wheels)
            {
                if (im.brake)
                {
                    wheel.brakeTorque = brakeStrength; // * Time.deltaTime;
                    wheel.motorTorque = 0f;
                }
                else
                {
                    wheel.motorTorque = torqueCoefficient * im.throttle;// * Time.deltaTime;
                                                                        //Debug.Log(im.throttle + "\n" + Time.deltaTime);
                    wheel.brakeTorque = 0f;
                }
            }

            foreach (WheelCollider wheel in steeringWheels)
            {
                wheel.steerAngle = maxTurn * im.steer;
                //float angle = 0;
                //if (wheel.name[wheel.name.Length - 1] == '1') angle = 180;
                wheel.transform.localEulerAngles = new Vector3(0f, im.steer * maxTurn, (Int32.Parse(wheel.name[wheel.name.Length - 1].ToString()) == 0 ? 0 : -180)); //il figlio del wheelcollider è il mesh della ruota che deve ruotare
                                                                                                                                                                        //La ruota con i = 3 viene ruotata in NewPlayerCotroller.
            }

            foreach (GameObject mesh in meshes)
            {
                //int a = 1;
                //if (mesh.name[mesh.name.Length - 1] == '2') a = -1;
                mesh.transform.Rotate(0f, rb.velocity.magnitude * (transform.InverseTransformDirection(rb.velocity).z >= 0 ? 1 : -1) / (2 * Mathf.PI * .2f), 0f);
            }

            //Recover from flipping car
            if (rb.transform.rotation.z < -.7f || rb.transform.rotation.z > .7f || rb.transform.rotation.x < -.7f || rb.transform.rotation.x > .7f)
            {
                rb.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
        }
    }
}
