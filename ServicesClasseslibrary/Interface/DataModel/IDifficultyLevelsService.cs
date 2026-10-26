using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary.Interface
{
   public interface IDifficultyLevelsService
    {
        void Add(DifficultyLevelsDTO difficultyLevels);
        void Edit(DifficultyLevelsDTO model);

        void Delete(int id);

        DifficultyLevelsDTO GetById(int id);

        List<DifficultyLevelsDTO> list();
    }
}
