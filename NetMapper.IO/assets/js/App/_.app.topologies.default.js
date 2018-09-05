$('#btn-create-new').click(function () { $('#mdl-create-new').modal("toggle"); });
$('#mdl-btn-create').click(function () {
    if ($('#mdl-txt-name').val() == "") {
        $('#mdl-txt-name').parent().addClass("has-error");
    }
    else {
        console.log($('#mdl-create-frm').serialize());
        $.post("/assets/cs/_.app.topologies.default.aspx", $('#mdl-create-frm').serialize(), function (data, textStatus) {
            if (data.Status != "Complete") {
                alert("There was an error completing your request.");
                console.log(data);
            }
            else
                document.location = data.Redirect;
        }, "json");
    }
});
$('.btn-del-top').click(function () {
    var postData = "reqType=deleteTopology&topologyId=" + this.id.split("/")[1];
    var parentNode = $(this).parent().parent();
    $.post("/assets/cs/_.app.topologies.default.aspx", postData, function (data, textStatus) {
        if (data.Status != "Complete") {
            alert("There was an error completing your request.");
            console.log(data);
        }
        else
            parentNode.remove();
    }, "json");
});