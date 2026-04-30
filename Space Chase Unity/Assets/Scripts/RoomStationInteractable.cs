using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStationInteractable : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private GameObject station;
    [SerializeField] private CardPickerUI cardPicker;
    [SerializeField] private CardUpgraderUI cardUpgrader;
    [SerializeField] private GameController gameController;
    [SerializeField] private string roomID;
    [SerializeField] private GameObject _outline;
    AudioSource m_MyAudioSource;

    void Start()
    {
        _player.StationInteract += OpenStation;
        _player.LeftStation += CloseStation;
        m_MyAudioSource = GetComponent<AudioSource>();
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

        bool isDamaged = gameController._damagedRooms.Exists(r =>
            r.ToLower() == roomID.ToLower());
        
        if (isDamaged)
        {
            if (station != null)
            {
                station.SetActive(true);
                RoomCardSlot slot = station.GetComponentInChildren<RoomCardSlot>();
                if (slot != null)
                    slot.UpdateStationMessage(true);
            }
        }
        else
        {
            if (cardPicker != null)
                cardPicker.OpenCardPicker(); //for engine and shields rn
            else if(cardUpgrader != null) {
                Debug.Log("card upgrader station");
                cardUpgrader.OpenCardUpgrader();
                return; }
            else if (station != null)
            {
                station.SetActive(true);
                m_MyAudioSource.Play();
                RoomCardSlot slot = station.GetComponentInChildren<RoomCardSlot>();
                if (slot != null) {
                    slot.UpdateStationMessage(false); } //updates damaged message to no longer appear
            }
        }
    }

    void CloseStation()
    {
        if (gameController._currentRoom.ToString().ToLower() != roomID.ToLower())
            return;

        bool isDamaged = gameController._damagedRooms.Exists(r =>
            r.ToLower() == roomID.ToLower());

        if (isDamaged)
        {
            if (station != null)
                station.SetActive(false);
        }
        else
        {
            if (cardPicker != null)
                cardPicker.CloseCardPicker();
            else if (station != null)
                station.SetActive(false);
        }
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