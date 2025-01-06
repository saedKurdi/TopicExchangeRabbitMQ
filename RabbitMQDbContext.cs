public class RabbitMQDbContext : DbContext
{
    public RabbitMQDbContext(DbContextOptions<RabbitMQDbContext> options) : base(options)
    {
    }

    // DbSet for TopicExchangeGroup
    public DbSet<TopicExchangeGroup> TopicExchangeGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuring TopicExchangeGroup
        modelBuilder.Entity<TopicExchangeGroup>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(e => e.ParentGroup)
                .WithMany(e => e.SubGroups)
                .HasForeignKey(e => e.ParentGroupId)
                .OnDelete(DeleteBehavior.Cascade); // Ensures subgroups are deleted if the parent is deleted
        });
    }
}