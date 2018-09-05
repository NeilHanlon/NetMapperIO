$(function () { $('[data-toggle="tooltip"]').tooltip() }); // Enabled BootStrap Tooltips on every object.
renderTopology(topologyId); // Renders the topology. This can be called at any time to refresh all information on the page.
setInterval(function () { if (runInit) { page_init(); runInit = false; } }, 1000); // Check to see if renderTopology has been called and finished. If it has; init the page.
var netIcons = [];
function page_init() {
    if (hasWrite) { $('.show-on-edit').removeClass("hidden"); } else { $('.show-on-edit').addClass("hidden"); }  // If the user has edit permissions; enable the controls to edit the topology.
    if (!isOwner) { $('#btn-edt-pem').addClass("disabled"); }
    topologyObj.on('dragEnd', function () { $('#btn-sve-pos').removeClass("disabled"); }); // When the user has dragged anything - allow them to "Save Positions"
    topologyObj.on('click', function (properties) { // On click of the topology
        var Nodes = nodes.get(properties.nodes);
        var Edges = edges.get(properties.edges);
        if (Nodes.length > 0) { // If the user clicked on a node
            $('#btn-del-sel').removeClass("disabled"); // Allow Deletion
            $('#btn-edt-sel').removeClass("disabled"); // Allow Edit
            $('#btn-del-sel').unbind("click"); // Unbind the previous action of the button
            $('#btn-edt-sel').unbind("click"); // Unbind the previous action of the button
            $('#btn-del-sel').click(function () {
                console.log(Nodes[0]);
                $('#mdl-cnf-msg').html("Are you sure you'd like to remove the node '" + Nodes[0]["label"] + "'?");
                $('#mdl-cnf').modal("show");
                $('#mdl-cnf-btn').unbind("click");
                $('#mdl-cnf-btn').click(function () {
                    $.post("/assets/cs/_.app.topologies.view.default.aspx", "reqType=removeNode&topologyId=" + topologyId + "&nodeId=" + Nodes[0]["id"], function (data, textStatus) {
                        if (data["Status"] != "Completed")
                            toastr["error"](data["Exception"]);
                        else {
                            toastr["success"]("The node '" + Nodes[0]["label"] + "' was deleted.");
                            $('#mdl-cnf').modal("hide");
                            renderTopology(topologyId);
                        }
                        console.log(data);
                    }, "json");
                });
            });
        }
        else { // If the user did not click a node
            if (Edges.length > 0) { // Was it an edge?
                $('#btn-del-sel').removeClass("disabled"); // Allow Deletion
                $('#btn-edt-sel').removeClass("disabled"); // Allow Edit
                $('#btn-del-sel').unbind("click"); // Unbind the previous action of the button
                $('#btn-edt-sel').unbind("click"); // Unbind the previous action of the button
                // The edge ID will be Edges[0]["id"]
            }
            else {
                $('#btn-edt-sel').addClass("disabled"); // Disable Deletion
                $('#btn-del-sel').addClass("disabled"); // Disable Edit
            }
        }
    });
}

/*
 * ALL THE BELOW FUNCTIONS ARE FOR BUTTON AND FORM CONTROL 
 * 
 * */
