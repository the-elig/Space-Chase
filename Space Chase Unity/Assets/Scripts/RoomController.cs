using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{

    [SerializeField] private GameController gameController;
    [SerializeField] private RoomStationInteractable station;
    [SerializeField] private GameObject warning;
    private bool damaged;
    [SerializeField] private int id;
    public List<PassageController> _adjacentPassages;

    void Start()
    {
        gameController.damageRoom += DamageRoom;
        damaged = false;
    }

    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            gameController.UpdatePlayerLocation(id);
        }
    }

    private void DamageRoom(int room_id)
    {
        if (room_id == id)
        {
            damaged = true;
            warning.SetActive(true);
        }
    } 
}
