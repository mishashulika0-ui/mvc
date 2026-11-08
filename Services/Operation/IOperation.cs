namespace p48_lessons_asp.Services.Operation;

/*
Створюємо абстракцію. Сенс якої не прив'язувати контролери до конкректної реалізації сервісу.
Виносимо інтерфейс сервісу, через який можна буде звертатися до конкреткного об'єкту, який створить DI-provider 
*/
public interface IOperation
{
    Guid OperationId { get; }
}