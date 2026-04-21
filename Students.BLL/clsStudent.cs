using System.Data;
using System.Runtime.CompilerServices;
using StudentDataAccessLayer;


namespace StudentAPIBusinessLayer
{
    public class Student
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public DateTime BirthDate { get; set; }
        public int Age { get; set; }

        public bool IsActive { get; set; }

        public StudentDTO SDTO
        {
            get { return (new StudentDTO(this.StudentId, this.FirstName, this.LastName, this.Email, this.BirthDate, this.Age,this.IsActive)); }
        }

        public Student (StudentDTO sdto , enMode mode = enMode.AddNew)
        {
            this.StudentId = sdto.StudentId;
            this.FirstName = sdto.FirstName;
            this.LastName = sdto.LastName;
            this.Email = sdto.Email;
            this.BirthDate = sdto.BirthDate;
            this.Age = sdto.Age;
            this.IsActive = sdto.IsActive;
            this.Mode = mode;
        }

        public static List<StudentDTO> GetAllStudents()
        {
            return StudentDataAccessLayer.clsStudentData.GetAllStudents();
        }

        public static Student GetStudentById(int studentId)
        {
            StudentDTO SDTO = StudentDataAccessLayer.clsStudentData.GetStudentById(studentId);

            if (SDTO != null)
            {
                return new Student(SDTO, enMode.Update);
            }
            else
                return null;
        }

        private bool _AddStudent()
        {
            this.StudentId = StudentDataAccessLayer.clsStudentData.AddStudent(SDTO);
            return (this.StudentId != -1);
        }
        private bool _UpdateStudent()
        {
            return StudentDataAccessLayer.clsStudentData.UpdateStudent(SDTO);
        }

        public static bool DeleteStudent(int studentID)
        {
            return StudentDataAccessLayer.clsStudentData.DeleteStudent(studentID);
        }
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if(_AddStudent())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    
                case enMode.Update:
                    return _UpdateStudent();
               
            }
            return false;
        }
    }
}
