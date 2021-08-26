using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CJG.Core.Interfaces.Service
{
    

    public interface IServiceCategoryService : IService
    {
        IEnumerable<ServiceCategory> GetAll();
        ServiceCategory Add(ServiceCategory serviceCategory);

        ServiceCategory Update(ServiceCategory serviceCategory);
        void Delete(ServiceCategory serviceCategory);
     
    }

    public interface IServiceLineService : IService
    {
        IEnumerable<ServiceLine> GetAllForServiceCategory(int serviceCategoryId);
        ServiceLine Add(ServiceLine serviceCategory);

        ServiceLine Update(ServiceLine serviceCategory);
        void Delete(ServiceLine serviceCategory);

    }
    //public interface IServiceLineBreakdownService : IService
    //{
    //    IEnumerable<ServiceLineBreakdown> GetAllForServiceLine(int serviceLineId);
    //    ServiceLineBreakdown Add(ServiceLineBreakdown serviceCategory);

    //    ServiceLineBreakdown Update(ServiceLineBreakdown serviceCategory);
    //    void Delete(ServiceLineBreakdown serviceCategory);
    //}
}
