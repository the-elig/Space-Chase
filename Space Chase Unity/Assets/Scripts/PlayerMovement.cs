using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform playerTransform;

    private bool canInteract;

    public delegate void EmptyDelegate();
    public event EmptyDelegate Interact;
    public event EmptyDelegate LeftInteractZone;

    void Start()
    {
        canInteract = false;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            playerTransform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerTransform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerTransform.Translate(Vector3.down * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerTransform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E) && canInteract)
        {
            Interact?.Invoke();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        canInteract = true;
    }
    void OnTriggerExit2D(Collider2D col)
    {
        canInteract = false;
        if(col.gameObject.CompareTag("Passage"))
        {
            LeftInteractZone?.Invoke();
        }
    }
}
