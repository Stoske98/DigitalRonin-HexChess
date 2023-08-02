using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapContainer : MonoBehaviour
{
    #region MapContainer Singleton
    private static MapContainer _instance;
    public static MapContainer Instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log("Network Manager instance already exist, destroying duplicate!");
                Destroy(value);
            }
        }
    }
    #endregion


    public Transform fields_container;
    public Transform units_container;
    public Material field_material;
    private void Awake()
    {
        Instance = this;
    }

    /*  private void Start()
      {
          Map map = new ChallengeRoyaleMap(4, 4, 1.05f, 1);
          Game game = new ChallengeRoyaleGame(map, 30, 10);
          //map_controller.SetMap(map);

          string json = NetworkManager.Serialize(game);
          //Debug.Log(json);
          File.WriteAllText("ChallengeRoyaleGame.json", json);
      }*/

    /*private void Start()
    {
        Game game = NetworkManager.Deserialize<ChallengeRoyaleGame>(File.ReadAllText("ChallengeRoyaleGame.json"));
        NetworkManager.Instance.games.Add(2, game);
        game.Init(2);
    }*/
}
