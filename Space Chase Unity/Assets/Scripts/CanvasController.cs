using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameController _gameController;

    // UI elements
    [SerializeField] private GameObject map;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text turnsLeftSmall;
    [SerializeField] private TMP_Text turnsLeftBig;
    [SerializeField] private TMP_Text enemyTurnText;


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
            if (mapActive == false)
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

        // update energy text
        energyText.text = "Energy = " + _gameController._energy;
        
        // update turns left text
        turnsLeftSmall.text = "Turns Left: " + _gameController._turnsLeft;
        turnsLeftBig.text = "Turns Left Until Rescue: " + _gameController._turnsLeft;
    }
    public void endPlayerTurn()
    {
        // called with UI button to manually end player turn
        _gameController._energy = 0;
    }
   
}
