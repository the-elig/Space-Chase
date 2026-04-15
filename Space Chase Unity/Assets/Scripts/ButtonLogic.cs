using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLogic : MonoBehaviour
{
    [SerializeField] private GameController _gameController;

    [SerializeField] private Transform _shipMap;
    // [SerializeField] private Transform _bridge;
    // [SerializeField] private Transform _weapons;
    // [SerializeField] private Transform _engine;
    // [SerializeField] private Transform _shields;
    // [SerializeField] private Transform _communications;

    public void clickedBridge()
    {
        int travelDistance = 0;

        if (_gameController._energy - travelDistance > 0)
        {
            _shipMap.gameObject.SetActive(false);
            // _bridge.gameObject.SetActive(true);
        }
    }
}
