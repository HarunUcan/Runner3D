public class CharacterStats
{
    public int coinCount;
    public int highScore;


    public CharacterStats()
    {
        this.coinCount = PlayerStats.coinCount;
        this.highScore = PlayerStats.highScore;
    }
}

