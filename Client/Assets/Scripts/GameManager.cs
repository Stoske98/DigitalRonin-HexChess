using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Playables;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public Action<Hex> OnSelectUnit;

    //private PlayerInputHandler input_handler;

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
           /* if (game != null)
            {
                List<Unit> units = game.object_manager.objects.OfType<Unit>().ToList();
                foreach (Unit unit in units)
                {
                    Debug.Log(unit.class_type.ToString() + "_" + unit.unit_type.ToString());
                }
            }*/
        }
        else if (Input.GetKeyDown(KeyCode.K))
            Save();
    }
}