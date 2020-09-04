using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {

        DontDestroyOnLoad(gameObject);
        source = GetComponent<AudioSource>();
        source.Play();
        source.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!source.isPlaying)
            source.PlayScheduled(250);
    }
}
