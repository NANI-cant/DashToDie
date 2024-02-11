using CodeBase.Gameplay.Skills.Configs;
using Cysharp.Threading.Tasks;

namespace CodeBase.Gameplay.Skills.Factory {
    public interface ISkillFactory {
        UniTask<ISkill> Create(ISkillConfig config, ASkillHolder holder);
    }
}