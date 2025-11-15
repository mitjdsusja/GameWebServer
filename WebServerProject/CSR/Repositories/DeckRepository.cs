using SqlKata.Execution;

namespace WebServerProject.CSR.Repositories
{
    public interface IDeckRepository
    {

    }

    public class DeckRepository : IDeckRepository
    {
        private readonly QueryFactory _db;

        public DeckRepository(QueryFactory db)
        {
            _db = db;
        }
    }
}
