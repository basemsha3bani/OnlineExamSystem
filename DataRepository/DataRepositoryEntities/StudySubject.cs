using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataRepository.DataRepositoryEntities
{
    public class StudySubject:IRepository
    {
        public int Id { get; set; }


        public string SubjectName { get; set; }
    }
}
