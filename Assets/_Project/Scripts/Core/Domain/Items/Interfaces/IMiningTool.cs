namespace Domain.Items
{
    interface IMiningTool : IItem
    {
        MiningSpeed GetMiningSpeed();
        void UseDurability();
    }
}