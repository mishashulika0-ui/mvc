using System;

namespace p48_lessons_asp.Services.Operation;

public class ScopedOperation : IOperation
{
    public Guid OperationId { get; } = Guid.NewGuid();
}
