namespace Basket.API.Basket.DeleteBasket;

public record DeleteBasketCommand(Guid UserId) : ICommand<DeleteBasketResult>;
public record DeleteBasketResult(bool IsSuccess);


public class DeleteBasketValidator : AbstractValidator<DeleteBasketCommand>
{
    public DeleteBasketValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
    }
}


internal class DeleteBasketHandler(IBasketRepository repository) : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        bool isSuccess = await repository.DeleteBasket(command.UserId, cancellationToken);

        return isSuccess 
            ? new DeleteBasketResult(isSuccess) 
            : throw new BasketNotFoundException(command.UserId);
    }
}
