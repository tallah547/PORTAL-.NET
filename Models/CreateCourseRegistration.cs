
namespace PORTAL.Models
{
    public class CreateCourseRegistration
    {
        public string Student_Code { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string SurName { get; set; }
        public string E_Mail { get; set; }
        public string Phone_No { get; set; }
        public string Gender { get; set; }
        public string Residency { get; set; }
        public string Portal_User_Id { get; set; }
        public string Created_By { get; set; }
        public string Department_Code { get; set; }
        public string Department_Name { get; set; }
        public string Programme_Code { get; set; }
        public string Programme_Name { get; set; }
        public string Year_of_Study_Semester { get; set; }
        public bool Posted { get; set; }


        public static implicit operator CreateCourseRegistration(CourseRegistration v)
        {
            throw new NotImplementedException();
        }
    }
}
