namespace mini_ITS.Core.Database
{
    public abstract class SqlPagedQueryBase
    {
        public int Page { get; set; } = 1;
        public int Results { get; set; } = 10;
    }
}