using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Moq;

namespace CJG.Testing.UnitTests
{
    public static class EntityFrameworkMockHelpers
    {
        public static Mock<DbSet<T>> CreateDbAddMock<T>(IEnumerable<T> elements) where T : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbAddMock = new Mock<DbSet<T>>();

            dbAddMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
            dbAddMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
            dbAddMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
            dbAddMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

            //dbAddMock.As<IQueryable<T>>().Setup(m => m.AsNoTracking()).Returns(elementsAsQueryable);

            return dbAddMock;
        }
    }
}
