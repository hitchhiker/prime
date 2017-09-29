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

        public static OpResult From(int insertedCount)
        {
            return new OpResult() {IsSuccess = true, InsertCount = insertedCount };
        }

        public int InsertCount { get; private set; }
    }
}