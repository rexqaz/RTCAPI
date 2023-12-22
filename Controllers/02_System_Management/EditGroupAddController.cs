using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rc_interface_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.Controllers._02_System_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditGroupAddController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_EditGroupInfoModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            string group_code = string.Empty;
            string group_name = string.Empty;
            string note = string.Empty;

            if (Data.group_code != null)
            {
                group_code = APCommonFun.CDBNulltrim(Data.group_code);
            }

            if (Data.group_name != null)
            {
                group_name = APCommonFun.CDBNulltrim(Data.group_name);
            }

            if (Data.note != null)
            {
                note = APCommonFun.CDBNulltrim(Data.note);
            }

            //第一步 : 先判斷有沒有必填未填寫，
            if (group_code == "" || group_name == "") //必填                       
            {
                if (group_code == "")
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-group_code 為必填欄位";
                }
                else
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-group_name 為必填欄位";
                }
            }

            //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
            if (InputIsok == "N")
            {
                APCommonFun.Error("[EditGroupAddController]90-" + ReturnErr);
                return new
                {
                    Result = "R",
                    Message = ReturnErr,
                    Data = ""
                };
            }

            string sql = string.Empty;
            string sql2 = "select * FROM  groups where Upper(GRPID)='" + group_code + "' ";


            try
            {
                APCommonFun.ExecSqlCommand(sql2);
                DataTable dt2 = APCommonFun.GetDataTable(sql2);
                if (dt2.Rows.Count == 0)
                {
                    sql = "Insert Into groups(GRPID, GRPNAME, GRPdes) Values ('"
                            + group_code + "', '" + group_name + "', '" + note + "' )";
                    APCommonFun.ExecSqlCommand(sql);
                }
                else
                {
                    return new
                    {
                        Result = "T",
                        Message = "群組代碼重複，請重新輸入員工代碼"
                    };
                }
                return new
                {
                    Result = "T",
                    Message = "成功"
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[EditGroupAddController]99：" + ex.ToString());
                APCommonFun.Error("[EditGroupAddController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString()
                };
            }

        }
    }
}
