using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;


    public int _energy; // add five at the beginning of each player turn
    public int _currentRoom; // can be 1 - 11
    public List<int> _damagedRooms; // can be 1 - 11

    private void Start()
    {
        _player.Interact += useDoor;
        _player.StationInteract += useStation;

        _energy = 0;
        _currentRoom = 5; //sets starting location to engine

        enemyTurn();
    }

    private void Update()
    {
        Debug.Log("Energy = " +  _energy);

        if (_energy > 0)
        {
            //player turn logic
        }
        else
        {
            enemyTurn();
        }
    }


    private void enemyTurn() // called with button 
    {
        // disable player buttons
        

        // check for game over
        if (_damagedRooms.Count >= 5) // 5 is arbitrary rn
        {
            playerLoss();
        }


        // damage player ship
        int ran = Random.Range(1, 12);
        while (_damagedRooms.Contains(ran))
        {
            ran = Random.Range(1, 12);
        }
        

        if (ran != 12)
        {
            Debug.Log("Enemy damaged room " + ran);
            _damagedRooms.Add(ran);

            if (ran == _currentRoom)
            {
                // special event or whatever that occurs when you're in damaged room
            }
        }
        else
        {
            Debug.Log("Enemy missed.");
        }

        _energy += 5; // goes to player's turn
    }

    private void useDoor()
    {
        _energy -= 1;
        Debug.Log("Used door");
    }

    private void useStation()
    {
        _energy -= 2;
        Debug.Log("Used station");
    }


    
    private void playerLoss()
    {
        Debug.Log("player has lost");
    }
    
    private void playerWin()
    {
        Debug.Log("player has won");
    }
}
