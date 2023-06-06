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
    }
}
