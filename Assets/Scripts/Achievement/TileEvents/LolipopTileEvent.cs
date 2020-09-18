public class LolipopTileEvent : TileEvent
{
    private int matchCount;
    private int requiredAmount;

    public LolipopTileEvent(int amount)
    {
        requiredAmount = amount;
    }

    public override void OnMatch()
    {
        matchCount++;
    }

    public override bool AchievementCompleted()
    {
        if (matchCount == requiredAmount)
        {
            return true;
        }

        return false;
    }
}
