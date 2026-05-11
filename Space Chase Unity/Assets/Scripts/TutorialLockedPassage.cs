using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialLockedPassage : MonoBehaviour
{
    private TMP_Text messageText;
    private string message;
    private float duration;
    private TutorialManager manager;
    private Func<bool> shouldShowMessage; // returns true only when message is appropriate
    private bool coroutineRunning = false;

    /// <summary>
    /// shouldShow: a lambda that returns true when this passage should show a message.
    /// Before that returns true, interaction is silently blocked.
    /// </summary>
    public void Initialize(TMP_Text text, string msg, float dur, TutorialManager mgr,
        Func<bool> shouldShow = null)
    {
        messageText = text;
        message = msg;
        duration = dur;
        manager = mgr;
        shouldShowMessage = shouldShow ?? (() => true);
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.transform.position);
        if (dist < 2.5f && !coroutineRunning && shouldShowMessage())
            StartCoroutine(ShowMessage());
    }

    private IEnumerator ShowMessage()
    {
        coroutineRunning = true;
        if (messageText != null)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            messageText.gameObject.SetActive(false);
        }
        coroutineRunning = false;
    }
}