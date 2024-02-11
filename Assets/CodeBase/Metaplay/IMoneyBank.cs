using System;

namespace CodeBase.Metaplay {
    public interface IMoneyBank {
        int Amount { get; }

        event Action Changed;
        event Action Earned;
        event Action Spent;

        bool CanSpend(int value);
        
        void Earn(int value);
        bool TrySpend(int value);
    }
}