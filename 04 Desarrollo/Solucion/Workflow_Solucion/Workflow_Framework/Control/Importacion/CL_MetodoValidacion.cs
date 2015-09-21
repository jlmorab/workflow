using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Workflow_Data;

namespace Workflow.Framework.Control.Importacion
{
    public class CL_MetodoValidacion
    {

        //----------------------------
        private string strNombre;
        private string strParametros;
        private string strEjemplo;
        private string strRutaSistema;
        private string strDescripcion;
        //----------------------------
        private int intIdMetodo;
        //----------------------------
        
        internal dbInterface db;

        #region Contructor

        public CL_MetodoValidacion(dbInterface DB) 
        {
            db = DB;

            // Inicializa variables
            strNombre = string.Empty;
            strParametros = string.Empty;
            strEjemplo = string.Empty;
            strRutaSistema = string.Empty;
            strDescripcion = string.Empty;
        }

        public CL_MetodoValidacion(int IdMetodoValidacion, dbInterface DB)
        {
            db = DB;

            // Inicializa variables
            strNombre = string.Empty;
            strParametros = string.Empty;
            strEjemplo = string.Empty;
            strRutaSistema = string.Empty;
            strDescripcion = string.Empty;

            ObtenerMetodoValidacion(IdMetodoValidacion);
        }

        #endregion

        #region Propiedades

        public int IdMetodo 
        {
            get { return intIdMetodo; }
            set 
            { 
                intIdMetodo = value;

                ObtenerMetodoValidacion(intIdMetodo);
            }
        }

        public string Nombre 
        {
            get { return strNombre; }
            set { strNombre = value; }
        }

        public string Parametros 
        {
            get { return strParametros; }
            set { strParametros = value; }
        }

        public string EjemploSintaxis 
        {
            get { return strEjemplo; }
            set { strEjemplo = value; }
        }

        public string RutaEnSistema 
        {
            get { return strRutaSistema; }
            set { strRutaSistema = value; }
        }

        public string Descripcion 
        {
            get { return strDescripcion; }
            set { strDescripcion = value; }
        }

        #endregion

        #region Listas

        #endregion

        #region MetodosPublicos

        #endregion

        #region MetodosPrivados

        private void ObtenerMetodoValidacion(int IdMetodo, int Status = -1)
        {
            try
            {
                DataTable dt;

                db.Connection_Check();

                System.Data.OleDb.OleDbParameter[] parametros = new System.Data.OleDb.OleDbParameter[1];
                for (int i = 0; i < 2; i++)
                {
                    parametros[i] = new System.Data.OleDb.OleDbParameter();
                }

                parametros[0].ParameterName = "P_ID_METODO";
                parametros[0].OleDbType = System.Data.OleDb.OleDbType.Integer;
                parametros[0].Value = IdMetodo;

                parametros[1].ParameterName = "P_STATUS";
                parametros[1].OleDbType = System.Data.OleDb.OleDbType.SmallInt;
                parametros[1].Value = Status;

                dt = db.GetTable("SEL_METODO_VALIDACION", parametros);

                // Asignación de valores obtenidos
                foreach (DataRow row in dt.Rows)
                {
                    strNombre = row["MVA_NOM"].ToString();
                    strParametros = row["MVA_PAR"].ToString();
                    strEjemplo = row["MVA_ESX"].ToString();
                    strRutaSistema = row["MVA_RSY"].ToString();
                    strDescripcion = row["MVA_DES"].ToString();
                }
            }
            catch (Exception Error)
            {
                string strMsgError = Error.Message;
            }
        }

        #endregion
    }
}
