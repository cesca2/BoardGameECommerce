public static class ApiResultFactory<T>
{
    public static ApiResult<T> Ok(T response) =>
        new ApiResult<T> { Success = true, Response = response };

    public static ApiResult<T> Fail(string error) =>
        new ApiResult<T> { Success = false, Error = error };
}
