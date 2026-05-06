using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public bool start = false;
    public float duration = 6.0f;

    private void Start()
    {
        ShakeWrap();
    }
    // Update is called once per frame
    void Update()
    {
        if(start)
        {
            start = false;
            StartCoroutine(Shaking());
        }
    }

    IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = startPosition + Random.insideUnitSphere;
            yield return null;
        }
        transform.position = startPosition;
        Debug.Log("woah");
    }

    public void ShakeWrap()
    {
        StartCoroutine(Shaking());
    }
}
