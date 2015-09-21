using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Workflow_Data;

namespace Workflow.Framework.Control.Importacion
{
    public class CL_Layout_Campos
    {

        //-------------------------------
        private CL_MetodoValidacion validacion;
        //-------------------------------
        private string strNombreCampoReferencia;
        private string strParametrosValidacion;
        private string strMetodoValidacion;
        private string strTipoCampo;
        private string strNombre;
        //-------------------------------
        private int intMetodoValidacion;
        private int intCampoReferencia;
        private int intCaracterInicial;
        private int intCaracterFinal;
        private int intTipoCampo;
        //-------------------------------
        private long lngFilaReferencia;
        //-------------------------------
        private bool blnExigirCoincidenciaNombre;
        private bool blnValidar;
        //-------------------------------
        private Type typTipoCampo;
        //-------------------------------

        internal dbInterface db;

        #region Constructor

        public CL_Layout_Campos(dbInterface DB) { }

        #endregion

        #region Propiedades

        public CL_MetodoValidacion Validacion 
        {
            get { return validacion; }
            set { validacion = value; }
        }

        public string Nombre 
        {
            get { return strNombre; }
            set { strNombre = value; }
        }

        public int IdTipoCampo 
        {
            get { return intTipoCampo; }
            set 
            { 
                intTipoCampo = value;

                ObtenerTipoCampo(intTipoCampo);
            }
        }

        public string NombreTipoCampo 
        {
            get { return strTipoCampo; }
            set { strTipoCampo = value; }
        }

        public Type TypeTipoCampo 
        {
            get { return typTipoCampo; }
            set { typTipoCampo = value; }
        }

        public string NombreCampoReferencia
        {
            get { return strNombreCampoReferencia; }
            set { strNombreCampoReferencia = value; }
        }

        public bool ExigirCoincidenciaNombre
        {
            get { return blnExigirCoincidenciaNombre; }
            set { blnExigirCoincidenciaNombre = value; }
        }

        public int ColumnaReferencia
        {
            get { return intCampoReferencia; }
            set { intCampoReferencia = value; }
        }

        public long FilaReferencia
        {
            get { return lngFilaReferencia; }
            set { lngFilaReferencia = value; }
        }

        public int CaracterInicial
        {
            get { return intCaracterInicial; }
            set { intCaracterFinal = value; }
        }

        public int CaracterFinal
        {
            get { return intCaracterFinal; }
            set { intCaracterFinal = value; }
        }

        public bool RequiereValidacion
        {
            get { return blnValidar; }
            set { blnValidar = value; }
        }

        public int IdMetodoValidacion
        {
            get { return intMetodoValidacion; }
            set 
            { 
                intMetodoValidacion = value;

                CL_MetodoValidacion MetodoValidacion = new CL_MetodoValidacion(intMetodoValidacion, db);
                Validacion = MetodoValidacion;
            }
        }

        public string MetodoValidacion
        {
            get { return strMetodoValidacion; }
            set { strMetodoValidacion = value; }
        }

        public string ParametrosValidacion
        {
            get { return strParametrosValidacion; }
            set { strParametrosValidacion = value; }
        }

        #endregion

        #region Listas

        #endregion

        #region MetodosPublicos

        #endregion

        #region MetodosPrivados

        private void ObtenerTipoCampo(int IdTipoCampo, int Status = -1)
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

                parametros[0].ParameterName = "P_ID_TIPODATO";
                parametros[0].OleDbType = System.Data.OleDb.OleDbType.Integer;
                parametros[0].Value = IdTipoCampo;

                parametros[1].ParameterName = "P_STATUS";
                parametros[1].OleDbType = System.Data.OleDb.OleDbType.SmallInt;
                parametros[1].Value = Status;

                dt = db.GetTable("SEL_TIPO_DATO", parametros);

                // Asignación de valores obtenidos
                foreach (DataRow row in dt.Rows)
                {
                    strTipoCampo = row["TCA_NOM"].ToString();

                    Type tipo = null;

                    switch (strTipoCampo)
                    {
                        case "BOOLEANO":
                            tipo = Type.GetType("System.Boolean");
                            break;

                        case "ENTERO CORTO":
                            tipo = Type.GetType("System.Int16");
                            break;
                        
                        case "ENTERO":
                            tipo = Type.GetType("System.Int32");
                            break;

                        case "ENTERO LARGO":
                            tipo = Type.GetType("System.Int64");
                            break;

                        case "DECIMAL":
                            tipo = Type.GetType("System.Decimal");
                            break;

                        case "TEXTO":
                            tipo = Type.GetType("System.String");
                            break;

                        case "FECHA":
                            tipo = Type.GetType("System.DateTime");
                            break;

                        case "HORA":
                            tipo = Type.GetType("System.DateTime");
                            break;

                        default:
                            break;
                    }

                    typTipoCampo = tipo;
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
