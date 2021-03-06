﻿using System.Data.SqlClient;
using System.Data;
using Framework.Libs;
using System;
using System.Collections.Specialized;

namespace Framework.DataAccess
{
    class CSQLServerConnect : CBaseConnect
    {
        #region 变量定义
        private string _strConnectString;
        private string _strServerNm;
        private string _strDataBase;
        private string _strUserName;
        private string _strPassWord;

        /// <summary>
        /// 数据库执行对象
        /// </summary>
        public SqlConnection m_SqlConn;

        public SqlTransaction Tran;

        #endregion

        #region CBaseConnect 成员

        #region 属性设置

        string CBaseConnect.strConnectString
        {
            get
            {
                return _strConnectString;
            }
            set
            {
                if (_strConnectString == value)
                    return;
                _strConnectString = value;
            }
        }

        string CBaseConnect.strServerNm
        {
            get
            {
                return _strServerNm;
            }
            set
            {
                if (_strServerNm == value)
                    return;
                _strServerNm = value;
            }
        }

        string CBaseConnect.strDataBase
        {
            get
            {
                return _strDataBase;
            }
            set
            {
                if (_strDataBase == value)
                    return;
                _strDataBase = value;
            }
        }

        string CBaseConnect.strUserName
        {
            get
            {
                return _strUserName;
            }
            set
            {
                if (_strUserName == value)
                    return;
                _strUserName = value;
            }
        }

        string CBaseConnect.strPassWord
        {
            get
            {
                return _strPassWord;
            }
            set
            {
                if (_strPassWord == value)
                    return;
                _strPassWord = value;
            }
        }

        #endregion

        #region 数据库初始化处理

        /// <summary>
        /// 数据库连接参数设置
        /// </summary>
        void CBaseConnect.SetParameter()
        {

            this._strConnectString = "SERVER=" + Common._sysrun.ServerName.Trim() + ";"
                                           + "UID=" + Common._sysrun.UserName.Trim() + ";"
                                           + "PWD=" + Common._sysrun.PassWord.Trim() + ";"
                                           + "DATABASE=" + Common._sysrun.DataBaseName.Trim();
        }

        /// <summary>
        /// 数据库连接参数设置
        /// </summary>
        /// <param name="strServerNm">服务器地址</param>
        /// <param name="strDataBaseNm">数据库名</param>
        /// <param name="strUserNm">用户名</param>
        /// <param name="strPswd">用户密码</param>
        void CBaseConnect.SetParameter(string strServerNm, string strDataBaseNm, string strUserNm, string strPswd)
        {
            this._strServerNm = strServerNm;
            this._strDataBase = strDataBaseNm;
            this._strUserName = strUserNm;
            this._strPassWord = strPswd;

            this._strConnectString = "SERVER=" + this._strServerNm.Trim() + ";"
                                           + "UID=" + this._strUserName.Trim() + ";"
                                           + "PWD=" + this._strPassWord.Trim() + ";"
                                           + "DATABASE=" + this._strDataBase.Trim();
        }

