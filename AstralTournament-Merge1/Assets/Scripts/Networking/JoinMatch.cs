using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class JoinMatch : MonoBehaviour
{
    public InputField textbox;

    private Global global;
    // Start is called before the first frame update
    void Start()
    {
        global = Global.Instance;
        Button button = GetComponent<Button>();
        button.onClick.AddListener(onClick);
    }

    private void onClick()
    {
        try
        {

            /*if(!global.netManager.IsClientConnected())
            {
                //string text = textbox.text;
                //string match = "[0-9]{1,3}(\.?)[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}";
                //global.netManager.networkAddress = text;
                //global.netManager.networkPort = 7778;
                global.netManager.StartClient();
                print(string.Format("Client binding on {0}:{1}", text, global.netManager.networkAddress));
            }*/
            //global.matchMaker.FindInternetMatch(textbox.text);
        }
        catch(Exception e)
        {
            print("Exception: " + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
