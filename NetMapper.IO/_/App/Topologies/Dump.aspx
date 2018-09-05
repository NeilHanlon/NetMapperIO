<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dump.aspx.cs" Inherits="NetMapper.IO.__.App.Topologies.Dump" %>
<html>
    <head>
        <title>Topology Dump</title>
    </head>
    <body>
        <div id="topologyDump"><asp:Literal runat="server" ID="dataDump"></asp:Literal></div>
        <script type="text/javascript">
            var topologyDump = document.getElementById('topologyDump');
            topologyDump.innerHTML = "<pre>" + JSON.stringify(JSON.parse(topologyDump.innerHTML), null, "\t") + "</pre>";
        </script>
    </body>
</html>