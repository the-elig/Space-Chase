using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum PlayerLocation
    {
        comms, engine, weapons, bridge, shields, passage
    }
    public PlayerLocation _currentRoom;

    [Header("References")]
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private CanvasController _canvas;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _endTurnButton;
    [SerializeField] private GameObject _seeDeckButton;
    [SerializeField] private GameObject _stationUI;
    [SerializeField] private GameObject _mapUI;

    public delegate void IntDelegate(int x);
    public delegate void EmptyDelegate();
    public event IntDelegate DamageRoom;

    [Header("Variables")]
    public int _energy; // add five at the beginning of each player turn
    [SerializeField] private int _gainEnergy;
    public int _turnsLeft;
    public int _enemyTurnsTaken;
    [SerializeField] private int _enemyScalingSpeed; // int how many turns it takes for the enemy to scale more
    [SerializeField] private int _damageCount;
    [SerializeField] private int _damageLoss;
    private string _winOrLoseText;
    public bool _isEnemyTurn;
    private int _lastHit; // make sure turn count doesn't lower multiple times when enemy attacks multiple times
    public bool _endGameState;

    [Header("Lists of Rooms")]
    public List<string> _rooms;
    public List<string> _damagedRooms;
    public int recentlyDamagedRoom;


    AudioSource m_MyAudioSource;

    [HideInInspector] public bool skipEnemyTurnOnStart = false;
    public Shake Shake;

    private void Start()
{
    m_MyAudioSource = GetComponent<AudioSource>();

    _endGameState = false;
    _energy = 0;
    _lastHit = 0;
    _damageCount = 1;
    _turnsLeft = 15;
    _currentRoom = PlayerLocation.engine;

    _energy += _gainEnergy;

    if (Shake != null) Shake.ShakeWrap();

    if (!skipEnemyTurnOnStart)
        enemyTurn();
}

    private void Update()
    {
        ScaleEnemyDamage();

        if (_turnsLeft <= 0)
        {
            _endGameState = true;
            playerWin();
        }

        if (_stationUI.activeSelf || _mapUI.activeSelf)
        {
            _endTurnButton.SetActive(false);
            _seeDeckButton.SetActive(false);
            _player.canInteract = false;
            _player.forcePause = true;
        }
        else if(!_stationUI.activeSelf && !_mapUI.activeSelf && !_endGameState)
        {
            _endTurnButton.SetActive(true);
            _seeDeckButton.SetActive(true);
            _player.canInteract = true;
            _player.forcePause = false;
        }

        if (_endGameState)
        {
            _canvas.TurnOffPlayerTurnUI();
        }

        if(!_player.gameObject.activeSelf) // fix for player deactivating in tutorial scene?
        {
            _player.gameObject.SetActive(true);
        }
    }


    public void enemyTurn() // called with button 
    {
        _isEnemyTurn = true;
        // disable player buttons
        _player.forcePause = true;

        // check for game over
        _energy = 0;
        _energy += _gainEnergy;
        if (_damagedRooms.Count >= _damageLoss)
        {
            _endGameState = true;
            playerLoss();
        }
        for(int i = 0; i < _damageCount; i++)
        {
            int room_id = GetRan();
            DamagePlayerShip(room_id);
        }
    }

    private void ScaleEnemyDamage()
    {
        if(_enemyTurnsTaken > _enemyScalingSpeed) // every five/four turns, increase the amount of times the enemy attacks by one
        {
            _damageCount += 1;
            _enemyScalingSpeed += 5;
        }
    }

    private void DamagePlayerShip(int room_id)
    {
        _lastHit++;
        if (room_id != 11)
        {
            _damagedRooms.Add(_rooms[room_id]);

            for (int i = 0; i < _damagedRooms.Count; i++)
            {
                Debug.Log(_damagedRooms[i] + " is damaged!");
                m_MyAudioSource.Play();
            }
            recentlyDamagedRoom = room_id;
            DamageRoom?.Invoke(room_id); // sends out an event to all of the room controllers, child scripts handle if the number matches the room damaged
        }
        else
        {
            Debug.Log("Enemy missed.");
            recentlyDamagedRoom = 11;
        }
        if(_lastHit == _damageCount)
            StartCoroutine(WaitEnemyTurn()); // play cutscene
    }
    IEnumerator WaitEnemyTurn() // play enemy turn screen and pause player movement
    {
        _player.disableMovement = true;
        _turnsLeft--;
        _enemyTurnsTaken++;
        yield return new WaitForSeconds(3f);
        if(!_endGameState) // if not the end game, return to player turn
        {
            Debug.Log("player turn");
            _lastHit = 0;
            _isEnemyTurn = false;
            _player.disableMovement = false;
        } else // otherwise don't return to player turn
        {
            _canvas.EndGameUI(_winOrLoseText);
        }

    }

    private int GetRan() // gets a random value for damaged rooms
    {
        int ran = Random.Range(0, 12); // 0=Comms, 1=Engine, 2=Weapons, 3=Bridge, 4=Shields, 
        if (ran == recentlyDamagedRoom) // 5=En->Cm 6=En->Wp 7=En->Br 8=En->Sh 9=Br->Wp 10=Br->Sh
            ran++;
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

    
    private void playerLoss()
    {
        _player.disableMovement = true;
        _player.disableInteract = true;
        _player.disableTab = true;

        _winOrLoseText = "Game Over...";
    }
    
    private void playerWin()
    {
        _player.disableMovement = true;
        _player.disableInteract = true;
        _player.disableTab = true;

        _winOrLoseText = "You Win!!";
    }

    public void TriggerDamageRoom(int id)
    {
        DamageRoom?.Invoke(id);
    }


}
