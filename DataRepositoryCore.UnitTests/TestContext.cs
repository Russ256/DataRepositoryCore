namespace DataRepositoryCore.UnitTests
{
    using Microsoft.EntityFrameworkCore;

    public class Customer : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public interface ITestContext : IDataContext
    {
        DbSet<Customer> Customers { get; }
    }

    public class TestContext : DataContext, ITestContext
    {
        public DbSet<Customer> Customers { get; set; }

        public TestContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}