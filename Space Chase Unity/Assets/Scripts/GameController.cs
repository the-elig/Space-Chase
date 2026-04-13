using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int _energy; // add five at the beginning of each player turn
    public int _currentRoom; // can be 1 - 11
    public List<int> _damagedRooms; // can be 1 - 11


    public void enemyTurn() // called with button 
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


    public void playerLoss()
    {
        Debug.Log("player has lost");
    }
    
    public void playerWin()
    {
        Debug.Log("player has won");
    }
}
