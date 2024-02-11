using System.Collections.Generic;

namespace CodeBase.Gameplay.PlayerStats {
    public interface ICompositeStat: IStat {
        IEnumerable<IStat> Stats { get; }
    }
}