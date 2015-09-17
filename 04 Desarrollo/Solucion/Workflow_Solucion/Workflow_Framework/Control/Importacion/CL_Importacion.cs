using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Workflow_Data;

namespace Workflow.Framework.Control.Importacion
{
    public class CL_Importacion
    {
        
        //----------------------------
        private string strObservaciones;
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

        internal dbInterface db;
        
        #region Constructor

        public CL_Importacion(Infra.INF_Archive archivo, CL_Layout layout, CL_Layout_Campos campos) 
        { 
            // Inicializa variables
            dteInicio = DateTime.Now;
            dteTermino = null;
            lngCorrectos = 0;
            lngOmitidos = 0;
            lngErroneos = 0;
        }

        #endregion

        #region Propiedades

        public DateTime Inicio 
        {
            get { return dteInicio; }
            set { dteInicio = value; }
        }

        public DateTime Termino 
        {
            get { return dteTermino; }
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
