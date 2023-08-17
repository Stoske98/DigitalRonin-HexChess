using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class RandomSeedsGenerator
{
    private int max_percent_seeds;
    private int max_ids_seeds;
    private Random random;
    [JsonRequired][JsonConverter(typeof(CustomConverters.FloatQueueConverter))] public Queue<float> random_percent_seeds { get; set; }
    [JsonRequired][JsonConverter(typeof(CustomConverters.StringQueueConverter))] public Queue<string> random_ids_seeds { get; set; }

    public RandomSeedsGenerator() 
    { 
        random_percent_seeds = new Queue<float>();
        random_ids_seeds = new Queue<string>();
        random = new Random(); 
    }
    public RandomSeedsGenerator(int _max_percent_seeds, int _max_ids_seeds) 
    { 
        max_percent_seeds = _max_percent_seeds;
        max_ids_seeds = _max_ids_seeds;
        random_percent_seeds = new Queue<float>();
        random_ids_seeds = new Queue<string>();
        random = new Random();
        GenerateRandomPercentSeeds(_max_percent_seeds);
        GenerateRandomIdsSeeds(_max_ids_seeds);
    }

    private void GenerateRandomPercentSeeds(int number_of_seeds)
    {
        for (int i = 0; i < number_of_seeds; i++)
            random_percent_seeds.Enqueue((float)random.NextDouble());
    }
    private void GenerateRandomIdsSeeds(int number_of_seeds)
    {
        for (int i = 0; i < number_of_seeds; i++)
            random_ids_seeds.Enqueue(Guid.NewGuid().ToString());
    }

    //float
    public float GetRandomPercentSeed()
    {
        int random_seeds_count = random_percent_seeds.Count;

        if (random_seeds_count <= max_percent_seeds / 4)
        {
            UnityEngine.Debug.Log("PERCENT SEED REFILL");
            //SEND TO CLIENT ARRAY OF NEW SEEDS
            GenerateRandomPercentSeeds(max_ids_seeds - max_percent_seeds / 4);
        }
        
        return random_percent_seeds.Dequeue();
    }

    public bool PercentCalc(float percent)
    {
        float random_seed = GetRandomPercentSeed() * 100;
        return percent > random_seed;
    }

    //ids
    public string GetRandomIdsSeed()
    {
        int random_seeds_count = random_ids_seeds.Count;

        if (random_seeds_count <= max_ids_seeds / 5)
        {
            UnityEngine.Debug.Log("IDS SEED REFILL");
            //SEND TO CLIENT ARRAY OF NEW SEEDS
            GenerateRandomIdsSeeds(max_ids_seeds - max_ids_seeds / 4);
        }           

        return random_ids_seeds.Dequeue();
    }
    
}


