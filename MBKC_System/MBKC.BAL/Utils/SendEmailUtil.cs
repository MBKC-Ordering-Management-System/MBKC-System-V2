using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Utils
{
    public static class SendEmailUtil
    {
        public static async Task SendEmail(string email, string message, string roleName)
        {
            try
            {
                #region Init
                string mbkcEmail = "multibrandkitchencenter@gmail.com";
                string smtpPassword = "biwf jmju xqhl uibw";
                string smtpServer = "smtp.gmail.com";
                string subject = $"Tài khoản và mật khẩu cho {roleName} ";
                #endregion

                #region Connection
                int smtpPort = 587; // Port của SMTP server (thường là 587 hoặc 25)
                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
                smtpClient.Credentials = new NetworkCredential(mbkcEmail, smtpPassword);
                smtpClient.EnableSsl = true; // Sử dụng SSL để kết nối SMTP server nếu được yêu cầu

                MailMessage mailMessage = new MailMessage(mbkcEmail, email, subject, message);
                mailMessage.IsBodyHtml = true;
                await smtpClient.SendMailAsync(mailMessage);
                #endregion
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.InnerExceptions);
            }
        }

        #region Send email to company
        public static string Message(string email, string password, string roleName)
        {
            try
            {
                string message = String.Format(@"
                  <html>
                    <body>
                      <h2>Tài khoản và mật khẩu của {0}</h2>
                      <p>Đây là tài khoản và mật khẩu để đăng nhập vào hệ thống</p>
                      <h3>Email:</h3>
                      <p> {1} </p>
                      <h3>Password:</h3>
                      <p> {2} </p>
                   </body>
                  </html>",roleName, email, password);
                return message;
            }
            catch (AggregateException ex)
            {
                throw new AggregateException(ex.InnerExceptions);
            }
        }
        #endregion
    }
}
