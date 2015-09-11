using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Workflow_Data
{
    public class Configuracion
    {
        private string strCnnString;
        private string strServer;
        private string strUser;
        private string strPassword;
        private string strCollection;
        private Configuracion.Status lstEstatus;

        # region Constructor

        public Configuracion()
        {
            // Inicializa propiedades
            this.strServer = string.Empty;
            this.strUser = string.Empty;
            this.strPassword = string.Empty;
            this.strCollection = string.Empty;
            this.strCnnString = string.Empty;
            this.lstEstatus = Status.Incorrecto;
            
            // Obtiene configuración de conexión a base de datos
            string strXmlPath = @"C:\xmlConfiguracion.xml";

            if (File.Exists(strXmlPath))
            {
                XmlDocument xmlDoc = new XmlDocument();

                try
                {
                    xmlDoc.Load(strXmlPath);
                    
                    XmlNode root = xmlDoc.DocumentElement;
                    XmlNode nodo;
                    string elemento = string.Empty;

                    // Servidor
                    elemento = "Server";
                    nodo = root.SelectSingleNode("Data/" + elemento);
                    this.strServer = nodo.InnerText;

                    // Usuario
                    elemento = "User";
                    nodo = root.SelectSingleNode("Data/" + elemento);
                    this.strUser = nodo.InnerText;

                    // Password
                    elemento = "Password";
                    nodo = root.SelectSingleNode("Data/" + elemento);
                    this.strPassword = nodo.InnerText;

                    // Collection
                    elemento = "Collection";
                    nodo = root.SelectSingleNode("Data/" + elemento);
                    this.strCollection = nodo.InnerText;

                    // Conection String
                    GenerarConexionString();
                    
                }
                catch (Exception Error)
                {
                    string strMsgError = Error.Message;
                    this.lstEstatus = Status.Error;
                }
            }
            else
            {
                this.lstEstatus = Status.Error;
            }
            
        }

        #endregion

        #region Propiedades

        public string CnnString
        {
            get
            {
                GenerarConexionString();
                return strCnnString;
            }
        }

        public Status Estatus 
        {
            get { return lstEstatus; }
        }

        #endregion

        #region Listas

        public enum Status
        {
            Error = -1,
            Incorrecto = 0,
            Correcto = 1
        }

        #endregion

        #region MetodosPublicos

        #endregion

        #region MetodosPrivados

        private void GenerarConexionString()
        {
            try
            {
                if ((strServer != string.Empty) || (strUser != string.Empty) ||
                   (strPassword != string.Empty) || (strCollection != string.Empty))
                {
                    strCnnString = "Provider=IBMDA400.DataSource.1;" +
                                   "Password=" + strPassword.Trim() + ";" +
                                   "Persist Security Info=True;" +
                                   "User ID=" + strUser.Trim() + ";" +
                                   "Data Source=" + strServer.Trim() + ";" +
                                   "Protection Level=None;" +
                                   "Transport Product=Client Access;" +
                                   "SSL=DEFAULT;" +
                                   "Force Translate=1252;" +
                                   "Default Collection=" + strCollection.Trim() + ";" +
                                   "Convert Date Time To Char=TRUE;" +
                                   "Catalog Library List=SFBRINTDUT";

                    lstEstatus = Status.Correcto;
                }
                else
                {
                    strCnnString = string.Empty;
                }
            }
            catch (Exception Error)
            {
                string strMsgErr = Error.Message;
                strCnnString = string.Empty;
            }
        }

        #endregion

    }
}
