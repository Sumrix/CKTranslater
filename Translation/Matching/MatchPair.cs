namespace Translation.Matching
{
    public enum MatchVectorType
    {
        Value,
        Null
    }

    public enum MatchVectorPosition
    {
        Side0,
        Side1,
        Corner0,
        Corner1
    }

    public class MatchPair
    {
        public MatchVector ValueMatch;
        public MatchVector NullMatch;

        public (MatchVector vector, MatchVectorType type) Max()
        {
            return this.ValueMatch.PathAverage > this.NullMatch.PathAverage
                ? (this.ValueMatch, MatchVectorType.Value)
                : (this.NullMatch, MatchVectorType.Value);
        }

        public static (MatchVector vector, MatchVectorType type, MatchVectorPosition position) 
            MaxCorner(MatchPair corner0, MatchPair corner1)
        {
            var (vector0, type0) = corner0.Max();
            var (vector1, type1) = corner1.Max();


            return vector0.PathAverage > vector1.PathAverage
                ? (vector0, type0, MatchVectorPosition.Corner0)
                : (vector1, type1, MatchVectorPosition.Corner1);
        }

        public static (MatchVector vector, MatchVectorType type, MatchVectorPosition position)
            MaxCorner1(MatchPair corner0, MatchPair corner1)
        {
            return
                corner0.ValueMatch.PathAverage > corner0.NullMatch.PathAverage
                    ? corner1.ValueMatch.PathAverage > corner1.NullMatch.PathAverage
                        ? corner0.ValueMatch.PathAverage > corner1.ValueMatch.PathAverage
                            ? (corner0.ValueMatch, MatchVectorType.Value, MatchVectorPosition.Corner0)
                            : (corner1.ValueMatch, MatchVectorType.Value, MatchVectorPosition.Corner1)
                        : corner0.ValueMatch.PathAverage > corner1.NullMatch.PathAverage
                            ? (corner0.ValueMatch, MatchVectorType.Value, MatchVectorPosition.Corner0)
                            : (corner1.NullMatch, MatchVectorType.Null, MatchVectorPosition.Corner1)
                    : corner1.ValueMatch.PathAverage > corner1.NullMatch.PathAverage
                        ? corner0.NullMatch.PathAverage > corner1.ValueMatch.PathAverage
                            ? (corner0.NullMatch, MatchVectorType.Value, MatchVectorPosition.Corner0)
                            : (corner1.ValueMatch, MatchVectorType.Value, MatchVectorPosition.Corner1)
                        : corner0.NullMatch.PathAverage > corner1.NullMatch.PathAverage
                            ? (corner0.NullMatch, MatchVectorType.Value, MatchVectorPosition.Corner0)
                            : (corner1.NullMatch, MatchVectorType.Null, MatchVectorPosition.Corner1);
        }
    }
}
