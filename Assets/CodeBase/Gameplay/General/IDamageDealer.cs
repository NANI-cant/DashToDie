using CodeBase.Gameplay.ProgressiveValues;

namespace CodeBase.Gameplay.General {
    public interface IDamageDealer {
        ProgressiveInt Damage { get; }
    }
}