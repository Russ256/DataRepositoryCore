namespace DataRepositoryCore.UnitTests
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    [TestClass]
    public class DataRepositoryTests
    {
        [TestMethod]
        public void AddTest()
        {
            // Arrange
            Customer customer = new Customer() { Name = "New Customer" };
            Mock<ITestContext> mockContext = new Mock<ITestContext>();
            mockContext.Setup(m => m.AddEntity(customer));
            DataRepository<Customer, int> sut = new DataRepository<Customer, int>(mockContext.Object);

            // Act
            sut.Add(customer);

            // Assert
            mockContext.VerifyAll();
        }

        [TestMethod]
        public void DeleteTest()
        {
            // Arrange
            Customer customer = new Customer() { Name = "Customer" };
            Mock<DbSet<Customer>> mockDbSet = new Mock<DbSet<Customer>>();
            mockDbSet.Setup(m => m.Remove(customer));

            Mock<ITestContext> mockContext = new Mock<ITestContext>();
            mockContext.Setup(m => m.GetDbSet<Customer>()).Returns(mockDbSet.Object);
            DataRepository<Customer, int> sut = new DataRepository<Customer, int>(mockContext.Object);

            // Act
            sut.Delete(customer);

            // Assert
            mockDbSet.VerifyAll();
            mockContext.VerifyAll();
        }

        [TestMethod]
        public async Task FindTest()
        {
            // Arrange
            Customer customer = new Customer() { Name = "Customer" };
            Mock<DbSet<Customer>> mockDbSet = new Mock<DbSet<Customer>>();
            mockDbSet.Setup(m => m.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>())).ReturnsAsync(customer);

            Mock<ITestContext> mockContext = new Mock<ITestContext>();
            mockContext.Setup(m => m.GetDbSet<Customer>()).Returns(mockDbSet.Object);
            DataRepository<Customer, int> sut = new DataRepository<Customer, int>(mockContext.Object);

            // Act
            Customer result = await sut.FindAsync(1, default);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(customer, result);
            mockDbSet.VerifyAll();
            mockContext.VerifyAll();
        }
    }
}