namespace ReservasHotel.Api.Entities;

public class Room
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public RoomType Type { get; set; }
    public decimal PricePerNight { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}