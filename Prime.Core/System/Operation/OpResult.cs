namespace Prime.Core
{
    public class OpResult
    {
        public bool IsSuccess { get; set; }

        public static OpResult Success => new OpResult() {IsSuccess = true};

        public static OpResult Fail => new OpResult();

        public static OpResult From(bool isSuccess)
        {
            return new OpResult() {IsSuccess = isSuccess};
        }
    }
}