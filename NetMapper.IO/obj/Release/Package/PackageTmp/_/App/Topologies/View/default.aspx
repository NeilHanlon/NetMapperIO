<%@ Page Title="" Language="C#" MasterPageFile="~/_/NetMapper.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="NetMapper.IO.__.App.Topologies.View._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="pageTitle" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="pageHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageContentTitle" runat="server">
    <span id="txt-top-title"></span>&nbsp;<small><i class="fa fa-edit show-on-edit hidden" style="cursor: pointer;" data-toggle="modal" data-target="#mdl-edt-ttl"></i></small>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="pageContentSubtitle" runat="server">
    <span id="txt-top-desc"></span>&nbsp;<small><i class="fa fa-edit show-on-edit hidden" style="cursor: pointer;" data-toggle="modal" data-target="#mdl-edt-dsc"></i></small>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="pageContent" runat="server">
    <div class="row">
        <div class="col-md-6">
            <div class="col-md-6">
                <table class="table-condensed">
                    <tr>
                        <td><strong>First Created By:</strong></td>
                        <td id="txt-create-by"></td>
                    </tr>
                    <tr>
                        <td><strong>First Created On:</strong></td>
                        <td id="txt-create-on"></td>
                    </tr>
                </table>
            </div>
            <div class="col-md-6">
                <table class="table-condensed">
                    <tr>
                        <td><strong>Lasted Edited By:</strong></td>
                        <td id="txt-edit-by"></td>
                    </tr>
                    <tr>
                        <td><strong>Last Edited On:</strong></td>
                        <td id="txt-edit-on"></td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="col-md-6">
            <a id="btn-del-sel" href="#" class="btn btn-danger pull-right show-on-edit hidden disabled" style="margin-right: 10px;" data-toggle="tooltip" data-placement="top" title="Delete Selected"><i class="fa fa-times"></i></a>
            <a id="btn-edt-sel" href="#" class="btn btn-primary pull-right show-on-edit hidden disabled" style="margin-right: 10px;" data-toggle="tooltip" data-placement="top" title="Edit Selected"><i class="fa fa-pencil"></i></a>
            <a data-toggle="modal" data-target="#mdl-edt-grp" href="#" class="btn btn-info pull-right show-on-edit hidden" style="margin-right: 10px;" data-toggle="tooltip" data-placement="top" title="Edit Groups"><i class="fa fa-object-group"></i></a>
            <a id="btn-edt-pem" data-toggle="modal" data-target="#mdl-edt-pem" href="#" class="btn btn-info pull-right show-on-edit hidden" style="margin-right: 10px;" data-toggle="tooltip" data-placement="top" title="Edit Permissions"><i class="fa fa-users"></i></a>
            <a data-toggle="modal" data-target="#mdl-add-con" href="#" class="btn btn-primary pull-right show-on-edit hidden" style="margin-right: 10px;" data-toggle="tooltip" data-placement="top" title="New Connection"><i class="fa fa-bolt"></i></a>
            <a data-toggle="modal" data-target="#mdl-add-nde" href="#" class="btn btn-primary pull-right show-on-edit hidden" style="margin-right: 10px;" data-toggle="tooltip" data-placement="top" title="New Node"><i class="fa fa-sitemap"></i></a>
            <a id="btn-sve-pos" href="#" class="btn btn-default pull-right show-on-edit hidden disabled" style="margin-right: 10px;" data-toggle="tooltip" data-placement="top" title="Save Positions"><i class="fa fa-save"></i></a>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="pageContentLower">
    <div class="panel panel-headline">
        <div class="panel-heading">
            <div class="panel-body" id="networkTopology" style="height: 60vh;">
                <!-- Container for Network Topology -->
            </div>
        </div>
    </div>

    <!-- ALL PAGE MODALS TO GO BELOW THIS LINE -->

    <!-- Update the topology title modal -->
    <div class="modal fade" id="mdl-edt-ttl" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Change Site Name</h4>
                </div>
                <div class="modal-body">
                    <form id="mdl-edt-ttl-frm">
                        <input type="hidden" class="hidden" name="reqType" value="updateSiteName" />
                        <input type="hidden" class="hidden" name="topologyId" value="<%=Request.QueryString["Id"] %>" />
                        <div class="input-group">
                            <span class="input-group-addon">Site Name: </span>
                            <input type="text" class="form-control" name="siteName" />
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary btn-sm" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary btn-sm" id="mdl-edt-ttl-btn-sve">Save changes</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Update the toplogy description modal -->
    <div class="modal fade" id="mdl-edt-dsc" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Change Site Description</h4>
                </div>
                <div class="modal-body">
                    <form id="mdl-edt-dsc-frm">
                        <input type="hidden" class="hidden" name="reqType" value="updateSiteDesc" />
                        <input type="hidden" class="hidden" name="topologyId" value="<%=Request.QueryString["Id"] %>" />
                        <div class="input-group">
                            <span class="input-group-addon">Description: </span>
                            <textarea rows="3" class="form-control" name="topologyDescription"></textarea>
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary btn-sm" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary btn-sm" id="mdl-edt-dsc-btn-sve">Save changes</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Global Confirmation Modal -->
    <div class="modal fade" id="mdl-cnf" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Confirm to Continue</h4>
                </div>
                <div class="modal-body">
                    <p id="mdl-cnf-msg"></p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary btn-sm" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-warning btn-sm" id="mdl-cnf-btn">Continue</button>
                </div>
            </div>
        </div>
    </div>


    <!-- Create new connection Modal -->
    <div class="modal fade" id="mdl-add-con" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Add a Node</h4>
                </div>
                <div class="modal-body">
                    <form id="mdl-add-con-frm">
                        <input type="hidden" class="hidden" name="reqType" value="addEdge" />
                        <input type="hidden" class="hidden" name="topologyId" value="<%=Request.QueryString["Id"] %>" />
                        <div class="row">
                            <div class="col-md-12">
                                <div class="input-group">
                                    <span class="input-group-addon">Title (Optional): </span>
                                    <input type="text" class="form-control" name="edgeTitle" />
                                </div>
                                <br />
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-addon">From Node: </span>
                                    <select name="fromNodeId" id="mdl-add-con-frm-nde" class="form-control"></select>
                                </div>
                                <br />
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-addon">From Node Port: </span>
                                    <input type="text" class="form-control" name="fromNodePort" id="mdl-add-con-frm-nde-prt" />
                                </div>
                                <br />
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-addon">To Node: </span>
                                    <select name="toNodeId" id="mdl-add-con-to-nde" class="form-control"></select>
                                </div>
                                <br />
                            </div>
                            <div class="col-md-6">
                                <div class="input-group">
                                    <span class="input-group-addon">To Node Port: </span>
                                    <input type="text" class="form-control" name="toNodePort" id="mdl-add-con-to-nde-prt" />
                                </div>
                                <br />
                            </div>
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary btn-sm" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary btn-sm" id="mdl-add-con-btn">Add Connection</button>
                </div>
            </div>
        </div>
    </div>


    <!-- Edit Permissions Modal -->
    <div class="modal fade" id="mdl-edt-pem" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Edit Permissions</h4>
                </div>
                <div class="modal-body">
                    <form id="mdl-edt-pem-frm">
                        <table class="table table-condensed">
                            <thead>
                                <tr>
                                    <td class="text-center"><strong>Username</strong></td>
                                    <td class="text-center"><strong>Permissions</strong></td>
                                    <td></td>
                                </tr>
                            </thead>
                            <tbody id="mdl-edt-pem-tbl">
                                <tr></tr>
                            </tbody>
                            <tfoot>
                                <tr>
                                    <td><input type="text" class="form-control" placeholder="Username" id="txt-add-pem" /></td>
                                    <td><select class="form-control" id="ddl-add-pem"><option value="r">Read Only</option><option value="rw">Read/Write</option></select></td>
                                    <td><button type="button" class="btn btn-success btn-block" id="btn-add-pem"><i class="fa fa-plus"></i></button></td>
                                </tr>
                            </tfoot>
                        </table>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary btn-sm" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>


    <!-- Create new node Modal -->
    <div class="modal fade" id="mdl-add-nde" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Add a Node</h4>
                </div>
                <div class="modal-body">
                    <form id="mdl-add-nde-frm">
                        <div class="text-center">
                            <img id="mdl-add-nde-icn-img" src="#" style="height: 100px;" />
                        </div>
                        <br />
                        <input type="hidden" class="hidden" name="reqType" value="addNodeObject" />
                        <input type="hidden" class="hidden" name="topologyId" value="<%=Request.QueryString["Id"] %>" />
                        <div class="input-group">
                            <span class="input-group-addon">Title: </span>
                            <input type="text" name="nodeTitle" class="form-control" />
                        </div>
                        <br />
                        <div class="input-group">
                            <span class="input-group-addon">Hover Text: </span>
                            <input type="text" name="hoverText" class="form-control" />
                        </div>
                        <br />
                        <div class="input-group">
                            <span class="input-group-addon">Net Icon: </span>
                            <select name="netIcon" class="form-control" id="mdl-add-nde-icn-ddl"></select>
                        </div>
                        <br />
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary btn-sm" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary btn-sm" id="mdl-add-nde-btn-add">Add Node</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="pageFooter" runat="server">
    <script type="text/javascript">
        var topologyId = <%=Request.QueryString["Id"] %>;
        var topologyObj;
        var isOwner = false;
        var hasRead = true;
        var hasWrite = true;
        var runInit = false;
    </script>
    <script type="text/javascript" src="/assets/js/App/_.app.topologies.view.default.renderengine.js"></script>
    <script type="text/javascript" src="/assets/js/App/_.app.topologies.view.default.js"></script>
</asp:Content>
