﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Workflow.Framework.Infra
{
    public class INF_Archive
    {
        private string strNombreCompleto;
        private string strNombre;
        private string strRuta;
        private string strPrefijo;
        private string strSufijo;
        private string strExtension;
        private string strProceso;
        private string strSeparador;

        private int intPrefijo;
        private int intSufijo;

        #region Constructor

        public INF_Archive() { }

        public INF_Archive(string RutaArchivo) 
        {
            ObtenerParametros(RutaArchivo, -1, -1, null);
        }

        public INF_Archive(string RutaArchivo, int Prefijo) 
        {
            ObtenerParametros(RutaArchivo, Prefijo, -1, null);
        }

        public INF_Archive(string RutaArchivo, int Prefijo, int Sufijo) 
        {
            ObtenerParametros(RutaArchivo, Prefijo, Sufijo, null);
        }

        public INF_Archive(string RutaArchivo, int Prefijo = -1, int Sufijo = -1, string Separador = null) 
        { 
            ObtenerParametros(RutaArchivo, Prefijo, Sufijo, Separador);
        }

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
            set 
            {
                ObtenerParametros(value, -1, -1, null);
                strRuta = value; 
            }
        }

        public int CaracteresPrefijo
        {
            get { return intPrefijo; }
            set 
            { 
                intPrefijo = value;

                if ((strNombre != null) && (strNombre.Trim() != ""))
                {
                    ObtencionPorCaracteres(strNombre, intPrefijo, true, false);
                }
            }
        }
        
        public string Prefijo 
        {
            get { return strPrefijo; }
            set { strPrefijo = value; } 
        }

        public int CaracteresSufijo
        {
            get { return intSufijo; }
            set 
            { 
                intSufijo = value;

                if ((strNombre != null) && (strNombre.Trim() != ""))
                {
                    ObtencionPorCaracteres(strNombre, intSufijo, false, true);
                }
            }
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

        public void AplicarSeparador(string Separador, bool Prefijo, bool Sufijo)
        {
            if ((Nombre != null) && (Nombre.Trim() != ""))
            {
                ObtencionPorSeparador(Nombre, Separador, Prefijo, Sufijo);
            }
        }

        #endregion

        #region MetodosPrivados

        private void ObtenerParametros(string Ruta, int Prefijo = -1, int Sufijo = -1, string Separador = "_")
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

        private void ObtenerPrefijos(string Archivo, int Caracteres, string Separador = "_")
        {
            // Incializa valores
            strPrefijo = "";
            
            if (Separador != null)
            {
                ObtencionPorSeparador(Archivo, Separador, true, false);
            }
            else
            {
                ObtencionPorCaracteres(Archivo, Caracteres, true, false);
            }
        }

        private void ObtenerSufijo(string Archivo, int Caracteres, string Separador = "_")
        {
            // Incializa valores
            strSufijo = "";

            if (Separador != null)
            {
                ObtencionPorSeparador(Archivo, Separador, false, true);
            }
            else
            {
                ObtencionPorCaracteres(Archivo, Caracteres, false, true);
            }
        }

        private void ObtencionPorSeparador(string Archivo, string Separador = "_", bool Prefijo = false, bool Sufijo = false)
        {   
            // Prefijo
            if (Prefijo)
            {
                // Obtiene posición del separador
                int pos = Archivo.IndexOf(Separador);

                if (pos - 1 >= 0)
                {
                    // Obtiene cadena de caracteres anteriores al separador
                    strPrefijo = Archivo.Substring(0, pos - 1).ToUpper();
                }
            }

            // Sufijo
            if (Sufijo)
            {
                // Obtiene última posición del separador
                int pos = Archivo.LastIndexOf(Separador);

                if (pos - 1 >= 0)
                {
                    // Obtiene cadena de caracteres posteriores al separador
                    strSufijo = Archivo.Substring(pos + 1).ToUpper();
                }
            }
        }

        private void ObtencionPorCaracteres(string Archivo, int Caracteres, bool Prefijo = false, bool Sufijo = false)
        {
            if (Caracteres > 0)
            {
                // Comprueba que el archivo tenga más de los caracteres definidos
                if (Archivo.Length > Caracteres)
                {
                    // Obtiene cadena de caracteres de acuerdo a caracteres definidos
                    // Prefijo
                    if (Prefijo)
                    {
                        strPrefijo = Archivo.Substring(0, Caracteres).ToUpper();
                    }

                    // Sufijo
                    if (Sufijo)
                    {
                        strSufijo = Archivo.Substring(Archivo.Length - Caracteres).ToUpper();
                    }
                }
            }
        }

        #endregion
    }
}
