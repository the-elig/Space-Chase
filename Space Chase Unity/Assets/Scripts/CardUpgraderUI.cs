using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardUpgraderUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameController gameController;
    [SerializeField] private CanvasController canvas;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Deck deck;
    [SerializeField] private GameObject cardUpgraderPanel;
    [SerializeField] private UpgradeCardSlot slot1;
    [SerializeField] private UpgradeCardSlot slot2;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private TMP_Text messageText;

    [SerializeField] private HorizontalCardHolder cardHolder;
    [SerializeField] private GameObject stationPanel;
    void Start()
    {
        player.LeftStation += CloseCardUpgrader;
        cardHolder = FindObjectOfType<HorizontalCardHolder>(true);
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();

        confirmButton.SetActive(false);
    }
    void Update() {}
    public void OpenCardUpgrader()
    {
        if (gameController._energy < 1)
        {
            if (messageText != null)
            {
                messageText.text = "Not enough energy!";
                messageText.gameObject.SetActive(true);
                StartCoroutine(HideMessage());
            }
            return;
        }
        cardUpgraderPanel.SetActive(true);
        // find StationPanel even if inactive
        if (stationPanel == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>(true);
            if (canvas != null)
                stationPanel = canvas.transform.Find("StationPanel")?.gameObject;
        }
        // enable StationPanel so cards work
        if (stationPanel != null)
        {
            stationPanel.SetActive(true);

            // hide weapon slot
            GameObject weaponSlot = stationPanel.transform.Find("WeaponSlot")?.gameObject;
            if (weaponSlot != null) weaponSlot.SetActive(false);
        }
    }
    IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(2f);
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }
    public void CloseCardUpgrader()
    {
        slot1.OnCancel();
        slot2.OnCancel();
        if (cardUpgraderPanel.activeSelf)
        {
            cardUpgraderPanel.SetActive(false);
            
            if (stationPanel != null)
            {
                confirmButton.SetActive(false);

                GameObject weaponSlot = stationPanel.transform.Find("WeaponSlot")?.gameObject;
                if (weaponSlot != null) weaponSlot.SetActive(true);

                stationPanel.SetActive(false);
            }
        }
    }
    
    public void AllowUpgrade()
    {
        confirmButton.SetActive(false);
        if(slot1.GetCardName() == slot2.GetCardName())
        {
            confirmButton.SetActive(true);
        }
    }

    public void ConfirmUpgrade()
    {
        // if you don't have enough energy, cancel both
        if (gameController._energy < 1)
        {
            slot1.OnCancel();
            slot2.OnCancel();
            return;
        } else
        {
            cardHolder.AddCardToHand(slot1.GetCardUpgrade());
            // delete cards and deduct energy
            slot1.OnConfirm();
            slot2.OnConfirm();
            gameController._energy -= 1;

            StartCoroutine(CloseAfterDelay());
        }
    }
    IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(0.4f);
        canvas.UIBackground(false);
        cardUpgraderPanel.SetActive(false);

        // restore hidden elements then close station panel
        if (stationPanel != null)
        {
            
            GameObject weaponSlot = stationPanel.transform.Find("WeaponSlot")?.gameObject;
            if (weaponSlot != null) weaponSlot.SetActive(true);

            stationPanel.SetActive(false);
        }

    }
}
