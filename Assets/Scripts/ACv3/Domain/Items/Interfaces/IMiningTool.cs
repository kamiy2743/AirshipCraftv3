namespace ACv3.Domain.Items
{
    interface IMiningTool : IItem
    {
        MiningSpeed GetMiningSpeed();
        void UseDurability();
    }
}