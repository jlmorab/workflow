using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Workflow.Framework.Infra;
using Workflow.Framework.Control.Importacion;

namespace ImportacionMasiva.Pages
{
    public partial class Importacion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            // Ruta de archivos temporales en servidor
            string strFolder = Server.MapPath("../Files/tmp/");

            if (getArchivo.Value != "")
            {
                // Recepción de archivo
                INF_Archive arcSorce = new INF_Archive(getArchivo.PostedFile.FileName);

                // En caso de no existir el folder, lo crea
                if (!Directory.Exists(strFolder))
                {
                    Directory.CreateDirectory(strFolder);
                }

                // Carga archivo en servidor
                INF_Archive arcTarget = new INF_Archive(strFolder + arcSorce.NombreCompleto);

                int i = 0;

                string nombreOriginal = arcTarget.Nombre;

                // Comprueba existencia en servidor
                while(File.Exists(arcTarget.Ruta))
                {
                    // Obtiene nombre de archivo consecutivo
                    i++;
                    arcTarget.Nombre = nombreOriginal + "(" + i + ")";
                    
                }

                // Carga archivo en servidor
                try
                {   
                    getArchivo.PostedFile.SaveAs(arcTarget.Ruta);

                    int IdNegocio = 1;
                    int IdLayout = 1;
                    
                    CL_ImportacionMasiva importar = new CL_ImportacionMasiva(IdNegocio, IdLayout, arcTarget);

                    if(importar.ExisteArchivoDesviaciones)
                    {
                        string strArchivo = importar.ArchivoDesviaciones.NombreCompleto;

                        Response.Clear();
                        Response.AddHeader("content-disposition", "attachment;filename=" + strArchivo);
                        Response.ContentType = "application/vnd.csv";
                        Response.Charset = "UTF-8";
                        Response.ContentEncoding = System.Text.Encoding.UTF8;
                        byte[] MyData = (byte[])System.IO.File.ReadAllBytes(Server.MapPath("~/bin/files/") + strArchivo);
                        Response.BinaryWrite(MyData);
                        Response.End();
                    }

                    lblUploadResult.Text = "Archivo cargado en: " + arcTarget.Ruta;
                }
                catch (Exception Error)
                {
                    lblUploadResult.Text = "Error al cargar archivo: " + Error.Message;
                }

            }
            else
            {
                lblUploadResult.Text = "Seleccione un archivo. De click en 'Explorar...'";
            }
            // Display the result of the upload.
            frmConfirmation.Visible = true;
        }
    }
}