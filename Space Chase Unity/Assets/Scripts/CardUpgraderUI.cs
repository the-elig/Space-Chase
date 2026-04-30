using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardUpgraderUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameController gameController;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Deck deck;
    [SerializeField] private GameObject cardUpgraderPanel;
    [SerializeField] private UpgradeCardSlot slot1;
    [SerializeField] private UpgradeCardSlot slot2;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private TMP_Text messageText;

    private HorizontalCardHolder cardHolder;
    private GameObject stationPanel;
    void Start()
    {
        player.LeftStation += CloseCardUpgrader;
        cardHolder = FindObjectOfType<HorizontalCardHolder>(true);
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();

        confirmButton.SetActive(false);
    }
    void Update()
    {

    }
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

            // hide station specific elements
            GameObject trippy = stationPanel.transform.Find("Trippy-BG")?.gameObject;
            if (trippy != null) trippy.SetActive(true);

            GameObject weaponSlot = stationPanel.transform.Find("WeaponSlot")?.gameObject;
            if (weaponSlot != null) weaponSlot.SetActive(false);

            GameObject crt = stationPanel.transform.Find("CRT")?.gameObject;
            if (crt != null) crt.SetActive(true);

            GameObject border = stationPanel.transform.Find("scBorder640x360")?.gameObject;
            if (border != null) border.SetActive(true);
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

                GameObject trippy = stationPanel.transform.Find("Trippy-BG")?.gameObject;
                if (trippy != null) trippy.SetActive(true);

                GameObject weaponSlot = stationPanel.transform.Find("WeaponSlot")?.gameObject;
                if (weaponSlot != null) weaponSlot.SetActive(true);

                GameObject crt = stationPanel.transform.Find("CRT")?.gameObject;
                if (crt != null) crt.SetActive(true);

                GameObject border = stationPanel.transform.Find("scBorder640x360")?.gameObject;
                if (border != null) border.SetActive(true);

                stationPanel.SetActive(false);
            }
        }
    }
    
    public void AllowUpgrade()
    {
        if(slot1.GetCardName() == slot2.GetCardName())
        {
            Debug.Log("upgrade confirmed!");
            confirmButton.SetActive(true);
        }

    }
}
