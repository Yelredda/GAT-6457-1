Module 0

Main scene: MiniGame

The project is an implementation of the Unity roll-a-ball tutorial found at the following link https://learn.unity.com/project/roll-a-ball.

The basic idea is that the player (the blue ball) will need to collect 8 items scattered around the stage while being chased by the enemy (the red cube). If the player manages to collect all 8 items then the game ends and "You Win!" is displayed. If the player is caught by the enemy then the game ends and "You Lose!" is displayed. 

Changes made to the default tutorial

1. Freezing net mechanic (press SPACE)

I added a mechanism to the game that allows the user to deploy a one-time, temporary "freezing net" that stops the enemy's movement for 5 seconds if they make contact with the net. There were a variety of changes made to implment this.

1.1 - Changes to the scene

First, I created a cube (XYZ dimensions: 4 x 0.5 x 4) to represent the net. I added a box collider to the object with "Is Trigger" selected. Note: this also required adding a RigidBody to the EnemyBody object in order for the collision to work as expected.   

1.2 - Creation of NetController.cs

I also created a class (NetController.cs, code below) to handle the net's freezing and unfreezing of the enemy upon contact.

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using TMPro;
	using UnityEngine;

	public class NetController : MonoBehaviour {

		private EnemyMovement enemyMovement;

		void OnTriggerEnter(Collider other) { 
			if (other.CompareTag("Enemy")) {
				enemyMovement = other.transform.parent.GetComponent<EnemyMovement>();
				Debug.Log("Freeze Enemy!");
				enemyMovement.Freeze();
			}
		}

		private void OnDestroy() {
			if (enemyMovement != null) {
				enemyMovement.UnFreeze();
			}
		}
	}

This also required implementing Freeze() and UnFreeze() functions on the original EnemyMovement class.

1.3 - Changes to EnemyMovement.cs

I created a boolean value (isNotFrozen) to keep track of whether the enemy should or should not be moving at any given moment. 

    // Added to facilitate the net stopping/starting the enemy
    // Additional check during Update()
    private bool isNotFrozen = true;

EnemyMovement.Update() was updated to evaluate whether the isNotFrozen value is true before setting the enemy destination. 
 
    // Update is called once per frame
    void Update() {
        if (player != null && isNotFrozen) {
            navMeshAgent.SetDestination(player.position);
        }
    }

Lastly, the functions EnemyMovement.Freeze() and EnemyMovement.UnFreeze() were created to stop enemy movement. They are called by the NetController.OnTriggerEnter() function upon collision. They stop or start the navMeshAgent and set isNotFrozen to true or false as needed. 

    // Added to facilitate the net stopping/starting the enemy
    public void Freeze() {
        navMeshAgent.isStopped = true;
        isNotFrozen = false;
    }

    public void UnFreeze() {
        navMeshAgent.isStopped = false;
        isNotFrozen = true;
    }

1.4 - Changes made to PlayerController.cs

I added three variables to the PlayerController class: public GameObject net, private bool isNetDeployed = false, and private float netCooldown = 5f.

net is the reference to the net object itself. isNetDeployed is a boolean variable meant to track whether the player has already used the net; it is intended to be a one-time tool so the value flips from false to true upon use, preventing its reuse. netCooldown is fed to the Destroy() function called to remove the net 5 seconds after an instance of the prefab has been instantiated. 

HUD now displays "Net Deployed: " with a true/false value indicating whether it is available. It starts as false and then flips once the player uses the net. 
	
This required adding a line to the PlayerController.SetCountText() function: 

    void SetCountText() {
        countText.text = "Yelredda \n" + "Net Deployed: " + isNetDeployed + "\nCount: " + count.ToString();

        if (count >= 8) {
            winTextObject.SetActive(true);
            Destroy(gameObject);
        }
    }

The actual creation of the net object is handled in the PlayerController.FixedUpdate() function: 

        if (Input.GetKeyDown(KeyCode.Space) && !isNetDeployed) {
            isNetDeployed = true;
            net = Instantiate(net, transform.position, Quaternion.identity);
            Destroy(net, netCooldown);
        }

This checks for whether the player pressed the space key and -- as long as the net has not been previously deployed -- it will set the isNetDeployed boolean to true to prevent the net from being created again, create an instance of the net prefab on the player's position, and then immediately call the method to destroy the net with a 5 second offset. 

2. Changes to the 3D graphics content of the game.

2.1 - PickUp objects 

I added logic to the Update method for the PickUp objects to make them change to a random colour with every update. The end result is a "sparkling" effect that makes the objects more noticeable. 

The change required a new Rendered object (pickUpRenderer), a new Rotator.Start() function that calls GetComponent to link the PickUp object's renderer, and then a call to the renderer to change the colour to three random RGB values.

    private void Start() {
        pickUpRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update() {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);

        // Adding a random colour change to the PickUp objects
        pickUpRenderer.material.color = new Color(Random.value, Random.value, Random.value);

    }

2.2 -  Static obstacles

The static obstacles also randomly change colour whenever the player collides into them. 

This required a modification to the PlayerController.OnCollisionEnter() function adding a new if... statement very similar to the logic used to randomly change the colour of the PickUp objects. This also required adding a new tag ("Building") to the objects to identify them. 

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            Destroy(gameObject);
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }

        if (collision.gameObject.CompareTag("Building")) {
            Renderer buildingRenderer = collision.gameObject.GetComponent<Renderer>();
            buildingRenderer.material.color = new Color(Random.value, Random.value, Random.value);
        }
         
    }

2.3 - DynamicBox animation 

I added an animation (DynamicBox) to the DynamicBox objects where their X and Y values scale from 1.1 and 3.1 to 1.0 and 3.0 respectively. It makes the objects appear as if they are pulsing. 