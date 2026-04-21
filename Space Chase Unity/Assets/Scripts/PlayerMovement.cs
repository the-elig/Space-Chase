using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private GameObject map;

    private bool canInteract;
    private bool mapActive;

    public delegate void EmptyDelegate();
    public event EmptyDelegate Interact;
    public event EmptyDelegate LeftInteractZone;

    void Start()
    {
        canInteract = false;
        mapActive = false;
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

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            //Key to open map
            if (mapActive == false)
            {
                map.SetActive(true);
                mapActive = true;
            }
            else
            {
                map.SetActive(false);
                mapActive = false;
            }
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
