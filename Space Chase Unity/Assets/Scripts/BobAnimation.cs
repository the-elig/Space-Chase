using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobAnimation : MonoBehaviour
{
    [SerializeField] private Transform transform;
    [SerializeField] private float upLimit;
    [SerializeField] private float downLimit;
    [SerializeField] private float speed;
    private bool moveUp = true;

    void FixedUpdate()
    {
        if (transform.position.y >= upLimit || transform.position.y <= downLimit) // if obj reaches one of the limits, swap the way its moving
        {
            SwapMoveUp();
        }
        Bob(moveUp);
    }
    void SwapMoveUp()
    {
        switch (moveUp)
        {
            case true: moveUp = false; break;
            case false: moveUp = true; break;
        }
    }

    void Bob(bool up)
    {
        switch(up)
        {
            case true: transform.Translate(Vector3.up * speed * Time.deltaTime); break;
            case false: transform.Translate(Vector3.down * speed * Time.deltaTime); break;
        }
    }
}
