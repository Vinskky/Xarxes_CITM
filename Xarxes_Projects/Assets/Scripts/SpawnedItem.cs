using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class SpawnedItem : MonoBehaviour
{
    public float couldown;
    private bool exit = false;

    // Start is called before the first frame update
    void Start()
    {
        Thread spawn = new Thread(threadSpawnObject);
        spawn.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void threadSpawnObject()
    {
        Debug.LogWarning("Starting Thread!");
        System.DateTime starTime;
        while (!exit)
        {
            starTime = System.DateTime.UtcNow;
            while ((System.DateTime.UtcNow - starTime).Seconds < couldown)
            {

            }
            Debug.Log(couldown + " seconds passed");
            exit = true;
        }
    }
}
