using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PointOfInterestWithEvents : MonoBehaviour
{
    public static event Action<PointOfInterestWithEvents> OnPointOfEventsEntered;

    [SerializeField]
    private string poiName;

    public string PoiName { get => poiName; }

    private void OnDisable()
    {
        if (OnPointOfEventsEntered != null)
            OnPointOfEventsEntered(this);
    }
}
