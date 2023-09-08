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
    public Material outer_field_material;
    private void Awake()
    {
        Instance = this;
    }
}
