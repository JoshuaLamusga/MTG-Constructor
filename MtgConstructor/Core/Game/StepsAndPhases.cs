namespace MtgConstructor.Game
{
    /// <summary>
    /// Discrete steps that occur in each turn, or phases if it has no steps.
    /// </summary>
    public enum StepsAndPhases
    {
        UntapStep,
        UpkeepStep,
        DrawStep,
        PreCombatMainPhase,
        BeginCombatStep,
        DeclareAttackersStep,
        DeclareBlockersStep,
        CombatDamageStep,
        PostCombatMainPhase,
        EndStep,
        CleanupStep
    }
}
