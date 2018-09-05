function renderTopology(topologyId) {
    $.post("/assets/cs/_.app.topologies.view.default.aspx", "reqType=Load&Id=" + topologyId, function (data) {
        if (data["Error"] != null) {
            toastr["error"](data["Exception"]);
            return;
        }
        hasRead = data["hasRead"];
        hasWrite = data["hasWrite"];
        isOwner = data["isOwner"];

        $('#txt-top-title').html(data["siteName"]);
        $('#txt-top-desc').html(data["topologyDescription"]);
        $('#txt-create-by').html(data["createByName"]);
        $('#txt-edit-by').html(data["lastEditByName"]);
        $('#txt-create-on').html(new Date(data["createTime"]).toLocaleDateString("en-GB"));
        $('#txt-edit-on').html(new Date(data["lastEditTime"]).toLocaleDateString("en-GB"));

        var Nodes = [];
        for (i = 0; i < data["Nodes"].length; i++) {
            console.log();
            var Node = { id: data["Nodes"][i]["NodeObjectId"] };
            Node["label"] = data["Nodes"][i]["Title"];
            if (data["Nodes"][i]["HoverText"] != null) Node["title"] = data["Nodes"][i]["HoverText"];

            if (data["Nodes"][i]["xPosition"] != -1) Node["x"] = data["Nodes"][i]["xPosition"];
            if (data["Nodes"][i]["yPosition"] != -1) Node["y"] = data["Nodes"][i]["yPosition"];
            if (data["Nodes"][i]["xPosition"] != -1) Node["physics"] = false;

            Node["shape"] = "image";
            Node["image"] = data["Nodes"][i]["NetIconPath"];
            Nodes.push(Node);
        }
        nodes = new vis.DataSet(Nodes);
        var Edges = [];
        for (i = 0; i < data["Edges"].length; i++) {
            var Edge = { id: data["Edges"][i]["EdgeId"] };
            Edge["label"] = data["Edges"][i]["Title"];
            Edge["from"] = data["Edges"][i]["FromNodeId"];
            Edge["to"] = data["Edges"][i]["ToNodeId"];
            Edge["color"] = { color: data["Edges"][i]["Colour"] };
            Edge["font"] = { align: "top" };
            Edge["physics"] = false;
            Edge["dashes"] = data["Edges"][i]["isDashed"];
            Edges.push(Edge);
        }
        edges = new vis.DataSet(Edges);
        var container = document.getElementById('networkTopology');
        var options = {
        };
        var data = {
            nodes: nodes,
            edges: edges
        };
        var network = new vis.Network(container, data, options);
        topologyObj = network;
        network.fit();
        runInit = true;
    }, "json");
}