$('#btn-add-pem').click(function () {
    if ($('#txt-add-pem').val() == "") {
        toastr["error"]("Please type a username to give permissions to.");
        return;
    }
    $.post("/assets/cs/_.app.topologies.view.default.aspx", "reqType=setPermission&topologyId=" + topologyId + "&username=" + $('#txt-add-pem').val() + "&permission=" + $('#ddl-add-pem').val(), function (data, textStatus) {
        console.log(data);
        if (data["Status"] != "Completed") {
            if (data["InnerException"] != null)
                toastr["error"](data["InnerException"]);
            else
                toastr["error"](data["Exception"]);
        }
        else {
            toastr["success"]("Successfully added permissions for the user '" + $('#txt-add-pem').val() + "'.");
            $('#txt-add-pem').val("");

            $.post("/assets/cs/_.app.topologies.view.default.aspx", "reqType=loadPermissions&topologyId=" + topologyId, function (data, textStatus) {
                $('#mdl-edt-pem-tbl').html("<tr></tr>");
                $.each(data.Permissions, function (i, item) {
                    let hasRead = "selected";
                    let hasWrite = "";
                    if (item.hasWrite) { hasRead = ""; hasWrite = "selected"; }
                    $('#mdl-edt-pem-tbl tr:last').after('<tr><td><span class="form-control text-center">' + item.UsersName + ' [' + item.Username + ']</span></td><td><select class="form-control chg-pem-ddl" data-username="' + item.Username + '"><option value="r" ' + hasRead + '>Read Only</option><option value="rw" ' + hasWrite + '>Read/Write</option></select></td><td><button type="button" class="btn btn-danger btn-block del-pem-btn" data-username="' + item.Username + '"><i class="fa fa-times"></i></button></td><tr>');
                });

                $('.chg-pem-ddl').unbind("change");
                $('.chg-pem-ddl').change(function (data) {
                    let user = $(this).attr("data-username");
                    $.post("/assets/cs/_.app.topologies.view.default.aspx", "reqType=setPermission&topologyId=" + topologyId + "&username=" + $(this).attr("data-username") + "&permission=" + $(this).val(), function (data, textStatus) {
                        if (data["Status"] != "Completed")
                            toastr["error"](data["Exception"]);
                        else {
                            toastr["success"]("Permissions updated for " + user + ".");
                        }
                    }, "json");
                });

                $('.del-pem-btn').unbind("change");
                $('.del-pem-btn').click(function (data) {
                    let user = $(this).attr("data-username");
                    let btn = $(this);
                    $.post("/assets/cs/_.app.topologies.view.default.aspx", "reqType=setPermission&topologyId=" + topologyId + "&username=" + $(this).attr("data-username") + "&permission=n", function (data, textStatus) {
                        if (data["Status"] != "Completed")
                            toastr["error"](data["Exception"]);
                        else {
                            toastr["success"]("Permissions removed for " + user + ".");
                            btn.parent().parent().remove();
                        }
                    }, "json");
                });
            }, "json");
        }
    }, "json");
});
$('#mdl-edt-pem').on('shown.bs.modal', function (e) {
    $.post("/assets/cs/_.app.topologies.view.default.aspx", "reqType=loadPermissions&topologyId=" + topologyId, function (data, textStatus) {
        $('#mdl-edt-pem-tbl').html("<tr></tr>");
        $.each(data.Permissions, function (i, item) {
            let hasRead = "selected";
            let hasWrite = "";
            if (item.hasWrite) { hasRead = ""; hasWrite = "selected"; }
            $('#mdl-edt-pem-tbl tr:last').after('<tr><td><span class="form-control text-center">' + item.UsersName + ' [' + item.Username + ']</span></td><td><select class="form-control chg-pem-ddl" data-username="' + item.Username + '"><option value="r" ' + hasRead + '>Read Only</option><option value="rw" ' + hasWrite + '>Read/Write</option></select></td><td><button type="button" class="btn btn-danger btn-block del-pem-btn" data-username="' + item.Username + '"><i class="fa fa-times"></i></button></td><tr>');
        });

        $('.chg-pem-ddl').unbind("change");
        $('.chg-pem-ddl').change(function (data) {
            let user = $(this).attr("data-username");
            $.post("/assets/cs/_.app.topologies.view.default.aspx", "reqType=setPermission&topologyId=" + topologyId + "&username=" + $(this).attr("data-username") + "&permission=" + $(this).val(), function (data, textStatus) {
                if (data["Status"] != "Completed")
                    toastr["error"](data["Exception"]);
                else {
                    toastr["success"]("Permissions updated for " + user + ".");
                }
            }, "json");
        });

        $('.del-pem-btn').unbind("change");
        $('.del-pem-btn').click(function (data) {
            let user = $(this).attr("data-username");
            let btn = $(this);
            $.post("/assets/cs/_.app.topologies.view.default.aspx", "reqType=setPermission&topologyId=" + topologyId + "&username=" + $(this).attr("data-username") + "&permission=n", function (data, textStatus) {
                if (data["Status"] != "Completed")
                    toastr["error"](data["Exception"]);
                else {
                    toastr["success"]("Permissions removed for " + user + ".");
                    btn.parent().parent().remove();
                }
            }, "json");
        });
    }, "json");
});

