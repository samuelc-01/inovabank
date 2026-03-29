using FluentValidation;
using InovaBank.Domain.Primitives;
using MediatR;

namespace InovaBank.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationFailures = _validators
            .Select(validator => validator.Validate(context))
            .SelectMany(validationResult => validationResult.Errors)
            .Where(validationFailure => validationFailure != null)
            .Select(failure => failure.ErrorMessage)
            .Distinct()
            .ToList();

        if (validationFailures.Count > 0)
        {
            var errorMessage = string.Join(" | ", validationFailures);

            return CreateFailureResult(errorMessage);
        }

        return await next(cancellationToken);
    }

    private static TResponse CreateFailureResult(string message)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)Result.Failure(message, 400);
        }

        var resultType = typeof(TResponse).GetGenericArguments()[0];
        var failureMethod = typeof(Result<>)
            .MakeGenericType(resultType)
            .GetMethod("Failure", [typeof(string), typeof(int)]);

        return (TResponse)failureMethod!.Invoke(null, [message, 422])!;
    }
}
