using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameController controller;
    [SerializeField] GameObject stationPanel;
    [SerializeField] GameObject cardPickerPanel;

    [Header("Variables")]
    [SerializeField] private float speed;
    [SerializeField] private Transform playerTransform;

    private bool canInteract;
    private bool inPassage;
    private bool atStation;
    public bool forcePause;
    private bool paused;
    public bool canLeaveStation;

    public delegate void EmptyDelegate();
    public event EmptyDelegate Interact;
    public event EmptyDelegate LeftInteractZone;

    public event EmptyDelegate LeftStation;
    public event EmptyDelegate StationInteract;

    AudioSource m_MyAudioSource;

    void Start()
    {
        canInteract = false;
        inPassage = false;
        atStation = false;

        m_MyAudioSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        if (!paused)
        {
            Movement();
        }
        if (!stationPanel.activeSelf && !cardPickerPanel.activeSelf && !forcePause)
        {
            PauseMovement(false);
            canLeaveStation = true;
        } else
        {
            PauseMovement(true);
        }
        if (Input.GetKey(KeyCode.Escape) && canLeaveStation)
        {
            LeftStation?.Invoke();
            PauseMovement(false);
        }

    }
    void Movement()
    {
        if(Input.GetKey(KeyCode.W))
            playerTransform.Translate(Vector3.up * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            playerTransform.Translate(Vector3.left * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            playerTransform.Translate(Vector3.down * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            playerTransform.Translate(Vector3.right * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            if (atStation && controller._energy >= 1) //stations
            {
                StationInteract?.Invoke();
                PauseMovement(true);
                canInteract = false;
            }
            else if (inPassage && controller._energy >= 1) //doors
            {
                m_MyAudioSource.Play();
                Interact?.Invoke();
                PauseMovement(false);
                canInteract = false;
            }
        }        
    }

    public void PauseMovement(bool pause)
    {
        paused = pause;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(!col.gameObject.CompareTag("RoomHitBox"))
        {
            canInteract = true;
        }
        if (col.gameObject.CompareTag("Passage") || 
    col.gameObject.CompareTag("DamagedPassage"))
{
    inPassage = true;
}
        if (col.gameObject.CompareTag("Station"))
        {
            atStation = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if(!col.gameObject.CompareTag("RoomHitBox"))
        {
            canInteract = false;
        }
        if (col.gameObject.CompareTag("Passage") || 
    col.gameObject.CompareTag("DamagedPassage"))
{
    LeftInteractZone?.Invoke();
    inPassage = false;
}
        if (col.gameObject.CompareTag("Station"))
        {
            LeftStation?.Invoke();
            atStation = false;
        }
    }
}
