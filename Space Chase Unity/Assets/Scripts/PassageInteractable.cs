using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageInteractable : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private GameObject door;

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

}
