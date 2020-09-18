using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : Subject
{
    [SerializeField]
    private string poiName;

    private void Start()
    {
        RegisterObserver(AchievementSystem.instance);
    }

    private void OnDisable()
    {
        Notify(poiName);
    }
}
