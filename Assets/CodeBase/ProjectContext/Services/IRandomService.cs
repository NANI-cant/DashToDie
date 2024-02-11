using System.Collections.Generic;

namespace CodeBase.ProjectContext.Services {
    public interface IRandomService {
        int Next();
        int Range(int from, int to);
        float Range(float from, float to);
        T[] Shuffle<T>(IEnumerable<T> array);
        T GetRandom<T>(IList<T> list);
    }
}