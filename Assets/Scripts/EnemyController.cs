﻿using UnityEngine;
//using Systems.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using C;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyController : StatefulMonoBehaviour<EnemyController> {
    /// <summary>
    /// This powerhouse of a script controls enemy behavior.
    /// It contains fields such as hp and is connected to the FSM and the Score Manager
    /// Some old approahes are commented out but remain here as reference.
    /// </summary>
	public Transform playerTransform;
	public List<Vector3> waypoints = new List<Vector3>();
    public ScoreManager myScoreManager;
    public GameObject bloodPrefab;
    public GameObject explosionPrefab;

    public int life = 5;
    public bool shouldPatrol = true;

    void Awake() {
		// fetch our waypoint positions so we have a purpose in life
        GameObject waypointRoot = GameObject.FindGameObjectWithTag(Tags.WAYPOINTS );
        Transform[] wayPoints = waypointRoot.GetComponentsInChildren<Transform>();

        // filter out the root objects position
        foreach ( Transform t in wayPoints ) {
            if (!t.Equals(waypointRoot.transform)) {
                waypoints.Add(t.position); //add new waypoints to the waypoints array
            }
		}

		fsm = new FSM<EnemyController>();
        if (shouldPatrol) { //a bool toggle which enables automatic patrolling
            fsm.Configure(this, new EnemyPatrol());
        }
    }

    private void Start() { 
        //playerTransform = GameObject.FindWithTag("PlayerObject").transform;
        playerTransform = FindClosestPlayer(gameObject).transform; //for finding the closest player transform
        myScoreManager = GameObject.FindGameObjectWithTag(Tags.SCOREMANAGER).GetComponent<ScoreManager>();
        //Debug.Log("Enemy believes the player is at " + playerTransform.position);
    }

    public void OnCollisionEnter(Collision o) { //hit by a bullet
        //Debug.Log("Enemy hit by something");
        if (o.gameObject.tag == "Bullet") { //If the object that triggered this collision is tagged "bullet"
            fsm.Configure(this, new EnemyChase()); //make the enemy chase the player

            life--;
            Debug.Log("Enemy hit by bullet and has " + life + " hp left");
            Instantiate(bloodPrefab, transform.position, Quaternion.identity);

            if (life < 1) { //when the enemy dies
                Debug.Log("Enemy should be dead");
                myScoreManager.AddToScore(1);
                AudioSource audio = GetComponent<AudioSource>();
                audio.Play();
                Instantiate(explosionPrefab, transform.position, Quaternion.identity); //new
                GameObject.Destroy(gameObject);  //Destroy(gameObject);
            }

            //flashRed();
        }
    }

    /*public void Update() {
        playerTransform = GameObject.FindWithTag("PlayerObject").transform;
        Debug.Log("I believe the player is at " + playerTransform.position);
    }*/

    
    public int GetLife() {
        return life;
    }

    public void StartChasing() { //used by the game controller for observer
        Debug.Log("Sure thing Boss");
        fsm.Configure(this, new EnemyChase());
    }

    public GameObject FindClosestPlayer(GameObject e) { //returns the game object of the closest player
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("PlayerObject");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = e.transform.position; //this enemy's position
        foreach (GameObject go in gos) {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude; //is squaring necessary here?
            if (curDistance < distance) {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    //removed because it was not working properly
    /*public void flashRed() {
        GetComponent<Renderer>().material.color = Color.red;
        Invoke("returnToNormalColor", 0.5f);
    }

    public void returnToNormalColor() { 
        GetComponent<Renderer>().material.color = Color.white;
    }*/
}