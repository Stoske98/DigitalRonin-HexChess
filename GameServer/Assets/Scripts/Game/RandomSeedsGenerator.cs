using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class RandomSeedsGenerator
{
    private int max_seeds;
    private Random random;
    [JsonConverter(typeof(CustomConverters.FloatQueueConverter))] public Queue<float> random_seeds { get; set; }

    public RandomSeedsGenerator() 
    { 
        random_seeds = new Queue<float>(); 
        random = new Random(); 
    }
    public RandomSeedsGenerator(int _max_seeds) 
    { 
        max_seeds = _max_seeds;
        random_seeds = new Queue<float>();
        random = new Random();
        GenerateRandomSeeds(_max_seeds);
    }

    private void GenerateRandomSeeds(int number_of_seeds)
    {
        for (int i = 0; i < number_of_seeds; i++)
            random_seeds.Enqueue((float)random.NextDouble());
    }

    public float GetRandomSeed()
    {
        int random_seeds_count = random_seeds.Count;

        if (random_seeds_count < max_seeds / 4)
            GenerateRandomSeeds(random_seeds_count - max_seeds / 4);

        //SEND TO CLIENT ARRAY OF NEW SEEDS

        return random_seeds.Dequeue();
    }
}


