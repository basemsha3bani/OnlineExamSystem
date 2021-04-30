using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository.ModelMapper.Interface
{
    interface IModelMapper<T> where T:class
    {
         T Map(IRepository repository);
    }
}
