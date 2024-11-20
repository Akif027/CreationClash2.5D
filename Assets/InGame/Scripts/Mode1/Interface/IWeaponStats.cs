public interface IWeaponStats
{
    int Damage { get; }
    float Range { get; }
    float AttackSpeed { get; }

    float LaunchForce { get; }
    float LaunchAngle { get; }
    WeaponType WeaponType { get; }
    // Add more properties as needed
}
public enum WeaponType
{
    Rock,
    Spear,
    Shuriken,
    Stick,
    None


}