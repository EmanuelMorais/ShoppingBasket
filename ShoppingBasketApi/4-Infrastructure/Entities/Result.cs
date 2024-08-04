
using ShoppingBasketApi.Infrastructure.Entities;

namespace ShoppingBasketApi.Infrastructure.Helpers
{
    public class Result<T>
    {
        private readonly T? value;
        private readonly ApplicationError applicationError;
        private readonly bool isSuccess;

        public Result(T value)
        {
            this.isSuccess = true;
            this.value = value;
            this.applicationError = ApplicationError.None;
        }

        public Result(ApplicationError error)
        {
            this.isSuccess = false;
            this.value = default;
            this.applicationError = error;
        }

        public static Result<T> Success(T value) => new(value);

        public static Result<T> Failure(string errorCode, string errorMessage)
            => new(new ApplicationError(errorCode, errorMessage));

        public static implicit operator Result<T>(T value) => new(value);

        public static implicit operator Result<T>(ApplicationError applicationError) => new(applicationError);

        public bool IsSuccess => isSuccess;

        public T? Value => isSuccess ? value : throw new InvalidOperationException("No value available for failure result");

        public ApplicationError Error => !isSuccess ? applicationError : default;

        public TResult Match<TResult>(Func<T, TResult> success, Func<ApplicationError, TResult> failure)
        {
            return this.isSuccess ? success(value) : failure(applicationError);
        }
    }
}

