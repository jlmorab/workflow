using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Diagnostics;
using System.Data.OleDb;

namespace Workflow_Data
{
    public class DB2
    {

        protected OleDbConnection _obCon;
        protected OleDbCommand _obCmd;
        protected OleDbDataReader _obDr;

        protected string _sAmbiente;
        protected string _sConDB;
        protected string _sMsgErro;

        #region "parameters"

        public string MessagemDeErro
        {
            get
            {
                return _sMsgErro;
            }
        }


        public string DBConString
        {
            set
            {
                _sConDB = value;
            }
        }

        public string DBAmbiente
        {
            set
            {
                _sAmbiente = value;
            }
        }

        public OleDbConnection Connection
        {
            get
            {
                return _obCon;
            }
            set
            {
                _obCon = value;
            }
        }

        #endregion

        #region "basic database methods for access"

        public OleDbConnection Connection_Check(object pConString)
        {
            _sConDB = pConString.ToString();
            return Connection_Check();
        }

        public OleDbConnection Connection_Check(string pConString)
        {
            _sConDB = pConString;
            return Connection_Check();
        }

        public OleDbConnection Connection_Check()
        {

            if (_obCon == null)
            {
                _obCon = new OleDbConnection();
            }

            if (_obCon.State == ConnectionState.Closed)
            {
                _obCon.ConnectionString = _sConDB;
                _obCon.Open();
            }

            return _obCon;
        }

        public void Connection_Close()
        {

            if (!(_obCon == null))
            {

                if (!((_obCon.State == ConnectionState.Closed)))
                {
                    _obCon.Close();
                }

                _obCon.Dispose();

            }

            if (!(_obCmd == null))
            {
                _obCmd.Dispose();
            }
        }

        #endregion

        public DB2()
        {


        }

        public OleDbDataReader ExecutaProcedure(string pSP)
        {
            return ExecutaProcedure(pSP, null);
        }

        public OleDbDataReader ExecutaProcedure(string StoreProcedure, OleDbParameter[] Parametro)
        {
            try
            {


                _obCmd = new OleDbCommand();
                _obCmd.Connection = Connection_Check();
                _obCmd.Connection = _obCon;
                _obCmd.CommandType = CommandType.StoredProcedure;

                _obCmd.Parameters.Clear();


                if (Parametro != null)
                {
                    for (int I = 0; I <= Parametro.Length - 1; I++)
                    {
                        _obCmd.Parameters.Add(Parametro[I]);
                    }
                }


                _obCmd.CommandText = StoreProcedure;

                return _obCmd.ExecuteReader();

            }

            catch (Exception ex)
            {

                _sMsgErro = ex.ToString();
                throw new ArgumentException("Procedure :" +
                                        _sAmbiente + "." +
                                        StoreProcedure + "<br>Erro: "
                                        + ex.ToString());

            }
            finally
            {
                _obCmd.Dispose();
            }
        }


        public DataTable GetTable(string pStoredProcedure)
        {
            return GetTable(pStoredProcedure, null);
        }

        public DataSet GetDataSet(string pStoredProcedure, OleDbParameter[] Parametro)
        {
            try
            {

                _obCmd = new OleDbCommand();
                _obCmd.Connection = Connection_Check();
                _obCmd.Parameters.Clear();
                _obCmd.Connection = _obCon;
                _obCmd.CommandType = CommandType.StoredProcedure;


                if (!(Parametro == null))
                {
                    for (int I = 0; I <= Parametro.Length - 1; I++)
                    {
                        _obCmd.Parameters.Add(Parametro[I]);
                    }
                }


                _obCmd.CommandText = pStoredProcedure;
                //_obCmd.CommandText = pStoredProcedure;
                OleDbDataAdapter obAD = new OleDbDataAdapter();
                DataSet ds = new DataSet();

                obAD.SelectCommand = _obCmd;
                obAD.Fill(ds);

                return ds;

            }
            catch (Exception ex)
            {

                _sMsgErro = ex.Message + "<br><br>" + ex.StackTrace;
                throw new ArgumentException("Procedure :" +
                                        _sAmbiente + "." +
                                        pStoredProcedure + "<br>Erro: "
                                        + ex.ToString());

            }
        }

