namespace ADF.Net.Data.DataAccess.EF
{
    public class UnitOfWork : BaseUnitOfWork<EfDbContext>
    {
        public UnitOfWork(EfDbContext context) : base(context)
        {
        }
    }
}
