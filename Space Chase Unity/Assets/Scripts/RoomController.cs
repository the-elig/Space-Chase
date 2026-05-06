using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{

    [SerializeField] private GameController gameController;
    [SerializeField] private RoomStationInteractable station;
    [SerializeField] public GameObject warning;
    [SerializeField] public ParticleSystem smoke;
    public bool damaged;
    [SerializeField] public int id;
    public List<PassageController> _adjacentPassages;

    void Awake()
{
    gameController.DamageRoom += DamageRoom;
    damaged = false;
    smoke.GetComponentInParent<Renderer>().enabled = false;
    }

    void Update()
    {
        if (damaged)
        {
            smoke.GetComponentInParent<Renderer>().enabled = true;
            //Debug.Log("smoki");
        }
        else
        {
            smoke.Stop();
            smoke.GetComponentInParent<Renderer>().enabled = false;
        }
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
