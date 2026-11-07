using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExamSystem.Application.Interface.DataRepository
{
   public interface IDifficultyLevelsOperations
    {
        void Add(DifficultyLevelsDTO difficultyLevels);
        void Edit(DifficultyLevelsDTO difficultyLevels);

       

        void Delete(int id);


        DifficultyLevelsDTO GetById(int id);

        List<DifficultyLevelsDTO> list();
    }
}