        /// <summary>
        /// 数据库连接打开处理
        /// </summary>
        /// <param name="intDBtype">数据源类型</param>
        /// <returns></returns>
        bool CBaseConnect.ConnectOpen()
        {
            try
            {
                if (this.m_SqlConn == null)
                {
                    this.m_SqlConn = new SqlConnection(_strConnectString);
                }

                if (this.m_SqlConn.State != ConnectionState.Open)
                {
                    this.m_SqlConn.Open();
                }

                return (this.m_SqlConn == null) || (this.m_SqlConn.State != ConnectionState.Open) ? false : true;

            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 数据库连接关闭处理
        /// </summary>
        /// <param name="intDBtype">数据源类型</param>
        /// <returns></returns>
        bool CBaseConnect.ConnectClose()
        {
            try
            {
                if (this.m_SqlConn.State == ConnectionState.Open)
                {
                    this.m_SqlConn.Close();
                }

                return true;

            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 数据库创建事务处理
        /// </summary>
        /// <param name="intDBtype">数据源类型</param>
        /// <returns></returns>
        bool CBaseConnect.CreateSqlTransaction()
        {
            try
            {

                this.Tran = this.m_SqlConn.BeginTransaction();

                return true;

            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 数据库事务提交处理
        /// </summary>
        /// <param name="intDBtype">数据源类型</param>
        /// <returns></returns>
        bool CBaseConnect.TransactionCommit()
        {
            try
            {

                this.Tran.Commit();

                return true;

            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 数据库事务回滚处理
        /// </summary>
        /// <param name="intDBtype">数据源类型</param>
        /// <returns></returns>
        bool CBaseConnect.TransactionRollback()
        {
            try
            {

                this.Tran.Rollback();

                return true;

            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 数据库查询处理

        /// <summary>
        /// 获取数据查询结果集
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns></returns>
        DataTable CBaseConnect.GetDataSet(string SQLString)
        {

            try
            {
                return GetDataSetProc(SQLString);

            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取数据查询结果集
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="param">参数对象</param>
        /// <returns></returns>
        DataTable CBaseConnect.GetDataSet(string SQLString, params object[] lstParam)
        {
            try
            {
                return GetDataSetProc(SQLString, lstParam);

            }
            catch (SqlException ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 返回某一个字段的值
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns></returns>
        object CBaseConnect.GetFieldValue(string SQLString)
        {
            SqlDataAdapter MyAdapter;
            DataSet MyDataSet = new DataSet();
            try
            {
                MyAdapter = new SqlDataAdapter(SQLString, this.m_SqlConn);
                MyAdapter.Fill(MyDataSet, "User_Data");

                if (MyDataSet.Tables[0].Rows.Count >= 1)

                    return MyDataSet.Tables[0].Rows[0][0];
                else
                    return null;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message + "\n当前执行SQL语句：" + GetSqlFromSqlPara(SQLString, null));
            }
        }

        /// <summary>
        /// 查询获取的记录集
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns></returns>
        Common.dbRecord CBaseConnect.GetRecord(string SQLString)
        {
            DataTable MyDataTable;
            Common.dbRecord MyRecord;

            try
            {

                MyDataTable = this.GetDataSetProc(SQLString);

                if (MyDataTable != null)
                {
                    MyRecord.AdoDataRst = MyDataTable;
                    MyRecord.RcdCnt = MyDataTable.Rows.Count;
                }
                else
                {
                    MyRecord.AdoDataRst = null;
                    MyRecord.RcdCnt = 0;
                }

                return MyRecord;

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message + "\n当前执行SQL语句：" + GetSqlFromSqlPara(SQLString, null));
            }
        }

        /// <summary>
        /// 查询获取的记录集
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        Common.dbRecord CBaseConnect.GetRecord(string SQLString, params object[] lstParam)
        {
            DataTable MyDataTable;
            Common.dbRecord MyRecord;

            try
            {

                MyDataTable = this.GetDataSetProc(SQLString, lstParam);

                if (MyDataTable != null)
                {
                    MyRecord.AdoDataRst = MyDataTable;
                    MyRecord.RcdCnt = MyDataTable.Rows.Count;
                }
                else
                {
                    MyRecord.AdoDataRst = null;
                    MyRecord.RcdCnt = 0;
                }

                return MyRecord;

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message + "\n当前执行SQL语句：" + GetSqlFromSqlPara(SQLString, lstParam));
            }
        }


        /// <summary>
        /// 获取表格显示的记录集
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns></returns>
        Common.View CBaseConnect.GetView(string SQLString)
        {
            DataTable MyDataTable;
            Common.View MyView;

            try
            {

                MyDataTable = this.GetDataSetProc(SQLString);

                if (MyDataTable != null)
                {
                    MyView.AdoViewRst = MyDataTable.DefaultView;
                    MyView.RcdCnt = MyDataTable.Rows.Count;
                }
                else
                {
                    MyView.AdoViewRst = null;
                    MyView.RcdCnt = 0;
                }

                return MyView;

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message + "\n当前执行SQL语句：" + GetSqlFromSqlPara(SQLString, null));
            }
        }

        /// <summary>
        /// 获取表格显示的记录集
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        Common.View CBaseConnect.GetView(string SQLString, params object[] lstParam)
        {
            DataTable MyDataTable;
            Common.View MyView;

            try
            {

                MyDataTable = this.GetDataSetProc(SQLString, lstParam);

                if (MyDataTable != null)
                {
                    MyView.AdoViewRst = MyDataTable.DefaultView;
                    MyView.RcdCnt = MyDataTable.Rows.Count;
                }
                else
                {
                    MyView.AdoViewRst = null;
                    MyView.RcdCnt = 0;
                }

                return MyView;

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message + "\n当前执行SQL语句：" + GetSqlFromSqlPara(SQLString, lstParam));
            }
        }

        #endregion

        #region 数据库执行处理


        /// <summary>
        /// 数据执行处理
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns></returns>
        int CBaseConnect.ExecuteNonQuery(string SQLString)
        {
            SqlCommand MySqlCommand;
            int w_intRtnValue = -1;

            try
            {

                if (this.m_SqlConn.State != ConnectionState.Open)
                {
                    this.m_SqlConn.Open();
                }


                MySqlCommand = new SqlCommand(SQLString, this.m_SqlConn);

                w_intRtnValue = MySqlCommand.ExecuteNonQuery();

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message + "\n当前执行SQL语句：" + GetSqlFromSqlPara(SQLString, null));
            }

            return w_intRtnValue;
        }


        /// <summary>
        /// 数据执行处理
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="param">参数对象</param>
        /// <returns></returns>
        int CBaseConnect.ExecuteNonQuery(string SQLString, params object[] lstParam)
        {
            SqlCommand MySqlCommand;
            int intParaIndex;
            int w_intRtnValue = -1;

            try
            {

                if (this.m_SqlConn.State != ConnectionState.Open)
                {
                    this.m_SqlConn.Open();
                }


                MySqlCommand = new SqlCommand(SQLString, this.m_SqlConn);
                MySqlCommand.Transaction = this.Tran;

                for (intParaIndex = 0; intParaIndex < lstParam.Length; intParaIndex++)
                {
                    if (lstParam[intParaIndex] != null)
                        MySqlCommand.Parameters.Add(lstParam[intParaIndex]);
                }

                w_intRtnValue = MySqlCommand.ExecuteNonQuery();

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message + "\n当前执行SQL语句：" + GetSqlFromSqlPara(SQLString, lstParam));
            }

            return w_intRtnValue;
        }


        /// <summary>
        /// 数据库执行处理
        /// </summary>
        /// <param name="StoreProcName"></param>
        /// <param name="MyChoose"> HasAOutput：仅有单个返回值;
        ///                         OnlyExecSp：仅执行无返回值;
        ///                         RetOneRecord：返回记录集DataTable</param>
        /// <param name="para"></param>
        /// <returns></returns>
        object CBaseConnect.SetExecuteSP(string StoreProcName, Common.Choose MyChoose, params object[] lstParam)
        {
            object RetValue = null;

            SqlCommand ParaCmd;
            object[] w_ParaList;

            try
            {

                if (this.m_SqlConn.State != ConnectionState.Open)
                {
                    this.m_SqlConn.Open();
                }

                if (string.IsNullOrEmpty(StoreProcName) == true)
                {
                    return null;
                }

                ParaCmd = new SqlCommand(StoreProcName, this.m_SqlConn);
                ParaCmd.Transaction = this.Tran;

                ParaCmd.Prepare();
                ParaCmd.CommandTimeout = 300;
                ParaCmd.CommandType = CommandType.StoredProcedure;

                w_ParaList = GetParaToCommand(StoreProcName, lstParam);
                for (int paraIndex = 0; paraIndex < w_ParaList.Length; paraIndex++)
                {
                    if (w_ParaList[paraIndex] != null)
                        ParaCmd.Parameters.Add(w_ParaList[paraIndex]);
                }

                switch (MyChoose)
                {
                    case Common.Choose.HasAOutput:

                        //有一个返回值,但定义是最后一个参数
                        ParaCmd.ExecuteNonQuery();
                        RetValue = ParaCmd.Parameters[ParaCmd.Parameters.Count - 1].Value;
                        break;

                    case Common.Choose.OnlyExecSp:

                        //仅仅进行数据处理，无返回值。  
                        ParaCmd.ExecuteNonQuery();
                        RetValue = 0;
                        break;

                    case Common.Choose.RetOneRecord:

                        //返回一个SqlDataReader。
                        RetValue = GetConvertDataReaderToDataTable(ParaCmd.ExecuteReader(CommandBehavior.Default));
                        break;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }

            return RetValue;
        }

        #endregion

        #endregion

        /// <summary>
        /// 获取数据查询结果集
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns></returns>
        private DataTable GetDataSetProc(string SQLString)
        {
            DataTable MyDataTable = null;
            DataSet MyDataSet = new DataSet();
            SqlCommand MySqlCommand = new SqlCommand();
            SqlDataAdapter MyAdapter = new SqlDataAdapter();

            try
            {

                if (this.m_SqlConn.State != ConnectionState.Open)
                {
                    this.m_SqlConn.Open();
                }

                MySqlCommand.Connection = this.m_SqlConn;
                MySqlCommand.CommandText = SQLString;

                if (Tran != null) MySqlCommand.Transaction = Tran;

                MyAdapter.SelectCommand = MySqlCommand;
                MyAdapter.SelectCommand.CommandTimeout = 300;


                MyAdapter.Fill(MyDataSet, "User_Data");

                if (MyDataSet != null)
                {
                    MyDataTable = MyDataSet.Tables[0];
                }

                return MyDataTable;

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message + "\n当前执行SQL语句：" + GetSqlFromSqlPara(SQLString, null));
            }
        }

        /// <summary>
        /// 获取数据查询结果集
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="param">参数对象</param>
        /// <returns></returns>
        private DataTable GetDataSetProc(string SQLString, params object[] lstParam)
        {

            DataTable MyDataTable = null;
            DataSet MyDataSet = new DataSet(); ;
            SqlCommand MySqlCommand = new SqlCommand();
            SqlDataAdapter MyAdapter = new SqlDataAdapter();
            int intParaIndex;

            try
            {

                if (this.m_SqlConn.State != ConnectionState.Open)
                {
                    this.m_SqlConn.Open();
                }

                MySqlCommand.Connection = this.m_SqlConn;
                MySqlCommand.CommandText = SQLString;

                if (Tran != null) MySqlCommand.Transaction = Tran;

                for (intParaIndex = 0; intParaIndex < lstParam.Length; intParaIndex++)
                {
                    if (lstParam[intParaIndex] != null)
                        MySqlCommand.Parameters.Add(lstParam[intParaIndex]);
                }

                MyAdapter.SelectCommand = MySqlCommand;
                MyAdapter.SelectCommand.CommandTimeout = 300;
                MyAdapter.Fill(MyDataSet, "User_Data");

                if (MyDataSet != null)
                {
                    MyDataTable = MyDataSet.Tables[0];
                }
                string sql = GetSqlFromSqlPara(SQLString, lstParam);
                return MyDataTable;

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message + "\n当前执行SQL语句：" + GetSqlFromSqlPara(SQLString, lstParam));
            }

        }

        /// <summary>
        /// 存储过程参数取得处理
        /// </summary>
        /// <param name="StoreProcName">存储过程名</param>
        /// <param name="paraValue">参数数组</param>
        /// <returns>返回一个对象数组</returns>
        public object[] GetParaToCommand(string StoreProcName, params object[] lstParam)
        {
            int rowIndex = 0;
            string cmdParaSQL;

            DataTable tblParaRst;
            SqlDbType cmdParaType = 0;
            SqlParameter[] cmdParamters = null;

            cmdParaSQL = "Select P.ParaName,T.Name as ParaType,P.ParaLen,P.ParaDirection From " +
                                 "(Select a.Name as ParaName,c.Name as ParaType,a.Length as ParaLen," +
                                 "a.isOutParam as ParaDirection,a.Colid as ParaID,c.xtype as ParaTypeID " +
                                 "From syscolumns a Left Outer Join sysobjects b On a.ID=b.ID and b.xtype='P' " +
                                 "Left Outer Join systypes c On a.xtype=c.xtype and a.xusertype=c.xusertype " +
                                 "Where b.Name='" + StoreProcName.Trim() + "') P Left Outer Join systypes T On " +
                                 "P.ParaTypeID=T.xtype and P.ParaTypeID=T.xusertype Order by P.ParaID";

            tblParaRst = GetDataSetProc(cmdParaSQL);

            if (tblParaRst != null && tblParaRst.Rows.Count > 0)
            {
                cmdParamters = new SqlParameter[tblParaRst.Rows.Count];
                foreach (DataRow ParaRow in tblParaRst.Rows)
                {
                    switch (ParaRow["ParaType"].ToString().ToUpper().Trim())
                    {
                        case "BIGINT":
                            cmdParaType = SqlDbType.BigInt;
                            break;
                        case "BINARY":
                            cmdParaType = SqlDbType.Binary;
                            break;
                        case "BIT":
                            cmdParaType = SqlDbType.Bit;
                            break;
                        case "CHAR":
                            cmdParaType = SqlDbType.Char;
                            break;
                        case "DATETIME":
                            cmdParaType = SqlDbType.DateTime;
                            break;
                        case "DECIMAL":
                            cmdParaType = SqlDbType.Decimal;
                            break;
                        case "FLOAT":
                            cmdParaType = SqlDbType.Float;
                            break;
                        case "IMAGE":
                            cmdParaType = SqlDbType.Image;
                            break;
                        case "INT":
                            cmdParaType = SqlDbType.Int;
                            break;
                        case "MONEY":
                            cmdParaType = SqlDbType.Money;
                            break;
                        case "NCHAR":
                            cmdParaType = SqlDbType.NChar;
                            break;
                        case "NTEXT":
                            cmdParaType = SqlDbType.NText;
                            break;
                        case "NVARCHAR":
                            cmdParaType = SqlDbType.NVarChar;
                            break;
                        case "REAL":
                            cmdParaType = SqlDbType.Real;
                            break;
                        case "SMALLDATETIME":
                            cmdParaType = SqlDbType.SmallDateTime;
                            break;
                        case "SMALLINT":
                            cmdParaType = SqlDbType.SmallInt;
                            break;
                        case "SMALLMONEY":
                            cmdParaType = SqlDbType.SmallMoney;
                            break;
                        case "TEXT":
                            cmdParaType = SqlDbType.Text;
                            break;
                        case "TIMESTAMP":
                            cmdParaType = SqlDbType.Timestamp;
                            break;
                        case "TINYINT":
                            cmdParaType = SqlDbType.TinyInt;
                            break;
                        case "UNIQUEIDENTIFIER":
                            cmdParaType = SqlDbType.UniqueIdentifier;
                            break;
                        case "VARBINARY":
                            cmdParaType = SqlDbType.VarBinary;
                            break;
                        case "VARCHAR":
                            cmdParaType = SqlDbType.VarChar;
                            break;
                    }
                    cmdParamters[rowIndex] = new SqlParameter(ParaRow["ParaName"].ToString().Trim()
                                                                                      , cmdParaType
                                                                                      , (System.Int16)(ParaRow["ParaLen"]));
                    cmdParamters[rowIndex].Direction = ((int)ParaRow["ParaDirection"] == 1 ? ParameterDirection.Output : ParameterDirection.Input);
                    cmdParamters[rowIndex].Value = lstParam[rowIndex];
                    rowIndex++;
                }

                tblParaRst.Dispose();
            }

            return cmdParamters;
        }

        /// <summary> 
        /// DataReader格式转换成DataTable 
        /// </summary> 
        /// <param name="DataReader">OleDbDataReader</param> 
        private DataTable GetConvertDataReaderToDataTable(SqlDataReader reader)
        {
            DataTable objDataTable = new DataTable("TmpDataTable");
            int intCounter;

            try
            {
                //获取当前行中的列数；
                int intFieldCount = reader.FieldCount;

                for (intCounter = 0; intCounter <= intFieldCount - 1; intCounter++)
                {
                    objDataTable.Columns.Add(reader.GetName(intCounter), reader.GetFieldType(intCounter));
                }

                //populate   datatable   
                objDataTable.BeginLoadData();

                //object[]   objValues   =   new   object[intFieldCount   -1];   
                object[] objValues = new object[intFieldCount];

                while (reader.Read())
                {
                    reader.GetValues(objValues);
                    objDataTable.LoadDataRow(objValues, true);
                }
                reader.Close();

                objDataTable.EndLoadData();

                return objDataTable;

            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 将带参数的语句转换为sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="lstParam"></param>
        /// <returns></returns>
        private string GetSqlFromSqlPara(string sql, params object[] lstParam)
        {
            string NewSql = sql;
            SqlParameter par;
            try
            {
                if (lstParam == null) return sql;
                for (int i = 0; i < lstParam.Length; i++)
                {
                    if (lstParam[i] == null) continue;
                    par = (SqlParameter)lstParam[i];
                    if (sql.IndexOf(lstParam[i].ToString()) > 0)
                    {
                        if (par.SqlDbType == SqlDbType.Bit)
                        {

                            NewSql = NewSql.Replace(lstParam[i].ToString(), par.Value.ToString().ToLower() == "true" ? "'1'" : "'0'");
                        }
                        else
                        {
                            NewSql = NewSql.Replace(lstParam[i].ToString(), "'" + par.Value.ToString() + "'");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
            return NewSql;

        }


        #region 数据业务操作

        /// <summary>
        /// 读取表格信息数据
        /// </summary>
        /// <param name="TableName">数据表名称</param>
        /// <param name="dicItemData">检索条件值</param>
        /// <param name="dicConds">精确查询列名</param>
        /// <param name="dicLikeConds">模糊查询列名</param>
        /// <param name="OrderBy">需要排序的列</param>
        /// <returns></returns>
        public DataTable GetTableInfo(string TableName,
                                StringDictionary dicItemData,
                                StringDictionary dicConds,
                                StringDictionary dicLikeConds,
                                string OrderBy)
        {
            return null;
        }

        /// <summary>
        /// 读取表格信息数据
        /// </summary>
        /// <param name="TableName">Sql语句</param>
        /// <param name="dicItemData">检索条件值</param>
        /// <param name="dicConds">精确查询列名</param>
        /// <param name="dicLikeConds">模糊查询列名</param>
        /// <param name="OrderBy">需要排序的列</param>
        /// <returns></returns>
        public DataTable GetTableInfoBySql(string Sql,
                                    StringDictionary dicItemData,
                                    StringDictionary dicConds,
                                    StringDictionary dicLikeConds,
                                    string OrderBy)
        {
            return null;
        }




        /// <summary>
        /// 读取表格信息数据
        /// </summary>
        /// <param name="TableName">数据表名称</param>
        /// <param name="dicItemData">检索条件值</param>
        /// <param name="dicConds">精确查询列名</param>
        /// <param name="dicBetweenConds">时间段查询列名</param>
        /// <param name="dicLikeConds">模糊查询列名</param>
        /// <param name="OrderBy">需要排序的列</param>
        /// <returns></returns>
        public DataTable GetTableInfo(string TableName,
                                   StringDictionary dicItemData,
                                   StringDictionary dicConds,
                                   StringDictionary dicBetweenConds,
                                   StringDictionary dicLikeConds,
                                   string OrderBy)
        {
            return null;
        }


        #endregion



        #region CBaseConnect 成员


        public DataTable GetTableInfo(string TableName, StringDictionary dicItemData, StringDictionary dicConds, StringDictionary dicLikeConds, string OrderBy, bool readUncommitted)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DataTable GetTableInfoBySql(string Sql, StringDictionary dicItemData, StringDictionary dicConds, StringDictionary dicLikeConds, string OrderBy, bool readUncommitted)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DataTable GetTableInfo(string TableName, StringDictionary dicItemData, StringDictionary dicConds, StringDictionary dicBetweenConds, StringDictionary dicLikeConds, string OrderBy, bool readUncommitted)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DataTable GetTableInfoBySql(string Sql, StringDictionary dicItemData, StringDictionary dicConds, StringDictionary dicBetweenConds, StringDictionary dicLikeConds, string OrderBy, bool readUncommitted)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DataTable GetTableInfoBySqlNoWhere(string Sql, StringDictionary dicItemData, StringDictionary dicConds, StringDictionary dicLikeConds, string OrderBy, bool readUncommitted)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DataTable GetTableInfoBySqlNoWhere(string Sql, StringDictionary dicItemData, StringDictionary dicConds, StringDictionary dicBetweenConds, StringDictionary dicLikeConds, string OrderBy, bool readUncommitted)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool GetRepNameCheck(string TableName, StringDictionary dicItemData, StringDictionary dicPrimarName, string strRepFiledName, Common.DataModifyMode ScanMode)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool GetExistDataItem(string TableName, StringDictionary dicItemData, StringDictionary dicPrimarName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetMaxNoteNo(string TableName, string FiledName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetInsertDataItem(string TableName, StringDictionary dicItemData, StringDictionary dicUserColum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetInsertDataItem(string TableName, StringDictionary dicItemFiledNm, StringDictionary dicItemData, StringDictionary dicUserColum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WriteLog(string FrmName, int OperateTyp, string OperateContent)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetModifyDataItem(string TableName, StringDictionary dicItemData, StringDictionary dicPrimarName, StringDictionary dicUserColum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetModifyDataItemBySql(string Sql, StringDictionary dicItemData, StringDictionary dicPrimarName, StringDictionary dicUserColum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetModifyDataItem(string TableName, StringDictionary dicItemFiledNm, StringDictionary dicItemData, StringDictionary dicPrimarName, StringDictionary dicUserColum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetDeleteDataItem(string TableName, StringDictionary dicItemData, StringDictionary dicPrimarName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
