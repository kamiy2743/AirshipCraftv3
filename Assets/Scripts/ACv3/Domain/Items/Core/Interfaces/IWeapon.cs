namespace ACv3.Domain.Items
{
    interface IWeapon : IItem
    {
        Strength GetStrength();
        void UseDurability();
    }
}