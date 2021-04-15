using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class DifficultyLevelsOperations : IDifficultyLevelsOperations
    {
        RepositoryGateWay<DifficultyLevels> DifficultyLevelsRepositoryGateWay = new RepositoryGateWay<DifficultyLevels>();
        public void Add(DifficultyLevelsDataModel difficultyLevels)
        {
            DifficultyLevels difficultyLevelsInstance = new DifficultyLevels();
            difficultyLevelsInstance.DifficultyLevelName = difficultyLevels.DifficultyLevelName;
            DifficultyLevelsRepositoryGateWay.Add(difficultyLevelsInstance);


        }

        public void Delete(int id)
        {
            DifficultyLevels difficultyLevelsInstance = new DifficultyLevels();
            difficultyLevelsInstance = DifficultyLevelsRepositoryGateWay.GetById(g => g.Id == id);
            DifficultyLevelsRepositoryGateWay.Delete(difficultyLevelsInstance);
        }

        public void Edit(DifficultyLevelsDataModel difficultyLevels)
        {
            
        }

        public DifficultyLevelsDataModel GetById(int id)
        {
            DifficultyLevels difficultyLevelsInstance = new DifficultyLevels();
            difficultyLevelsInstance = DifficultyLevelsRepositoryGateWay.GetById(g => g.Id == id);
            return new DataModel.DifficultyLevelsDataModel { Id = difficultyLevelsInstance.Id, DifficultyLevelName = difficultyLevelsInstance.DifficultyLevelName };
        }

        public List<DifficultyLevelsDataModel> list()
        {
            List<DifficultyLevels> difficultyLevels = DifficultyLevelsRepositoryGateWay.List();
            return (from df in difficultyLevels
                    select new DifficultyLevelsDataModel
                    {
                        Id = df.Id,
                        DifficultyLevelName = df.DifficultyLevelName
                    }).ToList();

        }
    }
}
