using System;
using System.IO;
using System.Data;
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
        private DataTable resultados;
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
        private long lngRegistrosProcesados;
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

                // Prepara contenedor de resultados
                this.resultados = new DataTable();
                
                this.resultados.Columns.Add("NUM_REGISTRO", Type.GetType("System.Int64"));
                this.resultados.Columns.Add("RES_ROK", Type.GetType("System.Boolean"));
                this.resultados.Columns.Add("RES_ROM", Type.GetType("System.Boolean"));
                this.resultados.Columns.Add("RES_RER", Type.GetType("System.Boolean"));
                this.resultados.Columns.Add("OBSERVACIONES", Type.GetType("System.String"));

                // Carga archivo
                CargarArchivo();

                dteTermino = DateTime.Now;

                // Registra en base de datos
                registrarDatosEnBD();

                // Transfiere a tabla correspondiente

                //File.Delete(archivo.Ruta);
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
            switch (this.archivo.Extension.ToLower())
            {
                case ".txt":
                case ".csv":
                    cargarArchivoTXT();
                    break;
                case ".xls": 
                case ".xlsx":
                    cargarArchivoXLS();
                    break;
                default:
                    break;
            }
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

                                        obtenerRegistro(
                                                        ref registros, 
                                                        i,                      // Columna
                                                        reg - regOmitir,        // Fila
                                                        valor.Trim()            // Valor
                                                       );
                                    }
                                }
                            }
                            // Ancho fijo
                            else
                            {
                                // Obtener encabezados
                                if (this.layout.PrimerRegistroEncabezados)
                                {
                                    this.encabezados = new string[this.layout.CamposDeLayout.Length];
                                }
                                
                                foreach(CL_Layout_Campos campo in this.layout.CamposDeLayout)
                                {
                                    // Obtiene valor de campo
                                    string valor = strLine.Substring(campo.CaracterInicial - 1, (campo.CaracterFinal - campo.CaracterInicial) + 1).Trim();
                                    
                                    // Obtener encabezados
                                    if ((this.layout.PrimerRegistroEncabezados) && (reg == this.layout.FilaInicial))
                                    {
                                        encabezados[campo.ColumnaReferencia - 1] = valor;
                                    }
                                    // Asigna valor de registro
                                    else
                                    {
                                        obtenerRegistro(
                                                        ref registros,
                                                        campo.ColumnaReferencia,    // Columna
                                                        reg - regOmitir,            // Fila
                                                        valor.Trim()                // Valor
                                                       );
                                    }
                                }
                            }
                        }
                    }
                }

                // Cargar registros obtenidos
                this.registrosCargados = new Registro[registros.Count];
                registros.CopyTo(this.registrosCargados, 0);
            }
        }

        private void obtenerRegistro(ref ArrayList contenedor,int columna, long fila, string valor)
        {
            Registro registro = new Registro();

            if (encabezados != null)                    // Encabezado
            {
                if ((encabezados[columna - 1] != "") && (encabezados[columna - 1] != string.Empty))
                {
                    registro.NombreCampo = encabezados[columna - 1];
                }
            }
            registro.Campo = columna;                   // Columna
            registro.NumeroRegistro = fila;             // Fila
            registro.Valor = valor.Trim();              // Valor
            switch (validarRegistro(ref registro))      // Resultado de validación
            {
                case Resultado.Correcto:
                    registro.Correcto = true;
                    break;
                case Resultado.Omitido:
                    registro.Omitido = true;
                    break;
                case Resultado.Erroneo:
                    registro.Erroneo = true;
                    break;
                default:
                    break;
            }

            contenedor.Add(registro);

            evaluarResultado(registro);
        }

        private Resultado validarRegistro(ref Registro registro)
        {
            bool omitir = false;
            bool error = false;
            
            try
            {
                // Validacion general
                foreach (CL_Layout_Campos campo in layout.CamposDeLayout)
                {
                    if (campo.ColumnaReferencia == registro.Campo)
                    {
                        // ERRORES
                        // Exigir coincidencia de nombre del campo (Archivo VS Configuracion)
                        if (campo.ExigirCoincidenciaNombre)
                        {
                            if (registro.NombreCampo != campo.NombreCampoReferencia)
                            {
                                error = true;
                                registro.Observaciones += ((registro.Observaciones == string.Empty) ? "" : "|") +
                                                          "C" + registro.Campo + ": Nombre de campo difiere del establecido, se esperaba '" + campo.NombreCampoReferencia + "'";
                            }
                        }
                        // OMISIONES
                        break;
                    }
                }

                // Validacion particular
                // Modulo de validaciones (por definir)
                // Validaciones (ref omitir, ref error);

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
                                          "C" + registro.Campo + ": " + Error.Message;
                return Resultado.Erroneo;
            }
        }

        private void evaluarResultado(Registro registro)
        {
            try
            {
                // Comprueba existencia de registro
                DataRow[] rowFind = resultados.Select("NUM_REGISTRO = " + registro.NumeroRegistro);

                if ((rowFind != null) && (rowFind.Length > 0))
                {
                    foreach (DataRow row in rowFind)
                    {
                        // Actualiza registro
                        if (registro.Correcto)
                            row["RES_ROK"] = true;
                        if (registro.Omitido)
                            row["RES_ROM"] = true;
                        if (registro.Erroneo)
                            row["RES_RER"] = true;
                        if ((registro.Observaciones != string.Empty) && (registro.Observaciones != ""))
                            row["OBSERVACIONES"] = ((row["OBSERVACIONES"].ToString() != "") ? "|" : "") +
                                                   registro.Observaciones;
                    }
                }
                else
                {
                    // Crea registro
                    DataRow rowNew = resultados.NewRow();

                    rowNew["NUM_REGISTRO"] = registro.NumeroRegistro;
                    if (registro.Correcto)
                        rowNew["RES_ROK"] = true;
                    if (registro.Omitido)
                        rowNew["RES_ROM"] = true;
                    if (registro.Erroneo)
                        rowNew["RES_RER"] = true;
                    if ((registro.Observaciones != string.Empty) && (registro.Observaciones != ""))
                        rowNew["OBSERVACIONES"] = ((rowNew["OBSERVACIONES"].ToString() != "") ? "|" : "") +
                                                  registro.Observaciones;

                    resultados.Rows.Add(rowNew);
                }
            }
            catch (Exception Error)
            {
                string strMsgError = Error.Message;
            }
        }

        private void registrarDatosEnDB()
        {

        }

        #endregion
    }
}
