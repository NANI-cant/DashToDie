namespace CodeBase.Gameplay.General.Impl {
    public class ShieldModifier : IHurtModifier {
        public int ProcessDamage(int damage, HurtProcessor processor) {
            if (damage == 0) return damage;
            
            processor.RemoveModifier(this);
            return 0;
        }
    }
}