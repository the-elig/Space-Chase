using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldsController : RoomController
{
    void Start()
    {
        gameController.damageRoom += DamageRoom;
        damaged = false;
    }

    void Update()
    {

    }
    public override void DamageRoom(int room_id)
    {
        if (room_id == 4)
        {
            damaged = true;
            warning.SetActive(true);
        }
    }
}
