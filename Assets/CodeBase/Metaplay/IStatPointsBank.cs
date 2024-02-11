using System;

namespace CodeBase.Metaplay {
    public interface IStatPointsBank {
        int All { get; }
        int Available { get; }

        event Action Changed;

        bool CanTake(int count);

        void Put(int count);
        void Take(int count);
        void Earn(int count);
    }
}