namespace PORTAL.Models
{
    public class Hostel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Total_Rooms { get; set; }
        public int Occupants { get; set; }
        public int Booked { get; set; }
        public bool Full { get; set; }
    }
}
