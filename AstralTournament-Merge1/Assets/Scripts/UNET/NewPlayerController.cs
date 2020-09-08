using Prototype.NetworkLobby;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NewPlayerController : NetworkBehaviour
{
    private CustomLobby netManager;
    private Global global;

    [SyncVar] public int cannon; //Componenti
    [SyncVar] public int armor;
    [SyncVar] public int engine;
    [SyncVar] public int wheel;
    [SyncVar] public Type type;

    //public GameObject vehiclePrefab;

    public float speed; //Statistiche
    public float acceleration;
    public float attack;
    public float defense;
    public float maneuverability;

    private List<WheelCollider> wheelColliders;
    private List<WheelCollider> steerWheels;
    private List<GameObject> wheels;

    private bool mouseButtonPressed;

    private Vector3 velocity; //Variabili per movimento
    private float drag;

    public Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        print("START NEW PLAYER CONTROLLER");
        wheelColliders = new List<WheelCollider>();
        steerWheels = new List<WheelCollider>();
        wheels = new List<GameObject>();
        global = Global.Instance;
        //global.player = GetComponent<NetworkVehicle>().gameObject;
        initMovementThings();
        NetworkVehicle vehicle = GetComponent<NetworkVehicle>();
        netManager = GameObject.Find("LobbyManager").GetComponent<CustomLobby>();
        print("NET MANAGER: " + netManager);
        if (isLocalPlayer)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject game = transform.GetChild(i).gameObject;
                if (game.GetComponent<Camera>())
                    game.SetActive(true);
            }
            //GetComponent<InputManager>().enabled = true;
            //GetComponent<CarMovements>().enabled = true;
            //GetComponent<AimingSystem>().enabled = true;
            name = "LocalVehicle";
            print("LOCAL CLIENT!");
            NetworkVehicle net = GetComponent<NetworkVehicle>();
            print("ARMOR: " + armor);
            CmdSetComponents(net.cannon, net.armor, net.engine, net.wheel, net.defenseType);
            global.player = net.gameObject;

        }
        else if (vehicle.cannon != 0 && vehicle.armor != 0 && vehicle.engine!=0 && vehicle.wheel!=0)
        {
            createVehicle();
        }
        
        
        mouseButtonPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        List<UnityEngine.Object> objs = GameObject.FindObjectsOfTypeAll(typeof(GameObject)).ToList();
        foreach(UnityEngine.Object obj in objs)
        {
            /*GameObject game = obj as GameObject;
            if(game.GetComponent<NetworkIdentity>()!=null)
            {
                print(string.Format("NETID OF {0}: {1}", game.name,game.GetComponent<NetworkIdentity>().netId.Value));
            }*/
        }
        if (isLocalPlayer)
        {
            MoveCommands();
            PowerUpCommands();
            ShootCommands();
            //Recover from flipping car
            if (rb.transform.rotation.z < -.7f || rb.transform.rotation.z > .7f || rb.transform.rotation.x < -.7f || rb.transform.rotation.x > .7f)
            {
                rb.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
        }
        else if(isServer && !isClient)
        {
            GameObject canvas = GameObject.Find("Canvas");
            if(canvas!=null) canvas.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("TRIGGERED");
        if (isLocalPlayer)
        {
            BoxPowerUp box = other.GetComponent<BoxPowerUp>();
            if (box != null)
            {
                int powerUpIndex = GetRandomPowerUpIndex();
                print(powerUpIndex);
                CmdTakePowerUp(other.gameObject, powerUpIndex);
            }
        }
    }


    private int GetRandomPowerUpIndex()
    {
        return UnityEngine.Random.Range(0, netManager.powerUpPrefabs.Count);
    }

    //Comandi e RPC
    [Command]
    public void CmdTakePowerUp(GameObject collider, int powerUpIndex)
    {
        if (!isClient)
        {
            PowerUp powerUp = netManager.powerUpPrefabs[powerUpIndex].GetComponent<PowerUp>();
            NetworkVehicle net = GetComponent<NetworkVehicle>();
            if (!net.IsPowerUpFull()) net.AddPowerUp(powerUp);
        }
        RpcTakePowerUp(collider, powerUpIndex);
        Destroy(collider);
    }

    [ClientRpc]
    public void RpcTakePowerUp(GameObject collider, int powerUpIndex)
    {
        PowerUp powerUp = netManager.powerUpPrefabs[powerUpIndex].GetComponent<PowerUp>();
        NetworkVehicle net = GetComponent<NetworkVehicle>();
        if (!net.IsPowerUpFull()) net.AddPowerUp(powerUp);
        Destroy(collider);
    }


    [Command]
    public void CmdSetComponents(int cannon, int armor, int engine, int wheel, Type type)
    {
        print("CMD SET COMPONENT");
        this.cannon = cannon;
        this.wheel = wheel;
        this.engine = engine;
        this.armor = armor;
        this.type = type;
        if (isLocalPlayer || !isClient) createVehicle();
        if (isLocalPlayer)
        {
            
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.y += 180;
            transform.rotation = Quaternion.Euler(rotation);

            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            Vector3 position = transform.position;
            position.y += 2.4f;
            position.z += 0.38f;
            camera.transform.position = position;
            camera.transform.SetParent(global.player.transform);
        }
        RpcSetComponents(cannon, armor, engine, wheel, type);
    }

    [ClientRpc]
    public void RpcSetComponents(int cannon, int armor, int engine, int wheel, Type type)
    {
        print("RPC SET COMPONENT");
        this.cannon = cannon;
        this.wheel = wheel;
        this.engine = engine;
        this.armor = armor;
        this.type = type;
        if (isLocalPlayer)
        {
            Vector3 rotation = transform.rotation.eulerAngles;
            //rotation.y += 180;
            transform.rotation = Quaternion.Euler(rotation);

            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            camera.transform.position = new Vector3(0, 2.4f, 0.38f);
            camera.transform.SetParent(global.player.transform);
        }
        createVehicle();
    }

    [Command]
    public void CmdRotate(float angle)
    {
        if (!isClient)
        {
            transform.Rotate(0, angle, 0);
        }
        RpcRotate(angle);
    }

    [ClientRpc]
    public void RpcRotate(float angle)
    {
        transform.Rotate(0, angle, 0);
    }

    [Command]
    public void CmdTranslate(Vector3 velocity)
    {
        if (!isClient)
        {
            transform.Translate(velocity);
        }
        RpcTranslate(velocity);
    }

    [ClientRpc]
    public void RpcTranslate(Vector3 velocity)
    {
        transform.Translate(velocity);
    }

    [Command]
    public void CmdUsePowerUp(int index, Vector3 force)
    {
        List<PowerUp> ups = GetComponent<NetworkVehicle>().powerUps;
        if (!isClient && ups.Count > 0)
        {
            print("CMD USE");
            print("INDEX - TOT: " + index + "-" + GetComponent<NetworkVehicle>().powerUps.Count);
            PowerUp powerUp = Instantiate<PowerUp>(GetComponent<NetworkVehicle>().powerUps[index]);
            GetComponent<NetworkVehicle>().powerUps.RemoveAt(index);
            NetworkVehicle net = GetComponent<NetworkVehicle>();
            powerUp.transform.position = net.spawnBullet.transform.position;
            CoroutineData data = new CoroutineData(powerUp, force);
            //StartCoroutine("UsePowerUp", data);
            UsePowerUp(data);

        }
        else print("UPS EMPTY");
        RpcUsePowerUp(index, force);
    }

    [ClientRpc]
    public void RpcUsePowerUp(int index, Vector3 force)
    {
        List<PowerUp> ups = GetComponent<NetworkVehicle>().powerUps;
        if(ups.Count>0)
        {
            print("RPC USE");
            NetworkVehicle net = GetComponent<NetworkVehicle>();
            print("INDEX - TOT: " + index + "-" + GetComponent<NetworkVehicle>().powerUps.Count);
            PowerUp powerUp = Instantiate<PowerUp>(GetComponent<NetworkVehicle>().powerUps[index]);
            powerUp.transform.position = net.spawnBullet.transform.position;
            CoroutineData data = new CoroutineData(powerUp, force);
            //StartCoroutine("UsePowerUp", data);
            UsePowerUp(data);
            GetComponent<NetworkVehicle>().powerUps.RemoveAt(index);
        }
        else print("UPS EMPTY");
    }

    [Command]
    public void CmdShootBullet(Vector3 trajectory)
    {
        if(!isClient)
        {
            Bullet bullet = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Bullet")).GetComponent<Bullet>();
            Vector3 position = GetComponent<NetworkVehicle>().spawnBullet.transform.position;
            bullet.transform.position = position;
            float cos = Vector3.Dot(Vector3.forward, trajectory.normalized);
            float sin = Vector3.Cross(Vector3.forward, trajectory.normalized).magnitude;
            double angle = Math.Atan2(cos, sin);
            //bullet.transform.Rotate(0, 0, Math.Round(angle));
            Rigidbody rigid = bullet.GetComponent<Rigidbody>();
            //bullet.speed *= GetComponent<Rigidbody>().velocity.magnitude;
            rigid.AddForce(bullet.speed * trajectory * 10);
            StartCoroutine("BulletTime");
        }
        RpcShootBullet(trajectory);
    }

    [ClientRpc]
    public void RpcShootBullet(Vector3 trajectory)
    {
        GameObject bullet = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Bullet"));
        bullet.GetComponent<Bullet>().attack = attack;
        /*Vector3 position = GetComponent<NetworkVehicle>().spawnBullet.transform.position;
        position.y -= 1;
        bullet.transform.position = position;*/
        //StartCoroutine("BulletTime");
        Vector3 position = GetComponent<NetworkVehicle>().spawnBullet.transform.position;
        bullet.transform.position = position;
        float cos = Vector3.Dot(Vector3.forward, trajectory.normalized);
        float sin = Vector3.Cross(Vector3.forward, trajectory.normalized).magnitude;
        double angle = Math.Atan2(cos, sin);
        //bullet.transform.Rotate(0, 0, Math.Round(angle));
        Rigidbody rigid = bullet.GetComponent<Rigidbody>();
        rigid.AddForce(bullet.GetComponent<Bullet>().speed * trajectory * 100);
        StartCoroutine("BulletTime",bullet);
    }

    //Funzioni per coroutines

    private void UsePowerUp(CoroutineData data)
    {
        //CoroutineData data = obj as CoroutineData;
        if(data!=null)
        {
            PowerUp powerUp = data.powerUp;
            Vector3 force = data.force;
            NetworkVehicle net = GetComponent<NetworkVehicle>();
            powerUp.use(net, force);
            //yield return null;
        }
    }

    private IEnumerator BulletTime(object obj)
    {
        GameObject bullet = obj as GameObject;
        yield return new WaitForSeconds(20);
        Destroy(bullet.gameObject);
    }

    //Funzioni supporto

    private void initMovementThings()
    {
        velocity = Vector3.zero;
        drag = 0.02f;
    }

    private void MoveCommands()
    {
        float dump = 1 - drag;
        if (Input.GetKey(KeyCode.W))
        {
            velocity.z += GetVelocity();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            velocity.z -= GetVelocity();
        }
        else velocity.z *= dump;
        if (Input.GetKey(KeyCode.A))
        {
            CmdRotate(-GetAngle());
        }
        else if (Input.GetKey(KeyCode.D))
        {
            CmdRotate(GetAngle());
        }
        velocity.z = ResizeVelocity(velocity.z);
        CmdTranslate(velocity);
    }

    private float GetVelocity()
    {
        return acceleration * Time.deltaTime;
    }

    private float GetAngle()
    {
        return maneuverability;
    }

    private float ResizeVelocity(float velocity)
    {
        return (velocity > 0) ? Mathf.Min(velocity, speed/2) : Mathf.Max(velocity, -speed/2);
    }

    private void createVehicle()
    {
        netManager = GameObject.Find("LobbyManager").GetComponent<CustomLobby>();
        List<GameObject> prefab = netManager.componentPrefabs;
        print("WHEEL: " + prefab[wheel]);
        Dictionary<string, VehicleComponent> set = new Dictionary<string, VehicleComponent>();
        set.Add("wheel", prefab[wheel].GetComponent<VehicleComponent>());
        set.Add("cannon", prefab[cannon].GetComponent<VehicleComponent>());
        set.Add("armor", prefab[armor].GetComponent<VehicleComponent>());
        set.Add("engine", prefab[engine].GetComponent<VehicleComponent>());
        setStats(set);
        GameObject vehicle = buildVehicle(set);
        vehicle.transform.SetParent(transform,false);
        print("CreateVehicle Succeeded!");
    }

    /*public GameObject buildVehicle()
    {
        GameObject vehicle = Instantiate(vehiclePrefab, transform);

        //global.player = vehicle.GetComponent<NetworkVehicle>();

        return vehicle;
    }*/
    public GameObject buildVehicle(Dictionary<string, VehicleComponent> set)
    {
        /*wheelColliders = new List<WheelCollider>();
        steerWheels = new List<WheelCollider>();
        wheels = new List<GameObject>();*/
        GameObject reference = new GameObject("Reference");
        GameObject board = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/TemplateComps/Board"), reference.transform);
        VehicleComponent cannon = Instantiate<VehicleComponent>(set["cannon"], reference.transform);

        //GetComponent<AimingSystem>().tower = cannon.transform;
        //GetComponent<AimingSystem>().cannon = cannon.transform.GetChild(0);

        GameObject spawn = new GameObject("SpawnBullet");
        GameObject cane = getCannonCane();
        print("CANE: " + cane);
        spawn.transform.SetParent(cane.transform);
        spawn.transform.position = new Vector3(cane.transform.position.x + 0.03f, cane.transform.position.y - 0.06f, cane.transform.position.z + 4.25f);
        GetComponent<NetworkVehicle>().spawnBullet = spawn;

        VehicleComponent armor = Instantiate<VehicleComponent>(set["armor"], reference.transform);
        VehicleComponent engine = Instantiate<VehicleComponent>(set["engine"], reference.transform);

        GameObject center = new GameObject("Center");
        center.transform.position = transform.position + board.transform.position + cannon.transform.position + armor.transform.position + engine.transform.position;
        for (int i = 0; i < 4; i++)
        {
            VehicleComponent wheel = buildWheel(set["wheel"], i, reference.transform);
            wheel.name = wheel.name.Substring(0, wheel.name.Length - 7)+i;
            /*wheels.Add(wheel.gameObject);
            wheelColliders.Add(wheel.GetComponent<WheelCollider>());
            if (i<=1) steerWheels.Add(wheel.GetComponent<WheelCollider>());*/
            center.transform.position += wheel.transform.position;

            /*WheelCollider collider = wheel.GetComponent<WheelCollider>();
            collider.radius = 0.63f;
            collider.center = new Vector3(0, 0, 0);*/
        }
        center.transform.position /= 9;
        center.transform.SetParent(transform);
        SetupCollider();
        /*CarMovements car = GetComponent<CarMovements>();
        car.wheels = wheelColliders;
        car.meshes = wheels;
        car.steeringWheels = steerWheels;
        car.CM = center.transform;*/
        return reference;

    }

    private GameObject getCannonCane()
    {

        CustomLobby lobby = GameObject.Find("LobbyManager").GetComponent<CustomLobby>();
        string cannonName = lobby.componentPrefabs[cannon].name+"(Clone)";
        print(cannonName);
        GameObject cane = GameObject.Find("Reference/" + cannonName + "/CannonCane");
        return cane;
    }

    private void SetupCollider()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        /*float sizeX = board.transform.localScale.x;
        float sizeY = board.transform.localScale.y + engine.transform.localScale.y + cannon.transform.localScale.y;
        float sizeZ = board.transform.localScale.z;*/
        collider.size = new Vector3(6.2940347f,2.506618f,6.808867f);
        collider.center = new Vector3(-0.05883098f,0.4f,-0.4374144f);
    }

    private VehicleComponent buildWheel(VehicleComponent wheel, int i, Transform parent)
    {

        VehicleComponent buildwheel = Instantiate<VehicleComponent>(wheel, parent);
        if (i==0)
        {
            //top left;
            buildwheel.transform.position = new Vector3(-2.463034f, 0.0188747f, 1.956946f);
        }
        else if (i == 1)
        {//top right;
            buildwheel.transform.position = new Vector3(2.463034f, 0.0188747f, 1.956946f);
            buildwheel.transform.Rotate(0, 0, -180);
        }
        else if (i == 2 )
        {//bottom left
            buildwheel.transform.position = new Vector3(-2.463034f, 0.0188747f, -2.8425256f);
        }
        else if (i == 3)
        { //bottom right
            buildwheel.transform.position = new Vector3(2.463034f, 0.0188747f, -2.8425256f);
            buildwheel.transform.Rotate(0, 0, -180);
        }

        return buildwheel;
    }


    private GameObject build(Dictionary<string, VehicleComponent> set) //assemblaggio effettivo
    {
        //Destroy(board);
        //global.removeVehicle();
        BoxCollider collider = GetComponent<BoxCollider>();
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
        float sizeX = board.transform.localScale.x;
        float sizeY = board.transform.localScale.y + engine.transform.localScale.y + cannon.transform.localScale.y;
        float sizeZ = board.transform.localScale.z;
        collider.size = new Vector3(7,7,7);
        collider.center = new Vector3(0, sizeY-sizeY/2, 0);
        setupRigidBody();
        return board;
    }

    private void setupRigidBody()
    {
        Rigidbody rigid = gameObject.GetComponent<Rigidbody>();
        rigid.freezeRotation = true;
    }

    private void setStats(Dictionary<string,VehicleComponent> set)
    {
        
        

        foreach(VehicleComponent comp in set.Values)
        {
            print("COMP: " + comp);
        }
        try
        {
            speed = set["engine"].values[0];
            acceleration = set["engine"].values[1];
            attack = set["cannon"].values[0];
            defense = set["armor"].values[0];
            maneuverability = (set["armor"].values[1] + set["wheel"].values[0]) / 2;
        }catch(ArgumentOutOfRangeException e)
        {
            //Prenditi il comp network vehicle
            //stampi i suoi indici
            NetworkVehicle netV = GetComponent<NetworkVehicle>();
            Debug.Log(netV.armor + " " + netV.engine + " " + netV.cannon + " " + netV.wheel);
        }
    }

    private VehicleComponent createCannon(GameObject board, VehicleComponent vehicleComponent)
    {
        
        VehicleComponent cannonInstance = copyObject(vehicleComponent);
        
        cannonInstance.transform.position = new Vector3(0, 0, 0);
        NetworkVehicle net = GetComponent<NetworkVehicle>();
        net.spawnBullet = new GameObject("SpawnBullet");
        //net.spawnBullet.AddComponent<Pointer>();
        net.spawnBullet.transform.position = new Vector3(0,1,-3);
        net.spawnBullet.transform.SetParent(cannonInstance.transform);
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
        print("COMPONENT: " + component);
        return Instantiate(component) as VehicleComponent;
    }

    private GameObject createObject(PrimitiveType type, string name, int x, int y, int z)
    {
        GameObject game = GameObject.CreatePrimitive(type);
        game.name = name;
        game.transform.localScale = new Vector3(x, y, z);
        return game;
    }

    private void PowerUpCommands()
    {
        NetworkVehicle net = GetComponent<NetworkVehicle>();
        int powerUpIndex = 0;
        if(Input.GetKeyDown(KeyCode.Alpha1) && net.powerUps.Count>=1) //Selezione power up: Tasti 1234 (quelli sopra WASD)
        {
            powerUpIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && net.powerUps.Count >= 2)
        {
            powerUpIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && net.powerUps.Count >= 3)
        {
            powerUpIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && net.powerUps.Count == 4)
        {
            powerUpIndex = 3;
        }
        if(Input.GetKeyDown(KeyCode.Mouse1) && net.powerUps.Count >= 1) //Il click destro del mouse permette di utilizzare il power up scelto;
        {
            Vector2 force = calcForce();
            CmdUsePowerUp(powerUpIndex,force);
        }
    }

    private Vector3 calcForce()
    {
        NetworkVehicle net = GetComponent<NetworkVehicle>();
        CustomLobby lobby = GameObject.Find("LobbyManager").GetComponent<CustomLobby>();
        GameObject cane = GameObject.Find("LocalVehicle/Reference/" + lobby.componentPrefabs[net.cannon].name + "(Clone)/CannonCane");
        Vector3 axe = net.spawnBullet.transform.position - cane.transform.position;
        return axe.normalized * Input.mousePosition.magnitude*5;
    }

    private Vector3 calcOrientationAxe()
    {
        NetworkVehicle net = GetComponent<NetworkVehicle>();
        Vector3 spawnPos = net.spawnBullet.transform.position;
        Vector3 netPos = net.transform.position;
        Vector3 axe = new Vector3(spawnPos.x-netPos.x,0,spawnPos.z - netPos.z);
        return axe;
    }

    private void ShootCommands()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 trajectory = calcForce().normalized;
            CmdShootBullet(trajectory);
        }
    }

}
