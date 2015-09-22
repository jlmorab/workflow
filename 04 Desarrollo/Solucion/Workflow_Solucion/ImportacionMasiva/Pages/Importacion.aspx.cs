using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

                // Comprueba existencia en servidor
                while(File.Exists(arcTarget.Ruta))
                {
                    // Obtiene nombre de archivo consecutivo
                    i++;
                    arcTarget.Nombre = arcTarget.Nombre + "(" + i + ")";
                }

                // Carga archivo en servidor
                try
                {
                    getArchivo.PostedFile.SaveAs(arcTarget.Ruta);

                    int IdNegocio = 1;
                    int IdLayout = 0;

                    CL_ImportacionMasiva importar = new CL_ImportacionMasiva(IdNegocio, IdLayout, arcTarget);

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