using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommsController : RoomController
{
    void Start()
    {
        gameController.damageRoom += DamageRoom;
        damaged = false;
    }

    void Update()
    {

    }
    public override void DamageRoom()
    {
        Debug.Log("comms damaged");
        damaged = true;
        warning.SetActive(true);
    }
}
