using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public PlayerStats psScript;
    private PlayerStats _enemyScript;
    public TMP_Text healthText;
    public TMP_Text staminaText;
    public TMP_Text enemyHealthText;
    public TMP_Text enemyStaminaText;

    public TMP_Text score;
    
    private const string DATA_DELIMITER = "#!#";
    
    private void Start()
    {
        _enemyScript = FindObjectOfType<PlayerStats>();
        healthText.text = "Health: " + Math.Floor(psScript.health);
        staminaText.text = "Stamina: " + Math.Floor(psScript.stamina);
        enemyHealthText.text = "Health: 100%";
        enemyStaminaText.text = "Stamina: 100%";
        LoadGame();
    }

    private void Update()
    {
        healthText.text = "Health: " + Math.Floor(psScript.health);
        staminaText.text = "Stamina: " + Math.Floor(psScript.stamina);
        enemyHealthText.text = "Health: " + Math.Floor(_enemyScript.health / _enemyScript.healthMax * 100) + "%";
        enemyStaminaText.text = "Stamina: " + Math.Floor(_enemyScript.stamina / _enemyScript.staminaMax * 100) + "%";
        //score is actually the number of deaths so it's backwards.
        score.text = _enemyScript.score + " - " + psScript.score;
        if (_enemyScript.score >= 3)
            SceneManager.LoadScene("Win");
        if(psScript.score >= 3)
            SceneManager.LoadScene("Lose");
        Save();
    }
    
    private void Save()
    {
        string[] contents = new String[]
        {
            _enemyScript.score.ToString(),
            psScript.score.ToString()
        };
        string saveString = string.Join(DATA_DELIMITER, contents);
        File.WriteAllText(Application.dataPath + "/save.txt", saveString);
    }

    private void LoadGame()
    {
        string saveString = File.ReadAllText(Application.dataPath + "/save.txt");
        string[] contents = saveString.Split(new[] { DATA_DELIMITER }, System.StringSplitOptions.None);
        _enemyScript.score = int.Parse(contents[0]);
        psScript.score = int.Parse(contents[1]);

        score.text = _enemyScript.score + " - " + psScript.score;
    }
}
