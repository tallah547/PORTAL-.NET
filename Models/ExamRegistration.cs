namespace PORTAL.Models
{
    public class ExamRegistration
    {
        public string Code { get; set; }
        public string? Year_of_Study { get; set; }
        public string? Student_Code { get; set; }
        public string? Student_Name { get; set; }
        public string? Portal_User_Id { get; set; }
        public string? Programme_COde { get; set; }
        public string? Programme_Name { get; set; }
        public string? Department_Code { get; set; }
        public string? Department_Name { get; set; }
        public bool Registered { get; set; }
    }
}
