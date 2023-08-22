using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region GameManager Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log("Game Manager instance already exist, destroying duplicate!");
                Destroy(value);
            }
        }
    }
    #endregion
    [JsonConverter(typeof(CustomConverters.GameConverter))] public Game game;
    public MapController map_controller;
    public PlayerInputHandler player_input_handler;

    private void Awake()
    {
        Instance = this;
    }

    public void Save()
    {
        Debug.Log("GAME SAVED");
        string json = NetworkManager.Serialize(game);
        File.WriteAllText("ChallengeRoyaleGame.json", json);
    }
    void Update()
    {
        game?.Update();
        if (Input.GetKeyDown("space"))
        {
            foreach (var item in game.map.hexes)
            {
                if (!item.IsWalkable())
                    item.SetColor(Color.black);
            }
        }
        else if (Input.GetKeyDown(KeyCode.K))
            Save();
    }
}