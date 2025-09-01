using Application.Domain.Enums;

namespace Application.Domain;
public record HallCall(int Floor, DirectionEnum Direction, TimeSpan CreatedAt);