using System.Data;
using System.Runtime.CompilerServices;
using APIDataAccessLayer;
using System.Threading.Tasks;


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

        public static async Task<List<PersonDTO>> GetPeopleAsync()
        {
            return await clsPersonData.GetPeopleAsync();
        }

        public static async Task<clsPerson> GetPersonByIdAsync(int PersonID)
        {
            PersonDTO SDTO = await clsPersonData.GetPersonByIdAsync(PersonID);

            if (SDTO != null)
            {
                return new clsPerson(SDTO);
            }
            else
                return null;
        }

        private async Task<bool> _AddPersonAsync()
        {
            this.PersonId = await clsPersonData.AddPersonAsync(PDTO);
            return (this.PersonId != -1);
        }
        private async Task<bool> _UpdatePersonAsync()
        {
            return await clsPersonData.UpdatePersonAsync(PDTO);
        }

        public static async Task<bool> DeletePersonAsync(int PersonID)
        {
            return await clsPersonData.DeletePersonAsync(PersonID);
        }
        public async Task<bool> SaveAsync()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(await _AddPersonAsync())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    
                case enMode.Update:
                    return await _UpdatePersonAsync();

            }
            return false;
        }

        public static async Task<bool> IsPersonExistAsync(int PersonID)
        {
            return await clsPersonData.IsPersonExistAsync(PersonID);
        }
    }
}
