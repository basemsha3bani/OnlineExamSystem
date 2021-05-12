using DataModel;
using DataRepository.DataRepositoryEntities.DataRepositoryOperationsInterface;
using DataRepository.GateWay;
using DataRepository.ModelMapper.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRepository.DataRepositoryEntities.DataRepositoryEntityOperationsClasses
{
    public class DifficultyLevelsOperations : IDifficultyLevelsOperations, IModelMapper<DifficultyLevelsDataModel>
    {

        //RepositoryGateWay<DifficultyLevels> DifficultyLevelsRepositoryGateWay = new RepositoryGateWay<DifficultyLevels>();
        ContextGateway<DifficultyLevels> DifficultyLevelsRepositoryGateWay;
        public DifficultyLevelsOperations()
        {
            ContextGateway<DifficultyLevels>.GetContextInstance();
        }
        public void Add(DifficultyLevelsDataModel difficultyLevels)
        {
            DifficultyLevels difficultyLevelsInstance = new DifficultyLevels();
            difficultyLevelsInstance.DifficultyLevelName = difficultyLevels.DifficultyLevelName;
            

            ContextGateway<DifficultyLevels>.Add(difficultyLevelsInstance);


        }

        public void Delete(int id)
        {
            //DifficultyLevels difficultyLevelsInstance = new DifficultyLevels();
            //difficultyLevelsInstance = DifficultyLevelsRepositoryGateWay.GetById(g => g.Id == id);
            //DifficultyLevelsRepositoryGateWay.Delete(difficultyLevelsInstance);
        }

        public void Edit(DifficultyLevelsDataModel difficultyLevels)
        {
            
        }

        public DifficultyLevelsDataModel GetById(int id)
        {
            //DifficultyLevels difficultyLevelsInstance = new DifficultyLevels();
            //difficultyLevelsInstance = new DifficultyLevels();
            //difficultyLevelsInstance = DifficultyLevelsRepositoryGateWay.GetById(g => g.Id == id);
            //return (DifficultyLevelsDataModel) this.Map(difficultyLevelsInstance);

            //return new DataModel.DifficultyLevelsDataModel { Id = difficultyLevelsInstance.Id, DifficultyLevelName = difficultyLevelsInstance.DifficultyLevelName };
            return null;
        }

        public List<DifficultyLevelsDataModel> list()
        {
            //List<DifficultyLevelsDataModel> listOfDifficultyLevels = DifficultyLevelsRepositoryGateWay.List().Select
            //     (s=>new DifficultyLevelsDataModel  { Id = s.Id, DifficultyLevelName = s.DifficultyLevelName }).ToList();
            // return listOfDifficultyLevels;
            List<DifficultyLevelsDataModel> listOfDifficultyLevels = ContextGateway<DifficultyLevels>.List().Select
                 (s => new DifficultyLevelsDataModel { Id = s.Id, DifficultyLevelName = s.DifficultyLevelName }).ToList();
            return listOfDifficultyLevels;
           
           // return null;

        }

        public DifficultyLevelsDataModel Map(IRepository RepoistoryObject)
        {
            //DifficultyLevels difficultyLevelsInstance = (DifficultyLevels)RepoistoryObject;

            //return new DataModel.DifficultyLevelsDataModel { Id = difficultyLevelsInstance.Id, DifficultyLevelName = difficultyLevelsInstance.DifficultyLevelName };

            return null;
        }

       




    }
    
}
