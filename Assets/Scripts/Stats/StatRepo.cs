/// <summary>
///     Controls all stats in game.
///     Be careful what values you change here as upon saving an auto-generate script will execute.
///     If a string is used in the editor it's references will be lost if you remove it from here.
/// </summary>
/// <seealso cref="StatsEnumPreGeneration"/>
/// <seealso cref="StatTypes"/>
public sealed class StatRepo
{
    public const string Unknown = "Unknown";

    public const string Health = "Health";

    public const string WalkSpeed = "WalkSpeed";
    public const string RunSpeed = "RunSpeed";
    public const string CrouchSpeed = "CrouchSpeed";

    public const string MaxJumpChargeTime = "MaxJumpChargeTime";
    public const string MaxJumpUnits = "MaxJumpUnits";
    public const string ShortJumpUnits = "ShortJumpUnits";

    public const string AttackSpeed = "AttackSpeed";
    public const string SmallAttack = "SmallAttack";
    public const string SmallAttackChance = "SmallAttackChance";
    public const string MediumAttack = "MediumAttack";
    public const string MediumAttackChance = "MediumAttackChance";
    public const string BigAttack = "BigAttack";
    public const string BigAttackChance = "BigAttackChance";

}