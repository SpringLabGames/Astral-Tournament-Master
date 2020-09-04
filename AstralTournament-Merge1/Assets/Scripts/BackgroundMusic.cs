using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source.loop = true;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
       /* if (source.loop && !source.isPlaying)
        {
            print("play");
            source.Play();
        }
        else print("not play");*/
    }
}
