public class GumTileEvent : TileEvent
{
    private int matchCount;
    private int requiredAmount;

    public GumTileEvent(int amount)
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
