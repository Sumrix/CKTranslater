namespace Core.Translation.Matching
{
    public class CalculationCell
    {
        public readonly MaxFRoute Route0 = new();
        public readonly MaxFRoute Route1 = new();

        public override string ToString()
        {
            FVector? maxVector = FVector.Max(
                this.Route0.Null,
                this.Route0.Value,
                this.Route1.Null,
                this.Route1.Value
            );

            return maxVector == null
                ? "null"
                : $"{maxVector.Letters0}; {maxVector.Letters1}; {maxVector.PathAverage}";
        }
    }
}