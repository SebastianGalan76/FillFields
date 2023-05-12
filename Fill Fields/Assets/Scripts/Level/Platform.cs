using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
    public int positionIndex;
    public PlatformStatus status;

    public Animator platform;
    public Animator block;

    public void ShowPlatform() {
        platform.SetBool("showPlatform", true);
    }

    public void HidePlatform() {
        platform.SetBool("showPlatform", false);
    }

    public void ShowFillBlock() {
        status = PlatformStatus.FILLED;

        block.SetBool("showBlock", true);
    }
    public void HideFillBlock() {
        status = PlatformStatus.UNLOCKED;

        block.SetBool("showBlock", false);
    }
}
