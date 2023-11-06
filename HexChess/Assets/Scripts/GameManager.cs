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
    public Canvas main_canvas;
    public Canvas game_canvas;

    public GameObject light_death_sphere;
    public GameObject dark_death_sphere;

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        game?.Update();
        if (Input.GetKeyDown("space"))
        {
            foreach (var item in game.map.hexes)
            {
                if (!item.IsWalkable())
                    item.SetColor(Color.magenta);
            }
        }
    }
}