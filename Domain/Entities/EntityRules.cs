using Domain.Aggregates;
using Domain.DTOs;
using Domain.Exceptions;

namespace Domain.Entities;

internal static class EntityRules
{
    public static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{name} is required.");
        return value.Trim();
    }
    public static int Positive(int value, string name)
    {
        if (value < 1) throw new DomainException($"{name} must be positive.");
        return value;
    }
    public static int NonNegative(int value, string name)
    {
        if (value < 0) throw new DomainException($"{name} cannot be negative.");
        return value;
    }
    public static T Exists<T>(T value, string name) where T : class => value ?? throw new DomainException($"{name} not found.");
}
