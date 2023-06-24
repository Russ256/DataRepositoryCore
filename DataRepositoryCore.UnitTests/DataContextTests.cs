namespace DataRepositoryCore.UnitTests
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;

    [TestClass]
    public class DaatContextTests
    {
        [TestMethod]
        public async Task TestContext()
        {
            TestContext sut = TestContextHelper.Create();
            Customer c = await sut.Customers.FirstOrDefaultAsync();
        }

        [TestMethod]
        public async Task TestAdd()
        {
            TestContext sut = TestContextHelper.Create();
            sut.AddEntity(new Customer() { Name = "test" });
            await sut.SaveChangesAsync();
        }
    }
}