using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStationInteractable : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private GameObject _outline;
    [SerializeField] private GameObject station;

    private bool stationBroken;

    void Start()
    {
        _player.StationInteract += OpenStation;
        _player.LeftStation += CloseStation;
    }
    void Update()
    {
        
    }

    void OpenStation()
    {
        station.SetActive(true);
    }

    void CloseStation()
    {
        station.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
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
