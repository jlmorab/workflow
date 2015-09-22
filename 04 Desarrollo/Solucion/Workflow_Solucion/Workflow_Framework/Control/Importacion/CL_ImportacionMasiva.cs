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

        //----------------------------
        private string strObservaciones;
        private string strNegocio;
        private string strLayout;
        //----------------------------
        private DateTime dteInicio;
        private Nullable<DateTime> dteTermino;
        //----------------------------
        private long lngCorrectos;
        private long lngOmitidos;
        private long lngErroneos;
        //----------------------------
        private Registro[] registros;
        //----------------------------

        #region Constructor

        public CL_ImportacionMasiva(int IdNegocio, int IdLayout, Infra.INF_Archive archivo)
        {
            // Inicializa variables
            dteInicio = DateTime.Now;
            dteTermino = null;
            lngCorrectos = 0;
            lngOmitidos = 0;
            lngErroneos = 0;
            
            if(cnf.Estatus == Configuracion.Status.Correcto)
            {
                db.DBConString = cnf.CnnString;

                CL_Layout layout = new CL_Layout(IdNegocio, IdLayout, db);
                strNegocio = layout.Negocio;
                strLayout = layout.Nombre;
            }
        }

        #endregion

        #region Propiedades

        public string Negocio 
        {
            get { return strNegocio; }
            set { strNegocio = value; }
        }

        public string Layout
        {
            get { return strLayout; }
            set { strLayout = value; }
        }

        public DateTime Inicio
        {
            get { return dteInicio; }
            set { dteInicio = value; }
        }

        public Nullable<DateTime> Termino
        {
            get
            {
                return dteTermino;
            }
            set { dteTermino = value; }
        }

        public long RegistrosCorrectos
        {
            get { return lngCorrectos; }
        }

        public long RegistrosOmitidos
        {
            get { return lngOmitidos; }
        }

        public long RegistrosErroneos
        {
            get { return lngErroneos; }
        }

        public long RegistrosTotales
        {
            get { return (lngCorrectos + lngOmitidos + lngErroneos); }
        }

        public string Observaciones
        {
            get { return strObservaciones; }
            set { strObservaciones = value; }
        }

        #endregion

        #region Listas

        #endregion

        #region MetodosPublicos

        #endregion

        #region MetodosPrivados

        #endregion
    }
}
