$('#create-btn').click(function () {
    $.post("/assets/cs/default.aspx", $('#frm-create').serialize(), function (data, textStatus) {
        if (data["Status"] != "Completed") {
            if (data["InnerException"] != null)
                toastr["error"](data["InnerException"]);
            else
                toastr["error"](data["Exception"]);
        }
        else
            document.location = data["Redirect"];
    }, "json");
});
$('#login-btn').click(function () {
    $('#login-btn').addClass("disabled");
    $.post("/assets/cs/default.aspx", $('#frm-login').serialize(), function (data, textStatus) {
        if (data["Status"] != "Completed") {
            if (data["InnerException"] != null)
                toastr["error"](data["InnerException"]);
            else
                toastr["error"](data["Exception"]);
            $('#login-btn').removeClass("disabled");
        }
        else
            document.location = data["Redirect"];
    }, "json");
});
$('.create-account').click(function () {
    $('#usr-login').addClass("hidden");
    $('#usr-signup').removeClass("hidden");
});
$('.login-account').click(function () {
    $('#usr-login').removeClass("hidden");
    $('#usr-signup').addClass("hidden");
});