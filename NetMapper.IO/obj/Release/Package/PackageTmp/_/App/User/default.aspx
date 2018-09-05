<%@ Page Title="" Language="C#" MasterPageFile="~/_/NetMapper.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="NetMapper.IO.__.App.User._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="pageTitle" runat="server">
    NetMapper.IO
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="pageHeader" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="pageContentTitle" runat="server">
    User Profile
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="pageContentSubtitle" runat="server">
    View and Edit your User Information
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="pageContent" runat="server">
    <form runat="server">
        <div class="row">
            <div class="col-md-8">
                <div class="panel panel-headline">
                    <div class="panel-heading">
                        <div class="panel-title">Your Details</div>
                    </div>
                    <div class="panel-body">
                        <asp:Panel runat="server" ID="userDetailError" Visible="false">
                            <asp:Literal runat="server" ID="userDetailErrorText"></asp:Literal>
                        </asp:Panel>
                        <div class="col-md-6">
                            <div class="input-group">
                                <span class="input-group-addon">Forename</span>
                                <asp:TextBox runat="server" ID="userForname" CssClass="form-control"></asp:TextBox>
                            </div>
                            <br />
                        </div>
                        <div class="col-md-6">
                            <div class="input-group">
                                <span class="input-group-addon">Surname</span>
                                <asp:TextBox runat="server" ID="userSurname" CssClass="form-control"></asp:TextBox>
                            </div>
                            <br />
                        </div>
                    </div>
                    <div class="panel-footer">
                        <div class="row">
                            <asp:Button runat="server" CssClass="btn btn-primary pull-right" Text="Update Details" ID="updateDetails" OnClick="updateDetails_Click" />
                        </div>
                    </div>
                </div>
                <div class="panel panel-headline">
                    <div class="panel-heading">
                        <div class="panel-title">Your Contacts</div>
                    </div>
                    <div class="panel-body">
                        <table class="table table-condensed">
                            <thead>
                                <tr>
                                    <td class="text-center"><strong>Username</strong></td>
                                    <td class="text-center"><strong>Contact Name</strong></td>
                                    <td class="text-center"><strong>Connected On</strong></td>
                                    <td class="text-center"><strong>Shared Topologies</strong></td>
                                    <td class="text-center"><strong>Common Groups</strong></td>
                                    <td class="text-center"><strong>Actions</strong></td>
                                </tr>
                            </thead>
                        </table>
                        <table class="table table-condensed">
                            <tbody>
                                <tr>
                                    <td class="text-center">You have no relationships at the moment</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="panel panel-headline">
                    <div class="panel-heading">
                        <div class="panel-title">Change Password</div>
                    </div>
                    <div class="panel-body">
                        <asp:Panel runat="server" ID="passwordError" Visible="false">
                            <asp:Literal runat="server" ID="passwordErrorText"></asp:Literal>
                        </asp:Panel>
                        <div class="input-group">
                            <span class="input-group-addon">Old Password</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="oldPassword" placeholder="Password" TextMode="Password"></asp:TextBox>
                        </div>
                        <br />
                        <div class="input-group">
                            <span class="input-group-addon">New Password</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="new1Password" placeholder="Password" TextMode="Password"></asp:TextBox>
                        </div>
                        <br />
                        <div class="input-group">
                            <span class="input-group-addon">Confirm New Password</span>
                            <asp:TextBox runat="server" CssClass="form-control" ID="new2Password" placeholder="Password" TextMode="Password"></asp:TextBox>
                        </div>
                    </div>
                    <div class="panel-footer">
                        <div class="row">
                            <asp:Button runat="server" ID="savePassword" CssClass="btn pull-right btn-primary" Text="Update Password" OnClick="savePassword_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="pageFooter" runat="server">
</asp:Content>
