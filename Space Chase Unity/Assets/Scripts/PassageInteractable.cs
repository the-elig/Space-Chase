using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageInteractable : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private GameObject door;

    private bool doorClosed;

    void Start()
    {
        _player.Interact += OpenDoor;
        _player.LeftInteractZone += CloseDoor;
    }

    void Update()
    {
        
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
