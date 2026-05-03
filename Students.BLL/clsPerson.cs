    using System.Data;
using System.Runtime.CompilerServices;
using APIDataAccessLayer;


namespace APIBusinessLayer
{
    public class clsPerson
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public DateTime BirthDate { get; set; }
        public int Age { get; set; }

        public bool IsActive { get; set; }

        public PersonDTO PDTO
        {
            get { return (new PersonDTO(this.PersonId, this.FirstName, this.LastName, this.Email, this.BirthDate, this.Age,this.IsActive)); }
        }

        public clsPerson()
        {
            this.PersonId = -1;
            this.FirstName = string.Empty;
            this.LastName = string.Empty;
            this.Email = string.Empty;
            this.BirthDate = DateTime.Now;
            this.Age = -1;
            this.IsActive = false;
            this.Mode = enMode.AddNew;
        }

        // This private constructor is used for creating an instance when data is retrieved from the database.
        // By making it private, we enforce data integrity, ensuring that an object in 'Update' mode 
        // can only be created through authorized methods (like GetPersonById), which verify the existence of the data first.
        private clsPerson (PersonDTO PDto)
        {
            this.PersonId = PDto.PersonId;
            this.FirstName = PDto.FirstName;
            this.LastName = PDto.LastName;
            this.Email = PDto.Email;
            this.BirthDate = PDto.BirthDate;
            this.Age = PDto.Age;
            this.IsActive = PDto.IsActive;
            this.Mode = enMode.Update;
        }

        public static List<PersonDTO> GetPeople()
        {
            return APIDataAccessLayer.clsPersonData.GetPeople();
        }

        public static clsPerson GetPersonById(int PersonID)
        {
            PersonDTO SDTO = APIDataAccessLayer.clsPersonData.GetPersonById(PersonID);

            if (SDTO != null)
            {
                return new clsPerson(SDTO);
            }
            else
                return null;
        }

        private bool _AddPerson()
        {
            this.PersonId = APIDataAccessLayer.clsPersonData.AddPerson(PDTO);
            return (this.PersonId != -1);
        }
        private bool _UpdatePerson()
        {
            return APIDataAccessLayer.clsPersonData.UpdatePerson(PDTO);
        }

        public static bool DeletePerson(int PersonID)
        {
            return APIDataAccessLayer.clsPersonData.DeletePerson(PersonID);
        }
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(_AddPerson())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    
                case enMode.Update:
                    return _UpdatePerson();
               
            }
            return false;
        }

        public static bool IsPersonExist(int PersonID)
        {
            return clsPersonData.IsPersonExist(PersonID);
        }
    }
}
