public class TopicExchangeGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentGroupId { get; set; }
    public TopicExchangeGroup? ParentGroup { get; set; }
    public List<TopicExchangeGroup> SubGroups { get; set; } = new List<TopicExchangeGroup>();

    public string GetFullName() => ParentGroup == null ? Name : $"{ParentGroup.GetFullName()}.{Name}";
}
