using Domain.Entities;
using Domain.Aggregates;
using Domain.DTOs;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using OnlineExamSystem.Application.Interface;
using OnlineExamSystem.Application.Interface.DataRepository;
using OnlineExamSystem.Application.Interface.Logging;

namespace OnlineExamSystem.Application
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




 