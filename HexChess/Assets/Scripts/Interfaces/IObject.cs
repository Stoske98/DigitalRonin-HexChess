using UnityEngine;

public interface IObject
{
    public string id { get; set; }
    public string game_object_path { get; set; }
    public GameObject game_object { get; set; }
    public Visibility visibility { get; set; }
    public ClassType class_type { get; set; }
    public static void ObjectVisibility(IObject obj, Visibility object_visibility)
    {
        if (object_visibility == Visibility.NONE)
        {
            obj.game_object.SetActive(false);
            obj.visibility = object_visibility;
            return;
        }
        else if (object_visibility == Visibility.BOTH)
        {
            obj.game_object.SetActive(true);
            obj.visibility = object_visibility;
            return;
        }

        if (object_visibility == Visibility.LIGHT && NetworkManager.Instance.player.data.class_type == ClassType.Light)
        {
            obj.game_object.SetActive(true);
            obj.visibility = object_visibility;
            return;
        }
        else if(object_visibility == Visibility.LIGHT && NetworkManager.Instance.player.data.class_type != ClassType.Light)
        {
            obj.game_object.SetActive(false);
            obj.visibility = object_visibility;
            return;
        }

        if (object_visibility == Visibility.DARK && NetworkManager.Instance.player.data.class_type == ClassType.Dark)
        {
            obj.game_object.SetActive(true);
            obj.visibility = object_visibility;
            return;
        }
        else if (object_visibility == Visibility.DARK && NetworkManager.Instance.player.data.class_type != ClassType.Dark)
        {
            obj.game_object.SetActive(false);
            obj.visibility = object_visibility;
            return;
        }
    }
}
public interface IDamageableObject : IObject
{
    public void ReceiveDamage(Damage damage);
    public bool IsDead();
    public void Die(Hex hex);
    bool is_immune_to_magic { get; set; }
}
public enum Visibility
{
    NONE = 0, LIGHT = 1, DARK = 2, BOTH = 3
}


