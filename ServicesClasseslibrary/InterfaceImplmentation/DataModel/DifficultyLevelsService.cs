using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;
using ServicesClasseslibrary.Interface.DataRepository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Abstractions;
using ServicesClasseslibrary.Interface;
using ServicesClasseslibrary.Interface.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace ServicesClasseslibrary
{
   
    public class DifficultyLevelsService : IDifficultyLevelsService

    {
        private readonly IDifficultyLevelsOperations _difficultyLevelsOperations;
        private readonly ILoggingQueue _logQueue;
        public DifficultyLevelsService(IDifficultyLevelsOperations difficultyLevelsOperations,ILoggingQueue logQueue)
        {
            _difficultyLevelsOperations = difficultyLevelsOperations;
            _logQueue = logQueue;

        }
       
        public void Add(DifficultyLevelsDTO difficultyLevels)
        {
            _difficultyLevelsOperations.Add(difficultyLevels);
            _logQueue.Enqueue(new LogEntry
            {
                Message = $"DifficultyLevel Added: {difficultyLevels.DifficultyLevelName}",
                EventLogLevel=EventLogLevel.Informational,

                
            });
            
           
           
       
        }

        public void Delete(int id)
        {
            _difficultyLevelsOperations.Delete(id);
        }

        public void Edit(DifficultyLevelsDTO model)
        {
            _difficultyLevelsOperations.Edit(model);
        }

        public DifficultyLevelsDTO GetById(int id)
        {
            return _difficultyLevelsOperations.GetById(id);

        }

        public List<DifficultyLevelsDTO> list()
        {
            return _difficultyLevelsOperations.list();
        }
    }

 
}




 