using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class SceneTest : MonoBehaviour
{
    public MMFeedbacks LoadLevelFeedbacks;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            LoadLevelFeedbacks?.PlayFeedbacks();
        }
    }
}
