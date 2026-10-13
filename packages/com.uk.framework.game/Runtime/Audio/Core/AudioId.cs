namespace UKFramework.Game.Audio
{
    public enum AudioId
    {
        None,

        ButtonClick,

        CannonFire,

        ProjectileHit,

        WoodHit,
        WoodBreak,

        GlassHit,
        GlassBreak,

        StoneHit,
        StoneBreak,

        Explosion,

        Coin,

        Victory,
        Failed,
        BackgroundMusic,
        ClayBreak,
        Star,
        Character,
        // Shooter Run - Weapon Events
        WeaponFire_Rifle = 50,
        WeaponFire_MachineGun,
        WeaponFire_ShotGun,
        WeaponFire_DoubleGun,
        WeaponFire_RocketLauncher,
        WeaponReload,
        WeaponEmpty,
        WeaponImpact,
        WeaponEquip,

        // Zombies
        ZombieIdle = 80,
        ZombieAlert,
        ZombieAttack,
        ZombieHit,
        ZombieDeath,

        // Player
        PlayerHit = 100,
        PlayerDeath,

        // Gameplay
        BoosterCollected = 120,
        BoosterHit,

        WaveStart,
        WaveComplete
    }
}