        public DataTable GetTable(string pStoredProcedure, OleDbParameter[] Parametro)
        {
            try
            {

                _obCmd = new OleDbCommand();
                _obCmd.Connection = Connection_Check();
                _obCmd.Parameters.Clear();
                _obCmd.Connection = _obCon;
                _obCmd.CommandType = CommandType.StoredProcedure;


                if (!(Parametro == null))
                {
                    for (int I = 0; I <= Parametro.Length - 1; I++)
                    {
                        _obCmd.Parameters.Add(Parametro[I]);
                    }
                }


                //_obCmd.CommandText = _sAmbiente + "." + pStoredProcedure;
                _obCmd.CommandText = pStoredProcedure;

                OleDbDataAdapter obAD = new OleDbDataAdapter();
                DataSet ds = new DataSet();

                obAD.SelectCommand = _obCmd;
                obAD.Fill(ds);

                return ds.Tables[0];

            }
            catch (Exception ex)
            {

                _sMsgErro = ex.Message + "<br><br>" + ex.StackTrace;
                throw new ArgumentException("Procedure :" +
                                        _sAmbiente + "." +
                                        pStoredProcedure + "<br>Erro: "
                                        + ex.ToString());

            }
        }

        public string GetStringField(string pStoredProcedure, OleDbParameter[] Parametro)
        {
            try
            {

                _obCmd = new OleDbCommand();
                _obCmd.Connection = Connection_Check();
                _obCmd.Parameters.Clear();
                _obCmd.Connection = _obCon;
                _obCmd.CommandType = CommandType.StoredProcedure;


                if (!(Parametro == null))
                {
                    for (int I = 0; I <= Parametro.Length - 1; I++)
                    {
                        _obCmd.Parameters.Add(Parametro[I]);
                    }
                }


                //_obCmd.CommandText = pStoredProcedure;
                _obCmd.CommandText = pStoredProcedure;

                OleDbDataAdapter obAD = new OleDbDataAdapter();
                DataSet ds = new DataSet();

                obAD.SelectCommand = _obCmd;
                obAD.Fill(ds);

                return ds.Tables[0].Rows[0][0].ToString();

            }
            catch (Exception ex)
            {

                _sMsgErro = ex.Message + "<br><br>" + ex.StackTrace;
                throw new ArgumentException("Procedure :" +
                                        _sAmbiente + "." +
                                        pStoredProcedure + "<br>Erro: "
                                        + ex.ToString());

            }
        }

        public void ExecutaProcedureNonQuery(string StoreProcedure, OleDbParameter[] Parametro)
        {
            try
            {
                _obCmd = new OleDbCommand();
                _obCmd.Connection = Connection_Check();
                _obCmd.Parameters.Clear();
                _obCmd.Connection = _obCon;
                _obCmd.CommandType = CommandType.StoredProcedure;

                if (!(Parametro == null))
                {
                    for (int I = 0; I <= Parametro.Length - 1; I++)
                    {
                        _obCmd.Parameters.Add(Parametro[I]);
                    }
                }

                //_obCmd.CommandText = _sAmbiente + "." + StoreProcedure;
                _obCmd.CommandText = StoreProcedure;

                _obCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Procedure :" +
                                        _sAmbiente + "." +
                                        StoreProcedure + "<br>Erro: "
                                        + ex.ToString());
            }
            finally
            {

                if (_obCmd != null)
                {
                    _obCmd.Dispose();
                    _obCmd = null;
                }

            }
        }

        protected void ExecutaProcedureNonQuery(string pSP)
        {
            ExecutaProcedureNonQuery(pSP, null);
        }



    }
}
