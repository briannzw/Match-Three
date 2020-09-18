using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSystem : Observer
{
    public Image achievementBanner;
    public Text achievementText;

    TileEvent cookiesEvent, cakeEvent, gumEvent;

    public static AchievementSystem instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        PlayerPrefs.DeleteAll();

        cookiesEvent = new CookiesTileEvent(3);
        cakeEvent = new CakeTileEvent(10);
        gumEvent = new GumTileEvent(5);

        //not working. (karena candy terlebih dahulu di disable sehingga tidak dapat ditemukan)
        foreach(var poi in FindObjectsOfType<PointOfInterest>())
        {
            poi.RegisterObserver(this);
        }
    }

    public override void OnNotify(string value)
    {
        string key;

        if(value.Equals("Cookies event"))
        {
            cookiesEvent.OnMatch();
            if (cookiesEvent.AchievementCompleted())
            {
                key = "Match first cookies";
                NotifyAchievement(key, value);
            }
        }

        if(value.Equals("Cake event"))
        {
            cakeEvent.OnMatch();
            if (cakeEvent.AchievementCompleted())
            {
                key = "Match 10 cakes";
                NotifyAchievement(key, value);
            }
        }

        if(value.Equals("Gum event"))
        {
            gumEvent.OnMatch();
            if (gumEvent.AchievementCompleted())
            {
                key = "Match 5 gums";
                NotifyAchievement(key, value);
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
