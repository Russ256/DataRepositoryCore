namespace DataRepositoryCore.UnitTests
{
    using DataRepositoryCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Linq;

    [TestClass]
    public class ReadDataRepositoryTests
    {
     //   [TestMethod]

    }

    public class TestRepo : ReadDataRepository<Customer, int>
    {
        public TestRepo(IDataContext dataContext, ILogger<ReadDataRepository<Customer, int>> logger = null) 
            : base(dataContext, logger)
        {
        }

        public override IQueryable<Customer> AsQueryable()
        {
            return base.AsQueryable();
        }
    }
}