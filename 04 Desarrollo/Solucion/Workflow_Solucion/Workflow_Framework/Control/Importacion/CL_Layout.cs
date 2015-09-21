using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using Workflow_Data;

namespace Workflow.Framework.Control.Importacion
{
    class CL_Layout
    {

        //-------------------------------
        private CL_Layout_Campos[] camposDeLayout;
        private DataTable dtCampos;
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

        public CL_Layout_Campos[] CamposDeLayout
        {
            get { return camposDeLayout; }
            set { camposDeLayout = value; }
        }

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

        private void ObtenerLayoutCampos(int IdLayout, int Status = -1)
        {
            try
            {
                db.Connection_Check();

                System.Data.OleDb.OleDbParameter[] parametros = new System.Data.OleDb.OleDbParameter[1];
                for (int i = 0; i < 2; i++)
                {
                    parametros[i] = new System.Data.OleDb.OleDbParameter();
                }

                parametros[0].ParameterName = "P_ID_LAYOUT";
                parametros[0].OleDbType = System.Data.OleDb.OleDbType.Integer;
                parametros[0].Value = IdLayout;

                parametros[1].ParameterName = "P_STATUS";
                parametros[1].OleDbType = System.Data.OleDb.OleDbType.SmallInt;
                parametros[1].Value = Status;

                dtCampos = db.GetTable("SEL_LAYOUT_CAMPOS", parametros);
            }
            catch (Exception Error)
            {
                string strMsgError = Error.Message;
            }
        }
        
        private void ObtenerLayoutCampos(int IdLayout)
        {
            ObtenerLayoutCampos(IdLayout, 1);

            if (dtCampos.Rows.Count > 0)
            {
                ArrayList Campos = new ArrayList();

                foreach (DataRow row in dtCampos.Rows)
                {
                    CL_Layout_Campos campo = new CL_Layout_Campos(db);

                    campo.Nombre = row["LYC_NOM"].ToString();
                    campo.IdTipoCampo = int.Parse(row["TCA_CVE"].ToString());
                    if (row["LYC_CRN"] != DBNull.Value)
                        campo.NombreCampoReferencia = row["LYC_CRN"].ToString();
                    campo.ExigirCoincidenciaNombre = ((row["LYC_ENC"].ToString() == "1") ? true : false ) ;
                    campo.ColumnaReferencia = int.Parse(row["LYC_CRE"].ToString());
                    campo.FilaReferencia = long.Parse(row["LYC_FRE"].ToString());
                    if (row["LYC_CIN"] != DBNull.Value)
                        campo.CaracterInicial = int.Parse(row["LYC_CIN"].ToString());
                    if (row["LYC_CFI"] != DBNull.Value)
                        campo.CaracterFinal = int.Parse(row["LYC_CFI"].ToString());
                    if (row["LYC_VAL"].ToString() != "0")
                        campo.RequiereValidacion = true;
                    if (row["MVA_CVE"] != DBNull.Value)
                        campo.IdMetodoValidacion = int.Parse(row["MVA_CVE"].ToString());
                    if (row["LYC_MVP"] != DBNull.Value)
                        campo.ParametrosValidacion = row["LYC_MVP"].ToString();

                    Campos.Add(campo);
                }

                this.CamposDeLayout = new CL_Layout_Campos[Campos.Count];
                Campos.CopyTo(this.CamposDeLayout, 0);
            }
            
            


        }

        #endregion

    }
}
