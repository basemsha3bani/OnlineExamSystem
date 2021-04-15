using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface
{
   public interface IDifficultyLevelsOperations
    {
        void Add(DifficultyLevelsDataModel difficultyLevels);
        void Edit(DifficultyLevelsDataModel difficultyLevels);

       

        void Delete(int id);


        DifficultyLevelsDataModel GetById(int id);

        List<DifficultyLevelsDataModel> list();
    }
}
