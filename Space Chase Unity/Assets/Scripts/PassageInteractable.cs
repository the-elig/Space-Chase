using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassageInteractable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasController canvas;
    [SerializeField] private GameController _gameController;
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject _outline;
    [SerializeField] private GameObject _bandage;
    [SerializeField] private GameObject stationPanel;
    [SerializeField] private RoomCardSlot cardSlot;
    [SerializeField] private GameObject warningText;
    [SerializeField] private TMP_Text usedEnergy;
    [SerializeField] private GameObject energyFix;

    private bool doorClosed;
    private bool damaged;
    private bool playerInRange;
    public bool isTutorial; // only for tutorial

    void Awake()
    {
        isTutorial = false;
        damaged = false;
        playerInRange = false;
        if (_outline != null) _outline.SetActive(false);
    }

    void Start()
    {
        _player.Interact += OpenDoor;
        _player.LeftInteractZone += CloseDoor;
    }

    void Update()
    {
        _bandage.SetActive(damaged);
    }

    public void ToggleDamage(bool damage)
    {
        damaged = damage;
        gameObject.tag = "DamagedPassage";
    }

    public void DisableInteraction()
    {
        _player.Interact -= OpenDoor;
        _player.LeftInteractZone -= CloseDoor;
    }

    void OpenDoor()
    {
        if (!playerInRange) return;

        if (damaged)
        {
            doorClosed = true;
            door.SetActive(doorClosed);
            stationPanel.SetActive(true);
            warningText.SetActive(true);
            canvas.UIBackground(true);

            RoomCardSlot slot = stationPanel.GetComponentInChildren<RoomCardSlot>();
            if (slot != null)
            {
                slot.openedFromPassage = true;
                slot.currentPassage = this;
                slot.HideStationMessages();
            }
        }
        else
        {
            _gameController._energy -= 1;
            energyFix.SetActive(true);
            Invoke("removeNotice", 3);
            doorClosed = false;
            door.SetActive(doorClosed);

            RoomCardSlot slot = stationPanel.GetComponentInChildren<RoomCardSlot>();
            if (slot != null && !slot.openedFromPassage)
            {
                slot.openedFromPassage = false;
                slot.currentPassage = null;
            }
        }
    }

    public void CloseDoor()
    {
        if(isTutorial) return;
        if (damaged)
        {
            if (stationPanel.activeSelf)
            {
                RoomCardSlot slot = stationPanel.GetComponentInChildren<RoomCardSlot>();
                if (slot != null)
                {
                    slot.openedFromPassage = false;
                    slot.currentPassage = null;
                }
            }
            stationPanel.SetActive(false);
            canvas.UIBackground(false);
        }
        else
        {
            doorClosed = true;
            door.SetActive(doorClosed);
        }
    }

    public void RepairPassage()
    {
        damaged = false;
        gameObject.tag = "Passage";
        doorClosed = false;
        door.SetActive(doorClosed);

        PassageController[] controllers = FindObjectsOfType<PassageController>();
        foreach (PassageController controller in controllers)
        {
            if (controller.passage == this)
            {
                controller.RepairPassage();
                break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            if (!damaged)
                _outline.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            cardSlot.OnCancel();
            warningText.SetActive(false);
            _outline.SetActive(false);
        }
    }

    public void removeNotice()
    {
        energyFix.SetActive(false);
    }

    public void EnableInteraction()
    {
        _player.Interact += OpenDoor;
        _player.LeftInteractZone += CloseDoor;
    }
}