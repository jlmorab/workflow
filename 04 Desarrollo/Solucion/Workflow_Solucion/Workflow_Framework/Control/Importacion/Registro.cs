using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workflow.Framework.Control.Importacion
{
    public class Registro
    {
        //----------------------------
        private string strObservaciones;
        private string strNombreCampo;
        private string strValor;
        //----------------------------
        private int intCampo;
        //----------------------------
        private long lngRegistro;
        //----------------------------
        private bool blnCorrecto;
        private bool blnOmitido;
        private bool blnErroneo;
        //----------------------------

        #region Constructor

        public Registro() 
        { 
            // Inicializa variables
            strNombreCampo = string.Empty;
            intCampo = -1;
            lngRegistro = -1;
            strValor = string.Empty;
            blnCorrecto = false;
            blnOmitido = false;
            blnErroneo = false;
            strObservaciones = string.Empty;
        }

        #endregion

        #region Propiedades

        public string NombreCampo 
        {
            get { return strNombreCampo; }
            set { strNombreCampo = value; }
        }

        public int Campo 
        {
            get { return intCampo; }
            set { intCampo = value; }
        }

        public long Registro
        {
            get { return lngRegistro; }
            set { lngRegistro = value; }
        }

        public string Valor
        {
            get { return strValor; }
            set { strValor = value; }
        }

        public bool Correcto
        {
            get { return blnCorrecto; }
            set { blnCorrecto = value; }
        }

        public bool Omitido
        {
            get { return blnOmitido; }
            set { blnOmitido = value; }
        }

        public bool Erroneo
        {
            get { return blnErroneo; }
            set { blnErroneo = value; }
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
