namespace Domain.Items
{
    internal interface IMiningTool : IItem
    {
        MiningSpeed GetMiningSpeed();
        void UseDurability();
    }
}