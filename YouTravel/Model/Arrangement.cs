using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YouTravel.Model
{
    public enum ArrangementStatus { UPCOMING, ACTIVE, FINISHED, }
    public class Arrangement
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; } = "New Arrangement Description";
        public string Name { get; set; } = "New Arrangement";
        public double Price { get; set; }
        public DateTime Start { get; set; } = DateTime.Now.AddDays(1);
        public DateTime End { get; set; } = DateTime.Now.AddDays(3);
        public string ImageFname { get; set; } = "";

        public string DateRange
        {
            get
            {
                return $"{Start.ToShortDateString()} - {End.ToShortDateString()}";
            }
        }

        public string PriceRsd
        {
            get
            {
                return $"{Price} RSD";
            }
        }

        public IList<Place> Places { get; } = new List<Place>();
        public IList<Reservation> Reservations { get; } = new List<Reservation>();
        public ArrangementStatus Status
        {
            get
            {
                if (IsFinished()) return ArrangementStatus.FINISHED;
                if (IsActive()) return ArrangementStatus.ACTIVE;
                if (IsUpcoming()) return ArrangementStatus.UPCOMING;
                throw new InvalidOperationException();
            }
        }

        private bool IsFinished()
        {
            return End < DateTime.Now;
        }

        private bool IsActive()
        {
            return Start < DateTime.Now && DateTime.Now < End;
        }

        private bool IsUpcoming()
        {
            return DateTime.Now < End;
        }
    }
}
