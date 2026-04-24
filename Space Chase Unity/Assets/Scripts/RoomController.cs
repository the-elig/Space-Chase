using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] protected GameController gameController;
    [SerializeField] protected GameObject station;
    [SerializeField] protected GameObject warning;
    protected bool damaged;

    public virtual void DamageRoom(int room_id) { }
}
