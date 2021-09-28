using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class GameManager : MonoBehaviour
{
    public GameObject cube;
    public GameObject player;
    public Transform spawner;
    public float spawnTime;

    private bool exit = false;
    private bool spawnedItem = true;
    private bool destroyItem = false;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyItem)
        {
            Destroy(player);
            destroyItem = false;
        }

        if (Input.GetMouseButtonDown(0) && spawnedItem == true)
        {
            exit = false;
            spawnedItem = false;
            player = Instantiate(cube, spawner);
            Thread spawn = new Thread(threadSpawnObject);
            spawn.Start();
        }
            
    }

    void threadSpawnObject()
    {
        Debug.LogWarning("Starting Thread!");
        System.DateTime starTime;
        while(!exit)
        {
            starTime = System.DateTime.UtcNow;
            while ((System.DateTime.UtcNow - starTime).Seconds < spawnTime)
            {
                
            }
            Debug.Log(spawnTime + " seconds passed");
            exit = true;
            spawnedItem = true;
            destroyItem = true;
        }
    }
}
