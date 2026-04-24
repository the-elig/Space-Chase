using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GameController controller;

    [SerializeField] private float speed;
    [SerializeField] private Transform playerTransform;

    private bool canInteract;
    private bool inPassage;
    private bool atStation;

    public delegate void EmptyDelegate();
    public event EmptyDelegate Interact;
    public event EmptyDelegate LeftInteractZone;

    public event EmptyDelegate LeftStation;
    public event EmptyDelegate StationInteract;

    void Start()
    {
        canInteract = false;
        inPassage = false;
        atStation = false;
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

        if (Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            if (atStation && controller._energy >= 1) //stations
            {
                StationInteract?.Invoke();
                canInteract = false;
            }
            else if (!atStation && controller._energy >= 1) //doors
            {
                Interact?.Invoke();
                canInteract = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        canInteract = true;
        if (col.gameObject.CompareTag("Passage"))
        {
            inPassage = true;
        }
        if (col.gameObject.CompareTag("Station"))
        {
            atStation = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        canInteract = false;
        if(col.gameObject.CompareTag("Passage"))
        {
            LeftInteractZone?.Invoke();
            inPassage = false;
        }
        if (col.gameObject.CompareTag("Station"))
        {
            LeftStation?.Invoke();
            atStation = false;
        }
    }
}
