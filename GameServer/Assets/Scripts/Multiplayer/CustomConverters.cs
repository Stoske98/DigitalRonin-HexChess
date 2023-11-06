using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class CustomConverters
{
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
    public class SubscibersListConverter : JsonConverter<List<ISubscribe>>
    {
        public override void WriteJson(JsonWriter writer, List<ISubscribe> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (ISubscribe obj in value)
            {
                JObject jo = new JObject();
                jo.Add("Type", obj.GetType().Name);
                jo.Add("Value", JToken.FromObject(obj, serializer));

                jo.WriteTo(writer);
            }

            writer.WriteEndArray();
        }
        public override List<ISubscribe> ReadJson(JsonReader reader, Type objectType, List<ISubscribe> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj_list = new List<ISubscribe>();

            JArray jsonArray = JArray.Load(reader);

            foreach (JObject jObject in jsonArray.Children<JObject>())
            {
                var type = jObject.GetValue("Type").ToString();
                var value = jObject.GetValue("Value").CreateReader();
                var obj = serializer.Deserialize(value, Type.GetType(type)) as ISubscribe;
                obj_list.Add(obj);
            }

            return obj_list;
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

                string name = obj.game_object.name;
                Vector3 position = obj.game_object.transform.position;
                Quaternion rotation = obj.game_object.transform.rotation;

                UnityEngine.Object.Destroy(obj.game_object);

                obj.game_object = GameObject.Instantiate(Resources.Load<GameObject>(obj.game_object_path), position, rotation);
                obj.game_object.name = name;

                if (obj is Unit unit)
                    unit.sprite = Resources.Load<Sprite>(unit.sprite_path);
                else if (obj is Flag flag)
                    if(flag.is_ocuppied)
                        flag.OccupiedField();


                obj.game_object.transform.SetParent(MapContainer.Instance.fields_container);

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

                obj.sprite = Resources.Load<Sprite>(obj.sprite_path);
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

                obj.sprite = Resources.Load<Sprite>(obj.sprite_path);
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

            JToken name_token = json_object["Name"];
            string name = name_token.ToObject<string>();

            JToken positionToken = json_object.GetValue("Position");
            Vector3 position = positionToken.ToObject<Vector3>();

            JToken rotationToken = json_object.GetValue("Rotation");
            Quaternion rotation = rotationToken.ToObject<Quaternion>();

            GameObject gameObject = new GameObject(name);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;

            return gameObject;
        }
    }
    public class HexGameObjectConverter : JsonConverter<GameObject>
    {
        public override void WriteJson(JsonWriter writer, GameObject value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("$type");
            writer.WriteValue("Game Object");

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

            JToken name_token = json_object["Name"];
            string name = name_token.ToObject<string>();

            JToken positionToken = json_object.GetValue("Position");
            Vector3 position = positionToken.ToObject<Vector3>();

            JToken rotationToken = json_object.GetValue("Rotation");
            Quaternion rotation = rotationToken.ToObject<Quaternion>();

            GameObject game_object = Spawner.CreateHex(name, position, rotation);

            game_object.transform.SetParent(MapContainer.Instance.fields_container);
            return game_object;
        }
    }

    public class OuterGameObjectConverter : JsonConverter<List<GameObject>>
    {
        public override void WriteJson(JsonWriter writer, List<GameObject> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (GameObject gameObj in value)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("$type");
                writer.WriteValue("Game Object");

                writer.WritePropertyName("Name");
                serializer.Serialize(writer, gameObj.name);

                writer.WritePropertyName("Position");
                serializer.Serialize(writer, gameObj.transform.position);

                writer.WritePropertyName("Rotation");
                serializer.Serialize(writer, gameObj.transform.rotation);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        public override List<GameObject> ReadJson(JsonReader reader, Type objectType, List<GameObject> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            List<GameObject> gameObjects = new List<GameObject>();

            JArray jsonArray = JArray.Load(reader);

            Material material = MapContainer.Instance.outer_field_material;
            foreach (JObject jsonObject in jsonArray.Children<JObject>())
            {
                JToken nameToken = jsonObject.GetValue("Name");
                string name = nameToken.ToObject<string>();

                JToken positionToken = jsonObject.GetValue("Position");
                Vector3 position = positionToken.ToObject<Vector3>();

                JToken rotationToken = jsonObject.GetValue("Rotation");
                Quaternion rotation = rotationToken.ToObject<Quaternion>();

                GameObject game_object = Spawner.CreateHex(name, position, rotation);
                game_object.GetComponent<Renderer>().material = material;
                game_object.transform.SetParent(MapContainer.Instance.fields_container);

                gameObjects.Add(game_object);
            }

            return gameObjects;
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
