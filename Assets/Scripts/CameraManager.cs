using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public CinemachineFreeLook flCam;
    public Slider slider;
    private void Start()
    {
        if (PlayerPrefs.GetFloat("Mouse Sensitivity") != 0)
        {
            slider.value = PlayerPrefs.GetFloat("Mouse Sensitivity");
        }
    }

    public void AdjustMouseSensitivity(float newSens)
    {
        PlayerPrefs.SetFloat("Mouse Sensitivity", newSens);
        flCam.m_XAxis.m_MaxSpeed = newSens;
        flCam.m_YAxis.m_MaxSpeed = newSens / 50;
    }
}
