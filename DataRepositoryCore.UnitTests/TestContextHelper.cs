namespace DataRepositoryCore.UnitTests
{
    using Microsoft.EntityFrameworkCore;

    public class TestContextHelper
    {
        public static TestContext Create()
        {
            DbContextOptions<TestContext> options = new DbContextOptionsBuilder<TestContext>()
                .UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UnitTest;Trusted_Connection=True;MultipleActiveResultSets=true;")
                .Options;

            TestContext ctx = new TestContext(options);
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();

            return ctx;
        }
    }
}