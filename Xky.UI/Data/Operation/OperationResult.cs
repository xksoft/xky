using Xky.UI.Data.Enum;

namespace Xky.UI.Data.Operation
{
    public class OperationResult
    {
        public static OperationResult<bool> Failed(string message = "")
        {
            return new OperationResult<bool>
            {
                ResultType = ResultType.Failed,
                Message = message,
                Data = false
            };
        }

        public static OperationResult<bool> Success(string message = "")
        {
            return new OperationResult<bool>
            {
                ResultType = ResultType.Success,
                Message = message,
                Data = true
            };
        }
    }
}