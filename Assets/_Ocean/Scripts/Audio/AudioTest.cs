using System;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using UnityEngine;

public class AudioTest : MonoBehaviour
{

    public void ButtonPlaySound()
    {
        bool b = MasterAudio.PlaySoundAndForget("NFF-gun-miss");
        if (b)
        {
            Debug.Log("playsound");
        }
    }
}
