using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameState : MonoBehaviour
{
    public PlayerStats psScript;
    public TMP_Text healthText;
    public TMP_Text staminaText;

    private void Start()
    {
        healthText.text = "Health: " + Math.Floor(psScript.health);
        staminaText.text = "Stamina: " + Math.Floor(psScript.stamina);
    }

    private void Update()
    {
        healthText.text = "Health: " + Math.Floor(psScript.health);
        staminaText.text = "Stamina: " + Math.Floor(psScript.stamina);
    }
}
