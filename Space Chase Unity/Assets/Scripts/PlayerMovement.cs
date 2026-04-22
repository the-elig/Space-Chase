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
        mapActive = false;
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

        if (Input.GetKey(KeyCode.E) && canInteract)
        {
            if (atStation)
            {
                StationInteract?.Invoke();
            }
            else
            {
                Interact?.Invoke();
            }
        }


        if (Input.GetKeyDown(KeyCode.Tab))
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
