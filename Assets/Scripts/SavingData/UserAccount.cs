using System;

public class UserAccount
{
    private readonly string _username;
    private int _highScore;
    public GameSave Save;

    public UserAccount(string username)
    {
        _username = username;
        _highScore = 0;
        Save = null;
    }

    public string getUsername()
    {
        return _username;
    }

    public int getHighscore()
    {
        return _highScore;
    }
    
    public void UpdateHighScore(int score)
    {
        _highScore = Math.Max(score, _highScore);
    }
}
