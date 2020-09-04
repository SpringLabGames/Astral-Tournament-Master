
using Prototype.NetworkLobby;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupLocalPlayer : NetworkBehaviour
{
    private GameObject player;
    private Global global;
    public NetworkVehicle net;

    public int cannon;
    public int armor;
    public int engine;
    public int wheel;

    public float speed;
    public float acceleration;
    public float attack;
    public float defense;
    public float maneuverability;

    public float drag;

    public Vector3 velocity;

    public LobbyManager netManager;

    void Start()
    {
        netManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        print("CREAZIONE PLAYER");
        velocity = new Vector3(0, 0, 0);
        drag = 0.02f;
        global = Global.Instance;
        net = GetComponent<NetworkVehicle>();
        print(net);
        if (isLocalPlayer)
        {
            
            //player = GameObject.Find("Player");
            SetComponents();
            //print("COMANDO ESEGUITO");
        }
        //else create();
        
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            //print("Attendo movimento");
            MoveCommands();
        }
    }

    private void SetComponents()
    {
        print("CANNON CANNON: " + netManager.spawnPrefabs[net.cannon]);
        this.cannon = net.cannon;
        this.armor = net.armor;
        this.engine = net.engine;
        this.wheel = net.wheel;
        CmdSetComponents(cannon,armor, engine, wheel);
    }

    [Command]
    public void CmdSetComponents(int cannon, int armor, int engine, int wheel)
    {
       
        if(isServer)
        {
            net.cannon = this.cannon = cannon;
            net.armor = this.armor = armor;
            net.engine = this.engine = engine;
            net.wheel = this.wheel = wheel;
            print("STO PER CREARE IL VEICOLO");
            create();
            print("STO PER FARE RPC");
            RpcSetComponents(cannon, armor, engine, wheel);
        }
        
    }

    [ClientRpc]
    public void RpcSetComponents(int cannon, int armor,int engine, int wheel)
    {
       if(isClient)
        {
            net.cannon = this.cannon = cannon;
            net.armor = this.armor = armor;
            net.engine = this.engine = engine;
            net.wheel = this.wheel = wheel;
            create();
            ClientScene.RegisterPrefab(netManager.spawnPrefabs[0]);
        }
    }

    private void create()
    {
        List<GameObject> prefab = netManager.spawnPrefabs;
        //print("WHEEL: " + prefab[wheel]);
        Dictionary<string, VehicleComponent> set = new Dictionary<string, VehicleComponent>();
        set.Add("wheel",prefab[wheel].GetComponent<VehicleComponent>());
        set.Add("cannon", prefab[cannon].GetComponent<VehicleComponent>());
        set.Add("armor", prefab[armor].GetComponent<VehicleComponent>());
        set.Add("engine", prefab[engine].GetComponent<VehicleComponent>());
        setStats(set);
        GameObject vehicle=build(set);
        vehicle.transform.SetParent(transform);
    }

    private GameObject build(Dictionary<string, VehicleComponent> set) //assemblaggio effettivo
    {
        //Destroy(board);
        //global.removeVehicle();
        GameObject board = createObject(PrimitiveType.Cube, "Board", 5, 1, 7); //Creazione della board
        for (int i = 0; i < 4; i++) //crea 4 ruote
        {
            VehicleComponent wheel = createWheel(board, set["wheel"], i);
            wheel.transform.SetParent(board.transform);
        }
        VehicleComponent engine = createEngine(board, set["engine"]);
        engine.transform.SetParent(board.transform);
        VehicleComponent armor = createArmor(set["armor"]);
        armor.transform.SetParent(board.transform);
        VehicleComponent cannon = createCannon(board, set["cannon"]);
        cannon.transform.SetParent(board.transform);
        return board;
    }

    private VehicleComponent createCannon(GameObject board, VehicleComponent vehicleComponent)
    {
        VehicleComponent cannonInstance = copyObject(vehicleComponent);
        cannonInstance.transform.position = new Vector3(0, 0, 0);
        cannonInstance.transform.localScale = new Vector3(1, 1, 1);
        cannonInstance.transform.localScale = 2 * cannonInstance.transform.localScale;
        float y = 3.5f * board.transform.localScale.y + 2.5f;
        cannonInstance.transform.Translate(0, y, 0);
        return cannonInstance;
    }

    private VehicleComponent createArmor(VehicleComponent vehicleComponent)
    {
        VehicleComponent armorInstance = copyObject(vehicleComponent);
        armorInstance.transform.position = new Vector3(0, 0, 0);
        armorInstance.transform.localScale = new Vector3(6, 4, 6);
        armorInstance.transform.Translate(0, 0.5f + armorInstance.transform.localScale.y, 0);
        armorInstance.transform.localScale = armorInstance.transform.localScale / 4;
        armorInstance.transform.Rotate(0, 180, 0);
        return armorInstance;
    }

    private VehicleComponent createEngine(GameObject board, VehicleComponent vehicleComponent)
    {
        VehicleComponent engineInstance = copyObject(vehicleComponent);
        engineInstance.transform.position = new Vector3(0, 0, 0);
        engineInstance.transform.localScale = new Vector3(2, 2, 2);
        engineInstance.transform.Translate(0, engineInstance.transform.localScale.y / 2, -board.transform.localScale.z / 2);
        return engineInstance;
    }

    private VehicleComponent createWheel(GameObject board, VehicleComponent component, int index)
    {
        VehicleComponent wheelInstance = copyObject(component);
        wheelInstance.transform.position = new Vector3(0, 0, 0);
        wheelInstance.transform.rotation = Quaternion.Euler(90, 0, 90);
        Vector3 pos = wheelInstance.transform.position;
        wheelInstance.transform.position = setupPos(board, wheelInstance, pos, index);
        pos.x += board.transform.position.x;
        pos.z += board.transform.position.z;
        return wheelInstance;
    }

    private Vector3 setupPos(GameObject board, VehicleComponent game, Vector3 pos, int i) // setta la posizione delle route
    {
        float x = board.transform.localScale.x / 2 + game.transform.localScale.y;
        float z = board.transform.localScale.z / 2 + game.transform.localScale.y;
        if (i == 0)
        {
            pos.x += x;
            pos.z += z;
        }
        else if (i == 1)
        {
            pos.x -= x;
            pos.z += z;
        }
        else if (i == 2)
        {
            pos.x += x;
            pos.z -= z;
        }
        else if (i == 3)
        {
            pos.x -= x;
            pos.z -= z;
        }
        return pos;
    }

    private VehicleComponent copyObject(VehicleComponent component) // copia l'oggetto dato
    {
        return Instantiate(component) as VehicleComponent;
    }

    private GameObject createObject(PrimitiveType type, string name, int x, int y, int z)
    {
        GameObject game = GameObject.CreatePrimitive(type);
        game.name = name;
        game.transform.localScale = new Vector3(x, y, z);
        return game;
    }

    private void setStats(Dictionary<string,VehicleComponent> set) //aggiorna le statistiche
    {
        if(set.Count==4)
        {
            foreach(VehicleComponent comp in set.Values)
            {
                print("COMP: " + comp);
            }
            /*for (int i = 0; i < netManager.spawnPrefabs.Count; i++)
                print("SPAWN: "+netManager.spawnPrefabs[i]);*/
            /*attack=net.attack = set["cannon"].values[0];
            defense=net.defense = set["armor"].values[0];
            speed=net.speed = set["engine"].values[0];
            acceleration=net.acceleration = set["engine"].values[1];
            maneuverability = net.maneuverability = set["armor"].values[1] + set["wheel"].values[0];*/
        }
    }

    private void MoveCommands()
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
        transform.Rotate(0, angle, 0);
    }
}
