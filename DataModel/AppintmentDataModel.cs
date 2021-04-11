using System;
using System.ComponentModel.DataAnnotations;
using DevExpress.Xpo;

namespace DataModel
{

    public class AppintmentDataModel
    { 
        [Required]
        public DateTime AppointmentDate { get; set; }

       
        public string AppointmentDateAsString { get; set; }
        [Required(ErrorMessage ="Please select a vaccintationtype")]
        public int? VaccinationType { get; set; }
        [Required(ErrorMessage = "Please choose at least one registrar")]
        public string RegistrarIds { get; set; }
    }

}