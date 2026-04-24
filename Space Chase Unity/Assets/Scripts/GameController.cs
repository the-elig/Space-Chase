using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private GameObject _camera;
    public delegate void IntDelegate(int x);
    public event IntDelegate damageRoom;

    public int _energy; // add five at the beginning of each player turn
    public int _turnsLeft;
    public int _currentRoom; // can be 1 - 11

    public List<string> _rooms;
    public List<string> _damagedRooms;

    private void Start()
    {
        _player.Interact += useDoor;
        _player.StationInteract += useStation;

        _energy = 0;
        _turnsLeft = 15;
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
        _player.PauseMovement(true);
        // play cut scenes

        // check for game over
        _energy = 0;
        if (_damagedRooms.Count >= 5) // 5 is arbitrary rn
        {
            playerLoss();
        }
        int room_id = GetRan();
        _energy += 5; // energy gain is before damage so we'll need to add another thing later to switch to player turn!
        Debug.Log("Energy = " + _energy);
        DamagePlayerShip(room_id);
    }
    private void DamagePlayerShip(int room_id)
    {
        if (room_id != 11)
        {
            _damagedRooms.Add(_rooms[room_id]);
            if (_damagedRooms.Count != _damagedRooms.Distinct().Count())
            {
                Debug.Log("duplicate room damaged, rerolling...");
                _damagedRooms.RemoveAt(_damagedRooms.Count - 1);
                enemyTurn(); // restarts the enemy turn to reroll value if duplicate value
                return;
            }

            for (int i = 0; i < _damagedRooms.Count; i++)
            {
                Debug.Log(_damagedRooms[i] + " is damaged!");
            }
            damageRoom?.Invoke(room_id); // sends out an event to all of the room controllers, child scripts handle if the number matches the room damaged

            if (room_id == _currentRoom)
            {
                // special event or whatever that occurs when you're in damaged room
            }
        }
        else
        {
            Debug.Log("Enemy missed.");
        }
        _turnsLeft--;
        _player.PauseMovement(false);
    }

    private int GetRan() // gets a random value for damaged rooms
    {
        int ran = Random.Range(0, 12); // 0=Comms, 1=Engine, 2=Weapons, 3=Bridge, 4=Shields, 
                                    // 5=En->Cm 6=En->Wp 7=En->Br 8=En->Sh 9=Br->Wp 10=Br->Sh
        Debug.Log("ran value = " + ran);
        return ran;
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
