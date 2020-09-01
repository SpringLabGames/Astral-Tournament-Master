using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class PowerUp : MonoBehaviour
{
    public Texture image;

    protected abstract void OnUsePowerUp();

    public void use()
    {
        OnUsePowerUp();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
