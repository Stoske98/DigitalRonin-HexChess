using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectManager
{
    [JsonRequired][JsonConverter(typeof(CustomConverters.ObjectListConverter))] public List<IObject> objects { set; get; }
    [JsonIgnore] private List<IObject> objects_to_add { set; get; }
    [JsonIgnore] private List<IObject> objects_to_remove { set; get; }
    [JsonIgnore] private List<IActiveObject> active_objects { set; get; }
    [JsonRequired][JsonConverter(typeof(CustomConverters.SubscibersListConverter))] public List<ISubscribe> subscribes { set; get; }

    public ObjectManager()
    {
        objects = new List<IObject>();
        objects_to_add = new List<IObject>();
        objects_to_remove = new List<IObject>();
        active_objects = new List<IActiveObject>();
        subscribes = new List<ISubscribe>();
    }

    public void Init()
    {
        active_objects.AddRange(objects.OfType<IActiveObject>());
    }

    public void Update()
    {
        UpdateObjects();

        ProcessPendingActions();
    }

    public bool IsObjectsWorking()
    {
        foreach (var active_obj in active_objects)
            if (active_obj.IsWork())
                return true;

        return false;
    }

    public void AddObject(IObject obj)
    {
        objects_to_add.Add(obj);
    }

    public void RemoveObject(IObject obj)
    {
        GameObject.Destroy(obj.game_object);

        /*if(obj is ISubscribe subscriber)
            subscriber.UnregisterEvents();*/
        if (obj is Unit unit)
            foreach (var b in unit.behaviours)
                if (b is ISubscribe subscriber)
                {
                    subscriber.UnregisterEvents();
                    RemoveSubscriber(subscriber);
                }

        objects_to_remove.Add(obj);
    }
    public void AddSubscriber(ISubscribe subscriber)
    {
        subscribes.Add(subscriber);
    }

    public void RemoveSubscriber(ISubscribe subscribe)
    {
        subscribes.Remove(subscribe);
    }
    private void UpdateObjects()
    {
        foreach (var active_obj in active_objects)
            active_obj.Update();
    }

    public void ProcessPendingActions()
    {
        if (objects_to_add.Count > 0)
        {
            objects.AddRange(objects_to_add);
            active_objects.AddRange(objects_to_add.OfType<IActiveObject>());

            objects_to_add.Clear();
        }

        if (objects_to_remove.Count > 0)
        {
            objects.RemoveAll(obj => objects_to_remove.Contains(obj));
            active_objects.RemoveAll(obj => objects_to_remove.Contains(obj));

            objects_to_remove.Clear();
        }
    }
}