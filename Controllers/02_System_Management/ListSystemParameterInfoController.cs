using LTCH_API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
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
    public class ListSystemParameterInfoController : ControllerBase
    {
        private string mStrFuncName = "ListSystemParameterInfoController";
        [HttpPost]
        public object FormAll(Info_ListSystemParameterInfoModel Data)
        {
            string project_id = "";
            if (Data.project_id != null) { project_id = APCommonFun.CDBNulltrim(Data.project_id); }

            if (project_id.Length <= 0)
            {
                return APCommonFun.ReturnError(mStrFuncName, "No project_id is specified.", "R", new JArray());
            }

            OracleConnection conn = APCommonFun.GetOracleConnection();
            if (conn == null)
            {
                return APCommonFun.ReturnError(mStrFuncName, "Oracle connecting fault.", "R", new JArray());
            }

            string sqlcommand = "select post1 from ZCPST11 where upper(PSPNR)='"+project_id.ToUpper().Trim()+"'";

            bool _has_fault = false;
            OracleCommand cmd;
            string _proj_name = "";
            string _manage_percent = APCommonFun.GetDefaultManagerPencent();
            
            try
            {
                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                //_proj_name = cmd.ExecuteScalar().ToString().Trim();
                object _nameobj = cmd.ExecuteScalar();
                if(_nameobj != null)
                {
                    _proj_name = _nameobj.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnError(mStrFuncName, "Common exception", ex.ToString(), "F", new JArray());
            }

            //計算百分比
            sqlcommand = "SELECT VALUE FROM  ManagerPercent where upper(PSPNR)='" + project_id.ToUpper().Trim() + "'";
            try
            {
                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;
                //_manage_percent = cmd.ExecuteScalar().ToString().Trim();
                object _tmpobj = cmd.ExecuteScalar();
                if(_tmpobj == null)
                {
                    _manage_percent = APCommonFun.GetDefaultManagerPencent();
                }
                else
                {
                    _manage_percent = _tmpobj.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                _manage_percent = APCommonFun.GetDefaultManagerPencent();
            }
            double dbl_manage_percent = double.Parse(_manage_percent) * 100;

            JArray newJa = new JArray();
            // get CCTRs
            DataSet dscctv = new DataSet();
            int _total_cctv = 0;
            sqlcommand = "SELECT PSPNR,CCTVURL,MSG FROM PORJECTCCTVURL where upper(PSPNR)='" + project_id.ToUpper().Trim() + "'";
            try
            {
                cmd = new OracleCommand(sqlcommand, conn);
                cmd.CommandType = CommandType.Text;

                OracleDataAdapter _da = new OracleDataAdapter();
                _da.SelectCommand = cmd;
                _total_cctv = _da.Fill(dscctv);
                JArray newJa02 = new JArray(); //第二層

                for(int index = 0;index < _total_cctv;index++)
                {
                    JObject item = new JObject();
                    item.Add(new JProperty("CCTV_URL", dscctv.Tables[0].Rows[index]["CCTVURL"] == null ? "" : dscctv.Tables[0].Rows[index]["CCTVURL"].ToString().Trim()));
                    item.Add(new JProperty("description", dscctv.Tables[0].Rows[index]["MSG"] == null ? "" : dscctv.Tables[0].Rows[index]["MSG"].ToString().Trim()));

                    newJa02.Add(item);
                }
                //第一層
                JObject tmpJoLay01 = new JObject();
                tmpJoLay01.Add(new JProperty("project_name", _proj_name));
                tmpJoLay01.Add(new JProperty("ManagerPencent", string.Format("{0}", dbl_manage_percent)));
                tmpJoLay01.Add(new JProperty("CCTV_setting", newJa02));

                newJa.Add(tmpJoLay01);

                return APCommonFun.ReturnSuccess(newJa, _total_cctv);
            }
            catch (Exception ex)
            {
                try { conn.Close(); } catch { }
                return APCommonFun.ReturnError(mStrFuncName, "Common exception.", ex.ToString(), "F", new JArray());
            }


            
            //JArray newJa_Final = new JArray();


            
            //try
            //{

            //    JArray newJa02 = new JArray(); //第二層
            //    JObject tmpJoLay02 = new JObject();

            //    tmpJoLay02.Add(new JProperty("CCTV_URL", "http://111.70.4.193"));
            //    tmpJoLay02.Add(new JProperty("description", "南港玉成CCTV"));

            //    newJa02.Add(tmpJoLay02);

            //    JObject tmpJoLay03 = new JObject();

            //    tmpJoLay03.Add(new JProperty("CCTV_URL", "http://111.70.4.194"));
            //    tmpJoLay03.Add(new JProperty("description", "南港玉成CCTV2"));

            //    newJa02.Add(tmpJoLay03);

            //    //第一層
            //    JObject tmpJoLay01 = new JObject();
            //    tmpJoLay01.Add(new JProperty("project_name", "潤弘 - 三元能源科技高雄鋰電池廠新建工程"));
            //    tmpJoLay01.Add(new JProperty("ManagerPencent", "6"));
            //    tmpJoLay01.Add(new JProperty("CCTV_setting", newJa02));

            //    newJa.Add(tmpJoLay01);



            //    return new
            //    {
            //        Result = "T",
            //        Message = "成功",
            //        Data = newJa
            //    };
            //}
            //catch (Exception ex)
            //{
            //    APCommonFun.Error("[Info_ListSystemParameterInfoModel]99：" + ex.ToString());
            //    //APCommonFun.Error("[Info_ListSystemParameterInfoModel]99：" + sql);

            //    return new
            //    {
            //        Result = "F",
            //        Message = ex.ToString(),
            //        Data = newJa
            //    };
            //}

        }
    }
}
