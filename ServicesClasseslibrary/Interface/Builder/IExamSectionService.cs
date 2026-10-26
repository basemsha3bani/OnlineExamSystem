using Domain.Entities;
using Domain.Aggregates;
﻿using Domain.DTOs;
using System.Collections.Generic;

namespace ServicesClasseslibrary
{
    public interface IExamSectionService
    {
        public List<ExamSectionsDTO> List(int examId);
        ExamSectionsDTO GetById(int id);
        void Edit(ExamSectionsDTO model);
        public void Add(ExamSectionsDTO m);
    }
}

