using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject enemyShip;
    [SerializeField] private GameObject playerShip;
    private bool movePlayerShip;
    private bool moveEnemyShip;
    public void PlayGame()
    {
        StartCoroutine(PlayStartAnimation());
    }

    void FixedUpdate()
    {
        if(movePlayerShip) playerShip.transform.position += new Vector3(0.12f, -0.075f, 0f);
        if(moveEnemyShip) enemyShip.transform.position += new Vector3(0.18f, -0.1f, 0f);
    }

    IEnumerator PlayStartAnimation()
    {
        movePlayerShip = true;
        yield return new WaitForSeconds(0.5f);
        moveEnemyShip = true;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
