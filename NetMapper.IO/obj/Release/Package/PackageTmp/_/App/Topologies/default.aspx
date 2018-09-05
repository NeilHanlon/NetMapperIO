<%@ Page Title="" Language="C#" MasterPageFile="~/_/NetMapper.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="NetMapper.IO.__.App.Topologies._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="pageTitle" runat="server">
    NetMapper.IO
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="pageHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageContentTitle" runat="server">
    Your Topologies
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="pageContentSubtitle" runat="server">
    View all your topologies
    <a href="#" class="btn btn-default pull-right" id="btn-create-new"><i class="fa fa-bolt"></i>Create New</a>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="pageContent" runat="server">
    <div class="row">
        <div class="col-md-12">
            <table class="table table-condensed">
                <thead>
                    <tr>
                        <td class="text-center"><strong>Site Name</strong></td>
                        <td class="text-center"><strong>Created On</strong></td>
                        <td class="text-center"><strong>Last Edited</strong></td>
                        <td class="text-center"><strong>Permissions</strong></td>
                        <td class="text-center"><strong>Actions</strong></td>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater runat="server" ID="topologyTableRepeater">
                        <ItemTemplate>
                            <tr>
                                <td class="text-center"><%#Eval("SiteName") %></td>
                                <td class="text-center"><%#Eval("CreatedOn") %></td>
                                <td class="text-center"><%#Eval("LastEdited") %></td>
                                <td class="text-center"><%#Eval("Permissions") %></td>
                                <td class="text-center">
                                    <a href="/_/App/Topologies/View/?Id=<%#Eval("topologyId") %>" class="btn btn-sm btn-primary" style="padding-bottom: 2px; padding-top: 2px;">View</a>
                                    <a href="/_/App/Topologies/Dump.aspx?Id=<%#Eval("topologyId") %>" target="_blank" class="btn btn-sm btn-default" style="padding-bottom: 2px; padding-top: 2px;">Dump</a>
                                    <a href="#" class="btn btn-sm btn-danger btn-del-top <%#Eval("deleteBtn") %>" id="top/<%#Eval("topologyId") %>" style="padding-bottom: 2px; padding-top: 2px;">Delete</a>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </div>
    <div class="modal" tabindex="-1" role="dialog" id="mdl-create-new">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title"><strong>Create New Topology</strong></h4>
                </div>
                <div class="modal-body">
                    <form id="mdl-create-frm">
                        <input type="hidden" class="hidden" name="reqType" value="createNew" />
                        <div class="row">
                            <div class="col-md-12">
                                <div class="input-group">
                                    <span class="input-group-addon">Site Name</span>
                                    <input type="text" class="form-control" maxlength="64" id="mdl-txt-name" name="siteName" />
                                </div>

                            </div>
                            <br />
                            <div class="col-md-12">
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon">Description</span>
                                    <textarea rows="4" class="form-control" id="mdl-txt-desc" name="siteDescription"></textarea>
                                </div>
                            </div>
                            <br />
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                    <button type="button" class="btn btn-success" id="mdl-btn-create">Create</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="pageFooter" runat="server">
    <script type="text/javascript" src="/assets/js/App/_.app.topologies.default.js"></script>
</asp:Content>
