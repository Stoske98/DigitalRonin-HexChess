using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnUnitDeath : MonoBehaviour
{
    public GameObject prefab_destroyed_model;

    public void Activate()
    {
        GameObject go = Instantiate(prefab_destroyed_model);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        go.transform.Rotate(Vector3.right, 90f, Space.Self);
        Destroy(go, 2);
        gameObject.SetActive(false);
    }
}
