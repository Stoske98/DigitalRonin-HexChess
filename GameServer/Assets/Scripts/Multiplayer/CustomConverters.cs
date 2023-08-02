using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class CustomConverters
{
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
            writer.WriteValue("Hex Game Object");

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

            GameObject hex_gameObject = new GameObject();

            hex_gameObject.name = name;
            hex_gameObject.transform.position = position;
            hex_gameObject.transform.rotation = rotation;
            if(NetworkManager.Instance.gameobject_visibility)
            {
                hex_gameObject.AddComponent<MeshFilter>();
                hex_gameObject.AddComponent<MeshRenderer>();

                Mesh mesh = hex_gameObject.GetComponent<MeshFilter>().mesh;
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

                hex_gameObject.transform.SetParent(MapContainer.Instance.fields_container);
            }

            hex_gameObject.transform.SetParent(MapContainer.Instance.fields_container);
            return hex_gameObject;
        }
    }
    public class UnitGameObjectConverter : JsonConverter<GameObject>
    {
        public override void WriteJson(JsonWriter writer, GameObject value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("$type");
            writer.WriteValue("Unit Game Object");

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

            GameObject unit_gameobject = new GameObject();

            unit_gameobject.name = name;
            unit_gameobject.transform.position = position;
            unit_gameobject.transform.rotation = rotation;

            unit_gameobject.transform.SetParent(MapContainer.Instance.units_container);
            return unit_gameobject;
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
}
