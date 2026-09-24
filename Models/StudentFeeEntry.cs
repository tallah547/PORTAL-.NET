namespace PORTAL.Models
{
    public class StudentFeeEntry
    {
        public string Document_No { get; set; }
        public decimal Amount_LCY { get; set; }
        public string Posting_Date { get; set; }
        public string Entry_Type { get; set; }
        public string Document_Type { get; set; }
        public string Customer_No { get; set; }
    }
}