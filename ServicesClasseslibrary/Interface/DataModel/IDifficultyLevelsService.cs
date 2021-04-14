using DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary
{
   public interface IDifficultyLevelsService
    {
        void Add(DifficultyLevelsDataModel difficultyLevels);
        void Edit(DifficultyLevelsDataModel model);

        void Delete(int id);

        DifficultyLevelsDataModel GetById(int id);

        List<DifficultyLevelsDataModel> list();
    }
}
