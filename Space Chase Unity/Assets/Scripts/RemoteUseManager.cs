using System.Collections;
using UnityEngine;

public class RemoteUseManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameController gameController;
    [SerializeField] private MapSelector map;
    [SerializeField] private GameObject mapSelectorObj;
    [SerializeField] private GameObject stationPanel;
    [SerializeField] private CanvasController canvas;
    [SerializeField] private PlayerMovement player;

    [Header("Station Interactables")]
    [SerializeField] private RoomStationInteractable commsStation;
    [SerializeField] private RoomStationInteractable engineStation;
    [SerializeField] private RoomStationInteractable weaponsStation;
    [SerializeField] private RoomStationInteractable bridgeStation;
    [SerializeField] private RoomStationInteractable shieldsStation;

    private bool remoteUseActive = false;
    private GameController.PlayerLocation realLocation;

    public static RemoteUseManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();

        map.endMapSelection += OnRemoteStationSelected;
    }

    public void StartRemoteUse()
    {
        remoteUseActive = true;

        // Save real location
        realLocation = gameController._currentRoom;

        // Open map with all stations
        mapSelectorObj.SetActive(true);
        map.AllowPresses(1);
        map.UpdateMapState(5); // show all buttons
    }

    private void OnRemoteStationSelected()
    {
        if (!remoteUseActive) return;
        remoteUseActive = false;

        mapSelectorObj.SetActive(false);

        if (map.roomsToRepair.Count == 0) return;

        int selectedRoom = map.roomsToRepair[0];
        map.roomsToRepair.Clear();

        // Set player location to selected room
        gameController.UpdatePlayerLocation(selectedRoom);

        // Open that station's UI
        StartCoroutine(OpenRemoteStation(selectedRoom));
    }

    private IEnumerator OpenRemoteStation(int roomId)
    {
        yield return new WaitForEndOfFrame();

        stationPanel.SetActive(true);
        canvas.UIBackground(true);

        RoomCardSlot slot = stationPanel.GetComponentInChildren<RoomCardSlot>();
        if (slot != null)
        {
            bool isDamaged = gameController._damagedRooms.Exists(r =>
                r.ToLower() == RoomIdToName(roomId).ToLower());
            slot.UpdateStationMessage(isDamaged);

            slot.OnCardConfirmed.AddListener((card) => RestoreLocation());
        }
    }

    public void RestoreLocation()
    {
        // Reset back to real location
        gameController.UpdatePlayerLocation(LocationToId(realLocation));
    }

    private string RoomIdToName(int id)
    {
        switch (id)
        {
            case 0: return "comms";
            case 1: return "engine";
            case 2: return "weapons";
            case 3: return "bridge";
            case 4: return "shields";
            default: return "";
        }
    }

    private int LocationToId(GameController.PlayerLocation location)
    {
        switch (location)
        {
            case GameController.PlayerLocation.comms: return 0;
            case GameController.PlayerLocation.engine: return 1;
            case GameController.PlayerLocation.weapons: return 2;
            case GameController.PlayerLocation.bridge: return 3;
            case GameController.PlayerLocation.shields: return 4;
            default: return 1;
        }
    }
}