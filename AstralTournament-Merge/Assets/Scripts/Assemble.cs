using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//NON PIù UTILIZZATA: la nuova classe è Vehicle
//Classe per assemblare il veicolo

public class Assemble : MonoBehaviour
{
    public List<Text> stats;

    public VehicleComponent cannon;
    public VehicleComponent armor;
    public VehicleComponent engine;
    public VehicleComponent wheel;

    private GameObject board;

    public bool changed;
    private bool first;
    private Global global;

    public float speed;
    public float acceleration;
    public float attack;
    public float defense;
    public float maneuverability;
    public bool blocked;

    //private GameObject astroMachine;

    // Start is called before the first frame update

    private VehicleComponent getInstanceObject(VehicleComponent game,float x, float y, float z)
    {
        VehicleComponent instance = instanceObject(game);
        instance.transform.localScale = new Vector3(x, y, z);
        return instance;
    }

    void Update()
    {
        if(first)
        {
            Select.changed = first = true;
        }
        //created = false;
        //if (armor != oldArmor && cannon != oldCannon && engine != oldEngine && wheel != oldWheel)
        if(Select.changed && !blocked)
        {
            //astroMachine = new GameObject();
            //GameObject wheels = new GameObject();
            //wheels.name = "Wheels";
            Destroy(board);
            board = createObject(PrimitiveType.Cube, "Board", 5, 1, 7);
            for (int i = 0; i < 4; i++)
            {
                /*GameObject wheel = createObject(PrimitiveType.Cylinder, "Wheel " + i, 2, 0.25f, 2);*/

                VehicleComponent wheelInstance = getInstanceObject(wheel, 2, 0.25f, 2);
                wheelInstance.transform.rotation = Quaternion.Euler(90, 0, 90);
               // wheelInstance.transform.localScale = new Vector3(2, 0.25f, 2);
                Vector3 pos = wheelInstance.transform.position;
                pos=setupPos(board, wheelInstance, pos, i);

                wheelInstance.transform.position = pos;

                pos.x += board.transform.position.x;
                pos.z += board.transform.position.z;
                wheelInstance.transform.SetParent(board.transform);

            }

            //GameObject engine = createObject(PrimitiveType.Cube,"Engine", 2,2,2);
            VehicleComponent engineInstance = getInstanceObject(engine, 2, 2, 2);
            //engineInstance.transform.localScale = new Vector3(2, 2, 2);
            engineInstance.transform.Translate(0, engineInstance.transform.localScale.y / 2, -board.transform.localScale.z / 2);
            //GameObject armor = createObject(PrimitiveType.Capsule, "Armor", 6f, 4, 6f);
            VehicleComponent armorInstance = getInstanceObject(armor, 6, 4, 6);
            //armorInstance.transform.localScale = new Vector3(6, 4, 6);
            armorInstance.transform.Translate(0, 0.5f + armorInstance.transform.localScale.y, 0);
            armorInstance.transform.localScale = armorInstance.transform.localScale / 4;
            armorInstance.transform.Rotate(0, 180,0);
            engineInstance.transform.SetParent(board.transform);
            armorInstance.transform.SetParent(board.transform);

            //GameObject cannon = getPrefab("Prefabs/Cannon");
            VehicleComponent cannonInstance = getInstanceObject(cannon, 1, 1, 1);
            cannon.transform.position = new Vector3(0, 0, 0);
            //cannonInstance.transform.localScale = new Vector3(1, 1, 1);
            cannonInstance.transform.localScale = 2 * cannon.transform.localScale;
           
            cannonInstance.transform.SetParent(board.transform);
            float y = 3.5f*board.transform.localScale.y + 2.5f;
            cannonInstance.transform.Translate(0, y, 0);



            board.transform.position = transform.position;
            board.transform.rotation = transform.rotation;
            board.transform.SetParent(transform);
            board.transform.localScale = 3 * board.transform.localScale;
            //created = true;
            //board.transform.localScale = transform.localScale;
            /*if (!changed)
            {
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/Resources/Prefabs/Astromachine.prefab");
                
            }*/
            setStats();
            Select.changed = false;
        }
       
        
    }

    private void setStats()
    {
        attack = cannon.values[0];
        statString(attack, 2);
        defense = armor.values[0];
        statString(defense,3);
        speed = engine.values[0];
        statString(speed,0);
        acceleration = engine.values[1];
        statString(acceleration,1);
        maneuverability = armor.values[1] + wheel.values[0];
        statString(maneuverability,4);
    }

    private void statString(float stat, int v)
    {
        stats[v].text = stat.ToString();
    }

    private void setStats(VehicleComponent game, int v)
    {
        float value = int.Parse(stats[v].text);
        value = game.values[0];
        stats[v].text = value.ToString();
    }

    private void setStats(VehicleComponent game, int v, int w)
    {
        float value = int.Parse(stats[v].text);
        value = game.values[0];
        stats[v].text = value.ToString();
        value = int.Parse(stats[w].text);
        value = game.values[1];
        stats[w].text = value.ToString();
    }

    public VehicleComponent instanceObject(VehicleComponent gameObject)
    {
        VehicleComponent game=Instantiate(gameObject) as VehicleComponent;
        game.transform.position = board.transform.position;
        game.transform.rotation = board.transform.rotation;
        //game.transform.localScale = board.transform.localScale.magnitude * game.transform.localScale;
        return game;
    }

    private Vector3 setupPos(GameObject board,VehicleComponent game,Vector3 pos, int i)
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

    private GameObject getPrefab(string path)
    {
        var prefab = Resources.Load(path) as GameObject;
        prefab.transform.position = new Vector3(0, 0, 0);
        return GameObject.Instantiate(prefab);
    }
    private GameObject createObject(PrimitiveType type,string name,float x, float y, float z)
    {
        GameObject game= GameObject.CreatePrimitive(type);
        game.transform.localScale = new Vector3(x,y,z);
        game.name = name;
        return game;
    }

    // Update is called once per frame
    void Start()
    {
        
        //Update();
        first = true;
        global = Global.Instance;
    }
}
