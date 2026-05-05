using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameController _gameController;

    [Header("UI Elements")]
    [SerializeField] private RawImage _bg;
    [SerializeField] private GameObject uiTemplate;
    [SerializeField] private GameObject map;

    [SerializeField] private TMP_Text energyText;
    [SerializeField] private GameObject energyTextObj;

    [SerializeField] private TMP_Text roomText;
    [SerializeField] private GameObject roomTextObj;

    [SerializeField] private TMP_Text turnsLeftSmall;
    [SerializeField] private GameObject turnsLeftSmallObj;
    [SerializeField] private GameObject endTurnObj;

    [SerializeField] private TMP_Text turnsLeftBig;
    [SerializeField] private TMP_Text damagedRoomBig;
    [SerializeField] private GameObject enemyTurnObj;


    // logic variables
    private float backgroundX;
    private bool mapActive;
    private string[] roomNames =
        {"Communications", "Engine", "Weapons", "Bridge", "Shields",
        "Hallway", "Hallway", "Hallway", "Hallway", "Hallway", "Hallway"};


    private void Start()
    {
        backgroundX = 0.05f;
        mapActive = false;
    }

    private void Update()
    {
        //scroll background
        _bg.uvRect = new Rect(_bg.uvRect.position + new Vector2(backgroundX, 0) * Time.deltaTime, _bg.uvRect.size);


        // open/close map with TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!mapActive)
            {
                map.SetActive(true);
                mapActive = true;
            }
            else
            {
                map.SetActive(false);
                mapActive = false;
            }
        }

        roomText.text = "Current Room: " + _gameController.GetPlayerLocation();

        playEnemyTurn(_gameController._isEnemyTurn);

        // update energy text
        energyText.text = "Energy = " + _gameController._energy;

        // update turns left text
        turnsLeftSmall.text = "Turns Left: " + _gameController._turnsLeft;
    }
    public void endPlayerTurn()
    {
        // called with UI button to manually end player turn
        _gameController._energy = 0;
        _gameController.enemyTurn();
    }

    private void playEnemyTurn(bool enemyTurn)
    {
        int roomID = _gameController.recentlyDamagedRoom;

        enemyTurnObj.SetActive(enemyTurn);
        if (mapActive)
        {
            map.SetActive(!enemyTurn);
        }
        roomTextObj.SetActive(!enemyTurn);
        energyTextObj.SetActive(!enemyTurn);
        turnsLeftSmallObj.SetActive(!enemyTurn);
        endTurnObj.SetActive(!enemyTurn);

        turnsLeftBig.text = "Turns Left Until Rescue: " + _gameController._turnsLeft;

        if (roomID <= 10)
            damagedRoomBig.text = roomNames[roomID] + " was damaged";
        else
            damagedRoomBig.text = "Enemy missed";


    }

    public void UIBackground(bool active)
    {
        uiTemplate.SetActive(active);
    }
   
}
