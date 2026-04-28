using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageInteractable : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject _outline;
    [SerializeField] private GameObject _bandage;

    private bool doorClosed;
    private bool damaged;

    void Start()
    {
        damaged = false;
        _player.Interact += OpenDoor;
        _player.LeftInteractZone += CloseDoor;
        
    }

    void Update()
    {
        if(damaged)
        {
            _bandage.SetActive(true);
        } else
        {
            _bandage.SetActive(false);
        }
    }
    public void ToggleDamage(bool damage)
    {
        damaged = damage;
        Debug.Log("ouch");
        gameObject.tag = "DamagedPassage";
    }

    void OpenDoor()
    {
        doorClosed = false;
        door.SetActive(doorClosed);
    }

    void CloseDoor()
    {
        doorClosed = true;
        door.SetActive(doorClosed);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && !damaged)
        {
            _outline.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _outline.SetActive(false);
        }
    }
}
