using MBKC.BAL.DTOs.Verifications;
using MBKC.DAL.Enums;
using MBKC.DAL.RedisModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Utils
{
    public static class EmailUtil
    {
        public static string GetHTMLToResetPassword(string systemName, string receiverEmail, string OTPCode)
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

        public static string MessageRegisterAccount(string systemName, string receiverEmail, string password)
        {
            string emailBody = "";
            string htmlTableDivStart = "<table style=\"border-collapse: collapse; width: 50%; margin: 20px auto; border: 1px solid #ddd;\">";
            string htmlTableDivEnd = "</div>";

            string htmlTable = String.Format(@"
                                        <table>
                                      <tr>
                                         <th>Tài khoản</th>
                                         <th>Mật khẩu</th>
                                      </tr>
                                    <tr>
                                      <td>{0}</td>
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
                        + systemName + htmlSystemNameSpanEnd + htmlSystemNameDivEnd
                        + htmlBreakLine;

            emailBody += htmlHeaderBodyStart + $"Hi {receiverEmail}," + htmlHeaderBodyEnd;
            emailBody += htmlBodyStart + $"We've received a request to regiser account from {receiverEmail}. " +
                                         $"Here is email and password to access to the system" + htmlBodyEnd;
            emailBody += htmlTableDivStart + htmlTable + htmlTableDivEnd;
            emailBody += htmlFooterBodyStart + "Regards," + htmlBreakLine + systemName + htmlFooterBodyEnd;
            emailBody += htmlMainDivEnd;
            emailBody += htmlParentDivEnd;

            return emailBody;
        }

        public static EmailVerification SendEmailToResetPassword(Email email, EmailVerificationRequest receiverEmail)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mailMessage.From = new MailAddress(email.Sender);
                mailMessage.To.Add(new MailAddress(receiverEmail.Email));
                mailMessage.Subject = "Reset your MBKC password";
                mailMessage.IsBodyHtml = true;
                string otpCode = GenerateOTPCode();
                mailMessage.Body = GetHTMLToResetPassword(email.SystemName, receiverEmail.Email, otpCode);
                smtp.Port = email.Port;
                smtp.Host = email.Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(email.Sender, email.Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mailMessage);
                EmailVerification emailVerification = new EmailVerification()
                {
                    Email = receiverEmail.Email,
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

        public static async Task SendEmailAndPasswordToEmail(Email email, string reciever, string message, string roleName)
        {
            try
            {
                
                string subject = $"Tài khoản và mật khẩu cho {roleName} ";
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

        private static string GenerateOTPCode()
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
