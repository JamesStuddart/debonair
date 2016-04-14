namespace Debonair.Entities
{
    public enum DebonairEntityStatus
    {
        Live = 0,
        Deleted = 100
    }

    public enum SqlAdapter
    {
        SqlServer2008 = 0,
        SqlServer2012 = 100
    }

    public enum SelectFunction
    {
        COUNT = 0,
        DISTINCT = 100,
        SUM = 200,
        MIN = 300,
        MAX = 400,
        AVG = 500
    }

    public enum LikeMethod
    {
        Like = 0,
        StartsWith = 100,
        EndsWith = 200,
        Contains = 300,
        Equals = 400
    }
}
