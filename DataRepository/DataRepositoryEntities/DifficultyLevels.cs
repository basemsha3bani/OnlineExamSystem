using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository.DataRepositoryEntities
{
    public class DifficultyLevels:IRepository
    {
        public int Id { get; set; }

        public String DifficultyLevelName { get; set; }
    }
}
