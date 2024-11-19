using UnityEngine;

public class GameView : manager
{
    private GameData gameData;

    private void Awake()
    {
        LoadGameData();
    }

    private void LoadGameData()
    {
        gameData = Resources.Load<GameData>("GameData");

        if (gameData == null)
        {
            Debug.LogError("GameData asset not found in Resources folder!");
        }
    }

    public GameData GetGameData()
    {
        return gameData;
    }
}
