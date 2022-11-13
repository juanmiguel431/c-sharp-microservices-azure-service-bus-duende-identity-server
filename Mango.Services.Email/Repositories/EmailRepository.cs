using Mango.Services.Email.DbContexts;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.Email.Repositories;

public class EmailRepository : IEmailRepository
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContext;

    public EmailRepository(DbContextOptions<ApplicationDbContext> dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
    {
        var emailLog = new EmailLog()
        {
            Email = message.Email,
            Log = $"OrderId {message.OrderId} has been created successfully.",
            EmailSent = DateTime.Now
        };
        
        await using var applicationDbContext = new ApplicationDbContext(_dbContext);
        applicationDbContext.EmailLogs.Add(emailLog);

        await applicationDbContext.SaveChangesAsync();
    }
}