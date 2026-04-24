using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageController : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private PassageInteractable passage;
    [SerializeField] private GameObject warning;
    [SerializeField] private int id;
    private bool damaged;
    void Start()
    {
        gameController.damageRoom += DamagePassage;
    }
    void Update()
    {

    }
    private void DamagePassage(int pass_id)
    {
        if (pass_id == id)
        {
            warning.SetActive(true);
        }
    }
}
