using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //if the player hits something besides himself lmao
        if (!other.CompareTag("Player"))
        {
            ObjectStats objStats = other.GetComponent<ObjectStats>();
            //TODO: get rid of magic number. make it sword damage stat or something
            objStats.health -= 3;
        }
    }
}
