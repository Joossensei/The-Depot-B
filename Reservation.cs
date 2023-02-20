using System;

namespace ReservationSystem
{
    public class Reservation
    {
        public string Code { get; }
        public string EntryTicketCode { get; }
        public int GroupSize { get; }
        public string Name { get; }
        public DateTime ReservationTime { get; }

        

        public Reservation(string entryTicketCode, int groupSize, DateTime reservationTime, string name)
        {
            Code = GenerateReservationCode();
            EntryTicketCode = entryTicketCode;
            GroupSize = groupSize;
            Name = name;
            ReservationTime = reservationTime;
        }

        private string GenerateReservationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return code;
        }
    }
}