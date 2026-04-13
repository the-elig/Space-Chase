using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int _energy;
    public int _currentRoom; // can be 1 - 11
    public List<int> _damagedRooms; // can be 1 - 11


    public void enemyTurn()
    {
        int ran = Random.Range(1, 12);
        while (_damagedRooms.Contains(ran))
        {
            ran = Random.Range(1, 12);
        }
        

        if (ran != 12)
        {
            Debug.Log("Enemy damaged room " + ran);
            _damagedRooms.Add(ran);
        }
        else
        {
            Debug.Log("Enemy missed.");
        }
    }
}
