using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassageInteractable : MonoBehaviour
{
    [SerializeField] private CanvasController canvas;
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject _outline;
    [SerializeField] private GameObject _bandage;
    [SerializeField] private GameObject stationPanel;

    private bool doorClosed;
    private bool damaged;
    private bool playerInRange;

    void Awake()
    {
        damaged = false;
        playerInRange = false;
        if (_outline != null) _outline.SetActive(false);
        _player.Interact += OpenDoor;
        _player.LeftInteractZone += CloseDoor;
    }

    void Update()
    {
        if (damaged)
        {
            _bandage.SetActive(true);
        }
        else
        {
            _bandage.SetActive(false);
        }
    }

    public void ToggleDamage(bool damage)
    {
        damaged = damage;
        Debug.Log("ouch");
        gameObject.tag = "DamagedPassage";
    }

    void OpenDoor()
    {
        if (!playerInRange) return;

        if (damaged)
        {
            doorClosed = true;
            door.SetActive(doorClosed);
            stationPanel.SetActive(true);
            canvas.UIBackground(true);

            RoomCardSlot slot = stationPanel.GetComponentInChildren<RoomCardSlot>();
            if (slot != null)
            {
                slot.openedFromPassage = true;
                slot.currentPassage = this;
                slot.HideStationMessages();
            }

            TMP_Text[] texts = stationPanel.GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text text in texts)
            {
                if (text.gameObject.name == "MessageText")
                {
                    text.text = "Uh oh! This passageway has been damaged! Use a repair card to fix it.";
                    text.gameObject.SetActive(true);
                    break;
                }
            }
        }
        else
        {
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

    void CloseDoor()
    {
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
            _outline.SetActive(false);
        }
    }
}