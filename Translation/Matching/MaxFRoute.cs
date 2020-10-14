namespace Translation.Matching
{
    public class MaxFRoute
    {
        public FVector Value;
        public FVector Null;

        public override string ToString()
        {
            FVector maxVector = FVector.Max(
                this?.Null,
                this?.Value
            );

            return maxVector == null
                ? "null"
                : $"{maxVector?.Letters0}; {maxVector?.Letters1}; {maxVector?.PathAverage}";
        }
    }
}
