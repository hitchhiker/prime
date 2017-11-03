namespace Prime.Common
{
    public class OpResult
    {
        public bool IsSuccess { get; private set; }

        public int InsertedDeletedCount { get; private set; }

        public static OpResult Success => new OpResult() {IsSuccess = true};

        public static OpResult Fail => new OpResult();

        public static OpResult From()
        {
            return new OpResult() {IsSuccess = true};
        }

        public static OpResult From(bool count)
        {
            return new OpResult() { IsSuccess = true, InsertedDeletedCount = count ? 1 : 0 };
        }

        public static OpResult From(int count)
        {
            return new OpResult() {IsSuccess = true, InsertedDeletedCount = count };
        }
    }
}