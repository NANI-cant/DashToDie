using System;
using System.Collections.Generic;

namespace CodeBase.ProjectContext.Services.Impl {
    public class RandomService : IRandomService {
        private readonly Random _randomizer;

        public RandomService(int seed = 0) {
            _randomizer = seed == 0 
                ? new Random() 
                : new Random(seed);
        }

        public int Next() => _randomizer.Next();
        public int Range(int from, int to) => _randomizer.Next(from, to);
        
        public float Range(float from, float to) {
            float range = to - from;
            float sample = (float) _randomizer.NextDouble();
            float scaled = (sample * range) + from;
            return scaled;
        }

        public T[] Shuffle<T>(IEnumerable<T> array) {
            List<T> buffer = new List<T>(array);
            int n = buffer.Count;
            while (n > 1) {
                int k = _randomizer.Next(n--);
                (buffer[n], buffer[k]) = (buffer[k], buffer[n]);
            }

            return buffer.ToArray();
        }

        public T GetRandom<T>(IList<T> list) => list[Range(0, list.Count)];
    }
}