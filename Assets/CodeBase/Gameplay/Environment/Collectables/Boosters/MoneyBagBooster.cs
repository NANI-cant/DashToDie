using CodeBase.Metaplay;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Environment.Collectables.Boosters {
    public class MoneyBagBooster : MonoBehaviour, IBooster {
        [SerializeField] [Min(0)] private int _moneyAdd;
        
        private IMoneyBank _moneyBank;

        [Inject]
        public void Construct(IMoneyBank moneyBank) {
            _moneyBank = moneyBank;
        }
        
        public void Apply(GameObject target) {
            _moneyBank.Earn(_moneyAdd);    
        }
    }
}