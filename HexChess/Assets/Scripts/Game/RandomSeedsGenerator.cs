using Newtonsoft.Json;
using System.Collections.Generic;

public class RandomSeedsGenerator
{
    [JsonConverter(typeof(CustomConverters.FloatQueueConverter))] public Queue<float> random_percent_seeds { get; set; }
    [JsonRequired][JsonConverter(typeof(CustomConverters.StringQueueConverter))] public Queue<string> random_ids_seeds { get; set; }

    public RandomSeedsGenerator()
    {
        random_percent_seeds = new Queue<float>();
        random_ids_seeds = new Queue<string>();
    }
    //float
    public float GetRandomPercentSeed()
    {
        return random_percent_seeds.Dequeue();
    }
    public bool PercentCalc(float percent)
    {
        float random_seed = GetRandomPercentSeed() * 100;
        return percent > random_seed;
    }
    //ids
    public string GetRandomIdSeed() 
    {
        return random_ids_seeds.Dequeue();
    }
}
