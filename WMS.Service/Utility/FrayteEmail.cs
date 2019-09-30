using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using WMS.Model.Email;

namespace WMS.Services.Utility
{
    public static class EmailService
    {
        public static bool SendMail(SMTPConfiguration emailSetting)
        {
            try
            { 
                using (MailMessage mail = new MailMessage())
                {
                    //HOST
                    string host = emailSetting.MailHostName;

                    //PORT
                    int port = Convert.ToInt32(emailSetting.SMTPport);

                    //USER NAME
                    string userName = emailSetting.SMTPUserName;

                    //PASSWORD
                    string password = emailSetting.SMTPPassword;

                    //Enable Ssl
                    string enableSsl = emailSetting.EnableSsl;

                    //BCC
                    string bccEmail = emailSetting.BCC;

                    //FROM MAIL
                    string fromMail = emailSetting.FromMail;
                    MailAddress MailFrom = new MailAddress(fromMail, emailSetting.DisplayName);
                    mail.From = MailFrom;

                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = emailSetting.To.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    if (!string.IsNullOrEmpty(emailSetting.CC))
                    {
                        string[] CCarray = emailSetting.CC.Split(new string[] { ";" }, StringSplitOptions.None);
                        string[] DistiCC = CCarray.Distinct().ToArray();
                        for (int i = 0; i < DistiCC.Length; i++)
                        {
                            strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                        }
                    }
                   // else { strCCEMail = "meena@hexamind.com"; }
                    //CC 
                    if (!string.IsNullOrEmpty(strCCEMail))
                    {
                        string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                        for (int i = 0; i < strCCAdress.Length; i++)
                        {
                            if (Convert.ToString(strCCAdress[i]).Trim() != "")
                            {
                                MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                                mail.CC.Add(MailCC);
                            }
                        }
                    }

                    //BCC
                    if (!string.IsNullOrEmpty(bccEmail))
                    {
                        mail.Bcc.Add(bccEmail);
                    }

                    //To Dos
                    //mail.ReplyTo

                    //SUBJECT
                    mail.Subject = emailSetting.Subject;

                    //BODY
                    mail.Body = emailSetting.MailContent;
                    mail.IsBodyHtml = true;
                    if (!string.IsNullOrEmpty(emailSetting.Attachments))
                    {
                        string[] strAttachmentPath = emailSetting.Attachments.Split(new string[] { ";" }, StringSplitOptions.None);
                        for (int i = 0; i < strAttachmentPath.Length; i++)
                        {
                            if (strAttachmentPath[i].Trim() != "")
                            {
                                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                                mail.Attachments.Add(attachment);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(emailSetting.HeaderLogo))
                    {
                        Attachment logoImageAtt = new Attachment(emailSetting.HeaderLogo);
                        mail.Attachments.Add(logoImageAtt);
                        logoImageAtt.ContentDisposition.Inline = true;
                        logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        logoImageAtt.ContentId = "Logo";
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}