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

    //player turn ui elements
    [SerializeField] private GameObject HUD;
    [SerializeField] private TMP_Text roomText;
    [SerializeField] private TMP_Text turnsLeftSmall;
    [SerializeField] private TMP_Text usedEnergy;
    [SerializeField] private GameObject energyFix;
    
    //enemy turn ui elements
    [SerializeField] private TMP_Text turnsLeftBig;
    [SerializeField] private TMP_Text damagedRoomBig;
    [SerializeField] private GameObject enemyTurnObj;

    [SerializeField] private RawImage energy1;
    [SerializeField] private RawImage energy2;
    [SerializeField] private RawImage energy3;
    [SerializeField] private RawImage energy4;
    [SerializeField] private RawImage energy5;

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

        roomText.text = "" + _gameController.GetPlayerLocation();

        playEnemyTurn(_gameController._isEnemyTurn);

        //Remove energy symbol
        if(_gameController._energy <= 4)
        {
            energy1.enabled = false;
        }
        if (_gameController._energy <= 3)
        {
            energy2.enabled = false;
        }
        if (_gameController._energy <= 2)
        {
            energy3.enabled = false;
        }
        if (_gameController._energy <= 1)
        {
            energy4.enabled = false;
        }
        if (_gameController._energy <= 0)
        {
            energy5.enabled = false;
        }


        // update turns left text
        turnsLeftSmall.text = "" + _gameController._turnsLeft;
    }
    public bool isTutorial = false;

    public void endPlayerTurn()
    {
        if (isTutorial) return; // let TutorialManager handle it
        _gameController._energy = 0;
        _gameController.enemyTurn();
        energy1.enabled = true;
        energy2.enabled = true;
        energy3.enabled = true;
        energy4.enabled = true;
        energy5.enabled = true;
    }

    private void playEnemyTurn(bool enemyTurn)
    {
        int roomID = _gameController.recentlyDamagedRoom;

        enemyTurnObj.SetActive(enemyTurn);
        if (mapActive)
        {
            map.SetActive(!enemyTurn);
        }

        HUD.SetActive(!enemyTurn);


        turnsLeftBig.text = "Turns Left Until Rescue: " + _gameController._turnsLeft;

        if (roomID <= 10)
        {
            damagedRoomBig.text = roomNames[roomID] + " was damaged";
        }
        else
        {
            damagedRoomBig.text = "Enemy missed";
        }

    }

    public void UIBackground(bool active)
    {
        uiTemplate.SetActive(active);
    }
    public void TurnOffPlayerTurnUI()
    {
        HUD.SetActive(false);
    }
   
    public void removeNotice()
    {
        energyFix.SetActive(false);
    }
}