$('#mdl-add-con').on('shown.bs.modal', function (e) {
    $('#mdl-add-con-frm-nde').children('option').remove();
    $('#mdl-add-con-to-nde').children('option').remove();

    $('#mdl-add-con-to-nde').parent().removeClass("has-error");
    $('#mdl-add-con-frm-nde').parent().removeClass("has-error");
    $('#mdl-add-con-frm-nde-prt').parent().removeClass("has-error");
    $('#mdl-add-con-to-nde-prt').parent().removeClass("has-error");

    $.each(topologyObj.body.nodes, function (i, item) {
        if (!isNaN(item.id)) {
            $('#mdl-add-con-frm-nde')
                .append($("<option></option>")
                    .attr("value", item.id)
                    .text(item.options.label));
            $('#mdl-add-con-to-nde')
                .append($("<option></option>")
                    .attr("value", item.id)
                    .text(item.options.label));
        }
    });
})
$('#mdl-add-con-btn').click(function () {
    if ($('#mdl-add-con-to-nde').val() == $('#mdl-add-con-frm-nde').val()) {
        toastr["error"]("You cannot connect a node to it self.");
        $('#mdl-add-con-to-nde').parent().addClass("has-error");
        $('#mdl-add-con-frm-nde').parent().addClass("has-error");
        return;
    }
    if ($('#mdl-add-con-frm-nde-prt').val() == "" || $('#mdl-add-con-to-nde-prt').val() == "") {
        toastr["error"]("You must specify which ports the connection is attached to on each node.");
        $('#mdl-add-con-frm-nde-prt').parent().addClass("has-error");
        $('#mdl-add-con-to-nde-prt').parent().addClass("has-error");
        return;
    }
    $.post("/assets/cs/_.app.topologies.view.default.aspx", $('#mdl-add-con-frm').serialize(), function (data, textStatus) {
        if (data["Status"] != "Completed")
            toastr["error"](data["Exception"]);
        else {
            toastr["success"]("Connection has been added successfully.");
            $('#mdl-add-con').modal("hide");
            renderTopology(topologyId);
        }
    }, "json");
});
$('#btn-sve-pos').click(function () {
    $('#btn-sve-pos').addClass("disabled");
    let xOffset = 0;
    let yOffset = 0;
    $.each(topologyObj.body.nodes, function (i, item) {
        if (Math.abs(item.y) > Math.abs(yOffset)) {
            yOffset = item.y;
        }
        if (Math.abs(item.x) > Math.abs(xOffset))
            xOffset = item.x;
    });
    $.each(topologyObj.body.nodes, function (i, item) {
        if (!isNaN(item.id))
            $.post("/assets/cs/_.app.topologies.view.default.aspx", "reqType=moveNodePos&xPos=" + (item.x - xOffset) + "&yPos=" + (item.y - yOffset) + "&nodeId=" + item.id, function (data, textStatus) {
                console.log(data);
                if (data["Status"] != "Completed")
                    toastr["error"](data["Exception"]);
            }, "json");
    })
});
$('#mdl-add-nde-btn-add').click(function () {
    $.post("/assets/cs/_.app.topologies.view.default.aspx", $('#mdl-add-nde-frm').serialize(), function (data, textStatus) {
        if (data["Status"] != "Completed") {
            toastr["error"](data["Exception"]);
        }
        else {
            toastr["success"]("Node has been successfully created.");
            $('#mdl-add-nde').modal("toggle");
            renderTopology(topologyId);
        }
    }, "json");
});
$('#mdl-edt-ttl-btn-sve').click(function () { // When the user clicks "save title" button
    $.post("/assets/cs/_.app.topologies.view.default.aspx", $('#mdl-edt-ttl-frm').serialize(), function (data, textStatus) { // Send data to code behind
        if (data["Status"] != "Completed") // If the end result did not end with "completed;"
            toastr["error"](data["Exception"]); // Show an error alert with the error
        else { // Otherwise
            toastr["success"]("Site name has been updated"); // Show a success alert telling the user it's been updated
            $('#mdl-edt-ttl').modal("toggle"); // Toggle the modal closed
            $('#txt-top-title').html(data["SiteName"]); // Update the title
        }
    }, "json");
});
$('#mdl-edt-dsc-btn-sve').click(function () { // When the user clicks the "save description" button
    $.post("/assets/cs/_.app.topologies.view.default.aspx", $('#mdl-edt-dsc-frm').serialize(), function (data, textStatus) { // Send data to the code behind
        if (data["Status"] != "Completed") // If the end result did not end with "completed;"
            toastr["error"](data["Exception"]); // Show an error alert with the error
        else { // Otherwise
            toastr["success"]("Description has been updated."); // Show a success alert telling the user it's been updates 
            $('#mdl-edt-dsc').modal("toggle"); // Toggle the modal closed
            $('#txt-top-desc').html(data["TopologyDescription"]); // Update the description
        }
    }, "json");
});
$('#mdl-add-nde-icn-ddl').change(function () { // When the selected icon inside add node is changed.
    $.each(netIcons, function (i, item) {
        if ($('#mdl-add-nde-icn-ddl').val() == item.NetIconId)
            $('#mdl-add-nde-icn-img').attr("src", item.IconPath);
    });
});
$.post("/assets/cs/_.app.topologies.view.default.aspx", "reqType=loadNetIcons", function (data, textStatus) { // Download NetIcons
    if (data["Exception"] != null)
        toastr["error"](data["Exception"]);
    else {
        $.each(data, function (i, item) {
            netIcons.push(item);
            $('#mdl-add-nde-icn-ddl').append($('<option>', {
                value: item.NetIconId,
                text: item.IconName
            }));
        });
        $('#mdl-add-nde-icn-ddl').change();
    }
}, "json");