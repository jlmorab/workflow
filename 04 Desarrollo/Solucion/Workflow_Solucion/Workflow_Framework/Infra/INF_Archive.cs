using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

        public string NombreCompleto 
        {
            get { return strNombreCompleto; }
            set { strNombreCompleto = value; } 
        }

        public string Nombre 
        {
            get { return strNombre; }
            set { strNombre = value; } 
        }

        public string Ruta 
        {
            get { return strRuta; }
            set { strRuta = value; }
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

        public string Proceso 
        {
            get { return strProceso; }
            set { strProceso = value; }
        }

        #endregion

        #region Listas

        #endregion

        #region MetodosPublicos

        #endregion

        #region MetodosPrivados

        private void ObtenerParametros(string Ruta)
        {
            // Nombre Completo
            strNombreCompleto = Path.GetFileName(Ruta);

            // Extension
            strExtension = Path.GetExtension(Ruta);

            // Nombre
            strNombre = Path.GetFileNameWithoutExtension(Ruta);

            // Ruta
            strRuta = Path.GetDirectoryName(Ruta);
        }

        private void ObtenerPrefijos(string Archivo, int Caracteres, string Separador = null)
        {
            if (Separador != null)
            {
                int pos = Archivo.IndexOf(Separador);

                if (pos - 1 >= 0)
                {
                    strPrefijo = Archivo.Substring(0,pos - 1);
                }
            }
        }

        #endregion
    }
}
