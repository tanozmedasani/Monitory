namespace monitory.Infrastructure.Interfaces
{
    public interface IEmailActions
    {
        void SendAlert(string format);
    }
}