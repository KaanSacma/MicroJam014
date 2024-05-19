using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    public Image HealthBar;
    public Sprite[] healthSprites;
    public Image AmmoBar;
    public Sprite[] ammoSprites;
    
    public void UpdateHealth(float percent)
    {
        if (percent <= 0f) {
            HealthBar.sprite = healthSprites[0];
        } else if (percent <= 0.125f) {
            HealthBar.sprite = healthSprites[1];
        } else if (percent <= 0.25f) {
            HealthBar.sprite = healthSprites[2];
        } else if (percent <= 0.5f) {
            HealthBar.sprite = healthSprites[3];
        } else if (percent <= 0.75f) {
            HealthBar.sprite = healthSprites[4];
        } else if (percent <= 0.875f) {
            HealthBar.sprite = healthSprites[5];
        } else {
            HealthBar.sprite = healthSprites[6];
        }
    }
    
    public void UpdateAmmo(float percent)
    {
        if (percent <= 0f) {
            AmmoBar.sprite = ammoSprites[0];
        } else if (percent <= 0.125f) {
            AmmoBar.sprite = ammoSprites[1];
        } else if (percent <= 0.25f) {
            AmmoBar.sprite = ammoSprites[2];
        } else if (percent <= 0.5f) {
            AmmoBar.sprite = ammoSprites[3];
        } else if (percent <= 0.75f) {
            AmmoBar.sprite = ammoSprites[4];
        } else if (percent <= 0.875f) {
            AmmoBar.sprite = ammoSprites[5];
        } else {
            AmmoBar.sprite = ammoSprites[6];
        }
    }
}
