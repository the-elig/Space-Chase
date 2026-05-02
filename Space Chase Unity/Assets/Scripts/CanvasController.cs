using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameController _gameController;

    [Header("UI Elements")]
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
    [SerializeField] private GameObject enemyTurnObj;


    // logic variables
    private bool mapActive;

    private void Start()
    {
        mapActive = false;
    }

    private void Update()
    {
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

    private void playEnemyTurn(bool enemy)
    {
        enemyTurnObj.SetActive(enemy);
        if (mapActive)
        {
            map.SetActive(!enemy);
        }
        roomTextObj.SetActive(!enemy);
        energyTextObj.SetActive(!enemy);
        turnsLeftSmallObj.SetActive(!enemy);
        endTurnObj.SetActive(!enemy);
        turnsLeftBig.text = "Turns Left Until Rescue: " + _gameController._turnsLeft;
    }

    public void UIBackground(bool active)
    {
        uiTemplate.SetActive(active);
    }
   
}
