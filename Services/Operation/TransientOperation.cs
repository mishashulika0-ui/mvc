namespace p48_lessons_asp.Services.Operation;

public class TransientOperation : IOperation
{
    public Guid OperationId { get; } = Guid.NewGuid();
}