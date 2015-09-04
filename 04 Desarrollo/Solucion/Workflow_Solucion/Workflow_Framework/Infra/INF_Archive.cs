using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workflow.Framework.Infra
{
    public class INF_Archive : ICloneable
    {
        private string strNombreCompleto;
        private string strNombre;
        private string strRuta;
        private string strPrefijo;
        private string strSufijo;
        private string strExtension;
        private string strProceso;

        #region Constructor

        public INF_Archive() { }

        public INF_Archive(string RutaArchivo) { }

        public INF_Archive(string RutaArchivo, int Prefijo) { }

        public INF_Archive(string RutaArchivo, int Prefijo, int Sufijo) { }

        public INF_Archive(string RutaArchivo, int Prefijo = -1, int Sufijo = -1, char Separador = '\\') { }

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
