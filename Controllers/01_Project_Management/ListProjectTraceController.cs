using LTCH_API;
using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using rc_interface_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace rc_interface_API.Controllers._01_Project_Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListProjectTraceController : ControllerBase
    {
        [HttpPost]
        public object FormAll(Info_ListProjectTraceModel Data)
        {
            JArray newJa = new JArray();

            string PSPNR = string.Empty;
            string type = string.Empty;
            string page = string.Empty;
            string start_date = string.Empty;
            string end_date = string.Empty;
            string fetch_subStr = string.Empty;
            string where_subStr = "  ";

            if (Data.PSPNR != null) { PSPNR = APCommonFun.CDBNulltrim(Data.PSPNR); }
            if (Data.type != null) { type = APCommonFun.CDBNulltrim(Data.type); }
            if (Data.page != null) { page = APCommonFun.CDBNulltrim(Data.page); }

            if (Data.page != null && !string.IsNullOrEmpty(Data.page))
            {
                page = APCommonFun.CDBNulltrim(Data.page);
                if (!page.Contains(","))
                {
                    string ReturnErr = "執行動作錯誤-page 欄位格式錯誤";
                    APCommonFun.Error("[ListProjectTraceController]90-" + ReturnErr);
                    return new
                    {
                        Result = "R",
                        Message = ReturnErr,
                        Data = ""
                    };
                }
                else
                {
                    string[] page_data = page.Split(',');
                    fetch_subStr = "OFFSET " + ((Convert.ToInt32(page_data[0].ToString()) - 1) * Convert.ToInt32(page_data[1].ToString())).ToString() + " rows fetch first " + page_data[1].ToString() + " rows only ";
                }
            }

            if (Data.start_date != null && !string.IsNullOrEmpty(Data.start_date))
            {
                start_date = APCommonFun.CDBNulltrim(Data.start_date);
                if (!start_date.Contains("/"))
                {
                    string ReturnErr = "執行動作錯誤-start_date 欄位格式錯誤";
                    APCommonFun.Error("[ListProjectTraceController]90-" + ReturnErr);
                    return new
                    {
                        Result = "R",
                        Message = ReturnErr,
                        Data = ""
                    };
                }
                string[] sDate = start_date.Split('/');
                where_subStr += "  AND c.strmn >=  '" + sDate[0] + sDate[1] + sDate[2] + "' ";
            }

            if (Data.end_date != null && !string.IsNullOrEmpty(Data.end_date))
            {
                end_date = APCommonFun.CDBNulltrim(Data.end_date);
                if (!end_date.Contains("/"))
                {
                    string ReturnErr = "執行動作錯誤-end_date 欄位格式錯誤";
                    APCommonFun.Error("[ListProjectTraceController]90-" + ReturnErr);
                    return new
                    {
                        Result = "R",
                        Message = ReturnErr,
                        Data = ""
                    };
                }
                string[] eDate = end_date.Split('/');
                where_subStr += "  AND c.strmn >=  '" + eDate[0] + eDate[1] + eDate[2] + "' ";
            }

            string sql = "select  c.qmart,c.qmnum,c.strmn,c.fetxt,c.zltext,c.bezdt, d.oldFile,d.oldFileExc,d.newFile,d.newFileExc from zcpst12 c ,(select a.qmnum,a.filep as oldFile ,a.dappl as oldFileExc, b.filep as newFile,b.dappl as newFileExc from zcpst13 a,zcpst13 b where a.qmnum = b.qmnum(+) and a.doktl='000' and b.doktl ='001') d where c.qmnum = d.qmnum(+) and c.qmart = '"
                + type + "' and c.pspnr ='" + PSPNR + "'  " + where_subStr
                + "order by c.qmart desc " + fetch_subStr;

            try
            {
                DataTable dt = APCommonFun.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string QMART = APCommonFun.CDBNulltrim(dr["QMART"].ToString());
                        string QMNUM = APCommonFun.CDBNulltrim(dr["QMNUM"].ToString());
                        string STRMN = APCommonFun.CDBNulltrim(dr["STRMN"].ToString());
                        string FETXT = APCommonFun.CDBNulltrim(dr["FETXT"].ToString());
                        string ZLTEXT = APCommonFun.CDBNulltrim(dr["ZLTEXT"].ToString());
                        string BEZDT = APCommonFun.CDBNulltrim(dr["BEZDT"].ToString());
                        string oldFile = APCommonFun.CDBNulltrim(dr["oldFile"].ToString());
                        string newFile = APCommonFun.CDBNulltrim(dr["newFile"].ToString());

                        JObject tmpJoLay01 = new JObject();
                        tmpJoLay01.Add(new JProperty("QMART", QMART));
                        tmpJoLay01.Add(new JProperty("QMNUM", QMNUM));
                        tmpJoLay01.Add(new JProperty("STRMN", STRMN));
                        tmpJoLay01.Add(new JProperty("FETXT", FETXT));
                        tmpJoLay01.Add(new JProperty("ZLTEXT", ZLTEXT));
                        tmpJoLay01.Add(new JProperty("BEZDT", BEZDT));
                        if (oldFile.Length > 0)
                        {
                            string _content = APCommonFun.ReadImageFileToBase64(Startup.ReadFromAppSettings("file_path") + oldFile);
                            if (_content.Length > 0)
                            {
                                tmpJoLay01.Add(new JProperty("oldFile", _content));
                            }
                            else
                            {
                                tmpJoLay01.Add(new JProperty("oldFile", "No image content."));
                            }
                        }
                        else
                        {
                            tmpJoLay01.Add(new JProperty("oldFile", "No image file."));
                        }

                        if (newFile.Length > 0)
                        {
                            string _content = APCommonFun.ReadImageFileToBase64(Startup.ReadFromAppSettings("file_path") + newFile);
                            if (_content.Length > 0)
                            {
                                tmpJoLay01.Add(new JProperty("newFile", _content));
                            }
                            else
                            {
                                tmpJoLay01.Add(new JProperty("newFile", "No image content."));
                            }
                        }
                        else
                        {
                            tmpJoLay01.Add(new JProperty("newFile", "No image file."));
                        }
                        newJa.Add(tmpJoLay01);
                    }
                }

                return new
                {
                    Result = "T",
                    Message = "成功",
                    TotalRec = newJa.Count,
                    Data = newJa
                };
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[ListProjectTraceController]99：" + ex.ToString());
                APCommonFun.Error("[ListProjectTraceController]99：" + sql);

                return new
                {
                    Result = "F",
                    Message = ex.ToString(),
                    Data = newJa
                };
            }

        }
    }
}
