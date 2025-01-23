using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    private Renderer pickUpRenderer;

    private void Start() {
        pickUpRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update() {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);

        // Adding a random colour change to the PickUp objects
        pickUpRenderer.material.color = new Color(Random.value, Random.value, Random.value);

    }
}
