using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomStationInteractable : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private CardPickerUI cardPicker;
    [SerializeField] private CardUpgraderUI cardUpgrader;
    [SerializeField] private RoomCardSlot cardSlot;
    [SerializeField] private GameController gameController;
    [SerializeField] private CanvasController canvas;

    [Header("Object References")]
    [SerializeField] private GameObject station;
    [SerializeField] private GameObject _outline;
    [SerializeField] private GameObject _choiceMenu;
    [SerializeField] private ParticleSystem smoke;
    AudioSource m_MyAudioSource;

    [SerializeField] private string roomID;

    void Start()
    {
        if (_choiceMenu != null) _choiceMenu.SetActive(false);
        _player.StationInteract += OpenStation;
        _player.LeftStation += CloseStation;
        m_MyAudioSource = GetComponent<AudioSource>();
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();
    }

    void OpenStation()
{
    if (_player.disableInteract) return;

    if (gameController._currentRoom.ToString().ToLower() != roomID.ToLower())
        return;

    RoomCardSlot cardSlotRef = station != null ? station.GetComponentInChildren<RoomCardSlot>() : null;
    if (cardSlotRef != null) cardSlotRef.HideStationMessages();

    bool isDamaged = gameController._damagedRooms.Exists(r =>
        r.ToLower() == roomID.ToLower());

    if (isDamaged)
    {
        if (station != null)
        {
            station.SetActive(true);
            canvas.UIBackground(true);
            RoomCardSlot slot = station.GetComponentInChildren<RoomCardSlot>();
            if (slot != null)
                slot.UpdateStationMessage(true);
        }
    }
    else
    {
        canvas.UIBackground(true);

        if (cardPicker != null)
        {
            _player.canLeaveStation = false;

            TutorialCardPickerOverride tutorialOverride = cardPicker.GetComponent<TutorialCardPickerOverride>();
            if (tutorialOverride != null)
                tutorialOverride.OpenTutorialCardPicker();
            else
                cardPicker.OpenCardPicker();

            m_MyAudioSource.Play();
        }
        else if (cardUpgrader != null)
        {
            m_MyAudioSource.Play();
            if (_choiceMenu != null) _choiceMenu.SetActive(true);
            return;
        }
        else if (station != null)
        {
            station.SetActive(true);
            m_MyAudioSource.Play();
            RoomCardSlot slot = station.GetComponentInChildren<RoomCardSlot>();
            if (slot != null)
                slot.UpdateStationMessage(false);
        }
    }
}
    public void OpenUpgraderUI()
    {
        _choiceMenu.SetActive(false);
        if (_choiceMenu != null) cardUpgrader.OpenCardUpgrader();
    }
    public void OpenDeckUI()
    {
        if(_choiceMenu != null) _choiceMenu.SetActive(false);
        station.SetActive(true);
        m_MyAudioSource.Play();
        RoomCardSlot slot = station.GetComponentInChildren<RoomCardSlot>();
        if (slot != null)
        {
            slot.UpdateStationMessage(false);
        } //updates damaged message to no longer appear
    }

    void CloseStation()
{
    if (gameController._currentRoom.ToString().ToLower() != roomID.ToLower())
        return;

    bool isDamaged = gameController._damagedRooms.Exists(r =>
        r.ToLower() == roomID.ToLower());

    cardSlot.OnCancel();

    // Always close choice menu
    if (_choiceMenu != null) _choiceMenu.SetActive(false);

    canvas.UIBackground(false);

    if (isDamaged)
    {
        if (station != null)
            station.SetActive(false);
    }
    else
    {
        if (cardPicker != null)
        {
            cardPicker.CloseCardPicker();
            if (station != null) station.SetActive(false);
        }
        else if (cardUpgrader != null)
        {
            cardUpgrader.CloseCardUpgrader();
        }
        else if (station != null)
        {
            station.SetActive(false);
        }
    }
}

    [HideInInspector] public bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
           if (_outline != null)
                _outline.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            if (_outline != null)
                _outline.SetActive(false);
        }
    }
}