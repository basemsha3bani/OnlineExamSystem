using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository.ModelMapper.Interface
{
    interface IModelMapper<T,Y> where T:class where Y : IRepository
    {
         T Map(Y repository);
    }
}
