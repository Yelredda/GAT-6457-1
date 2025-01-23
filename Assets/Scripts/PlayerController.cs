using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour {

    public float speed = 0f;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    private Rigidbody rb;
    private float movementX, movementY;
    private int count;

    // Created mechanic to create a net once that traps the enemy
    public GameObject net;
    private bool isNetDeployed = false;
    private float netCooldown = 5f; // Destroy requires a float 

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
    }

    private void FixedUpdate() {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);

        if (Input.GetKeyDown(KeyCode.Space) && !isNetDeployed) {
            isNetDeployed = true;
            net = Instantiate(net, transform.position, Quaternion.identity);
            Destroy(net, netCooldown);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("PickUp")) {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    void OnMove(InputValue movementValue) {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText() {
        countText.text = "Theo Adderley \n" + "Net Deployed: " + isNetDeployed + "\nCount: " + count.ToString();

        if (count >= 8) {
            winTextObject.SetActive(true);
            Destroy(gameObject);
        }
    }

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
}
