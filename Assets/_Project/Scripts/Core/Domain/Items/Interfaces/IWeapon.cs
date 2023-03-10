namespace Domain.Items
{
    internal interface IWeapon : IItem
    {
        Strength GetStrength();
        void UseDurability();
    }
}