namespace Basket.API.Exceptions;

internal class BasketNotFoundException : NotFoundException
{
    public BasketNotFoundException(Guid Id) : base("Basket", $"UserId {Id}")
    {
    }
}
