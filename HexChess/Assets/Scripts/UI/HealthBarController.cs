using UnityEngine;

public class HealthBarController 
{
    GameObject health_bar_canvas;
    MOBAEnergyBar health_bar;
    Camera cam;
    Unit unit; 
    Vector3 offset;
    public bool is_active;
    public HealthBarController(Unit _unit, float y_offset,Camera _cam) 
    {
        unit = _unit;
        cam = _cam;
        offset = new Vector3(0,y_offset,0);
        is_active = false;
        health_bar_canvas = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/HealthBar"), unit.game_object.transform.position + offset, unit.game_object.transform.rotation);
        health_bar_canvas.GetComponent<Canvas>().worldCamera = _cam;
        health_bar_canvas.transform.SetParent(unit.game_object.transform);
        health_bar = health_bar_canvas.GetComponentInChildren<MOBAEnergyBar>();
        health_bar.MaxValue = unit.stats.max_health;
        health_bar.SetValueNoBurn(unit.stats.current_health);
        health_bar_canvas.SetActive(is_active);
        _unit.events.OnRecieveDamage_Local += OnReceiveDamage;
    }

    private void OnReceiveDamage(Hex hex)
    {
        health_bar.Value = unit.stats.current_health;
    }

    public void Update()
    {
        LookInCamera();
    }
    public void OnUpgrade()
    {
        health_bar.MaxValue = unit.stats.max_health;
        health_bar.SetValueNoBurn(unit.stats.current_health);
    }
    public void ShowHealthBar()
    {
        is_active = true;
        health_bar_canvas.SetActive(is_active);
        health_bar.MaxValue = unit.stats.max_health;
        health_bar.SetValueNoBurn(unit.stats.current_health);
    }
    public void HideHealthBar()
    {
        is_active = false;
        health_bar_canvas.SetActive(is_active);
    }
    private void LookInCamera()
    {
        health_bar_canvas.transform.position = health_bar_canvas.transform.parent.position + offset;
        health_bar_canvas.transform.rotation = Quaternion.LookRotation(-cam.transform.forward, cam.transform.up);
    }
}
