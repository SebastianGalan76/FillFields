using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using GoogleMobileAds.Api;

public class UIError : MonoBehaviour
{
    private static UIError instance;

    private Text errorText;
    private Animator errorAnim;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            MobileAds.Initialize(initStatus => { });

            errorText = transform.GetChild(0).GetComponent<Text>();
            errorAnim = GetComponent<Animator>();
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void Show(string error)
    {
        errorText.text = error;
        errorAnim.Play("ShowError");
    }

    public static UIError GetInstance() {
        return instance;
    }
}
