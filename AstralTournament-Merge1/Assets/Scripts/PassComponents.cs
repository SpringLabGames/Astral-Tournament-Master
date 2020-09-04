using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassComponents : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Select> selectors;
    public Assemble astroMachine;


    void Start()
    {
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        astroMachine.cannon = selectors[0].getSelected();
        astroMachine.armor = selectors[1].getSelected();
        astroMachine.engine = selectors[2].getSelected();
        astroMachine.wheel = selectors[3].getSelected();
        //astroMachine.changed = Select.changed;
    }
}
