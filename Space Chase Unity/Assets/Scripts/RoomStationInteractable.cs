using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStationInteractable : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private GameObject station;

    private bool stationBroken;

    // Start is called before the first frame update
    void Start()
    {
        _player.StationInteract += OpenStation;
        _player.LeftStation += CloseStation;
    }

    // Update is called once per frame
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
}
