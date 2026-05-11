using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{

    [Header("Scene Transition")]
    [SerializeField] private string mainSceneName = "MainScene";

    [Header("Core References")]
    [SerializeField] private GameController gameController;
    [SerializeField] private PlayerMovement player;

    [Header("Tutorial Card Picker")]
    [SerializeField] private CardPickerUI cardPickerUI;
    [SerializeField] private CardData tutorialRepairCard;
    [SerializeField] private CardData tutorialCommsCard;

    [Header("Forced Damage Setup")]
    [SerializeField] private PassageController engineToCommsPassage;
    [SerializeField] private PassageInteractable engineToCommsPassageInteractable;
    [SerializeField] private RoomController commsRoom;

    [Header("Locked Passages")]
    [Tooltip("Passages that are never usable in the tutorial")]
    [SerializeField] private List<PassageInteractable> permanentlyLockedPassages;
    [Tooltip("Passages that show a message after comms hallway is repaired")]
    [SerializeField] private List<PassageInteractable> postRepairLockedPassages;
    [SerializeField] private string wrongPassageMessage = "That's not the right hallway! Find the damaged one.";

    [Header("Map Reference")]
    [Tooltip("The map GameObject opened with Tab")]
    [SerializeField] private GameObject mapUI;

    [Header("Tutorial UI")]
    [SerializeField] private GameObject tutorialOverlay;
    [SerializeField] private GameObject tutorialBox;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private GameObject tutorialArrow;
    [SerializeField] private TMP_Text lockedMessageText;
    [SerializeField] private float lockedMessageDuration = 2f;

    private bool transitionTriggered = false;
    private bool playerHasMoved = false;
    private bool playerHasOpenedMap = false;
    private bool playerHasClosedMap = false;
    private bool playerHasInteractedWithEngine = false;
    private bool hallwayRepaired = false;
    private bool commsStationUsed = false;
    private bool postRepairLockActive = false;

    void Awake()
    {
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();
        if (player == null)
            player = FindObjectOfType<PlayerMovement>();

        gameController.skipEnemyTurnOnStart = true;
    }

    void Start()
    {
        player.disableMovement = true;
        player.disableInteract = true;
        player.disableTab = true;

        SetupTutorialDamage();
        SetupLockedPassages();
        OverrideCardPicker();

        if (lockedMessageText != null)
            lockedMessageText.gameObject.SetActive(false);

        RoomCardSlot slot = FindObjectOfType<RoomCardSlot>();
        if (slot != null)
            slot.OnCardConfirmed.AddListener(OnCardConfirmed);

        StartCoroutine(TutorialSequence());
    }


    private IEnumerator TutorialSequence()
    {
        yield return StartCoroutine(UITour());
        yield return StartCoroutine(TeachMovement());
        yield return StartCoroutine(TeachMap());
        yield return StartCoroutine(TeachEngineStation());
        yield return StartCoroutine(TeachRepairHallway());
        yield return StartCoroutine(TeachCommsStation());
        yield return StartCoroutine(TeachEndTurn());
    }

    private IEnumerator UITour()
    {
        ShowOverlay(true);

        yield return StartCoroutine(ShowTutorialMessage(
            "Welcome to Space Chase, Captain! Let's get you familiar with the interface."));

        yield return StartCoroutine(ShowTutorialMessage(
            "In the top left corner you will find your Current Room. This tells you where you are on the ship, and may come in handle when you need to navigate to a different room."));

        yield return StartCoroutine(ShowTutorialMessage(
            "The top right shows your Turns Left. This is how long you have to survive before a rescue ship responds to your distress calls."));

        yield return StartCoroutine(ShowTutorialMessage(
            "Below that is your Energy. You spend energy to use cards and move through hallways, and you have a limited amount of energy each turn."));


        ShowOverlay(false);
        HideTutorialBox();
    }

    private IEnumerator TeachMovement()
    {
        yield return StartCoroutine(ShowTutorialMessage(
            "Use WASD to move around the ship. Try moving now!"));

        player.disableMovement = false;
        player.OnPlayerMoved += OnFirstMove;

        yield return new WaitUntil(() => playerHasMoved);
        player.OnPlayerMoved -= OnFirstMove;

        yield return StartCoroutine(ShowTutorialMessage(
            "Great! You've got the hang of moving around. Press Space to continue."));
        HideTutorialBox();
    }

    private void OnFirstMove() => playerHasMoved = true;

    private IEnumerator TeachMap()
{
    if (mapUI != null) mapUI.SetActive(false);

    player.disableTab = false;

    yield return StartCoroutine(ShowTutorialMessage(
        "Press Tab to open and close the ship map. It shows all rooms and their current status! If a room or passageway has a hazard symbol, it means that it has been damaged in an enemy attack!"));

    player.disableTab = true;
    if (mapUI != null) mapUI.SetActive(false); // make sure it's closed after

    yield return StartCoroutine(ShowTutorialMessage(
        "Good! You can check the map any time during the real game."));
    HideTutorialBox();
}

    private IEnumerator TeachEngineStation()
{
    player.disableInteract = false; // unlock FIRST
    player.OnPlayerInteracted += OnEngineInteracted;

    yield return StartCoroutine(ShowTutorialMessage(
        "See the Engine station over there? Walk up to it and press E to interact. To interact with any station or hallway aboard your ship, press E."));

    yield return new WaitUntil(() => playerHasInteractedWithEngine);
    player.OnPlayerInteracted -= OnEngineInteracted;

    player.disableInteract = true;

    yield return StartCoroutine(ShowTutorialMessageNoClick(
        "You can pick a new card to add to your hand! Select the Fix It Repair Card and press confirm. Press Space to continue."));

    HideTutorialBox();
}

    private void OnEngineInteracted()
    {
        if (gameController._currentRoom == GameController.PlayerLocation.engine)
            playerHasInteractedWithEngine = true;
    }

    private IEnumerator TeachRepairHallway()
{
    yield return new WaitForSeconds(1f);

    player.disableInteract = false; // unlock FIRST

    yield return StartCoroutine(ShowTutorialMessage(
        "It looks like the enemy damaged the hallway to Communications! Find it and press E to open the repair panel. Any hallways that have been damaged will have a bandage icon over them!"));

    yield return StartCoroutine(ShowTutorialMessage(
        "Drag your repair card onto the slot and confirm to fix it."));

    yield return new WaitUntil(() => 
        gameController._currentRoom == GameController.PlayerLocation.comms);

    player.disableInteract = true;
    HideTutorialBox();
}


    private IEnumerator TeachCommsStation()
{
    postRepairLockActive = true;
    player.disableInteract = false;

    yield return StartCoroutine(ShowTutorialMessage(
        "You made it to Communications! Notice the smoke, this station is damaged! Walk up to the station and press E to use a repair card."));

    yield return StartCoroutine(ShowTutorialMessage(
        "Drag your repair card onto the slot and confirm. Some cards can only be used in certain places, so make sure to read them carefully."));

    // Wait for comms station to be repaired
    yield return new WaitUntil(() => 
        !gameController._damagedRooms.Exists(r => r.ToLower() == "comms"));

    yield return StartCoroutine(ShowTutorialMessage(
        "Station repaired! Now use your Communications card on the station to reduce your turns needed to escape."));

        HorizontalCardHolder cardHolder = FindObjectOfType<HorizontalCardHolder>();
yield return new WaitUntil(() => cardHolder != null && cardHolder.cards.Count <= 0);


    yield return StartCoroutine(ShowTutorialMessage(
        "Station repaired! Now use your Communications card on the station to reduce your turns needed to escape."));
    HideTutorialBox();
}

    private IEnumerator TeachEndTurn()
    {
        // Unlock everything for normal play
        player.disableMovement = false;
        player.disableInteract = false;
        player.disableTab = false;

        yield return StartCoroutine(ShowTutorialMessage(
            "That's everything! Press the End Turn button when you're ready to start the real game."));
        HideTutorialBox();
    }

    private void OnCardConfirmed(Card card)
{
    if (card.cardData == null) return;

    Debug.Log("Card confirmed: " + card.cardData.cardName + 
              " | Requirement: " + card.cardData.requirement + 
              " | AllowedStation: " + card.cardData.allowedStation);

    if (card.cardData.requirement == CardRequirement.StationDamaged)
        hallwayRepaired = true;

    if (card.cardData.allowedStation == StationType.Comms)
        commsStationUsed = true;
}

    private void SetupTutorialDamage()
    {
        if (engineToCommsPassageInteractable != null)
            engineToCommsPassageInteractable.ToggleDamage(true);

        if (engineToCommsPassage != null)
        {
            engineToCommsPassage.passage?.ToggleDamage(true);
            ForceDamageViaEvent(5);
        }

        if (commsRoom != null)
            ForceDamageViaEvent(0);
    }

    private void ForceDamageViaEvent(int id)
    {
        string roomName = RoomIdToName(id);
        if (!string.IsNullOrEmpty(roomName) &&
            !gameController._damagedRooms.Exists(r => r.ToLower() == roomName.ToLower()))
        {
            gameController._damagedRooms.Add(roomName);
        }
        gameController.TriggerDamageRoom(id);
    }

    private string RoomIdToName(int id)
    {
        if (id < gameController._rooms.Count)
            return gameController._rooms[id];
        return string.Empty;
    }

    private void SetupLockedPassages()
    {
        foreach (PassageInteractable passage in permanentlyLockedPassages)
        {
            if (passage == null) continue;
            passage.DisableInteraction();
            passage.enabled = false;
        }

        foreach (PassageInteractable passage in postRepairLockedPassages)
        {
            if (passage == null) continue;
            passage.DisableInteraction();
            passage.enabled = false;
            passage.gameObject.AddComponent<TutorialLockedPassage>()
                .Initialize(lockedMessageText, wrongPassageMessage, lockedMessageDuration, this,
                    () => postRepairLockActive);
        }
    }

    private void OverrideCardPicker()
    {
        if (cardPickerUI == null) return;
        TutorialCardPickerOverride overrider =
            cardPickerUI.gameObject.AddComponent<TutorialCardPickerOverride>();
        overrider.Initialize(cardPickerUI, tutorialRepairCard, tutorialCommsCard);
    }

    public void OnEndTurnPressed()
    {
        if (transitionTriggered) return;
        transitionTriggered = true;
        StartCoroutine(TransitionToMain());
    }

    private IEnumerator TransitionToMain()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(mainSceneName);
    }

    private IEnumerator ShowTutorialMessage(string message)
{
    if (tutorialBox != null) tutorialBox.SetActive(true);
    if (tutorialText != null) tutorialText.text = message;
    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) ||
                                     Input.GetMouseButtonDown(0) ||
                                     Input.GetKeyDown(KeyCode.Tab) ||
                                     Input.GetKeyDown(KeyCode.E));
    yield return null;
}

private IEnumerator ShowTutorialMessageNoClick(string message)
{
    if (tutorialBox != null) tutorialBox.SetActive(true);
    if (tutorialText != null) tutorialText.text = message;
    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) ||
                                     Input.GetKeyDown(KeyCode.Tab));
    yield return null;
}

    private void HideTutorialBox()
    {
        if (tutorialBox != null) tutorialBox.SetActive(false);
        if (tutorialArrow != null) tutorialArrow.SetActive(false);
    }

    private void ShowOverlay(bool show)
    {
        if (tutorialOverlay != null) tutorialOverlay.SetActive(show);
    }

    public IEnumerator ShowLockedMessage()
    {
        if (lockedMessageText == null) yield break;
        lockedMessageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(lockedMessageDuration);
        lockedMessageText.gameObject.SetActive(false);
    }
}