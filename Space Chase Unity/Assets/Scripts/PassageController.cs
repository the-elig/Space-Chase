using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageController : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] public PassageInteractable passage;
    [SerializeField] private GameObject warning;
    [SerializeField] private int id;
    private bool damaged;

    void Awake()
    {
        gameController.DamageRoom += DamagePassage;
        damaged = false;
    }

    private void DamagePassage(int pass_id)
    {
        if (pass_id == id)
        {
            damaged = true;
            warning.SetActive(true);
            passage.ToggleDamage(damaged);
        }
    }
    public void RepairPassage()
    {
        damaged = false;
        warning.SetActive(false);
        passage.CloseDoor();

        gameController._damagedRooms.Remove(gameController._rooms[id]);
    }
}
