using MBKC.Repository.Enums;
using MBKC.Repository.Redis.Models;
using MBKC.Repository.SMTPs.Models;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace MBKC.Repository.SMTPs.Repositories
{
    public class EmailRepository
    {
        public EmailRepository()
        {

        }

        private Email GetEmailProperty()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            return new Email()
            {
                Host = configuration.GetSection("Verification:Email:Host").Value,
                Port = int.Parse(configuration.GetSection("Verification:Email:Port").Value),
                SystemName = configuration.GetSection("Verification:Email:SystemName").Value,
                Sender = configuration.GetSection("Verification:Email:Sender").Value,
                Password = configuration.GetSection("Verification:Email:Password").Value,
            };
        }

        private string GetMessageToResetPassword(string systemName, string receiverEmail, string OTPCode)
        {
            string emailBody = "";
            string htmlParentDivStart = "<div style=\"font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2\">";
            string htmlParentDivEnd = "</div>";
            string htmlMainDivStart = "<div style=\"margin:50px auto;width:70%;padding:20px 0\">";
            string htmlMainDivEnd = "</div>";
            string htmlSystemNameDivStart = "<div style=\"border-bottom:1px solid #eee\">";
            string htmlSystemNameDivEnd = "</div";
            string htmlSystemNameSpanStart = "<span style=\"font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600\">";
            string htmlSystemNameSpanEnd = "</span>";
            string htmlHeaderBodyStart = "<p style=\"font-size:1.1em\">";
            string htmlHeaderBodyEnd = "</p>";
            string htmlBodyStart = "<p>";
            string htmlBodyEnd = "</p>";
            string htmlOTPCodeStart = "<h2 style=\"background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;\">";
            string htmlOTPCodeEnd = "</h2>";
            string htmlFooterBodyStart = "<p style=\"font-size:0.9em;\">";
            string htmlBreakLine = "<br />";
            string htmlFooterBodyEnd = "</p>";

            emailBody += htmlParentDivStart;
            emailBody += htmlMainDivStart;
            emailBody += htmlSystemNameDivStart + htmlSystemNameSpanStart + systemName + htmlSystemNameSpanEnd + htmlSystemNameDivEnd + htmlBreakLine;
            emailBody += htmlHeaderBodyStart + $"Hi {receiverEmail}," + htmlHeaderBodyEnd;
            emailBody += htmlBodyStart + $"We've received a request to reset the password from {receiverEmail}. " +
                $"Use the following OTP to complete your reset password procedures. OTP is valid for 10 minutes." + htmlBodyEnd;
            emailBody += htmlOTPCodeStart + OTPCode + htmlOTPCodeEnd;
            emailBody += htmlFooterBodyStart + "Regards," + htmlBreakLine + systemName + htmlFooterBodyEnd;
            emailBody += htmlMainDivEnd;
            emailBody += htmlParentDivEnd;

            return emailBody;
        }

        public string GetMessageToRegisterAccount(string receiverEmail, string password, string messageBody)
        {
            Email email = GetEmailProperty();
            string emailBody = "";
            string htmlTableDivStart = "<table style=\"border-collapse: collapse; width: 50%; margin: 20px auto; border: 1px solid #ddd;\">";
            string htmlTableDivEnd = "</div>";

            string htmlTable = String.Format(@"
                                        <table>
                                      <tr>
                                         <th>Email</th>
                                         <td>{0}</td>
                                      </tr>
                                    <tr>
                                      <th>Password</th>
                                      <td>{1}</td>
                                    </tr>
                                       </table>
                                  ", receiverEmail, password);

            string htmlParentDivStart = "<div style=\"font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2\">";
            string htmlParentDivEnd = "</div>";
            string htmlMainDivStart = "<div style=\"margin:50px auto;width:70%;padding:20px 0\">";
            string htmlMainDivEnd = "</div>";
            string htmlSystemNameDivStart = "<div style=\"border-bottom:1px solid #eee\">";
            string htmlSystemNameDivEnd = "</div";
            string htmlSystemNameSpanStart = "<span style=\"font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600\">";
            string htmlSystemNameSpanEnd = "</span>";
            string htmlHeaderBodyStart = "<p style=\"font-size:1.1em\">";
            string htmlHeaderBodyEnd = "</p>";
            string htmlBodyStart = "<p>";
            string htmlBodyEnd = "</p>";
            string htmlFooterBodyStart = "<p style=\"font-size:0.9em;\">";
            string htmlBreakLine = "<br />";
            string htmlFooterBodyEnd = "</p>";

            emailBody += htmlParentDivStart;
            emailBody += htmlMainDivStart;

            emailBody += htmlSystemNameDivStart + htmlSystemNameSpanStart
                        + email.SystemName + htmlSystemNameSpanEnd + htmlSystemNameDivEnd
                        + htmlBreakLine;

            emailBody += htmlHeaderBodyStart + $"Hi {receiverEmail}," + htmlHeaderBodyEnd;
            emailBody += htmlBodyStart + messageBody + htmlBodyEnd;
            emailBody += htmlTableDivStart + htmlTable + htmlTableDivEnd;
            emailBody += htmlFooterBodyStart + "Regards," + htmlBreakLine + email.SystemName + htmlFooterBodyEnd;
            emailBody += htmlMainDivEnd;
            emailBody += htmlParentDivEnd;

            return emailBody;
        }

        public string GetMessageToNotifyNonMappingProduct(string receiverEmail, string partnerName, string messageBody)
        {
            Email senderEmail = GetEmailProperty();
            string emailBody = "";
            string htmlParentDivStart = "<div style=\"font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2\">";
            string htmlParentDivEnd = "</div>";
            string htmlMainDivStart = "<div style=\"margin:50px auto;width:70%;padding:20px 0\">";
            string htmlMainDivEnd = "</div>";
            string htmlSystemNameDivStart = "<div style=\"border-bottom:1px solid #eee\">";
            string htmlSystemNameDivEnd = "</div";
            string htmlSystemNameSpanStart = "<span style=\"font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600\">";
            string htmlSystemNameSpanEnd = "</span>";
            string htmlHeaderBodyStart = "<p style=\"font-size:1.1em\">";
            string htmlHeaderBodyEnd = "</p>";
            string htmlBodyStart = "<p>";
            string htmlBodyEnd = "</p>";
            string htmlFooterBodyStart = "<p style=\"font-size:0.9em;\">";
            string htmlBreakLine = "<br />";
            string htmlFooterBodyEnd = "</p>";

            emailBody += htmlParentDivStart;
            emailBody += htmlMainDivStart;
            emailBody += htmlSystemNameDivStart + htmlSystemNameSpanStart + senderEmail.SystemName + htmlSystemNameSpanEnd + htmlSystemNameDivEnd + htmlBreakLine;
            emailBody += htmlHeaderBodyStart + $"Hi {receiverEmail}," + htmlHeaderBodyEnd;
            emailBody += htmlBodyStart + messageBody + htmlBodyEnd;
            emailBody += htmlFooterBodyStart + "Regards," + htmlBreakLine + senderEmail.SystemName + htmlFooterBodyEnd;
            emailBody += htmlMainDivEnd;
            emailBody += htmlParentDivEnd;

            return emailBody;
        }

        public EmailVerification SendEmailToResetPassword(string receiverEmail)
        {
            try
            {
                Email email = GetEmailProperty();
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mailMessage.From = new MailAddress(email.Sender);
                mailMessage.To.Add(new MailAddress(receiverEmail));
                mailMessage.Subject = "Reset your MBKC password";
                mailMessage.IsBodyHtml = true;
                string otpCode = GenerateOTPCode();
                mailMessage.Body = GetMessageToResetPassword(email.SystemName, receiverEmail, otpCode);
                smtp.Port = email.Port;
                smtp.Host = email.Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(email.Sender, email.Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mailMessage);
                EmailVerification emailVerification = new EmailVerification()
                {
                    Email = receiverEmail,
                    OTPCode = otpCode,
                    CreatedDate = DateTime.Now,
                    IsVerified = Convert.ToBoolean((int)EmailVerificationEnum.Status.NOT_VERIFIRED)
                };
                return emailVerification;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendAccountToEmailAsync(string reciever, string message)
        {
            try
            {
                Email email = GetEmailProperty();
                string subject = $"Email and Password in MBKC System";
                SmtpClient smtpClient = new SmtpClient(email.Host, email.Port);
                smtpClient.Credentials = new NetworkCredential(email.Sender, email.Password);
                smtpClient.EnableSsl = true;
                MailMessage mailMessage = new MailMessage(email.Sender, reciever, subject, message);
                mailMessage.IsBodyHtml = true;
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.InnerExceptions);
            }
        }

        public async Task SendEmailToNotifyNonMappingProduct(string receiverEmail, string message, string partnerName, Attachment attachment)
        {
            try
            {
                Email email = GetEmailProperty();
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mailMessage.From = new MailAddress(email.Sender);
                mailMessage.To.Add(new MailAddress(receiverEmail));
                mailMessage.Subject = $"Non-Mapping Items From {partnerName}";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = message;
                mailMessage.Attachments.Add(attachment);
                smtp.Port = email.Port;
                smtp.Host = email.Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(email.Sender, email.Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtp.SendMailAsync(mailMessage);
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendEmailToNotifyCancelOrder(string receiverEmail, string message, Attachment attachment)
        {
            try
            {
                Email email = GetEmailProperty();
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mailMessage.From = new MailAddress(email.Sender);
                mailMessage.To.Add(new MailAddress(receiverEmail));
                mailMessage.Subject = $"Cancel unprocessed orders";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = message;
                mailMessage.Attachments.Add(attachment);
                smtp.Port = email.Port;
                smtp.Host = email.Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(email.Sender, email.Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtp.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GenerateOTPCode()
        {
            Random random = new Random();
            string otp = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                int tempval = random.Next(0, 10);
                otp += tempval;
            }
            return otp;
        }
    }
}
