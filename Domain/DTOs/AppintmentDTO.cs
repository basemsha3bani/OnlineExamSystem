using System;

namespace Domain.DTOs
{

    public class AppintmentDTO
    { 
        [System.Text.Json.Serialization.JsonInclude]
        public DateTime AppointmentDate { get; private set; }

       
        [System.Text.Json.Serialization.JsonInclude]
        public string AppointmentDateAsString { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int? VaccinationType { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public string RegistrarIds { get; private set; }
    
        [System.Text.Json.Serialization.JsonConstructor]

        private AppintmentDTO() { }
        public static AppintmentDTO Define(DateTime AppointmentDate = default, string AppointmentDateAsString = default, int? VaccinationType = default, string RegistrarIds = default)
        {
            var value = new AppintmentDTO();
            value.AppointmentDate = AppointmentDate;
            value.AppointmentDateAsString = AppointmentDateAsString;
            value.VaccinationType = VaccinationType;
            value.RegistrarIds = RegistrarIds;
            return value;
        }
        public void ChangeAppointmentDate( DateTime value) => AppointmentDate = value;
        public void ChangeAppointmentDateAsString( string value) => AppointmentDateAsString = value;
        public void ChangeVaccinationType( int? value) => VaccinationType = value;
        public void ChangeRegistrarIds( string value) => RegistrarIds = value;
}

}