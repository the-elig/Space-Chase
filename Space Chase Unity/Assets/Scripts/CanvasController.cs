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
        energyText.text = "energy = " + _gameController._energy;
    }
    public void endPlayerTurn()
    {
        // called with UI button to manually end player turn
        _gameController._energy = 0;
    }
   
}
