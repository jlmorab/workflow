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
        private string strUsuario;
        private string strNegocio;
        private string strLayout;
        //----------------------------
        private DateTime dteInicio;
        private Nullable<DateTime> dteTermino;
        //----------------------------
        private int intIdLayout;
        private int intIdNegocio;
        //----------------------------
        private long lngIdBitacora;
        private long lngRegistrosProcesados;
        private long lngCorrectos;
        private long lngOmitidos;
        private long lngErroneos;
        //----------------------------
        private Registro[] registrosCargados;
        //----------------------------

        #region Constructor

        public CL_ImportacionMasiva(int IdNegocio, int IdLayout, INF_Archive Archivo, string Usuario = "SYS")
        {
            // Inicializa variables
            archivo = Archivo;
            strUsuario = Usuario;
            intIdNegocio = IdNegocio;
            intIdLayout = IdLayout;
            dteInicio = DateTime.Now;
            dteTermino = null;
            lngCorrectos = 0;
            lngOmitidos = 0;
            lngErroneos = 0;
            strObservaciones = string.Empty;
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

                File.Delete(archivo.Ruta);
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

        public long IdBitacora
        {
            get { return lngIdBitacora; }
            set { lngIdBitacora = value; }
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
                        bool omitir = bool.Parse(row["RES_ROM"].ToString());
                        bool error = bool.Parse(row["RES_RER"].ToString());
                        
                        // Actualiza registro
                        if ((registro.Correcto) && (!omitir && !error))
                            row["RES_ROK"] = true;
                        if ((registro.Omitido) && (!error))
                        {
                            row["RES_ROM"] = true;
                            row["RES_ROK"] = false;
                        }
                        if (registro.Erroneo)
                        {
                            row["RES_ROK"] = false;
                            row["RES_ROM"] = false;
                            row["RES_RER"] = true;
                        }
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

        private void registrarDatosEnBD()
        {
            // Crea registro inicial en bitácora de importación
            registrarBitacora();

            if (this.lngIdBitacora != 0)
            {
                // Ingresa registros en base de datos
                foreach (Registro registro in this.registrosCargados)
                {
                    registraCampoEnBD(registro);
                }

                // Transfiere a tabla correspondiente
                transferirATabla(true, true, false);
                
                // Actualiza bitácora de importación con resultados obtenidos
                actualizarBitacora();
            }
        }

        private void registrarBitacora()
        {
            try
            {
                DataTable dt;
                
                db.Connection_Check();

                System.Data.OleDb.OleDbParameter[] parametros = new System.Data.OleDb.OleDbParameter[5];
                for (int i = 0; i < 5; i++)
                {
                    parametros[i] = new System.Data.OleDb.OleDbParameter();
                }

                parametros[0].ParameterName = "P_ID_LAYOUT";
                parametros[0].OleDbType = System.Data.OleDb.OleDbType.Integer;
                parametros[0].Value = this.intIdLayout;

                parametros[1].ParameterName = "P_USER";
                parametros[1].OleDbType = System.Data.OleDb.OleDbType.VarChar;
                parametros[1].Value = strUsuario;

                parametros[2].ParameterName = "P_ARCHIVO";
                parametros[2].OleDbType = System.Data.OleDb.OleDbType.VarChar;
                parametros[2].Value = archivo.Nombre;

                parametros[3].ParameterName = "P_EXTENSION";
                parametros[3].OleDbType = System.Data.OleDb.OleDbType.VarChar;
                parametros[3].Value = archivo.Extension;

                parametros[4].ParameterName = "P_OBS";
                parametros[4].OleDbType = System.Data.OleDb.OleDbType.VarChar;
                parametros[4].Value = strObservaciones;

                dt = db.GetTable("INS_BITACORA_IMPORTACION", parametros);

                foreach (DataRow row in dt.Rows)
                {
                    this.lngIdBitacora = long.Parse(row["BII_CVE"].ToString());
                }
            }
            catch (Exception Error)
            {
                string strMsgError = Error.Message;
            }
        }

        private void actualizarBitacora()
        {
            try
            {
                object rowQuery;
                
                // Obtiene valores totales de importación
                // Registros correctos
                rowQuery = resultados.Compute("Count(RES_ROK)","RES_ROK = true");
                this.lngCorrectos = long.Parse(rowQuery.ToString());

                // Registros omitidos
                rowQuery = resultados.Compute("Count(RES_ROM)", "RES_ROM = true");
                this.lngOmitidos = long.Parse(rowQuery.ToString());

                // Registros erróneos
                rowQuery = resultados.Compute("Count(RES_RER)", "RES_RER = true");
                this.lngErroneos = long.Parse(rowQuery.ToString());
                
                // Actualiza en base de datos
                db.Connection_Check();

                System.Data.OleDb.OleDbParameter[] parametros = new System.Data.OleDb.OleDbParameter[5];
                for (int i = 0; i < 5; i++)
                {
                    parametros[i] = new System.Data.OleDb.OleDbParameter();
                }

                parametros[0].ParameterName = "P_ID_BITACORA";
                parametros[0].OleDbType = System.Data.OleDb.OleDbType.BigInt;
                parametros[0].Value = this.lngIdBitacora;

                parametros[1].ParameterName = "P_REG_OK";
                parametros[1].OleDbType = System.Data.OleDb.OleDbType.BigInt;
                parametros[1].Value = this.lngCorrectos;

                parametros[2].ParameterName = "P_REG_OM";
                parametros[2].OleDbType = System.Data.OleDb.OleDbType.BigInt;
                parametros[2].Value = this.lngOmitidos;

                parametros[3].ParameterName = "P_REG_ER";
                parametros[3].OleDbType = System.Data.OleDb.OleDbType.BigInt;
                parametros[3].Value = this.lngErroneos;

                parametros[4].ParameterName = "P_OBS";
                parametros[4].OleDbType = System.Data.OleDb.OleDbType.VarChar;
                parametros[4].Value = this.strObservaciones;

                db.ExecutaProcedureNonQuery("UPD_BITACORA_IMPORTACION", parametros);
            }
            catch (Exception Error)
            {
                string strMsgError = Error.Message;
            }
        }

        private void registraCampoEnBD(Registro registro)
        {
            try
            {
                // Actualiza en base de datos
                db.Connection_Check();

                System.Data.OleDb.OleDbParameter[] parametros = new System.Data.OleDb.OleDbParameter[9];
                for (int i = 0; i < 9; i++)
                {
                    parametros[i] = new System.Data.OleDb.OleDbParameter();
                }

                parametros[0].ParameterName = "P_ID_BITACORA";
                parametros[0].OleDbType = System.Data.OleDb.OleDbType.BigInt;
                parametros[0].Value = this.lngIdBitacora;

                parametros[1].ParameterName = "P_NOM_CAMPO";
                parametros[1].OleDbType = System.Data.OleDb.OleDbType.VarChar;
                parametros[1].Value = registro.NombreCampo;

                parametros[2].ParameterName = "P_NUM_CAMPO";
                parametros[2].OleDbType = System.Data.OleDb.OleDbType.Integer;
                parametros[2].Value = registro.Campo;

                parametros[3].ParameterName = "P_NUM_REGISTRO";
                parametros[3].OleDbType = System.Data.OleDb.OleDbType.BigInt;
                parametros[3].Value = registro.NumeroRegistro;

                parametros[4].ParameterName = "P_VALOR";
                parametros[4].OleDbType = System.Data.OleDb.OleDbType.VarChar;
                parametros[4].Value = registro.Valor;

                parametros[5].ParameterName = "P_REG_OK";
                parametros[5].OleDbType = System.Data.OleDb.OleDbType.SmallInt;
                parametros[5].Value = ((registro.Correcto) ? 1 : 0);

                parametros[6].ParameterName = "P_REG_OM";
                parametros[6].OleDbType = System.Data.OleDb.OleDbType.SmallInt;
                parametros[6].Value = ((registro.Omitido) ? 1 : 0);

                parametros[7].ParameterName = "P_REG_ER";
                parametros[7].OleDbType = System.Data.OleDb.OleDbType.SmallInt;
                parametros[7].Value = ((registro.Erroneo) ? 1 : 0);

                parametros[8].ParameterName = "P_OBS";
                parametros[8].OleDbType = System.Data.OleDb.OleDbType.VarChar;
                parametros[8].Value = registro.Observaciones;

                db.ExecutaProcedureNonQuery("INS_REGISTRO_IMPORTACION", parametros);
            }
            catch (Exception Error)
            {
                string strMsgError = Error.Message;
            }
        }

        private void transferirATabla(bool Correctos, bool Omitidos, bool Erroneos)
        {
            try
            {
                // Estructurar datos obtenidos en forma de tabla y los inserta en la tabla deseada
                db.Connection_Check();

                System.Data.OleDb.OleDbParameter[] parametros = new System.Data.OleDb.OleDbParameter[6];
                for (int i = 0; i < 3; i++)
                {
                    parametros[i] = new System.Data.OleDb.OleDbParameter();
                }

                parametros[0].ParameterName = "P_ID_BITACORA";
                parametros[0].OleDbType = System.Data.OleDb.OleDbType.BigInt;
                parametros[0].Value = this.lngIdBitacora;

                parametros[1].ParameterName = "P_ID_LAYOUT";
                parametros[1].OleDbType = System.Data.OleDb.OleDbType.Integer;
                parametros[1].Value = this.intIdLayout;

                parametros[2].ParameterName = "P_TABLA_DESTINO";
                parametros[2].OleDbType = System.Data.OleDb.OleDbType.VarChar;
                parametros[2].Value = this.layout.TablaDestino;

                parametros[3].ParameterName = "P_REG_OK";
                parametros[3].OleDbType = System.Data.OleDb.OleDbType.Integer;
                parametros[3].Value = ((Correctos) ? 1 : 0);

                parametros[4].ParameterName = "P_REG_OM";
                parametros[4].OleDbType = System.Data.OleDb.OleDbType.Integer;
                parametros[4].Value = ((Omitidos) ? 1 : 0);

                parametros[5].ParameterName = "P_REG_ER";
                parametros[5].OleDbType = System.Data.OleDb.OleDbType.Integer;
                parametros[5].Value = ((Erroneos) ? 1 : 0);

                db.ExecutaProcedureNonQuery("INS_TABLA_IMPORTACION", parametros);
            }
            catch (Exception Error)
            {
                string strMsgError = Error.Message;
            }
        }

        #endregion
    }
}
