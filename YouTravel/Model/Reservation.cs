using System;
using System.ComponentModel.DataAnnotations;

namespace YouTravel.Model
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        public DateTime TimeOfReservation { get; set; }

        public int ArrangementId { get; set; }
        public Arrangement Arrangement { get; set; } = null!;
        public string? Username { get; set; }
        public int NumOfPeople { get; set; }
        public DateTime? PaidOn { get; set; }

    }
}
