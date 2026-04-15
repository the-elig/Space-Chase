using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLogic : MonoBehaviour
{
    [SerializeField] private Transform _shipMap;

    public void clickedBridge()
    {
        _shipMap.gameObject.SetActive(false);
    }
}
