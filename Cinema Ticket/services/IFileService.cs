namespace Cinema_Ticket.services
{
    public interface IFileService
    {


        Task<string> CreateFileAsync(IFormFile file);
    }
}
