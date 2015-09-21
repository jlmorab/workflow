using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Workflow_Data;

namespace Workflow.Framework.Control.Importacion
{
    public class CL_ImportacionMasiva
    {

        Configuracion cnf = new Configuracion();
        dbInterface db = new dbInterface();

        private string strNegocio;
        private string strLayout;

        #region Constructor

        public CL_ImportacionMasiva()
        {
            if(cnf.Estatus == Configuracion.Status.Correcto)
            {
                db.DBConString = cnf.CnnString;
            }
        }

        #endregion

        #region Propiedades

        #endregion

        #region Listas

        #endregion

        #region MetodosPublicos

        public void EjecucionDePrueba()
        {
            int IdNegocio = 1;
            int IdLayout = 1;

            CL_Layout layout = new CL_Layout(IdNegocio, IdLayout, this.db);
            CL_Layout_Campos campos = new CL_Layout_Campos(this.db);
        }

        #endregion

        #region MetodosPrivados

        #endregion
    }
}
