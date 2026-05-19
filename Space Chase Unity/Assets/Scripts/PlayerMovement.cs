using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject stationUI;
    [SerializeField] private GameObject cardPickerPanel;
    [SerializeField] private Transform playerTransform;

    [SerializeField] GameObject menuHolder;

    [Header("Variables")]
    [SerializeField] private float speed;

    private bool canInteract;
    private bool inPassage;
    private bool atStation;
    public bool forcePause;
    public bool paused;
    public bool canLeaveStation;

    // Tutorial flags � set by TutorialManager, ignored in main scene
    [HideInInspector] public bool disableMovement = false;
    [HideInInspector] public bool disableInteract = false;
    [HideInInspector] public bool disableTab = false;

    public delegate void EmptyDelegate();
    public event EmptyDelegate Interact;
    public event EmptyDelegate LeftInteractZone;
    public event EmptyDelegate LeftStation;
    public event EmptyDelegate StationInteract;

    // Tutorial events so TutorialManager can detect when player does something
    public event EmptyDelegate OnPlayerMoved;
    public event EmptyDelegate OnPlayerInteracted;

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
            Movement();

        if (!stationUI.activeSelf && !cardPickerPanel.activeSelf && !forcePause)
        {
            PauseMovement(false);
            canLeaveStation = true;
        }
        else
        {
            PauseMovement(true);
        }

        if (Input.GetKey(KeyCode.Escape) && canLeaveStation)
        {
            LeftStation?.Invoke();
            PauseMovement(false);
        }

        // Tab is blocked during tutorial if disableTab is set
        if (Input.GetKeyDown(KeyCode.Tab) && disableTab)
        {
            // swallow the input � do nothing
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            menuHolder.SetActive(true);
        }

    }

    void Movement()
    {
        bool moved = false;

        if (!disableMovement)
        {
            if (Input.GetKey(KeyCode.W)) { playerTransform.Translate(Vector3.up * speed * Time.deltaTime);    moved = true; }
            if (Input.GetKey(KeyCode.A)) { playerTransform.Translate(Vector3.left * speed * Time.deltaTime);  
            FlipSprite(true); moved = true; }
            if (Input.GetKey(KeyCode.S)) { playerTransform.Translate(Vector3.down * speed * Time.deltaTime);  moved = true; }
            if (Input.GetKey(KeyCode.D)) { playerTransform.Translate(Vector3.right * speed * Time.deltaTime);
            FlipSprite(false); moved = true; }

            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                animator.SetBool("isWalking", true);
            } else
            {
                animator.SetBool("isWalking", false);
            }

            if (moved)
                OnPlayerMoved?.Invoke();
        }

        if (!disableInteract && Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            if (atStation && controller._energy >= 1)
            {
                StationInteract?.Invoke();
                PauseMovement(true);
                canInteract = false;
                OnPlayerInteracted?.Invoke();
            }
            else if (inPassage && controller._energy >= 1)
            {
                m_MyAudioSource.Play();
                Interact?.Invoke();
                PauseMovement(false);
                canInteract = false;
                OnPlayerInteracted?.Invoke();
            }
        }
    }

    private void FlipSprite(bool right)
    {
        GetComponent<SpriteRenderer>().flipX = right;
    }

    public void PauseMovement(bool pause)
    {
        paused = pause;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("RoomHitBox"))
            canInteract = true;

        if (col.gameObject.CompareTag("Passage") ||
            col.gameObject.CompareTag("DamagedPassage"))
            inPassage = true;

        if (col.gameObject.CompareTag("Station"))
            atStation = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("RoomHitBox"))
            canInteract = false;

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