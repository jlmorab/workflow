using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Workflow_Data;

namespace Workflow.Framework.Control.Importacion
{
    class CL_Mapeo
    {
        private int intIdLayout;
        private DataTable dtMapeo;

        internal dbInterface db;

        #region Constructor

        public CL_Mapeo(dbInterface DB) 
        { 
            db = DB; 
            
            // Inicializa variables
            intIdLayout = -1;
        }

        public CL_Mapeo(int IdLayout, dbInterface DB) 
        { 
            db = DB; 

            // Inicializa variables
            this.IdLayout = IdLayout;
        }

        #endregion

        #region Propiedades

        public int IdLayout 
        {
            get { return intIdLayout; }
            set 
            { 
                intIdLayout = value;

                ObtenerMapeo(intIdLayout);
            }
        }

        public DataTable Mapeo
        {
            get { return dtMapeo; }
        }

        #endregion

        #region Listas

        #endregion

        #region MetodosPublicos

        #endregion

        #region MetodosPrivados

        private void ObtenerMapeo(int IdLayout, int Status = -1)
        {
            try
            {
                db.Connection_Check();

                System.Data.OleDb.OleDbParameter[] parametros = new System.Data.OleDb.OleDbParameter[2];
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

                this.dtMapeo = db.GetTable("SEL_MAPEO", parametros);
            }
            catch (Exception Error)
            {
                string strMsgError = Error.Message;
            }
        }

        #endregion
    }
}
