using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
            globalRef.player = GetComponent<NetworkVehicle>();
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
                wheel.transform.localEulerAngles = new Vector3(0f, im.steer * maxTurn, 90f); //il figlio del wheelcollider è il mesh della ruota che deve ruotare

            }

            foreach (GameObject mesh in meshes)
            {
                mesh.transform.Rotate(0f, rb.velocity.magnitude * (transform.InverseTransformDirection(rb.velocity).z >= 0 ? 1 : -1) / (2 * Mathf.PI * .2f), 0f);
            }
        }
    }
}
