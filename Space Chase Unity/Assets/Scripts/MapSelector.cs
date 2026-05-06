using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelector : MonoBehaviour
{
    [SerializeField] private GameObject b_Engine;
    [SerializeField] private GameObject b_Comms;
    [SerializeField] private GameObject b_Weapons;
    [SerializeField] private GameObject b_Bridge;
    [SerializeField] private GameObject b_Shields;

    public delegate void EmptyDelegate();
    public event EmptyDelegate endMapSelection;

    public enum RoomState
    {
        comms, engine, weapons, bridge, shields, all
    }
    public RoomState _currentMap;
    public List<int> roomsToRepair;
    [SerializeField] private int buttonPresses;

    void Start()
    {

    }

    private void DeactivateButtons()
    {
        b_Engine.SetActive(false);
        b_Comms.SetActive(false);
        b_Weapons.SetActive(false);
        b_Bridge.SetActive(false);
        b_Shields.SetActive(false);
    }
    private void ActivateAll()
    {
        b_Engine.SetActive(true);
        b_Comms.SetActive(true);
        b_Weapons.SetActive(true);
        b_Bridge.SetActive(true);
        b_Shields.SetActive(true);
    }

    void Update()
    {
        if(roomsToRepair.Count == buttonPresses) // close map selector and execute confirm
        {
            endMapSelection?.Invoke();
            roomsToRepair.Clear();
            buttonPresses = 5;
        }
    }

    public void AllowPresses(int allowed)
    {
        buttonPresses = allowed;
    }

    public void UpdateMapState(int id)
    {
        DeactivateButtons();
        switch(id)
        {
            case 0: _currentMap = RoomState.comms; break;
            case 1: _currentMap = RoomState.engine; break;
            case 2: _currentMap = RoomState.weapons; break;
            case 3: _currentMap = RoomState.bridge; break;
            case 4: _currentMap = RoomState.shields; break;
            case 5: _currentMap = RoomState.all; break;
        }
        if(_currentMap != RoomState.all)
        {
            AllowMapButtons();
        } else
        {
            ActivateAll();
        }
    }

    private void AllowMapButtons()
    {
        switch(_currentMap)
        {
            case RoomState.comms: b_Comms.SetActive(true); break;
            case RoomState.engine: b_Engine.SetActive(true); break;
            case RoomState.weapons: b_Weapons.SetActive(true); break;
            case RoomState.bridge: b_Bridge.SetActive(true); break;
            case RoomState.shields: b_Shields.SetActive(true); break;
        }
    }

    public void AddRoomToList(int id) // called with button
    {
        roomsToRepair.Add(id);
    }
}
