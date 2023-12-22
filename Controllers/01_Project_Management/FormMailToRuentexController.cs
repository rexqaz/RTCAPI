using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using rc_interface_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using LTCH_API;

namespace rc_interface_API.Controllers._01_Project_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormMailToRuentexController : ControllerBase
    {
        [HttpPost]
        public object FormAll([FromForm] Info_FormMailToRuentexModel Data)
        {           
            try
            {
                int fileCount = 0;
                string text1 = string.Empty;
                string text2 = string.Empty;
                Guid guid = Guid.NewGuid();
                string folder = "XingUpdateFile\\" + guid.ToString();

                if (Data.Attach1 != null)
                {
                    var size1 = Data.Attach1.Length;                    
                    
                    System.IO.Directory.CreateDirectory(folder);
                    if (size1 > 0)
                    {
                        var path = folder + "\\" + Data.Attach1.FileName;
                        text1 = path;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            Data.Attach1.CopyTo(stream);
                        }
                        fileCount++;
                    }
                }

                if (Data.Attach2 != null)
                {
                    var size2 = Data.Attach2.Length;
                    if (size2 > 0)
                    {
                        var path2 = folder + "\\" + Data.Attach2.FileName;
                        text2 = path2;
                        using (var stream2 = new FileStream(path2, FileMode.Create))
                        {
                            Data.Attach2.CopyTo(stream2);
                        }
                        fileCount++;
                    }
                }


                string InputIsok = "Y";
                string ReturnErr = "";
                string CommType = string.Empty;
                string MailBody = string.Empty;
                string Subject = string.Empty;

                if (Data.CommType != null)
                {
                    CommType = APCommonFun.CDBNulltrim(Data.CommType);
                }

                if (Data.Subject != null)
                {
                    Subject = APCommonFun.CDBNulltrim(Data.Subject);
                }

                if (Data.MailBody != null)
                {
                    MailBody = APCommonFun.CDBNulltrim(Data.MailBody);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (CommType == "" || Subject == "" || MailBody == "") //必填                       
                {
                    if (CommType == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-CommType 為必填欄位";
                    }
                    else if (Subject == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-Subject 為必填欄位";
                    }
                    else
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-MailBody 為必填欄位";
                    }
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[FormMailToRuentexController]90-" + ReturnErr);
                    return new
                    {
                        Result = "R",
                        Message = ReturnErr,
                        Data = ""
                    };
                }

                string server = Startup.ReadFromAppSettings("MailServer"); 
                string sender = Startup.ReadFromAppSettings("MailFrom");

                string recipient = string.Empty;
                string[] deptIDArray = CommType.Split(',');
                List<string> deptIDSqlStr = new List<string>();
                for (int i = 0; i < deptIDArray.Length; i++)
                {
                    deptIDSqlStr.Add("'" + deptIDArray[i] + "'");
                }

                string sql = "select Email from employees where DeptID in (" + String.Join(",", deptIDSqlStr.ToArray()) + ") and CanBeContactedByCust=1 ";

                DataTable dt = APCommonFun.GetDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string email = APCommonFun.CDBNulltrim(dr["Email"].ToString());
                        recipient = recipient + email + ";";
                    }
                }

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(sender + "@mail.ruentex.com.tw");
                    string[] Recipients = recipient.Split(';');
                    for (int i = 0; i < Recipients.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(Recipients[i]))
                        {
                            mail.To.Add(new MailAddress(Recipients[i]));
                        }
                    }
                    mail.Subject = Subject;
                    mail.Body = MailBody;

                    string[] attachents = new string[2] { text1, text2};

                    foreach (string text in attachents)
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            System.Net.Mail.Attachment attachment;
                            attachment = new System.Net.Mail.Attachment(text);
                            mail.Attachments.Add(attachment);
                        }
                    }

                    using (SmtpClient smtp = new SmtpClient(server))
                    {
                        smtp.Send(mail);
                    }
                }

                return new
                {
                    Result = "T",
                    Message = "成功"
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[FormMailToRuentexController]99：" + ex.ToString());

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }

        private void SendEmail(string Recipient, string Sender, string Subject, string Message, string Server, string[] attachents)
        {

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(Sender + "@mail.ruentex.com.tw");
                string[] Recipients = Recipient.Split(';');
                for (int i = 0; i < Recipients.Length; i++)
                {
                    if (!string.IsNullOrEmpty(Recipients[i]))
                    {
                        mail.To.Add(new MailAddress(Recipients[i]));
                    }
                }                
                mail.Subject = Subject;
                mail.Body = Message;

                foreach (string text in attachents)
                {
                    if (text != null)
                    {
                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(text);
                        mail.Attachments.Add(attachment);
                    }
                }

                using (SmtpClient smtp = new SmtpClient(Server))
                {
                    smtp.Send(mail);
                }
            }
        }

        public string GetToMail(string deptIDs)
        {
            string[] deptIDArray = deptIDs.Split(',');
            List<string> deptIDSqlStr = new List<string>();
            for (int i = 0; i < deptIDArray.Length; i++)
            {
                deptIDSqlStr.Add("'" + deptIDArray[i] + "'");
            }

            string sql = "select Email from employees where DeptID in (" + String.Join(",", deptIDSqlStr.ToArray()) + ") and CanBeContactedByCust=1 ";

            DataTable dt = APCommonFun.GetDataTable(sql);
            string text = "";

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string email = APCommonFun.CDBNulltrim(dr["Email"].ToString());
                    text = text + email + ";";
                }
            }
            return text;
        }
    }
}
