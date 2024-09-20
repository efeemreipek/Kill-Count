using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EscapeManager : MonoBehaviour
{
    [SerializeField] private GameObject escapeGO;
    [SerializeField] private Image escapeLarge;
    [SerializeField] private float escapeTime = 1f;

    private bool isEscPressed = false;
    private float time = 0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isEscPressed = true;
            escapeGO.SetActive(true);
        }

        if (isEscPressed && Input.GetKey(KeyCode.Escape))
        {
            HandleEscape();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isEscPressed = false;
            time = 0f; // Reset the timer
            escapeLarge.fillAmount = 0f; // Reset the fill amount
            escapeGO.SetActive(false); // Hide the escape object
        }
    }

    private void HandleEscape()
    {
        // Increment the time and update the fill amount
        if (time < escapeTime)
        {
            time += Time.deltaTime;
            escapeLarge.fillAmount = time / escapeTime;

            if (escapeLarge.fillAmount >= 1f)
            {
                // Do something when the escape is successful
                print("ESCAPED");
                escapeGO.SetActive(false);
                isEscPressed = false;
                SceneManager.LoadSceneAsync(0);
            }
        }
    }
}
