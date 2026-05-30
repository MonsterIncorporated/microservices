public class GeneratedNumberDto{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Value { get; set; }
    public DateTime GeneratedAt { get; set; }

    public static GeneratedNumber toEntity(GeneratedNumberDto generatedNumberDto)
    {
        return new GeneratedNumber
        {
            Id = generatedNumberDto.Id,
            UserId = generatedNumberDto.UserId,
            Value = generatedNumberDto.Value,
            GeneratedAt = generatedNumberDto.GeneratedAt
        };
    }

    public static GeneratedNumberDto toDto(GeneratedNumber generatedNumber)
    {
        return new GeneratedNumberDto
        {
            Id = generatedNumber.Id,
            UserId = generatedNumber.UserId,
            Value = generatedNumber.Value,
            GeneratedAt = generatedNumber.GeneratedAt
        };
    }
}