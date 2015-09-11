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

        #endregion

        #region MetodosPrivados

        #endregion
    }
}
