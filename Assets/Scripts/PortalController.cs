﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C;
using UnityEngine.Networking; //new

public class PortalController : MonoBehaviour {
    /// <summary>
    /// This is controller similar to the enemy controller and is used for the portal behavior
    /// It contains a lot of simple unity code.
    /// </summary>
    /// 
    public int life = 8; //a considerable amount
    public float respawnTime = 12.0f; //the time between enemy spawns (in seconds)
    public ScoreManager myScoreManager;
    public GameObject explosionPrefab;
    public GameObject spawnPrefab;
    public GameObject enemyPrefab;
    //public GameObject[] enemyPrefabs;

    // Use this for initialization
    void Start () {
        myScoreManager = GameObject.FindGameObjectWithTag(Tags.SCOREMANAGER).GetComponent<ScoreManager>();
        InvokeRepeating("SpawnEnemy", 1.0f, respawnTime); //maybe turn this into variables
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void SpawnEnemy() {
        //Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        Instantiate(spawnPrefab, transform.position, Quaternion.identity);
        var enemy = (GameObject)Instantiate(
        enemyPrefab,
        gameObject.transform.position,
        gameObject.transform.rotation);


        // Spawn the bullet on the Clients
        NetworkServer.Spawn(enemy);
    }

    public void OnCollisionEnter(Collision o) { //hit by a bullet
        if (o.gameObject.tag == "Bullet") { //If the object that triggered this collision is tagged "bullet"

            life--;
            Debug.Log("Portal hit by bullet and has " + life + " hp left");

            if (life < 1) { //when the portal is destroyed
                Debug.Log("A portal has been destroyed");
                myScoreManager.AddToScore(5);
                AudioSource audio = GetComponent<AudioSource>(); //play an explosion sound
                audio.Play();
                Instantiate(explosionPrefab, transform.position, Quaternion.identity); //create an explosion

                GameObject.Destroy(gameObject);
            }
        }
    }

}
