using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class AimingSystem : NetworkBehaviour
{
    public Transform tower;
    public Transform cannon;
    //public float towerSpeed;
    public float cannonSpeed;
    public GameObject crossHairGO;

    //private float towerAngle;
    private float cannonAngle;
    private Global global;
    //private Vector3 target;

    void Start()
    {
        if (isLocalPlayer)
        {
            crossHairGO = GameObject.Find("crosshair");
            //cannon = gameObject.transform.GetChild(1).GetChild(1).GetChild(0);
            //tower = gameObject.transform.GetChild(1).GetChild(1);
            global = Global.Instance;
            //cannon = global.player.transform.GetChild(1).GetChild(1).GetChild(0);
            //tower = global.player.transform.GetChild(1).GetChild(1);
            //Debug.Log(global.player.transform.childCount);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            string cannonName = getCannonName();
            //print(cannonName);
            /*GameObject cannonObj;
            GameObject towerObj;
            if (isServer)
            {
                cannonObj = GameObject.Find("LocalVehicle/Reference/" + cannonName + "(Clone)/CannonCane");
                towerObj = GameObject.Find("LocalVehicle/Reference/" + cannonName + "(Clone)");
            }
            else
            {
                //cannonObj = GameObject.Find("NetVehicle(Clone)/Reference/" + cannonName + "(Clone)/CannonCane");
                cannonObj = transform.GetChild(1).GetChild(1).GetChild(0).gameObject;
                //towerObj = GameObject.Find("NetVehicle(Clone)/Reference/" + cannonName + "(Clone)");
                towerObj = transform.GetChild(1).GetChild(1).gameObject;
            }
            Debug.Log(transform.GetChild(0) + "\n" + transform.GetChild(1));

            //cannonObj = transform.GetChild(1).GetChild(1).GetChild(0).gameObject;
            //towerObj = transform.GetChild(1).GetChild(1).gameObject;

            //if (cannonObj == null || towerObj == null) return;
            cannon = cannonObj.transform;
            tower = towerObj.transform;*/
            //Global.Instance.
            rotateTower();
            //rotateCannon();
            moveCrosshair();
        }
    }

    private string getCannonName()
    {
        CustomLobby lobby = GameObject.Find("LobbyManager").GetComponent<CustomLobby>();
        NetworkVehicle net = GetComponent<NetworkVehicle>();
        return lobby.componentPrefabs[net.cannon].name;
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
            if (tower != null)
            {
                tower.LookAt(new Vector3(pointToLook.x, tower.position.y, pointToLook.z));
                //cannon.LookAt(pointToLook,Vector3.up);
            }
        }

    }

    void rotateCannon()
    {
        if (cannon != null)
        {
            cannonAngle += Input.GetAxis("Mouse Y") * cannonSpeed * -Time.deltaTime;
            cannonAngle = Mathf.Clamp(cannonAngle, 70f, 110f);
            cannon.localRotation = Quaternion.AngleAxis(cannonAngle, Vector3.right);
        }

    }

    void moveCrosshair()
    {
        if (crossHairGO != null)
        {
            Vector3 chPos = Input.mousePosition;
            chPos.x = Mathf.Clamp(chPos.x, 0, Screen.width);
            chPos.y = Mathf.Clamp(chPos.y, 0, Screen.height);
            crossHairGO.GetComponent<RawImage>().transform.position = chPos;
        }

    }

}
