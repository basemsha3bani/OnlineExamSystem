using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;

using ServicesClasseslibrary.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesClasseslibrary
{
    public class DifficultyLevelsService : IDifficultyLevelsService

    {
        private readonly IDifficultyLevelsOperations _difficultyLevelsOperations;
       
        public DifficultyLevelsService(IDifficultyLevelsOperations difficultyLevelsOperations)
        {
            _difficultyLevelsOperations = difficultyLevelsOperations;
           
        }
        public void Add(DifficultyLevelsDataModel difficultyLevels)
        {
            _difficultyLevelsOperations.Add(difficultyLevels);
        }

        public void Delete(int id)
        {
            _difficultyLevelsOperations.Delete(id);
        }

        public void Edit(DifficultyLevelsDataModel model)
        {
            _difficultyLevelsOperations.Edit(model);
        }

        public DifficultyLevelsDataModel GetById(int id)
        {
            return _difficultyLevelsOperations.GetById(id);

        }

        public List<DifficultyLevelsDataModel> list()
        {
            return _difficultyLevelsOperations.list();
        }
    }
}
 