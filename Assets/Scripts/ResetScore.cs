using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResetScore : MonoBehaviour
{
    private const string DATA_DELIMITER = "#!#";
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        string[] contents = new string[]
        {
            0.ToString(),
            0.ToString()
        };
        string saveString = string.Join(DATA_DELIMITER, contents);
        File.WriteAllText(Application.dataPath + "/save.txt", saveString);
    }

    
}
