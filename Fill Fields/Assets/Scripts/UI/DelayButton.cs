using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DelayButton : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => ButtonDelay());
    }

    private void ButtonDelay()
    {
        button.interactable = false;

        StartCoroutine(Wait());
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(1f);
            button.interactable = true;
        }
    }
}
