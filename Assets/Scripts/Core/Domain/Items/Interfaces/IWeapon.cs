namespace Domain.Items
{
    interface IWeapon : IItem
    {
        Strength GetStrength();
        void UseDurability();
    }
}