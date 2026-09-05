
using Dtos;
namespace Services.Interfaces;

public interface IEmailService
{
    Task SendAsync(string subject, string content);
}