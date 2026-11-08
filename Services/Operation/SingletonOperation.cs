using System;

namespace p48_lessons_asp.Services.Operation;

public class SingletonOperation : IOperation
{
    public Guid OperationId { get; } = Guid.NewGuid();
}
