using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Workflow_Data;

namespace Workflow.Framework.Control.Importacion
{
    class CL_Layout
    {

        //-------------------------------
        private string strNegocio;
        private string strNombre;
        private string strPrefijo;
        private string strSufijo;
        private string strExtension;
        private string strHoja;
        private string strTablaDestino;
        private string strSeparador;
        //-------------------------------
        private int intIdLayout;
        private int intIdNegocio;
        private int intColumnaInicial;
        private int intFilaInicial;
        //-------------------------------

        internal dbInterface db;

        #region Constructor

        public CL_Layout() {}

        public CL_Layout(int IdNegocio, int IdLayout, dbInterface DB) 
        {
            intIdNegocio = IdNegocio;
            intIdLayout = IdLayout;
            db = DB;

            // Obtener configuración de layout
            ObtenerConfiguracionLayout(intIdLayout);
        }

        #endregion

        #region Propiedades

        public string Negocio 
        {
            get { return strNegocio; }
            set { strNegocio = value; }
        }

        public string Nombre 
        { 
            get { return strNombre; }
            set { strNombre = value; }
        }

        public string Prefijo 
        {
            get { return strPrefijo; }
            set { strPrefijo = value; }
        }

        public string Sufijo
        {
            get { return strSufijo; }
            set { strSufijo = value; }
        }

        public string Extension
        {
            get { return strExtension; }
            set { strExtension = value; }
        }

        public string Hoja
        {
            get { return strHoja; }
            set { strHoja = value; }
        }

        public string TablaDestino
        {
            get { return strTablaDestino; }
            set { strTablaDestino = value; }
        }

        public string Separador
        {
            get { return strSeparador; }
            set { strSeparador = value; }
        }

        public int ColumnaInicial
        {
            get { return intColumnaInicial; }
            set { intColumnaInicial = value; }
        }

        public int FilaInicial
        {
            get { return intFilaInicial; }
            set { intFilaInicial = value; }
        }

        #endregion

        #region Listas

        #endregion

        #region MetodosPublicos

        #endregion

        #region MetodosPrivados

        private void ObtenerConfiguracionLayout(int IdLayout)
        {
            try
            {
                DataTable dt;
                
                db.Connection_Check();

                System.Data.OleDb.OleDbParameter[] parametros = new System.Data.OleDb.OleDbParameter[1];
                for (int i = 0; i < 1; i++)
                {
                    parametros[i] = new System.Data.OleDb.OleDbParameter();
                }

                parametros[0].ParameterName = "P_ID_PRODUTO";
                parametros[0].OleDbType = System.Data.OleDb.OleDbType.Integer;
                parametros[0].Value = IdLayout;

                dt = db.GetTable("SEL_LAYOUT", parametros);
                
                // Asignación de configuración
                foreach (DataRow row in dt.Rows)
                {
                    strNegocio = row["NEG_NOM"].ToString();
                    strNombre = row["LAY_NOM"].ToString();
                    strPrefijo = row["LAY_PRF"].ToString();
                    strSufijo = row["LAY_SUF"].ToString();
                    strExtension = row["LAY_EXT"].ToString();
                    strHoja = row["LAY_HOJ"].ToString();
                    intColumnaInicial = int.Parse(row["LAY_CIN"].ToString());
                    intFilaInicial = int.Parse(row["LAY_FIN"].ToString());
                    strTablaDestino = row["LAY_TDE"].ToString();
                    strSeparador = row["LAY_SEP"].ToString();
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
