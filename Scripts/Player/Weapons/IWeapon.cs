namespace Horror.Scripts.Player.Weapons;

public interface IWeapon
{
    void Shoot();
    void TakeOut();
    void PutAway();
    bool CanShoot();
}