using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Workflow_Data;
using Workflow.Framework.Infra;

namespace Workflow.Framework.Control.Importacion
{
    public class CL_ImportacionMasiva
    {

        Configuracion cnf = new Configuracion();
        dbInterface db = new dbInterface();

        //----------------------------
        private INF_Archive archivo;
        private CL_Layout layout;
        private string[] encabezados;
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
        private Registro[] registrosCargados;
        //----------------------------

        #region Constructor

        public CL_ImportacionMasiva(int IdNegocio, int IdLayout, INF_Archive Archivo)
        {
            // Inicializa variables
            archivo = Archivo;
            dteInicio = DateTime.Now;
            dteTermino = null;
            lngCorrectos = 0;
            lngOmitidos = 0;
            lngErroneos = 0;
            encabezados = null;
            
            if(cnf.Estatus == Configuracion.Status.Correcto)
            {
                db.DBConString = cnf.CnnString;

                CL_Layout loadLayout = new CL_Layout(IdNegocio, IdLayout, db);
                this.layout = loadLayout;
                strNegocio = this.layout.Negocio;
                strLayout = this.layout.Nombre;
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

        enum Resultado
        {
            Correcto,
            Omitido,
            Erroneo
        }

        #endregion

        #region MetodosPublicos

        public void CargarArchivo()
        {
            
        }

        #endregion

        #region MetodosPrivados

        public void cargarArchivoXLS()
        {
            // Valida existencia de archivo
            if (File.Exists(this.archivo.Ruta))
            {
                ArrayList registros = new ArrayList();

                // Cargar registros obtenidos
                this.registrosCargados = new Registro[registros.Count];
                registros.CopyTo(this.registrosCargados, 0);
            }
        }

        public void cargarArchivoTXT()
        {
            // Valida existencia de archivo
            if (File.Exists(this.archivo.Ruta))
            {
                ArrayList registros = new ArrayList();
                long regOmitir = this.layout.FilaInicial - 
                                 ((this.layout.PrimerRegistroEncabezados) ? 0 : -1);

                // Abre archivo
                using (StreamReader pfile = new StreamReader(this.archivo.Ruta))
                {   
                    long reg = 0;

                    while (pfile.Peek() >= 0)
                    {
                        string strLine = pfile.ReadLine();
                        reg += 1;

                        // Procesa en inicio de documento, de acuerdo a configuración
                        if (reg >= this.layout.FilaInicial)
                        {
                            // Demilitado
                            if(this.layout.Separador != string.Empty)
                            {
                                // Obtiene campos
                                string[] cadena = strLine.Split(char.Parse(this.layout.Separador));
                                
                                // Obtener encabezados
                                if ((this.layout.PrimerRegistroEncabezados) && (reg == this.layout.FilaInicial))
                                {
                                    this.encabezados = new string[cadena.Length - 1];
                                    this.encabezados = cadena;
                                }
                                // Obtiene valores de la fila
                                else
                                {
                                    for (int i = 1 ; i <= cadena.Length; i++ )
                                    {
                                        string valor = cadena[i - 1];

                                        Registro registro = new Registro();

                                        if(encabezados != null)
                                        {
                                            registro.NombreCampo = encabezados[i - 1];
                                        }
                                        registro.Campo = i;
                                        registro.NumeroRegistro = reg - regOmitir;
                                        registro.Valor = valor.Trim();
                                        switch (validarRegistro(ref registro))
                                        {
                                            case Resultado.Correcto:
                                                break;
                                            case Resultado.Omitido:
                                                break;
                                            case Resultado.Erroneo:
                                                break;
                                            default:
                                                break;
                                        }

                                        registros.Add(registro);
                                    }
                                }
                            }
                            // Ancho fijo
                            else
                            {

                            }
                        }
                    }
                }

                // Cargar registros obtenidos
                this.registrosCargados = new Registro[registros.Count];
                registros.CopyTo(this.registrosCargados, 0);
            }
        }

        private Resultado validarRegistro(ref Registro registro)
        {
            bool omitir = false;
            bool error = false;
            
            try
            {
                // Validacion general
                

                // Validacion particular

                // Devuelve resultado
                if (error)
                {
                    return Resultado.Erroneo;
                }
                else
                {
                    if (omitir)
                    {
                        return Resultado.Omitido;
                    }
                    else
                    {
                        return Resultado.Correcto;
                    }
                }
            }
            catch (Exception Error)
            {
                registro.Observaciones += ((registro.Observaciones == string.Empty) ? "" : "|") +
                                          Error.Message;
                return Resultado.Erroneo;
            }
        }

        #endregion
    }
}
