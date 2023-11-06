using RayFire;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyOnDeath : MonoBehaviour
{
    private Animator unit_anim;
    public GameObject unit_game_object;
    public Animator invert_anim;
    public RayfireShatter ray_fire;
    public Material material;
    public Vector3 scale;

    public void Start()
    {

        Invoke("Destroy", 0.01f);
    }
    public void Destroy()
    {

        unit_anim = GetComponent<Animator>();
        unit_anim.speed = 0.0f;
        invert_anim.speed = 0.0f;

        ray_fire.material.innerMaterial = material;
        ray_fire.Fragment();
        unit_game_object.SetActive(false);
        GameObject mesh_object = ray_fire.gameObject;
        mesh_object.GetComponent<SkinnedMeshRenderer>().enabled = false;
        GameObject lastChild = invert_anim.transform.GetChild(invert_anim.transform.childCount - 1).gameObject;
        lastChild.transform.localScale = scale;
        lastChild.transform.localRotation = Quaternion.identity;
        lastChild.transform.Rotate(Vector3.right, -90f, Space.Self);

        /*invert_anim.gameObject.SetActive(true);
        var currentAnimatorState = unit_anim.GetCurrentAnimatorStateInfo(0);
        invert_anim.Play(currentAnimatorState.fullPathHash, 0, currentAnimatorState.normalizedTime);

        ray_fire.material.innerMaterial = material;
        ray_fire.Fragment();
        unit_game_object.SetActive(false);
        GameObject mesh_object = ray_fire.gameObject;
        mesh_object.GetComponent<SkinnedMeshRenderer>().enabled = false;
        GameObject lastChild = invert_anim.transform.GetChild(invert_anim.transform.childCount - 1).gameObject;
        lastChild.transform.localScale = scale;
        lastChild.transform.localRotation = Quaternion.identity;
        lastChild.transform.Rotate(Vector3.right, -90f, Space.Self);*/

        /*foreach (Transform t in lastChild.transform)
        {
            t.AddComponent<Rigidbody>();
            MeshCollider collider = t.AddComponent<MeshCollider>();
            collider.convex = true;
        }*/
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Destroy();
        }
    }
}
