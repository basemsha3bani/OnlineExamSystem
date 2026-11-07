using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineExamSystem.Application.Interface
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
