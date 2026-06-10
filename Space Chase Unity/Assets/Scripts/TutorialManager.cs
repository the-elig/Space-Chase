using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{

    [Header("Scene Transition")]
    [SerializeField] private string mainSceneName = "Main Scene";

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

    [Header("Tutorial Highlight")]
    [SerializeField] private GameObject tutorialHighlight;

    [Header("UI Tour Targets")]
    [SerializeField] private RectTransform currentRoomUI;
    [SerializeField] private RectTransform turnsLeftUI;
    [SerializeField] private RectTransform energyUI;
    [SerializeField] private RectTransform damageUI;
    [SerializeField] private RectTransform seeDeckUI;
    [SerializeField] private RectTransform endTurnUI;

    [Header("Station References")]
    [SerializeField] private RoomStationInteractable engineStation;

    private bool transitionTriggered = false;
    private bool playerHasMoved = false;
    private bool playerHasInteractedWithEngine = false;
    private bool hallwayRepaired = false;
    private bool commsStationUsed = false;
    private bool postRepairLockActive = false;
    private int turnsLeftAtCommsStart;

    public Shake Shake;

    void Awake()
    {
        CanvasController canvas = FindObjectOfType<CanvasController>();
        if (canvas != null) canvas.isTutorial = true;
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();
        if (player == null)
            player = FindObjectOfType<PlayerMovement>();

        gameController.skipEnemyTurnOnStart = true;
    }

    void Start()
    {
        if (engineToCommsPassageInteractable != null) 
            engineToCommsPassageInteractable.isTutorial = true;
        
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

        if (engineToCommsPassageInteractable != null)
        {
            engineToCommsPassageInteractable.enabled = true;
            engineToCommsPassageInteractable.EnableInteraction();
        }
    
        yield return StartCoroutine(TeachRepairHallway());
        yield return StartCoroutine(TeachCommsStation());
        yield return StartCoroutine(TeachEndTurn());
    }

    private IEnumerator UITour()
    {
        ShowOverlay(true);

        yield return StartCoroutine(ShowTutorialMessage(
            "Welcome to Space Chase, Captain! Let's get you familiar with the interface. Press Space to continue."));

        HighlightElement(currentRoomUI);

        yield return StartCoroutine(ShowTutorialMessage(
            "In the top left corner you will find your Current Room. This tells you where you are on the ship, and may come in handy when navigating to a different room."));

        HighlightElement(turnsLeftUI);

        yield return StartCoroutine(ShowTutorialMessage(
            "The top right shows your Turns Left. This is how long you have to survive before a rescue ship responds to your distress calls."));

        HighlightElement(energyUI);

        yield return StartCoroutine(ShowTutorialMessage(
            "In the middle is your Energy. You spend energy to use cards and move through hallways, and you have a limited amount of energy each turn."));

        HighlightElement(damageUI);

        yield return StartCoroutine(ShowTutorialMessage(
            "Below that is your ship's current damage. If 7 or more rooms and passageways are damaged, the ship will explode! Be careful monitoring this!"));

        HideHighlight();
        ShowOverlay(false);
        HideTutorialBox();
    }

    private IEnumerator TeachMovement()
    {
        yield return StartCoroutine(ShowTutorialMessageWASD(
            "Use WASD to move around the ship. Try it out now!"));

        player.disableMovement = false;
        player.OnPlayerMoved += OnFirstMove;

        yield return new WaitUntil(() => playerHasMoved);
        player.OnPlayerMoved -= OnFirstMove;

        yield return StartCoroutine(ShowTutorialMessage(
            "Great! You've got the hang of moving around."));
        HideTutorialBox();
    }

    private void OnFirstMove() => playerHasMoved = true;

    private IEnumerator TeachMap()
{
    if (mapUI != null) mapUI.SetActive(false);

    player.disableTab = false;

    yield return StartCoroutine(ShowTutorialMessage(
        "Your map shows all rooms and their current status! If a room or hallway has a hazard symbol, it means it has been damaged in an enemy attack! Press Tab to open and close the ship map."));

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

    yield return StartCoroutine(ShowTutorialMessageE(
        "See the Engine station over there? Walk up to it and press E to interact. To interact with any station or hallway aboard your ship, press E."));

    yield return new WaitUntil(() => playerHasInteractedWithEngine);
    player.OnPlayerInteracted -= OnEngineInteracted;

    player.disableInteract = true;

    HorizontalCardHolder cardHolder = FindObjectOfType<HorizontalCardHolder>(true);
int cardCountBefore = cardHolder != null ? cardHolder.cards.Count : 0;

if (tutorialBox != null) tutorialBox.SetActive(true);
if (tutorialText != null) tutorialText.text = "You can pick a new card to add to your hand! Select the Fix It Repair Card and press confirm.";

yield return new WaitUntil(() => cardHolder != null && cardHolder.cards.Count > cardCountBefore);

HideTutorialBox();

        Shake.ShakeWrap();

        HideTutorialBox();

        OnEngineInteracted();
}

    private void OnEngineInteracted()
    {
    if (gameController._currentRoom == GameController.PlayerLocation.engine 
        && engineStation.playerInRange)
        playerHasInteractedWithEngine = true;
}

    private bool playerHasOpenedPassage = false;

    private IEnumerator TeachRepairHallway()
{

    player.disableInteract = false;

    if (tutorialBox != null) tutorialBox.SetActive(true);
    if (tutorialText != null) tutorialText.text = "It looks like the enemy damaged the hallway to Communications! Find it and press E to open the repair panel. Any hallways that have been damaged will have a bandage icon over them!";

    // Wait until player is in the passage range AND presses E
    yield return new WaitUntil(() => 
        engineToCommsPassageInteractable != null && 
        engineToCommsPassageInteractable.playerInRange &&
        GameObject.Find("StationPanel") != null &&
        GameObject.Find("StationPanel").activeSelf);

    HideTutorialBox();

    yield return StartCoroutine(ShowTutorialMessage(
        "Drag your repair card onto the slot and confirm to fix it. Then walk through the hallway."));

    HideTutorialBox();

    yield return new WaitUntil(() =>
    gameController._currentRoom == GameController.PlayerLocation.comms);

    player.disableInteract = true;
}


    private IEnumerator TeachCommsStation()
{
    postRepairLockActive = true;
    player.disableInteract = false;

    yield return StartCoroutine(ShowTutorialMessage(
        "You made it to Communications! Notice the smoke? This station is damaged! Walk up to the station and press E to use a repair card."));

    if (tutorialBox != null) tutorialBox.SetActive(true);
    if (tutorialText != null) tutorialText.text = "Drag your repair card onto the slot and confirm. Some cards can only be used in certain places, so make sure to read them carefully.";

    yield return new WaitUntil(() => 
        !gameController._damagedRooms.Exists(r => r.ToLower() == "comms"));

    player.disableInteract = false;

    turnsLeftAtCommsStart = gameController._turnsLeft;

    if (tutorialText != null) tutorialText.text = "Station repaired! Now use your Communications card on the station to reduce your turns needed to escape.";

    yield return new WaitUntil(() => 
        gameController._turnsLeft < turnsLeftAtCommsStart);

    player.disableInteract = true;

    yield return StartCoroutine(ShowTutorialMessage(
        "Well done! Some cards only work in specific rooms, so always read them carefully before using them! Press space to continue."));

    yield return StartCoroutine(ShowTutorialMessage(
        "If you ever forget what cards you currently have, press the See Deck button in the right corner to view your inventory!"));

    yield return StartCoroutine(ShowTutorialMessage(
        "Once you run out of energy, you cannot use stations or move between rooms. You will have to press the End Turn button to move on to the next turn. Press space to continue."));
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
        if (engineToCommsPassageInteractable != null)
        {
            engineToCommsPassageInteractable.DisableInteraction();
            engineToCommsPassageInteractable.enabled = false;
        }

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
    HideTutorialBox();
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
                                     Input.GetKeyDown(KeyCode.Tab));
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

private IEnumerator ShowTutorialMessageWASD(string message)
{
    if (tutorialBox != null) tutorialBox.SetActive(true);
    if (tutorialText != null) tutorialText.text = message;
    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.W) ||
                                     Input.GetKeyDown(KeyCode.A)||
                                     Input.GetKeyDown(KeyCode.S)||
                                     Input.GetKeyDown(KeyCode.D));
    yield return null;
}

private IEnumerator ShowTutorialMessageE(string message)
{
    if (tutorialBox != null) tutorialBox.SetActive(true);
    if (tutorialText != null) tutorialText.text = message;
    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
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

    private void ReenableEngineToCommsPassage()
    {
        if (engineToCommsPassageInteractable == null) return;
        engineToCommsPassageInteractable.enabled = true;
    }

    private void HighlightElement(RectTransform target)
    {
        if (tutorialHighlight == null || target == null) return;
        tutorialHighlight.SetActive(true);
        RectTransform highlightRect = tutorialHighlight.GetComponent<RectTransform>();
        highlightRect.position = target.position;
        highlightRect.sizeDelta = target.sizeDelta + new Vector2(20, 20);
    }

    private void HideHighlight()
    {
        if (tutorialHighlight != null)
            tutorialHighlight.SetActive(false);
    }
}