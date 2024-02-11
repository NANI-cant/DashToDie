using System.Threading;
using Cysharp.Threading.Tasks;

namespace CodeBase.Gameplay.Skills {
    public abstract class ASkillBase: ISkill {
        private CancellationTokenSource _cancelSource;

        public bool IsExecuting => _cancelSource != null;
        public abstract IChargeCounter ChargeCounter { get; }

        public bool TryExecute(ASkillHolder holder, int holderTeam) {
            if (ChargeCounter.Charges == 0) return false;
            if (IsExecuting) return false;
            
            ChargeCounter.Charges--;
            
            _cancelSource = new CancellationTokenSource();
            ExecuteAsync(holder, holderTeam).Forget();
            
            return true;
        }

        private async UniTask ExecuteAsync(ASkillHolder holder, int holderTeam) {
            await ExecuteSkillAsync(holder, holderTeam, _cancelSource.Token);
            _cancelSource?.Dispose();
            _cancelSource = null;
        }

        protected abstract UniTask ExecuteSkillAsync(ASkillHolder holder, int holderTeam, CancellationToken cancelToken);

        public void Cancel() {
            _cancelSource?.Cancel();
            _cancelSource?.Dispose();
            _cancelSource = null;
        }
    }
}