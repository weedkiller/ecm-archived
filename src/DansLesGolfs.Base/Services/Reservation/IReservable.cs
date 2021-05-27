using System;

namespace DansLesGolfs.Base.Revervation
{
    public interface IReservable
    {
        bool AddReservation(DateTime reservationDate);
        bool ConfirmReservation(int reservationId);
        bool CancelReservation(int reservationId);
        bool DeleteReservation(int reservationId);
    }
}