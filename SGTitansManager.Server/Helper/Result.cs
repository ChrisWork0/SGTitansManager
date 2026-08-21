using SGTitansManager.Server.Dtos;

namespace SGTitansManager.Server.Helper;

public static class Result
{
    public static ResultDto Success(string message)
    {
        return new ResultDto
        {
            Message = message,
            Success = true
        };
    }

    public static ResultDto Success(object model, string? message = null)
    {
        return new ResultDto
        {
            Message = message,
            Success = true,
            Model = model
        };
    }

    public static ResultDto UnSuccess(string message)
    {
        return new ResultDto
        {
            Message = message,
            Success = false
        };
    }
    
    public static ResultDto Success(int statusCode, string? message = "")
    {
        return new ResultDto()
        {
            Success = true,
            StatusCode = statusCode,
            Message = message
        };
    }
    
    public static ResultDto UnSuccess(int statusCode, string message = "")
    {
        return new ResultDto()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message
        };
    }
}