using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum PlayerLocation
    {
        comms, engine, weapons, bridge, shields, passage
    }
    public PlayerLocation _currentRoom;

    [SerializeField] private PlayerMovement _player;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _bg;
    public delegate void IntDelegate(int x);
    public delegate void EmptyDelegate();
    public event IntDelegate damageRoom;

    public int _energy; // add five at the beginning of each player turn
    public int _turnsLeft;
    public bool _isEnemyTurn;

    public List<string> _rooms;
    public List<string> _damagedRooms;

    AudioSource m_MyAudioSource;


    private void Start()
    {
        _player.Interact += useDoor;
        _player.StationInteract += useStation;

        m_MyAudioSource = GetComponent<AudioSource>();

        _energy = 0;
        _turnsLeft = 15;
        _currentRoom = PlayerLocation.engine;

        enemyTurn();
    }

    private void Update()
    {
        if (_turnsLeft == 0)
        {
            playerWin();
        }
    }


    public void enemyTurn() // called with button 
    {
        _isEnemyTurn = true;
        // disable player buttons
        _player.PauseMovement(true);

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
                m_MyAudioSource.Play();
            }
            damageRoom?.Invoke(room_id); // sends out an event to all of the room controllers, child scripts handle if the number matches the room damaged
        }
        else
        {
            Debug.Log("Enemy missed.");
        }
        StartCoroutine(WaitEnemyTurn()); // play cutscene
    }
    IEnumerator WaitEnemyTurn() // play enemy turn screen and pause player movement
    {
        _bg.SetActive(true);
        _turnsLeft--;
        yield return new WaitForSeconds(3f);
        _isEnemyTurn = false;
        _bg.SetActive(false);
        _player.PauseMovement(false);
    }

    private int GetRan() // gets a random value for damaged rooms
    {
        int ran = Random.Range(0, 12); // 0=Comms, 1=Engine, 2=Weapons, 3=Bridge, 4=Shields, 
                                       // 5=En->Cm 6=En->Wp 7=En->Br 8=En->Sh 9=Br->Wp 10=Br->Sh
        return ran;
    }

    public void UpdatePlayerLocation(int location_id)
    {
        switch(location_id)
        {
            case 0: _currentRoom = PlayerLocation.comms; break;
            case 1: _currentRoom = PlayerLocation.engine; break;
            case 2: _currentRoom = PlayerLocation.weapons; break;
            case 3: _currentRoom = PlayerLocation.bridge; break;
            case 4: _currentRoom = PlayerLocation.shields; break;
            default: _currentRoom = PlayerLocation.passage; break;
        }
    }
    public string GetPlayerLocation()
    {
        switch(_currentRoom)
        {
            case PlayerLocation.comms: return "Communications";
            case PlayerLocation.engine: return "Engine";
            case PlayerLocation.weapons: return "Weapons";
            case PlayerLocation.bridge: return "Bridge";
            case PlayerLocation.shields: return "Shields";
            default: return "Passage";
        }
    }

    private void useDoor()
    {
        _energy -= 1;
        Debug.Log("Used door");
        Debug.Log("Energy = " + _energy);
    }

    private void useStation()
    {
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
