using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSystemWithEvents : MonoBehaviour
{
    public Image achievementBanner;
    public Text achievementText;

    TileEvent cookiesEvent, cakeEvent, gumEvent, lolipopEvent;

    private void Start()
    {
        PlayerPrefs.DeleteAll();

        cookiesEvent = new CookiesTileEvent(15);
        cakeEvent = new CakeTileEvent(20);
        gumEvent = new GumTileEvent(12);
        lolipopEvent = new LolipopTileEvent(7);

        PointOfInterestWithEvents.OnPointOfEventsEntered += PointOfInterestWithEvents_OnPointOfEventsEntered;
    }

    private void PointOfInterestWithEvents_OnPointOfEventsEntered(PointOfInterestWithEvents poi)
    {
        string achievementKey = "Achievement " + poi.PoiName;

        string key;

        if (poi.PoiName.Equals("Cookies event"))
        {
            cookiesEvent.OnMatch();
            if (cookiesEvent.AchievementCompleted())
            {
                key = "Match 15 cookies";
                NotifyAchievement(key, poi.PoiName);
            }
        }

        if (poi.PoiName.Equals("Cake event"))
        {
            cakeEvent.OnMatch();
            if (cakeEvent.AchievementCompleted())
            {
                key = "Match 20 cakes";
                NotifyAchievement(key, poi.PoiName);
            }
        }

        if (poi.PoiName.Equals("Gum event"))
        {
            gumEvent.OnMatch();
            if (gumEvent.AchievementCompleted())
            {
                key = "Match 12 gums";
                NotifyAchievement(key, poi.PoiName);
            }
        }

        if(poi.PoiName.Equals("Lolipop event"))
        {
            gumEvent.OnMatch();
            if (gumEvent.AchievementCompleted())
            {
                key = "Match 7 lolipops";
                NotifyAchievement(key, poi.PoiName);
            }
        }
    }

    private void NotifyAchievement(string key, string value)
    {
        if (PlayerPrefs.GetInt(value) == 1)
            return;

        PlayerPrefs.SetInt(value, 1);
        achievementText.text = key + " Unlocked!";

        StartCoroutine(ShowAchievementBanner());
    }

    private void ActivateAchievementBanner(bool active)
    {
        achievementBanner.gameObject.SetActive(active);
    }

    private IEnumerator ShowAchievementBanner()
    {
        ActivateAchievementBanner(true);
        yield return new WaitForSeconds(2f);
        ActivateAchievementBanner(false);
    }
}
