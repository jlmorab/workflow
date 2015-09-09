<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Importacion.aspx.cs" Inherits="ImportacionMasiva.Pages.Importacion" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Seleccina un archivo: 
        <input id="getArchivo" type="file" runat="server" name="oFile" />
        <asp:button id="btnUpload" type="submit" text="Upload" runat="server" OnClick="btnUpload_Click" />
        <asp:Panel ID="frmConfirmation" Visible="False" Runat="server">
            <asp:Label id="lblUploadResult" Runat="server"></asp:Label>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
