using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class CustomConverters
{
    public enum GameObjectEnum
    {
        HEX = 0,

        LIGHT_SWORDSMAN = 1,
        DARK_SWORDSMAN = 2,
        LIGHT_ARCHER = 3,
        DARK_ARCHER = 4,
        LIGHT_KNIGHT = 5,
        DARK_KNIGHT = 6,
        LIGHT_TANK = 7,
        DARK_TANK = 8,
        LIGHT_JESTER = 9,
        DARK_JESTER = 10,
        LIGHT_WIZARD = 11,
        DARK_WIZARD = 12,
        LIGHT_QUEEN = 13,
        DARK_QUEEN = 14,
        LIGHT_KING = 15,
        DARK_KING = 16,

        TRAP = 17,
        LIGHT_STONE = 18,
        DARK_STONE = 19,
    }
    public class GameConverter : JsonConverter<Game>
    {
        public override void WriteJson(JsonWriter writer, Game value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Type");
            writer.WriteValue(value.GetType().Name);

            writer.WritePropertyName("Value");
            serializer.Serialize(writer, value);

            writer.WriteEndObject();
        }

        public override Game ReadJson(JsonReader reader, Type objectType, Game existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            var type = jsonObject.GetValue("Type").ToString();
            var value = jsonObject.GetValue("Value").CreateReader();
            var obj = serializer.Deserialize(value, Type.GetType(type)) as Game;

            return obj;
        }

    }
    public class MapConverter : JsonConverter<Map>
    {
        public override void WriteJson(JsonWriter writer, Map value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Type");
            writer.WriteValue(value.GetType().Name);

            writer.WritePropertyName("Value");
            serializer.Serialize(writer, value);

            writer.WriteEndObject();
        }

        public override Map ReadJson(JsonReader reader, Type objectType, Map existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            var type = jsonObject.GetValue("Type").ToString();
            var value = jsonObject.GetValue("Value").CreateReader();
            var obj = serializer.Deserialize(value, Type.GetType(type)) as Map;

            return obj;
        }
    }
    public class ObjectListConverter : JsonConverter<List<IObject>>
    {
        public override void WriteJson(JsonWriter writer, List<IObject> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (IObject obj in value)
            {
                JObject jo = new JObject();
                jo.Add("Type", obj.GetType().Name);
                jo.Add("Value", JToken.FromObject(obj, serializer));

                jo.WriteTo(writer);
            }

            writer.WriteEndArray();
        }
        public override List<IObject> ReadJson(JsonReader reader, Type objectType, List<IObject> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj_list = new List<IObject>();

            JArray jsonArray = JArray.Load(reader);

            foreach (JObject jObject in jsonArray.Children<JObject>())
            {
                var type = jObject.GetValue("Type").ToString();
                var value = jObject.GetValue("Value").CreateReader();
                var obj = serializer.Deserialize(value, Type.GetType(type)) as IObject;
                obj_list.Add(obj);
            }

            return obj_list;
        }
    }
    public class DictionaryConverter : JsonConverter<Dictionary<KeyCode, Behaviour>>
    {
        public override void WriteJson(JsonWriter writer, Dictionary<KeyCode, Behaviour> dictionary, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (var element in dictionary)
            {
                JObject jo = new JObject();

                jo.Add("KeyCode", element.Key.ToString());
                jo.Add("Type", element.Value.GetType().Name);
                jo.Add("Value", JToken.FromObject(element.Value, serializer));

                jo.WriteTo(writer);
            }

            writer.WriteEndArray();
        }

        public override Dictionary<KeyCode, Behaviour> ReadJson(JsonReader reader, Type objectType, Dictionary<KeyCode, Behaviour> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj_list = new Dictionary<KeyCode, Behaviour>();

            JArray jsonArray = JArray.Load(reader);

            foreach (JObject jObject in jsonArray.Children<JObject>())
            {
                var str_key_code = jObject.GetValue("KeyCode").ToString();
                Enum.TryParse(str_key_code, out KeyCode enum_value);

                var type = jObject.GetValue("Type").ToString();
                var value = jObject.GetValue("Value").CreateReader();
                var obj = serializer.Deserialize(value, Type.GetType(type)) as Behaviour;

                obj_list.Add(enum_value, obj);
            }

            return obj_list;
        }
    }
    public class BehaviourListConverter : JsonConverter<List<Behaviour>>
    {
        public override void WriteJson(JsonWriter writer, List<Behaviour> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (Behaviour obj in value)
            {
                JObject jo = new JObject();
                jo.Add("Type", obj.GetType().Name);
                jo.Add("Value", JToken.FromObject(obj, serializer));

                jo.WriteTo(writer);
            }

            writer.WriteEndArray();
        }
        public override List<Behaviour> ReadJson(JsonReader reader, Type objectType, List<Behaviour> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj_list = new List<Behaviour>();

            JArray jsonArray = JArray.Load(reader);

            foreach (JObject jObject in jsonArray.Children<JObject>())
            {
                var type = jObject.GetValue("Type").ToString();
                var value = jObject.GetValue("Value").CreateReader();
                var obj = serializer.Deserialize(value, Type.GetType(type)) as Behaviour;
                obj_list.Add(obj);
            }

            return obj_list;
        }
    }
    public class CCListConverter : JsonConverter<List<CC>>
    {
        public override void WriteJson(JsonWriter writer, List<CC> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (CC obj in value)
            {
                JObject jo = new JObject();
                jo.Add("Type", obj.GetType().Name);
                jo.Add("Value", JToken.FromObject(obj, serializer));

                jo.WriteTo(writer);
            }

            writer.WriteEndArray();
        }
        public override List<CC> ReadJson(JsonReader reader, Type objectType, List<CC> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj_list = new List<CC>();

            JArray jsonArray = JArray.Load(reader);

            foreach (JObject jObject in jsonArray.Children<JObject>())
            {
                var type = jObject.GetValue("Type").ToString();
                var value = jObject.GetValue("Value").CreateReader();
                var obj = serializer.Deserialize(value, Type.GetType(type)) as CC;
                obj_list.Add(obj);
            }

            return obj_list;
        }
    }
    public class GameObjectConverter : JsonConverter<GameObject>
    {
        public override void WriteJson(JsonWriter writer, GameObject value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("$type");
            writer.WriteValue("Game Object");

            writer.WritePropertyName("Tag");
            serializer.Serialize(writer, value.tag);

            writer.WritePropertyName("Name");
            serializer.Serialize(writer, value.name);

            writer.WritePropertyName("Position");
            serializer.Serialize(writer, value.transform.position);

            writer.WritePropertyName("Rotation");
            serializer.Serialize(writer, value.transform.rotation);

            writer.WriteEndObject();
        }

        public override GameObject ReadJson(JsonReader reader, Type objectType, GameObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject json_object = JObject.Load(reader);

            JToken tag_token = json_object["Tag"];
            string tag = tag_token.ToObject<string>();

            JToken name_token = json_object["Name"];
            string name = name_token.ToObject<string>();
            Debug.Log("Name: " +name+"TAG: " + tag);

            JToken positionToken = json_object.GetValue("Position");
            Vector3 position = positionToken.ToObject<Vector3>();

            JToken rotationToken = json_object.GetValue("Rotation");
            Quaternion rotation = rotationToken.ToObject<Quaternion>();

            if (Enum.TryParse(tag, out GameObjectEnum value))
            {
                switch (value)
                {
                    case GameObjectEnum.HEX:
                        return CreateHex(tag, name, position, rotation);
                    case GameObjectEnum.LIGHT_SWORDSMAN:
                        return CreateUnitGameObjectByPath(ClassType.Light, UnitType.Swordsman, position, rotation);
                    case GameObjectEnum.DARK_SWORDSMAN:
                        return CreateUnitGameObjectByPath(ClassType.Dark, UnitType.Swordsman, position, rotation);
                    case GameObjectEnum.LIGHT_ARCHER:
                        return CreateUnitGameObjectByPath(ClassType.Light, UnitType.Archer, position, rotation);
                    case GameObjectEnum.DARK_ARCHER:
                        return CreateUnitGameObjectByPath(ClassType.Dark, UnitType.Archer, position, rotation);
                    case GameObjectEnum.LIGHT_KNIGHT:
                        return CreateUnitGameObjectByPath(ClassType.Light, UnitType.Knight, position, rotation);
                    case GameObjectEnum.DARK_KNIGHT:
                        return CreateUnitGameObjectByPath(ClassType.Dark, UnitType.Knight, position, rotation);
                    case GameObjectEnum.LIGHT_TANK:
                        return CreateUnitGameObjectByPath(ClassType.Light, UnitType.Tank, position, rotation);
                    case GameObjectEnum.DARK_TANK:
                        return CreateUnitGameObjectByPath(ClassType.Dark, UnitType.Tank, position, rotation);
                    case GameObjectEnum.LIGHT_JESTER:
                        return CreateUnitGameObjectByPath(ClassType.Light, UnitType.Jester, position, rotation);
                    case GameObjectEnum.DARK_JESTER:
                        return CreateUnitGameObjectByPath(ClassType.Dark, UnitType.Jester, position, rotation);
                    case GameObjectEnum.LIGHT_WIZARD:
                        return CreateUnitGameObjectByPath(ClassType.Light, UnitType.Wizard, position, rotation);
                    case GameObjectEnum.DARK_WIZARD:
                        return CreateUnitGameObjectByPath(ClassType.Dark, UnitType.Wizard, position, rotation);
                    case GameObjectEnum.LIGHT_QUEEN:
                        return CreateUnitGameObjectByPath(ClassType.Light, UnitType.Queen, position, rotation);
                    case GameObjectEnum.DARK_QUEEN:
                        return CreateUnitGameObjectByPath(ClassType.Dark, UnitType.Queen, position, rotation);
                    case GameObjectEnum.LIGHT_KING:
                        return CreateUnitGameObjectByPath(ClassType.Light, UnitType.King, position, rotation);
                    case GameObjectEnum.DARK_KING:
                        return CreateUnitGameObjectByPath(ClassType.Dark, UnitType.King, position, rotation);
                    case GameObjectEnum.TRAP:
                        return CreateTrap(tag, name, position, rotation, ClassType.Dark);
                    case GameObjectEnum.LIGHT_STONE:
                        return CreateUnitGameObjectByPath(ClassType.Light, UnitType.Stone, position, rotation);
                    case GameObjectEnum.DARK_STONE:
                        return CreateUnitGameObjectByPath(ClassType.Dark, UnitType.Stone, position, rotation);
                    default:
                        break;
                }
            }
            return null;
        }
        private GameObject CreateTrap(string tag, string name, Vector3 position, Quaternion rotation, ClassType class_type)
        {
            GameObject game_object = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Trap/prefab"));

            game_object.tag = tag;
            game_object.name = name;
            game_object.transform.position = position;
            game_object.transform.rotation = rotation;

            return game_object;
        }
        private GameObject CreateHex(string tag, string name, Vector3 position, Quaternion rotation)
        {
            GameObject game_object = new GameObject();

            game_object.tag = tag;
            game_object.name = name;
            game_object.transform.position = position;
            game_object.transform.rotation = rotation;

            game_object.AddComponent<MeshFilter>();
            game_object.AddComponent<MeshRenderer>();

            Mesh mesh = game_object.GetComponent<MeshFilter>().mesh;
            mesh.Clear();

            float angle = 0;
            Vector3[] vertices = new Vector3[7];
            vertices[0] = Vector3.zero;
            for (int i = 1; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0) * 1;
                angle += 60;

            }
            mesh.vertices = vertices;

            mesh.triangles = new int[]
            {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6,
            0, 6, 1
            };

            mesh.RecalculateNormals();

            game_object.transform.SetParent(MapContainer.Instance.fields_container);
            return game_object;
        }
        public GameObject CreateUnitGameObjectByPath(ClassType class_type, UnitType unit_type, Vector3 position, Quaternion rotation)
        {
            GameObject game_object = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/" + unit_type.ToString() + "/" + class_type.ToString() + "/prefab"));
            game_object.name = class_type.ToString() + "_" + unit_type.ToString();
            game_object.transform.position = position;
            game_object.transform.rotation = rotation;
            game_object.transform.SetParent(MapContainer.Instance.units_container);
            return game_object;
        }
    }
    public class Vector2IntConverter : JsonConverter<Vector2Int>
    {
        public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("$type");
            writer.WriteValue("Vector2Int");

            writer.WritePropertyName("X");
            writer.WriteValue(value.x);
            writer.WritePropertyName("Y");
            writer.WriteValue(value.y);

            writer.WriteEndObject();
        }

        public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            int x = jsonObject.GetValue("X").ToObject<int>();
            int y = jsonObject.GetValue("Y").ToObject<int>();

            return new Vector2Int(x, y);
        }

    }
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("$type");
            writer.WriteValue("Vector3");

            writer.WritePropertyName("X");
            writer.WriteValue(value.x);
            writer.WritePropertyName("Y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("Z");
            writer.WriteValue(value.z);

            writer.WriteEndObject();
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            float x = jsonObject.GetValue("X").ToObject<float>();
            float y = jsonObject.GetValue("Y").ToObject<float>();
            float z = jsonObject.GetValue("Z").ToObject<float>();

            return new Vector3(x, y, z);
        }

    }
    public class QuaternionConverter : JsonConverter<Quaternion>
    {
        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("$type");
            writer.WriteValue("Quaternion");

            writer.WritePropertyName("X");
            writer.WriteValue(value.x);
            writer.WritePropertyName("Y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("Z");
            writer.WriteValue(value.z);
            writer.WritePropertyName("W");
            writer.WriteValue(value.w);

            writer.WriteEndObject();
        }

        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            float x = jsonObject.GetValue("X").ToObject<float>();
            float y = jsonObject.GetValue("Y").ToObject<float>();
            float z = jsonObject.GetValue("Z").ToObject<float>();
            float w = jsonObject.GetValue("W").ToObject<float>();

            return new Quaternion(x, y, z, w);
        }
    }
    public class FloatQueueConverter : JsonConverter<Queue<float>>
    {
        public override void WriteJson(JsonWriter writer, Queue<float> value, JsonSerializer serializer)
        {
            JArray array = new JArray(value);
            array.WriteTo(writer);
        }

        public override Queue<float> ReadJson(JsonReader reader, Type objectType, Queue<float> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Queue<float> value = new Queue<float>();
            JArray jsonArray = JArray.Load(reader);

            foreach (var item in jsonArray)
                value.Enqueue((float)item);


            return value;
        }
    }
    public class StringQueueConverter : JsonConverter<Queue<string>>
    {
        public override void WriteJson(JsonWriter writer, Queue<string> value, JsonSerializer serializer)
        {
            JArray array = new JArray(value);
            array.WriteTo(writer);
        }

        public override Queue<string> ReadJson(JsonReader reader, Type objectType, Queue<string> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Queue<string> value = new Queue<string>();
            JArray jsonArray = JArray.Load(reader);

            foreach (var item in jsonArray)
                value.Enqueue((string)item);


            return value;
        }
    }
}
