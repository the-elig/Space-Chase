using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;

    public delegate void EmptyDelegate();
    public event EmptyDelegate damageRoom;

    public int _energy; // add five at the beginning of each player turn
    public int _currentRoom; // can be 1 - 11

    public List<string> _rooms;
    public List<string> _damagedRooms;

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
        if (_energy == 0)
        {
            enemyTurn();
        }
    }


    private void enemyTurn() // called with button 
    {
        // disable player buttons

        // play cut scenes

        // check for game over
        if (_damagedRooms.Count >= 5) // 5 is arbitrary rn
        {
            playerLoss();
        }

        // damage player ship
        int ran = Random.Range(0, 11);

        if (ran != 11)
        {
            _damagedRooms.Add(_rooms[0]); // temp code to always damage comms
            //_damagedRooms.Add(_rooms[ran]);
            //_damagedRooms.Add(ran);
            damageRoom?.Invoke();

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
        Debug.Log("Energy = " + _energy);
    }

    private void useDoor()
    {
        _energy -= 1;
        Debug.Log("Used door");
        Debug.Log("Energy = " + _energy);
    }

    private void useStation()
    {
        _energy -= 1;
        Debug.Log("Used station");
        Debug.Log("Energy = " + _energy);
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
