using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStationInteractable : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private GameObject station;
    [SerializeField] private CardPickerUI cardPicker;
    [SerializeField] private GameController gameController;
    [SerializeField] private string roomID;
    [SerializeField] private GameObject _outline;

    void Start()
    {
        _player.StationInteract += OpenStation;
        _player.LeftStation += CloseStation;

        if (gameController == null)
            gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
    }

    void OpenStation()
    {
        if (gameController._currentRoom.ToString().ToLower() != roomID.ToLower())
            return;

        if (cardPicker != null)
            cardPicker.OpenCardPicker();
        else if (station != null)
            station.SetActive(true);
    }

    void CloseStation()
{
    if (gameController._currentRoom.ToString().ToLower() != roomID.ToLower())
        return;

    if (cardPicker != null)
        cardPicker.CloseCardPicker();
    else if (station != null)
        station.SetActive(false);
}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (_outline != null)
                _outline.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (_outline != null)
                _outline.SetActive(false);
        }
    }
}