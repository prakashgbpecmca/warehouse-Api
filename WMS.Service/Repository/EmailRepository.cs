using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WMS.Model.Common;
using WMS.Model.DYO;
using WMS.Model.Email;
using WMS.Service.DataAccess;
using WMS.Services.Utility;

namespace WMS.Service.Repository
{
    public class EmailRepository
    {
        WMSEntities dbContext = new WMSEntities();

        #region Identity Email

        public bool SendForgetPasswordEmail(int userId, string secretToken)
        {
            try
            {
                EmailE1_2 emailE1_2 = new EmailE1_2();

                var user = dbContext.WMSUsers.Find(userId);
                var userDetail = (from u in dbContext.WMSUsers
                                  join ur in dbContext.WMSUserRoles on u.Id equals ur.UserId
                                  where u.Id == userId
                                  select new
                                  {
                                      RoleId = ur.RoleId,
                                      Name = u.ContactFirstName, //+ " " + u.ContactLastName,
                                      Email = u.Email,
                                      CompanyName = u.CompanyName,
                                  }).FirstOrDefault();
                if (userDetail != null)
                {
                    emailE1_2.Name = userDetail.Name;
                    emailE1_2.To = userDetail.Email;
                    emailE1_2.CC = "";
                    emailE1_2.BCC = "";
                }

                string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
                emailE1_2.RecoveryLink = string.Format(AppSettings.PublicSiteAddress + "/#/" + "reset-password?id={0}&token={1}", userId, secretToken);
                emailE1_2.SiteLink = AppSettings.PublicSiteAddress;
                emailE1_2.SiteLink = AppSettings.PublicSiteAddress;

                string template = File.ReadAllText(AppSettings.EmailServicePath + "/E1_2.cshtml");
                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, emailE1_2, null, null);
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = "Lionstar Design your Own (DYO) - Forget Password";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(AppSettings.TOCC) ? emailE1_2.To : AppSettings.TOCC;
                emailSetting.CC = string.IsNullOrEmpty(AppSettings.TOCC) ? emailE1_2.CC : "";
                emailSetting.BCC = AppSettings.BCC;
                emailSetting.Attachments = "";
                EmailService.SendMail(emailSetting);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region "E1 Email Templates"

        public bool Email_E1(EmailE1 emailE1)
        {
            try
            {
                string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
                FillEmail_E1Model(emailE1);
                emailE1.RecoveryLink = string.Format(AppSettings.PublicSiteAddress + "newPassword/{0}", emailE1.UserId);
                emailE1.To = emailE1.Email;
                string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E1.cshtml");
                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, emailE1, null, null);

                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = "Login Credentials";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(AppSettings.TOCC) ? emailE1.To : AppSettings.TOCC;
                emailSetting.CC = !string.IsNullOrEmpty(AppSettings.TOCC) ? emailE1.CC : "";
                emailSetting.BCC = AppSettings.BCC;
                emailSetting.Attachments = "";

                EmailService.SendMail(emailSetting);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void FillEmail_E1Model(EmailE1 emailE1)
        {
            var user = new WMSUserRepository().GetUser(emailE1.UserId);

            if (user != null)
            {
                emailE1.Name = user.ContactFirstName; //+ " " + user.ContactLastName;
                emailE1.Email = user.Email;
            }
        }

        #endregion

        #region "E2 Email Templates"

        public bool Email_E2(int orderId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            var emailDataSource = fillEmail_E2Model(orderId);
            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E2.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber+" - "+ "DYO: New Order Confirmation";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.To : AppSettings.TOCC;
                emailSetting.CC = !string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.CC : "";
                if (string.IsNullOrEmpty(emailSetting.CC))
                {
                    emailSetting.CC = "";
                }
                emailSetting.BCC = AppSettings.BCC;
                var isJobSheet = new DYORepository().IsJobSheetCreate(orderId);
                if (isJobSheet)
                {
                    emailSetting.Attachments = GetOrderJobSheetAttachments(orderId);
                }
                else
                {
                    emailSetting.Attachments = "";
                }

                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Customer Confirmation Email Order Number: " + emailDataSource.OrderNumber + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + emailDataSource.To));
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool Email_E2_1(int orderId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            var emailDataSource = fillEmail_E21Model(orderId);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E2_1.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: New Order Received";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.To : AppSettings.TOCC;
                emailSetting.CC = !string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.CC : "";
                if (string.IsNullOrEmpty(emailSetting.CC))
                {
                    emailSetting.CC = "";
                }
                emailSetting.BCC = AppSettings.BCC;
                var isJobSheet = new DYORepository().IsJobSheetCreate(orderId);
                if (isJobSheet)
                {
                    emailSetting.Attachments = GetOrderJobSheetAttachments(orderId);
                }
                else
                {
                    emailSetting.Attachments = "";
                }

                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Sales Coordinator New Order Email Order Number: " + emailDataSource.OrderNumber + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + emailDataSource.To));
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool Email_E2_2(int orderId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            var emailDataSource = fillEmail_E22Model(orderId, "cu", "");
            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E2_2.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Order Accepted";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(emailDataSource.To) ? AppSettings.TOCC : emailDataSource.To;
                emailSetting.CC = !string.IsNullOrEmpty(emailDataSource.CC) ? "" : emailDataSource.CC;
                emailSetting.BCC = AppSettings.BCC;

                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Sales Cordinator accepted the order: " + emailDataSource.OrderNumber + " " + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + " " + emailDataSource.To));
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool Email_E2_3(int orderId, string reason)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            var emailDataSource = fillEmail_E22Model(orderId, "cu", reason);
            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E2_3.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Order Rejected";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(emailDataSource.To) ? AppSettings.TOCC : emailDataSource.To;
                emailSetting.CC = !string.IsNullOrEmpty(emailDataSource.CC) ? "" : emailDataSource.CC;
                emailSetting.Attachments = "";
                emailSetting.BCC = AppSettings.BCC;

                EmailService.SendMail(emailSetting);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Email_E2_4(int orderId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            var emailDataSource = fillEmail_E22Model(orderId, "m", "");
            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E2_4.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: New Order Received";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(emailDataSource.To) ? AppSettings.TOCC : emailDataSource.To;
                emailSetting.CC = !string.IsNullOrEmpty(emailDataSource.CC) ? "" : emailDataSource.CC;
                emailSetting.BCC = AppSettings.BCC;
                emailSetting.Attachments = GetOrderJobSheetAttachmentsMerchandiser(orderId);

                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Merchanider New Order Received Email Order Number: " + emailDataSource.OrderNumber + " " + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + " " + emailDataSource.To));
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool Email_E2_5(int orderId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            var emailDataSource = fillEmail_E22Model(orderId, "cu", "");
            emailDataSource.CC = emailDataSource.coordmail;
            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E2_5.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Sample Creation Started";
                emailSetting.HeaderLogo = "";
                emailSetting.Attachments = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(emailDataSource.To) ? AppSettings.TOCC : emailDataSource.To;
                emailSetting.CC = string.IsNullOrEmpty(emailDataSource.CC) ? "" : emailDataSource.CC;
                emailSetting.BCC = AppSettings.BCC;

                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Email to customer Sapmle creation process  started Order Number: " + emailDataSource.OrderNumber + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + emailDataSource.To));
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool Email_E2_6(int orderId, string reason)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            var emailDataSource = fillEmail_E22Model(orderId, "c", reason);
            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E2_6.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Order Rejected";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(emailDataSource.To) ? AppSettings.TOCC : emailDataSource.To;
                emailSetting.CC = !string.IsNullOrEmpty(emailDataSource.CC) ? "" : emailDataSource.CC;
                emailSetting.BCC = AppSettings.BCC;

                EmailService.SendMail(emailSetting);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Email_E2_7(int orderId, List<StrikeOffSampleLogo> model)
        {
            try
            {
                string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
                var emailDataSource = fillEmail_E22Model(orderId, "cu", "");
                string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E2_7.cshtml");
                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, emailDataSource, null, null);

                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Sample Logo Uploaded";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(emailDataSource.To) ? AppSettings.TOCC : emailDataSource.To;
                emailSetting.CC = string.IsNullOrEmpty(emailDataSource.CC) ? "" : emailDataSource.CC;
                emailSetting.BCC = AppSettings.BCC;
                emailSetting.Attachments = GetSampleStrikeLogos(model);
                // var logos = GetSampleStrikeLogos(model);

                //foreach (var item in logos.Split(','))
                //{
                //    emailSetting.Attachments += item;
                //}

                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Email to customer new strikeoff uploaded Order Number: " + emailDataSource.OrderNumber + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + emailDataSource.To));
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool Email_E2_8(int orderId, string reason)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            var emailDataSource = fillEmail_E22Model(orderId, "m", reason);
            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E2_8.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Sample Logo rejected";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(emailDataSource.To) ? AppSettings.TOCC : emailDataSource.To;
                emailSetting.CC = !string.IsNullOrEmpty(emailDataSource.CC) ? "" : emailDataSource.CC;
                emailSetting.BCC = AppSettings.BCC;

                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Email to merchandiser Sample logo rejected Order: " + emailDataSource.OrderNumber + " " + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + " " + emailDataSource.To));

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool Email_E2_9(int orderId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            var emailDataSource = fillEmail_E22Model(orderId, "m", "");
            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E2_9.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Sample Logo accepted";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(emailDataSource.To) ? AppSettings.TOCC : emailDataSource.To;
                emailSetting.CC = !string.IsNullOrEmpty(emailDataSource.CC) ? "" : emailDataSource.CC;
                emailSetting.BCC = AppSettings.BCC;

                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Email to merchandiser Sample logo  accepted Order: " + emailDataSource.OrderNumber + " " + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + " " + emailDataSource.To));

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public EmailE_2 fillEmail_E2Model(int OrderId)
        {
            EmailE_2 emailModel = new EmailE_2();

            emailModel = (from Ord in dbContext.Orders
                          join OrdAdd in dbContext.OrderAddresses on Ord.OrderAddressId equals OrdAdd.Id
                          join Cnt in dbContext.Countries on OrdAdd.CountryId equals Cnt.CountryId
                          //join Usr in dbContext.WMSUsers on Ord.CustomerId equals Usr.Id
                          join Usr in dbContext.WMSUsers on Ord.CreatedBy equals Usr.Id
                          where Ord.Id == OrderId
                          select new EmailE_2
                          {
                              PONumber = Ord.PONumber,
                              OrderNumber = Ord.OrderNumber,
                              OrderName = Ord.OrderName,
                              DeliveryType = Ord.OrderType,
                              CustomerName = Usr.ContactFirstName,
                              To = Usr.Email,
                              TargetDeliveryDate = Ord.CreatedOnUtc,
                              RequestedDeliveryDateTime = Ord.RequestedDeliveryDate,
                              DeliveryAddress = new Model.Email.OrderAddress()
                              {
                                  FirstName = OrdAdd.ContactFirstName,
                                  LastName = OrdAdd.ContactLastName,
                                  Address = OrdAdd.Address,
                                  CompanyName = OrdAdd.CompanyName,
                                  Address2 = OrdAdd.Address2,
                                  Address3 = OrdAdd.Address3,
                                  City = OrdAdd.City,
                                  Phone = OrdAdd.PhoneNumber,
                                  Area = OrdAdd.Suburb,
                                  PostCode = OrdAdd.PostCode,
                                  State = OrdAdd.State,
                                  Email = OrdAdd.Email,
                                  Id = OrdAdd.Id,
                                  Country = new Model.User.WMSCountry()
                                  {
                                      CountryId = Cnt.CountryId,
                                      Code = Cnt.CountryCode,
                                      Code2 = Cnt.CountryCode2,
                                      Name = Cnt.CountryName,
                                      TimezoneId = Cnt.TimeZoneId != null ? Cnt.TimeZoneId.Value : 0
                                  }
                              }
                          }).FirstOrDefault();

            emailModel.ProductDetail = (from Ord in dbContext.Orders
                                        join OrdDe in dbContext.OrderDesigns on Ord.Id equals OrdDe.OrderId
                                        join Prdt in dbContext.ProductMasters on OrdDe.ProductId equals Prdt.Id
                                        join OrdDeDe in dbContext.OrderDesignDetails on OrdDe.Id equals OrdDeDe.OrderDesignId
                                        join PrdtSKU in dbContext.ProductSKUs on OrdDeDe.ProductSKUId equals PrdtSKU.Id
                                        join Clr in dbContext.Colors on PrdtSKU.ColorId equals Clr.ID into leftCpolor
                                        from tempColor in leftCpolor.DefaultIfEmpty()
                                        join ps in dbContext.ProductStyles on OrdDe.ProductColor equals ps.StyleCode into leftStyle
                                        from tempStyle in leftStyle.DefaultIfEmpty()
                                        join Sz in dbContext.Sizes on PrdtSKU.SizeId equals Sz.ID
                                        where Ord.Id == OrderId
                                        select new E_2ProductDetail
                                        {
                                            DesignNumber = OrdDe.DesignNumber,
                                            ProductName = Prdt.ProductName,
                                            ProductCode = Prdt.ProductCode,
                                            SKU = PrdtSKU.SKU,
                                            Color = tempColor == null ? (tempStyle == null ? "" : tempStyle.StyleCode) : tempColor.color1,
                                            Size = Sz.size1,
                                            Quantity = OrdDeDe.Quantity
                                        }).ToList();

            emailModel.TotalQuantity = emailModel.ProductDetail.Sum(a => a.Quantity);
            emailModel.TargetDeliveryDisplayDate = emailModel.TargetDeliveryDate.ToString("dd-MMM-yyyy");

            var Count = 1;

            foreach (var PD in emailModel.ProductDetail)
            {
                PD.SerialNo = Count;
                Count++;
            }

            if (emailModel.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
            {
                emailModel.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
            }
            if (emailModel.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
            {
                emailModel.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
            }
            if (emailModel.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
            {
                emailModel.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
            }

            var orderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == OrderId).ToList();
            if (orderDesigns.Count == 1)
            {
                emailModel.OrderName = orderDesigns[0].DesignName;
            }

            if (emailModel.RequestedDeliveryDateTime.HasValue)
            {
                emailModel.RequestedDeliveryDate = emailModel.RequestedDeliveryDateTime.Value.ToString("dd-MMM-yyyy");
            }
            else
            {
                emailModel.RequestedDeliveryDate = string.Empty;
            }

            return emailModel;
        }

        public EmailE_2 fillEmail_E21Model(int OrderId)
        {
            EmailE_2 emailModel = new EmailE_2();

            emailModel = (from Ord in dbContext.Orders
                          join OrdAdd in dbContext.OrderAddresses on Ord.OrderAddressId equals OrdAdd.Id
                          join Cnt in dbContext.Countries on OrdAdd.CountryId equals Cnt.CountryId
                          join Usr in dbContext.WMSUsers on Ord.SalesCordinator equals Usr.Id
                          where Ord.Id == OrderId
                          select new EmailE_2
                          {
                              OrderId = Ord.Id,
                              PONumber = Ord.PONumber,
                              OrderNumber = Ord.OrderNumber,
                              OrderName = Ord.OrderName,
                              DeliveryType = Ord.OrderType,
                              CustomerName = Usr.ContactFirstName, //+ " " + Usr.ContactLastName,
                              SalesCoordinatorId = Usr.Id,
                              To = Usr.Email,
                              TargetDeliveryDate = Ord.CreatedOnUtc,
                              RequestedDeliveryDateTime = Ord.RequestedDeliveryDate,
                              DeliveryAddress = new Model.Email.OrderAddress()
                              {
                                  FirstName = OrdAdd.ContactFirstName,
                                  LastName = OrdAdd.ContactLastName,
                                  Address = OrdAdd.Address,
                                  CompanyName = OrdAdd.CompanyName,
                                  Address2 = OrdAdd.Address2,
                                  Address3 = OrdAdd.Address3,
                                  City = OrdAdd.City,
                                  Phone = OrdAdd.PhoneNumber,
                                  Area = OrdAdd.Suburb,
                                  PostCode = OrdAdd.PostCode,
                                  State = OrdAdd.State,
                                  Email = OrdAdd.Email,
                                  Id = OrdAdd.Id,
                                  Country = new Model.User.WMSCountry()
                                  {
                                      CountryId = Cnt.CountryId,
                                      Code = Cnt.CountryCode,
                                      Code2 = Cnt.CountryCode2,
                                      Name = Cnt.CountryName,
                                      TimezoneId = Cnt.TimeZoneId != null ? Cnt.TimeZoneId.Value : 0
                                  }
                              }
                          }).FirstOrDefault();


            emailModel.ProductDetail = (from Ord in dbContext.Orders
                                        join OrdDe in dbContext.OrderDesigns on Ord.Id equals OrdDe.OrderId
                                        join Prdt in dbContext.ProductMasters on OrdDe.ProductId equals Prdt.Id
                                        join OrdDeDe in dbContext.OrderDesignDetails on OrdDe.Id equals OrdDeDe.OrderDesignId
                                        join PrdtSKU in dbContext.ProductSKUs on OrdDeDe.ProductSKUId equals PrdtSKU.Id
                                        join Clr in dbContext.Colors on PrdtSKU.ColorId equals Clr.ID into leftCpolor
                                        from tempColor in leftCpolor.DefaultIfEmpty()
                                        join ps in dbContext.ProductStyles on OrdDe.ProductColor equals ps.StyleCode into leftStyle
                                        from tempStyle in leftStyle.DefaultIfEmpty()
                                        join Sz in dbContext.Sizes on PrdtSKU.SizeId equals Sz.ID
                                        where Ord.Id == OrderId
                                        select new E_2ProductDetail
                                        {
                                            DesignNumber = OrdDe.DesignNumber,
                                            ProductName = Prdt.ProductName,
                                            ProductCode = Prdt.ProductCode,
                                            SKU = PrdtSKU.SKU,
                                            Color = tempColor == null ? (tempStyle == null ? "" : tempStyle.StyleCode) : tempColor.color1,
                                            Size = Sz.size1,
                                            Quantity = OrdDeDe.Quantity
                                        }).ToList();

            emailModel.TotalQuantity = emailModel.ProductDetail.Sum(a => a.Quantity);
            emailModel.TargetDeliveryDisplayDate = emailModel.TargetDeliveryDate.ToString("dd-MMM-yyyy");

            emailModel.AcceptLink = AppSettings.PublicSiteAddress + "#/order-confirm-reject/?user=" + emailModel.SalesCoordinatorId + "&id=" + emailModel.OrderId;
            emailModel.RejectLink = AppSettings.PublicSiteAddress + "#/order-confirm-reject/?user=" + emailModel.SalesCoordinatorId + "&id=" + emailModel.OrderId;

            if (emailModel.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
            {
                emailModel.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
            }
            if (emailModel.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
            {
                emailModel.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
            }
            if (emailModel.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
            {
                emailModel.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
            }
            var Count = 1;
            foreach (var PD in emailModel.ProductDetail)
            {
                PD.SerialNo = Count;
                Count++;
            }
            if (emailModel.RequestedDeliveryDateTime.HasValue)
            {
                emailModel.RequestedDeliveryDate = emailModel.RequestedDeliveryDateTime.Value.ToString("dd-MMM-yyyy");
            }
            else
            {
                emailModel.RequestedDeliveryDate = string.Empty;
            }
            var orderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == OrderId).ToList();
            if (orderDesigns.Count == 1)
            {
                emailModel.OrderName = orderDesigns[0].DesignName;
            }
            return emailModel;
        }

        public EmailE_2 fillEmail_E22Model(int OrderId, string type, string reason)
        {
            EmailE_2 emailModel = new EmailE_2();

            var obj = dbContext.Orders.Where(p => p.Id == OrderId).FirstOrDefault();
            string DateNew = obj.ConfirmedExFactoryDate != null ? obj.ConfirmedExFactoryDate.Value.ToString("dd-MMM-yyyy") : " ";

            emailModel = (from Ord in dbContext.Orders
                          join OrdAdd in dbContext.OrderAddresses on Ord.OrderAddressId equals OrdAdd.Id
                          join Cnt in dbContext.Countries on OrdAdd.CountryId equals Cnt.CountryId
                          join Usr in dbContext.WMSUsers on Ord.CreatedBy equals Usr.Id
                          join coord in dbContext.WMSUsers on Ord.SalesCordinator equals coord.Id into temp
                          from coord in temp.DefaultIfEmpty()
                          join merch in dbContext.WMSUsers on Ord.Mechandiser equals merch.Id into temp2
                          from merch1 in temp2.DefaultIfEmpty()
                          where Ord.Id == OrderId
                          select new EmailE_2
                          {
                              CreatedBy = Ord.CreatedBy,
                              OrderId = Ord.Id,
                              PONumber = Ord.PONumber,
                              OrderNumber = Ord.OrderNumber,
                              OrderName = Ord.OrderName,
                              DeliveryType = Ord.OrderType,
                              CustomerName = Usr.ContactFirstName,
                              custmail = Usr.Email,
                              TargetDeliveryDate = Ord.CreatedOnUtc,
                              OrderPlacedDate = Ord.CreatedOnUtc,
                              //CoordinatorName = type == "cu" ? (Usr.ContactFirstName + " " + Usr.ContactLastName) : (coord.ContactFirstName + " " + coord.ContactLastName),
                              //CoordinatorName = type == "cu" ? (Usr.ContactFirstName) : (coord.ContactFirstName),
                              CoordinatorName = coord.ContactFirstName,
                              MerchandiserName = merch1.ContactFirstName, //+ " " + merch1.ContactLastName,
                              MerchandiserDelDateInEmail = DateNew,
                              MerchandiserDelDate = Ord.ExpectedDeliveryOnUtc,
                              coordinatorid = Ord.SalesCordinator,
                              merchdiserid = Ord.Mechandiser,

                              coordmail = coord.Email,
                              merchmail = merch1.Email,
                              CC = "",
                              DeliveryAddress = new Model.Email.OrderAddress()
                              {
                                  FirstName = OrdAdd.ContactFirstName,
                                  LastName = OrdAdd.ContactLastName,
                                  Address = OrdAdd.Address,
                                  CompanyName = OrdAdd.CompanyName,
                                  Address2 = OrdAdd.Address2,
                                  Address3 = OrdAdd.Address3,
                                  City = OrdAdd.City,
                                  Phone = OrdAdd.PhoneNumber,
                                  Area = OrdAdd.Suburb,
                                  PostCode = OrdAdd.PostCode,
                                  State = OrdAdd.State,
                                  Email = OrdAdd.Email,
                                  Id = OrdAdd.Id,
                                  Country = new Model.User.WMSCountry()
                                  {
                                      CountryId = Cnt.CountryId,
                                      Code = Cnt.CountryCode,
                                      Code2 = Cnt.CountryCode2,
                                      Name = Cnt.CountryName,
                                      TimezoneId = Cnt.TimeZoneId != null ? Cnt.TimeZoneId.Value : 0
                                  }
                              }
                          }).FirstOrDefault();

            emailModel.ProductDetail = (from Ord in dbContext.Orders
                                        join OrdDe in dbContext.OrderDesigns on Ord.Id equals OrdDe.OrderId
                                        join Prdt in dbContext.ProductMasters on OrdDe.ProductId equals Prdt.Id
                                        join OrdDeDe in dbContext.OrderDesignDetails on OrdDe.Id equals OrdDeDe.OrderDesignId
                                        join PrdtSKU in dbContext.ProductSKUs on OrdDeDe.ProductSKUId equals PrdtSKU.Id
                                        join Clr in dbContext.Colors on PrdtSKU.ColorId equals Clr.ID into leftCpolor
                                        from tempColor in leftCpolor.DefaultIfEmpty()
                                        join ps in dbContext.ProductStyles on OrdDe.ProductColor equals ps.StyleCode into leftStyle
                                        from tempStyle in leftStyle.DefaultIfEmpty()
                                        join Sz in dbContext.Sizes on PrdtSKU.SizeId equals Sz.ID
                                        where Ord.Id == OrderId
                                        select new E_2ProductDetail
                                        {
                                            DesignNumber = OrdDe.DesignNumber,
                                            ProductName = Prdt.ProductName,
                                            ProductCode = Prdt.ProductCode,
                                            SKU = PrdtSKU.SKU,
                                            Color = tempColor == null ? (tempStyle == null ? "" : tempStyle.StyleCode) : tempColor.color1,
                                            Size = Sz.size1,
                                            Quantity = OrdDeDe.Quantity
                                        }).ToList();

            emailModel.TotalQuantity = emailModel.ProductDetail.Sum(a => a.Quantity);
            emailModel.TargetDeliveryDisplayDate = emailModel.TargetDeliveryDate.ToString("dd-MMM-yyyy");
            emailModel.OrderPlacedDisplayDate = emailModel.OrderPlacedDate.ToString("dd-MMM-yyyy");
            emailModel.MerchandiserDisplayDelDate = Convert.ToDateTime(emailModel.MerchandiserDelDate).ToString("dd-MMM-yyyy");

            emailModel.SiteAddress = AppSettings.PublicSiteAddress;

            emailModel.AcceptLink = AppSettings.PublicSiteAddress + "#/order--merchandiser-confirm-reject/?user=" + emailModel.merchdiserid + "&id=" + emailModel.OrderId;
            emailModel.RejectLink = AppSettings.PublicSiteAddress + "#/order--merchandiser-confirm-reject/?user=" + emailModel.merchdiserid + "&id=" + emailModel.OrderId;


            emailModel.LogoAcceptLink = AppSettings.PublicSiteAddress + "#/order-strike-off-confirm-reject/?user=" + emailModel.CreatedBy + "&id=" + emailModel.OrderId + "&action=accept";
            emailModel.LogoRejectLink = AppSettings.PublicSiteAddress + "#/order-strike-off-confirm-reject/?user=" + emailModel.CreatedBy + "&id=" + emailModel.OrderId + "&action=reject";
            emailModel.Reason = reason;
            if (type == "cu") { emailModel.To = emailModel.custmail; }
            else if (type == "c") { emailModel.To = emailModel.coordmail; }
            else if (type == "m") { emailModel.To = emailModel.merchmail; }

            var Count = 1;
            foreach (var PD in emailModel.ProductDetail)
            {
                PD.SerialNo = Count;
                Count++;
            }

            if (emailModel.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
            {
                emailModel.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
            }
            if (emailModel.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
            {
                emailModel.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
            }
            if (emailModel.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
            {
                emailModel.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
            }

            var orderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == OrderId).ToList();
            if (orderDesigns.Count == 1)
            {
                emailModel.OrderName = orderDesigns[0].DesignName;
            }

            return emailModel;
        }

        public string GetSampleStrikeLogos(List<StrikeOffSampleLogo> model)
        {
            try
            {
                string attachment = string.Empty;
                if (model.Count > 0)
                {
                    foreach (var item in model)
                    {
                        var data = dbContext.OrderEmblishments.Where(x => x.Id == item.OrderEmblishmentID).FirstOrDefault();
                        if (File.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + item.OrderID + "/" + data.OrderDesignId + "/" + item.FileName)))
                        {
                            attachment += HttpContext.Current.Server.MapPath("~/Files/Orders/" + item.OrderID + "/" + data.OrderDesignId + "/" + item.FileName) + ";";
                        }
                    }
                }
                return attachment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetPickupnoteAttachments(int orderid)
        {
            string filepath = string.Empty;
            var dbOrder = dbContext.Orders.Find(orderid);

            if (dbOrder != null)
            {
                string fileName = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderid + "/") + dbOrder.PickupNoteFile;

                if (File.Exists(fileName))
                {
                    filepath = fileName;
                }
            }
            return filepath;
        }

        public string GetOrderJobSheetAttachments(int orderId)
        {
            string attachment = string.Empty;
            var dbOrder = dbContext.Orders.Find(orderId);

            if (dbOrder != null)
            {
                if (!string.IsNullOrEmpty(dbOrder.JobSheetFile))
                {
                    if (File.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/" + dbOrder.JobSheetFile)))
                    {
                        attachment = HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/" + dbOrder.JobSheetFile);
                    }
                }
            }

            return attachment;
        }

        public string GetOrderJobSheetAttachmentsMerchandiser(int orderId)
        {
            string attachment = string.Empty;
            var dbOrder = dbContext.Orders.Find(orderId);

            if (dbOrder != null)
            {
                if (!string.IsNullOrEmpty(dbOrder.JobSheetFile))
                {
                    if (File.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/" + dbOrder.JobSheetFile)))
                    {
                        attachment += HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/" + dbOrder.JobSheetFile);
                    }
                    if (File.Exists(HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/" + dbOrder.JobSheetShippingDetailFile)))
                    {
                        attachment += ";" + HttpContext.Current.Server.MapPath("~/Files/Orders/" + orderId + "/" + dbOrder.JobSheetShippingDetailFile);
                    }
                }
            }

            return attachment;
        }

        #endregion

        #region "E3 Email Templates"

        private EmailE_3 fillEmail_E3Model(int orderid)
        {
            EmailE_3 emailModel = new EmailE_3();

            var obj = dbContext.Orders.Where(p => p.Id == orderid).FirstOrDefault();
            string DateNew = obj.ConfirmedExFactoryDate != null ? obj.ConfirmedExFactoryDate.Value.ToString("dd-MMM-yyyy") : " ";

            emailModel = (from Ord in dbContext.Orders
                          join OrdAdd in dbContext.OrderAddresses on Ord.OrderAddressId equals OrdAdd.Id
                          join Cnt in dbContext.Countries on OrdAdd.CountryId equals Cnt.CountryId
                          join Usr in dbContext.WMSUsers on Ord.WarehouseUserId equals Usr.Id
                          join cust in dbContext.WMSUsers on Ord.CreatedBy equals cust.Id
                          where Ord.Id == orderid
                          select new EmailE_3
                          {
                              OrderId = Ord.Id,
                              PONumber = Ord.PONumber,
                              OrderNumber = Ord.OrderNumber,
                              OrderName = Ord.OrderName,
                              DeliveryType = Ord.OrderType,
                              CustomerName = cust.ContactFirstName, //+ " " + Usr.ContactLastName,
                              WarehouseUserName = Usr.ContactFirstName,
                              To = Usr.Email,
                              //TargetDeliveryDate = Ord.ExpectedDeliveryOnUtc.HasValue ? Ord.ExpectedDeliveryOnUtc.Value : DateTime.Now,
                              MerchandiserDelDateInEmail = DateNew,
                              DeliveryAddress = new Model.Email.OrderAddress()
                              {
                                  FirstName = OrdAdd.ContactFirstName,
                                  LastName = OrdAdd.ContactLastName,
                                  Address = OrdAdd.Address,
                                  CompanyName = OrdAdd.CompanyName,
                                  Address2 = OrdAdd.Address2,
                                  Address3 = OrdAdd.Address3,
                                  City = OrdAdd.City,
                                  Phone = OrdAdd.PhoneNumber,
                                  Area = OrdAdd.Suburb,
                                  PostCode = OrdAdd.PostCode,
                                  State = OrdAdd.State,
                                  Email = OrdAdd.Email,
                                  Id = OrdAdd.Id,
                                  Country = new Model.User.WMSCountry()
                                  {
                                      CountryId = Cnt.CountryId,
                                      Code = Cnt.CountryCode,
                                      Code2 = Cnt.CountryCode2,
                                      Name = Cnt.CountryName,
                                      TimezoneId = Cnt.TimeZoneId != null ? Cnt.TimeZoneId.Value : 0
                                  }
                              }
                          }).FirstOrDefault();

            emailModel.ProductDetail = (from Ord in dbContext.Orders
                                        join OrdDe in dbContext.OrderDesigns on Ord.Id equals OrdDe.OrderId
                                        join Prdt in dbContext.ProductMasters on OrdDe.ProductId equals Prdt.Id
                                        join OrdDeDe in dbContext.OrderDesignDetails on OrdDe.Id equals OrdDeDe.OrderDesignId
                                        join PrdtSKU in dbContext.ProductSKUs on OrdDeDe.ProductSKUId equals PrdtSKU.Id
                                        join Clr in dbContext.Colors on PrdtSKU.ColorId equals Clr.ID into leftCpolor
                                        from tempColor in leftCpolor.DefaultIfEmpty()
                                        join ps in dbContext.ProductStyles on OrdDe.ProductColor equals ps.StyleCode into leftStyle
                                        from tempStyle in leftStyle.DefaultIfEmpty()
                                        join Sz in dbContext.Sizes on PrdtSKU.SizeId equals Sz.ID
                                        where Ord.Id == orderid
                                        select new E_2ProductDetail
                                        {
                                            DesignNumber = OrdDe.DesignNumber,
                                            ProductName = Prdt.ProductName,
                                            ProductCode = Prdt.ProductCode,
                                            SKU = PrdtSKU.SKU,
                                            Color = tempColor == null ? (tempStyle == null ? "" : tempStyle.StyleCode) : tempColor.color1,
                                            Size = Sz.size1,
                                            Quantity = OrdDeDe.Quantity
                                        }).ToList();

            emailModel.TotalQuantity = emailModel.ProductDetail.Sum(a => a.Quantity);
            emailModel.TargetDeliveryDisplayDate = emailModel.TargetDeliveryDate.ToString("dd-MMM-yyyy");

            if (emailModel.DeliveryType == WMSOrderDeliveryTypeEnum.StandardDelivery)
            {
                emailModel.DeliveryType = WMSOrderDeliveryTypeEnum.StandardDeliveryDisplay;
            }
            if (emailModel.DeliveryType == WMSOrderDeliveryTypeEnum.ExpressDelivery)
            {
                emailModel.DeliveryType = WMSOrderDeliveryTypeEnum.ExpressDeliveryDisplay;
            }
            if (emailModel.DeliveryType == WMSOrderDeliveryTypeEnum.UrgentDelivery)
            {
                emailModel.DeliveryType = WMSOrderDeliveryTypeEnum.UrgentDeliveryDisplay;
            }

            var orderDesigns = dbContext.OrderDesigns.Where(p => p.OrderId == orderid).ToList();
            if (orderDesigns.Count == 1)
            {
                emailModel.OrderName = orderDesigns[0].DesignName;
            }
            return emailModel;
        }

        public bool SendEmailE3(int orderid)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            EmailE_3 emailDataSource = fillEmail_E3Model(orderid);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E3.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Pick Up Request - " + emailDataSource.OrderNumber;
                emailSetting.HeaderLogo = "";
                emailSetting.Attachments = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.To : AppSettings.TOCC;
                emailSetting.CC = !string.IsNullOrEmpty(AppSettings.PrintCCEmail) ? AppSettings.PrintCCEmail : "";
                if (string.IsNullOrEmpty(emailSetting.CC))
                {
                    emailSetting.CC = "";
                }
                emailSetting.BCC = AppSettings.BCC;
                emailSetting.Attachments = GetPickupnoteAttachments(orderid);

                EmailService.SendMail(emailSetting);

                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Warehouse Pickup NoteEmail Order Number: " + emailDataSource.OrderNumber + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + emailDataSource.To));

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        #endregion

        #region "E4 Email Templates"

        public bool Email_E4(int orderId, int orderDeisgnId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            EmailE4 emailDataSource = fillEmail_E4Model(orderId, orderDeisgnId);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E4_2.cshtml");
            emailDataSource.SiteLink = AppSettings.PublicSiteAddress;

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Design resubmitted";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.To : AppSettings.TOCC;
                emailSetting.CC = !string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.CC : "";
                if (string.IsNullOrEmpty(emailSetting.CC))
                {
                    emailSetting.CC = "";
                }
                emailSetting.BCC = AppSettings.BCC;
                var isJobSheet = new DYORepository().IsJobSheetCreate(orderId);
                emailSetting.Attachments = GetOrderJobSheetAttachments(orderId);
                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Email to sales coordinator design resubmitted Order Number: " + emailDataSource.OrderNumber + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + emailDataSource.To));
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool Email_E41(int orderId, int orderDeisgnId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            EmailE4 emailDataSource = fillEmail_E41Model(orderId, orderDeisgnId);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E4_2.cshtml");
            emailDataSource.SiteLink = AppSettings.PublicSiteAddress;

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Design resubmitted";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.To : AppSettings.TOCC;
                emailSetting.CC = !string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.CC : "";

                if (string.IsNullOrEmpty(emailSetting.CC))
                {
                    emailSetting.CC = "";
                }

                emailSetting.BCC = AppSettings.BCC;
                var isJobSheet = new DYORepository().IsJobSheetCreate(orderId);
                emailSetting.Attachments = GetOrderJobSheetAttachments(orderId);
                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Email yo merchandiser design resubmitted Order Number: " + emailDataSource.OrderNumber + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + emailDataSource.To));
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool EMailE4_2(int orderId, int orderDesignId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            Email42 emailDataSource = fillEmail_E42Model(orderId, orderDesignId);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E4_1.cshtml");
            emailDataSource.SiteLink = AppSettings.PublicSiteAddress;

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Design rejected";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.To : AppSettings.TOCC;
                emailSetting.CC = !string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.CC : "";

                if (string.IsNullOrEmpty(emailSetting.CC))
                {
                    emailSetting.CC = "";
                }

                emailSetting.BCC = AppSettings.BCC;
                emailSetting.Attachments = "";

                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Email to customer Design Rejected by sales coordinator : " + emailDataSource.OrderNumber + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + emailDataSource.To));
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool EMailE4_3(int orderId, int orderDesignId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            Email42 emailDataSource = fillEmail_E43Model(orderId, orderDesignId);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E4_1.cshtml");
            emailDataSource.SiteLink = AppSettings.PublicSiteAddress;

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Design rejected";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.To : AppSettings.TOCC;
                emailSetting.CC = !string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.CC : "";
                if (string.IsNullOrEmpty(emailSetting.CC))
                {
                    emailSetting.CC = "";
                }
                emailSetting.BCC = AppSettings.BCC;
                var isJobSheet = new DYORepository().IsJobSheetCreate(orderId);
                emailSetting.Attachments = "";
                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Email to customer merchandiser rejected the design Order Number: " + emailDataSource.OrderNumber + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + emailDataSource.To));
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool EMailE4_4(int orderId, int orderDesignId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            Email44 emailDataSource = fillEmail_E44Model(orderId, orderDesignId);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E4.cshtml");
            emailDataSource.SiteLink = AppSettings.PublicSiteAddress;

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);


            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Design accepted";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.To : AppSettings.TOCC;
                emailSetting.CC = !string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.CC : "";

                if (string.IsNullOrEmpty(emailSetting.CC))
                {
                    emailSetting.CC = "";
                }

                emailSetting.BCC = AppSettings.BCC;
                var isJobSheet = new DYORepository().IsJobSheetCreate(orderId);
                emailSetting.Attachments = "";

                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Email to Customer Deisign accepted  Order Number: " + emailDataSource.OrderNumber + " " + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + "To:  " + emailDataSource.To));

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        public bool EMailE4_5(int orderId, int orderDesignId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/logo.png";
            Email44 emailDataSource = fillEmail_E45Model(orderId, orderDesignId);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "\\E4.cshtml");
            emailDataSource.SiteLink = AppSettings.PublicSiteAddress;

            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailDataSource, null, null);

            try
            {
                SMTPConfiguration emailSetting = dbContext.UserConfigurations.Where(p => p.UserId == 0).Select(p => new SMTPConfiguration
                {
                    DisplayName = p.SMTPDisplayName,
                    EnableSsl = p.EnableSsl,
                    FromMail = p.FromMail,
                    MailHostName = p.MailHostName,
                    SMTPport = p.SMTPport,
                    SMTPPassword = p.SMTPPassword,
                    SMTPUserName = p.SMTPUserName
                }).FirstOrDefault();

                emailSetting.Subject = emailDataSource.PONumber + " - " + "DYO: Design accepted";
                emailSetting.HeaderLogo = "";
                emailSetting.MailContent = EmailBody;
                emailSetting.To = string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.To : AppSettings.TOCC;
                emailSetting.CC = !string.IsNullOrEmpty(AppSettings.TOCC) ? emailDataSource.CC : "";

                if (string.IsNullOrEmpty(emailSetting.CC))
                {
                    emailSetting.CC = "";
                }

                emailSetting.BCC = AppSettings.BCC;
                var isJobSheet = new DYORepository().IsJobSheetCreate(orderId);
                emailSetting.Attachments = "";

                EmailService.SendMail(emailSetting);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Email to customer Merchandiser accepted the design  Order Number: " + emailDataSource.OrderNumber + DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt") + emailDataSource.To));

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        private Email44 fillEmail_E44Model(int orderId, int orderDesignId)
        {
            Email44 email = new Email44();

            var orderDesign = (from r in dbContext.OrderDesigns
                               join o in dbContext.Orders on r.OrderId equals o.Id
                               join p in dbContext.ProductMasters on r.ProductId equals p.Id
                               join c in dbContext.Colors on r.ProductColor equals c.code into tempColor
                               from dd in tempColor.DefaultIfEmpty()
                               where r.Id == orderDesignId
                               select new
                               {
                                   DesignName = r.DesignName,
                                   OrderCreatedOn = o.CreatedOnUtc,
                                   CustomerId = o.CreatedBy,
                                   SalesCordinator = o.SalesCordinator,
                                   Merchandiser = o.Mechandiser,
                                   DesignNumber = r.DesignNumber,
                                   PONumber = o.PONumber,
                                   OrderNumber = o.OrderNumber,
                                   StyleCode = p.ProductCode,
                                   Color = dd == null ? "" : dd.color1
                               }).FirstOrDefault();

            var user = dbContext.WMSUsers.Find(orderDesign.CustomerId);
            var user2 = dbContext.WMSUsers.Find(orderDesign.SalesCordinator);
            var details = dbContext.OrderDesignDetails.Where(p => p.OrderDesignId == orderDesignId).Select(p => new { p.Quantity }).ToList().Sum(p => p.Quantity);
            var commLog = dbContext.CommunicationLogs.Where(p => p.OrderDesignId == orderDesignId && p.UserID == orderDesign.SalesCordinator.Value && p.Type == "Accept").OrderByDescending(p => p.Date).FirstOrDefault();

            if (commLog != null && user != null)
            {
                email.DesignName = orderDesign.DesignName;
                email.PONumber = orderDesign.PONumber;
                email.OrderNumber = orderDesign.OrderNumber;
                email.Quantity = details;
                email.Color = orderDesign.Color;
                email.AcceptedBy = user2.ContactFirstName; //+ " " + user2.ContactLastName;
                email.CustomerName = user.ContactFirstName; //+ " " + user.ContactLastName;
                email.StyleNumber = orderDesign.StyleCode;
                email.Date = orderDesign.OrderCreatedOn.Date.ToString("dd-MMM-yyyy");
                email.DesignNumber = orderDesign.DesignNumber;
                email.Reason = commLog.Reason;
                email.To = user.Email;
            }

            return email;
        }

        private Email44 fillEmail_E45Model(int orderId, int orderDesignId)
        {
            Email44 email = new Email44();
            var orderDesign = (from r in dbContext.OrderDesigns
                               join o in dbContext.Orders on r.OrderId equals o.Id
                               join p in dbContext.ProductMasters on r.ProductId equals p.Id
                               join c in dbContext.Colors on r.ProductColor equals c.code into tempColor
                               from dd in tempColor.DefaultIfEmpty()
                               where r.Id == orderDesignId
                               select new
                               {
                                   DesignName = r.DesignName,
                                   OrderCreatedOn = o.CreatedOnUtc,
                                   CustomerId = o.CustomerId,
                                   SalesCordinator = o.SalesCordinator,
                                   Merchandiser = o.Mechandiser,
                                   DesignNumber = r.DesignNumber,
                                   PONumber = o.PONumber,
                                   OrderNumber = o.OrderNumber,
                                   StyleCode = p.ProductCode,
                                   Color = dd == null ? "" : dd.color1
                               }).FirstOrDefault();

            var user = dbContext.WMSUsers.Find(orderDesign.CustomerId);
            var user2 = dbContext.WMSUsers.Find(orderDesign.Merchandiser);
            var details = dbContext.OrderDesignDetails.Where(p => p.OrderDesignId == orderDesignId).Select(p => new { p.Quantity }).ToList().Sum(p => p.Quantity);
            var commLog = dbContext.CommunicationLogs.Where(p => p.OrderDesignId == orderDesignId && p.UserID == orderDesign.Merchandiser.Value && p.Type == "Accept").OrderByDescending(p => p.Date).FirstOrDefault();

            if (commLog != null && user != null)
            {
                email.DesignName = orderDesign.DesignName;
                email.PONumber = orderDesign.PONumber;
                email.OrderNumber = orderDesign.OrderNumber;
                email.Quantity = details;
                email.Color = orderDesign.Color;
                email.AcceptedBy = user2.ContactFirstName; //+ " " + user2.ContactLastName;
                email.CustomerName = user.ContactFirstName; //+ " " + user.ContactLastName;
                email.StyleNumber = orderDesign.StyleCode;
                email.Date = orderDesign.OrderCreatedOn.Date.ToString("dd-MMM-yyyy");
                email.DesignNumber = orderDesign.DesignNumber;
                email.Reason = commLog.Reason;
                email.To = user.Email;
            }

            return email;
        }

        private Email42 fillEmail_E43Model(int orderId, int orderDesignId)
        {
            Email42 email = new Email42();

            var orderDesign = (from r in dbContext.OrderDesigns
                               join o in dbContext.Orders on r.OrderId equals o.Id
                               join p in dbContext.ProductMasters on r.ProductId equals p.Id
                               join c in dbContext.Colors on r.ProductColor equals c.code into tempColor
                               from dd in tempColor.DefaultIfEmpty()
                               where r.Id == orderDesignId
                               select new
                               {
                                   DesignName = r.DesignName,
                                   OrderCreatedOn = o.CreatedOnUtc,
                                   CustomerId = o.CreatedBy,
                                   SalesCordinator = o.SalesCordinator,
                                   Merchandiser = o.Mechandiser,
                                   DesignNumber = r.DesignNumber,
                                   PONumber = o.PONumber,
                                   OrderNumber = o.OrderNumber,
                                   StyleCode = p.ProductCode,
                                   Color = dd == null ? "" : dd.color1
                               }).FirstOrDefault();

            var user = dbContext.WMSUsers.Find(orderDesign.CustomerId);
            var details = dbContext.OrderDesignDetails.Where(p => p.OrderDesignId == orderDesignId).Select(p => new { p.Quantity }).ToList().Sum(p => p.Quantity);
            var commLog = dbContext.CommunicationLogs.Where(p => p.OrderDesignId == orderDesignId && p.UserID == orderDesign.Merchandiser.Value && p.Type == "Reject").OrderByDescending(p => p.Date).FirstOrDefault();

            if (commLog != null && user != null)
            {
                email.DesignName = orderDesign.DesignName;
                email.PONumber = orderDesign.PONumber;
                email.OrderNumber = orderDesign.OrderNumber;
                email.Quantity = details;
                email.Color = orderDesign.Color;
                email.CustomerName = user.ContactFirstName; //+ " " + user.ContactLastName;
                email.StyleNumber = orderDesign.StyleCode;
                email.Date = orderDesign.OrderCreatedOn.Date.ToString("dd-MMM-yyyy");
                email.DesignNumber = orderDesign.DesignNumber;
                email.Reason = commLog.Reason;
                email.To = user.Email;
            }

            return email;
        }

        private Email42 fillEmail_E42Model(int orderId, int orderDesignId)
        {
            Email42 email = new Email42();
            var orderDesign = (from r in dbContext.OrderDesigns
                               join o in dbContext.Orders on r.OrderId equals o.Id
                               join p in dbContext.ProductMasters on r.ProductId equals p.Id
                               join c in dbContext.Colors on r.ProductColor equals c.code into tempColor
                               from dd in tempColor.DefaultIfEmpty()
                               where r.Id == orderDesignId
                               select new
                               {
                                   DesignName = r.DesignName,
                                   OrderCreatedOn = o.CreatedOnUtc,
                                   CustomerId = o.CreatedBy,
                                   SalesCordinator = o.SalesCordinator,
                                   Merchandiser = o.Mechandiser,
                                   DesignNumber = r.DesignNumber,
                                   PONumber = o.PONumber,
                                   OrderNumber = o.OrderNumber,
                                   StyleCode = p.ProductCode,
                                   Color = dd == null ? "" : dd.color1
                               }).FirstOrDefault();

            var user = dbContext.WMSUsers.Find(orderDesign.CustomerId);
            var details = dbContext.OrderDesignDetails.Where(p => p.OrderDesignId == orderDesignId).Select(p => new { p.Quantity }).ToList().Sum(p => p.Quantity);
            var commLog = dbContext.CommunicationLogs.Where(p => p.OrderDesignId == orderDesignId && p.UserID == orderDesign.SalesCordinator.Value && p.Type == "Reject").OrderByDescending(p => p.Date).FirstOrDefault();

            if (commLog != null && user != null)
            {
                email.DesignNumber = orderDesign.DesignNumber;
                email.DesignName = orderDesign.DesignName;
                email.OrderNumber = orderDesign.OrderNumber;
                email.PONumber = orderDesign.PONumber;
                email.Quantity = details;
                email.Color = orderDesign.Color;
                email.CustomerName = user.ContactFirstName; //+ " " + user.ContactLastName;
                email.StyleNumber = orderDesign.StyleCode;
                email.Date = orderDesign.OrderCreatedOn.Date.ToString("dd-MMM-yyyy");
                email.DesignNumber = orderDesign.DesignNumber;
                email.Reason = commLog.Reason;
                email.To = user.Email;
            }
            return email;
        }

        private EmailE4 fillEmail_E41Model(int orderId, int orderDeisgnId)
        {
            EmailE4 email = new EmailE4();

            var orderDesign = (from r in dbContext.OrderDesigns
                               join o in dbContext.Orders on r.OrderId equals o.Id
                               join p in dbContext.ProductMasters on r.ProductId equals p.Id
                               join c in dbContext.Colors on r.ProductColor equals c.code into tempColor
                               from dd in tempColor.DefaultIfEmpty()
                               where r.Id == orderDeisgnId
                               select new
                               {
                                   DesignName = r.DesignName,
                                   SalesCordinator = o.SalesCordinator,
                                   Merchandiser = o.Mechandiser,
                                   DesignNumber = r.DesignNumber,
                                   PONumber = o.PONumber,
                                   OrderNumber = o.OrderNumber,
                                   StyleCode = p.ProductCode,
                                   Color = dd == null ? "" : dd.color1
                               }).FirstOrDefault();

            var user = dbContext.WMSUsers.Find(orderDesign.Merchandiser);
            var details = dbContext.OrderDesignDetails.Where(p => p.OrderDesignId == orderDeisgnId).Select(p => new { p.Quantity }).ToList().Sum(p => p.Quantity);
            var commLog = dbContext.CommunicationLogs.Where(p => p.OrderDesignId == orderDeisgnId && p.UserID == orderDesign.Merchandiser.Value && p.Type == "Reject").OrderByDescending(p => p.Date).FirstOrDefault();

            if (commLog != null && user != null)
            {
                email.DesignName = orderDesign.DesignName;
                email.PONumber = orderDesign.PONumber;
                email.OrderNumber = orderDesign.OrderNumber;
                email.Quantity = details;
                email.Color = orderDesign.Color;
                email.CustomerName = user.ContactFirstName; //+ " " + user.ContactLastName;
                email.StyleNumber = orderDesign.StyleCode;
                email.Date = commLog.Date.Value.Date.ToString("dd-MMM-yyyy");
                email.DesignNumber = orderDesign.DesignNumber;
                email.To = user.Email;
            }

            return email;
        }

        private EmailE4 fillEmail_E4Model(int orderId, int orderDeisgnId)
        {
            EmailE4 email = new EmailE4();

            var orderDesign = (from r in dbContext.OrderDesigns
                               join o in dbContext.Orders on r.OrderId equals o.Id
                               join p in dbContext.ProductMasters on r.ProductId equals p.Id
                               join c in dbContext.Colors on r.ProductColor equals c.code into tempColor
                               from dd in tempColor.DefaultIfEmpty()
                               where r.Id == orderDeisgnId
                               select new
                               {
                                   DesignName = r.DesignName,
                                   SalesCordinator = o.SalesCordinator,
                                   Merchandiser = o.Mechandiser,
                                   DesignNumber = r.DesignNumber,
                                   PONumber = o.PONumber,
                                   OrderNumber = o.OrderNumber,
                                   StyleCode = p.ProductCode,
                                   Color = dd == null ? "" : dd.color1
                               }).FirstOrDefault();

            var user = dbContext.WMSUsers.Find(orderDesign.SalesCordinator);
            var details = dbContext.OrderDesignDetails.Where(p => p.OrderDesignId == orderDeisgnId).Select(p => new { p.Quantity }).ToList().Sum(p => p.Quantity);
            var commLog = dbContext.CommunicationLogs.Where(p => p.OrderDesignId == orderDeisgnId && p.UserID == orderDesign.SalesCordinator.Value && p.Type == "Reject").OrderByDescending(p => p.Date).FirstOrDefault();

            if (commLog != null && user != null)
            {
                email.DesignName = orderDesign.DesignName;
                email.PONumber= orderDesign.PONumber;
                email.OrderNumber = orderDesign.OrderNumber;
                email.Quantity = details;
                email.Color = orderDesign.Color;
                email.CustomerName = user.ContactFirstName; //+ " " + user.ContactLastName;
                email.StyleNumber = orderDesign.StyleCode;
                email.Date = commLog.Date.Value.Date.ToString("dd-MMM-yyyy");
                email.DesignNumber = orderDesign.DesignNumber;
                email.To = user.Email;
            }

            return email;
        }

        #endregion
    }
}
