using System.Net;
using System.Net.Mail;
using DreckTrack_API.Models.Entities;

namespace DreckTrack_API.Services;

public class MailService(IConfiguration configuration)
{
    public async Task SendEmailConfirmationMailAsync(ApplicationUser user, string confirmationLink)
    {
        if (user.Email == null)
            throw new InvalidOperationException("User email is not set");
        var subject = "Confirm your email";
        // Create the HTML message with a styled button
        var message = $@"
    <html>
    <body style='background-color: #2c2c2c; color: #ffffff; font-family: Arial, sans-serif; padding: 20px;'>
        <p>Hello <b>{user.UserName ?? "User"}</b>,</p>
        <p><b>Thank you for registering!</b><br>Please confirm your email address by clicking the button below:</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{confirmationLink}' style='
                background-color: #1e90ff;
                color: #ffffff;
                padding: 12px 24px;
                text-decoration: none;
                border-radius: 5px;
                font-weight: bold;
                display: inline-block;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                transition: background-color 0.3s ease;
            '>
                Confirm Email
            </a>
        </div>
        <p>If you did not create an account, please ignore this email.</p>
        <p style='font-size: 12px; color: #aaaaaa;'>&copy; {DateTime.UtcNow.Year} Dreckbude. All rights reserved.</p>
    </body>
    </html>";
        await SendEmailAsync(user.Email, subject, message);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var emailSettings = configuration.GetSection("EmailSettings");
        var port = emailSettings["SmtpPort"];
        var enableSsl = emailSettings["EnableSsl"];
        var senderEmail = emailSettings["SenderEmail"];
        var senderName = emailSettings["SenderName"];
        var smtpServer = emailSettings["SmtpServer"];
        var username = emailSettings["Username"];
        var password = emailSettings["Password"];

        if (port == null || enableSsl == null || senderEmail == null || senderName == null || smtpServer == null ||
            username == null || password == null)
            throw new InvalidOperationException("Email settings are not configured");

        var smtpClient = new SmtpClient(smtpServer)
        {
            Port = int.Parse(port),
            Credentials = new NetworkCredential(username, password),
            EnableSsl = bool.Parse(enableSsl)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, senderName),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log them)
            throw new InvalidOperationException("Email sending failed", ex);
        }
    }
}