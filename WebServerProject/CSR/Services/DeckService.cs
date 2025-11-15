using WebServerProject.CSR.Repositories;

namespace WebServerProject.CSR.Services
{
    public interface IDeckService
    {

    }
    public class DeckService : IDeckService
    {
        private readonly IDeckRepository _deckRepository;

        public DeckService(
            IDeckRepository deckRepository)
        {
            _deckRepository = deckRepository;
        }
    }
}
