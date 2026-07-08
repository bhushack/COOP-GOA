<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="ChangeMobile.aspx.cs" Inherits="GoaSocietyRegistration.Organization.ChangeMobile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <style>
        .society_list {
            background-color: #6c53dc !important;
            color: #ffffff;
            padding: 8px;
        } .right {
            float: right;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <div class="container-fluid">
        <section class="row">
            <div class="col-sm-12">
                <section class="row">
                    <asp:HiddenField ID="HiddenField1" runat="server" />
                    <div class="col-lg-12 mb-12 bg-default">
                        <div class="society_list">Change Mobile Number</div>
                        <div class="card">
                            <div class="card-block">
                                <div class="form-group row">
                                    <label class="col-md-3 col-form-label">Enter Token Number<span class="text-danger">*</span></label>
                                    <div class="col-md-9">
                                        <asp:TextBox ID="TxtBxLoginNumber" CssClass="form-control" ToolTip="Token Number" placeholder="Token Number" autocomplete="off" runat="server"></asp:TextBox>
                                    </div>
                                    <asp:LinkButton ID="LkSearch" OnClick="LkSearch_Click" CssClass="btn btn-success right" runat="server">Search</asp:LinkButton>
                                </div>
                                <asp:Label ID="label1" Visible="false" CssClass="text-danger" runat="server" Text="Error::"></asp:Label><asp:Label ID="lberror" Visible="false" CssClass="text-danger" runat="server" Text=""></asp:Label>
                                <div class="row " id="showData" runat="server" visible="false">
                                    <div class="form-group row">
                                        <label class="col-md-6 col-form-label">Applicant Name<span class="text-danger">*</span></label>
                                        <div class="col-md-6">
                                            <asp:TextBox ID="TxtBxAppname" Enabled="false" CssClass="form-control" ToolTip="Applicant Name" placeholder="Applicant Name" autocomplete="off" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group row">
                                        <label class="col-md-6 col-form-label">Old Number</label>
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtBxOldNumber" Enabled="false" CssClass="form-control" ToolTip="Old Number" placeholder="Old Number" autocomplete="off" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group row ">
                                        <label class="col-md-6 col-form-label">New Number<span class="text-danger">*</span></label>
                                        <div class="col-md-6">
                                            <asp:TextBox ID="TxtbxNewNumber" CssClass="form-control" ToolTip="New Number" placeholder="New Number" autocomplete="off" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <asp:LinkButton ID="lkUpdate" OnClick="lkUpdate_Click" CssClass="btn btn-primary right" runat="server">Update Mobile</asp:LinkButton>
                              
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
            </div>
        </section>
    </div>
    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="permission_error_modal" class="modal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label19" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label5" runat="server" ForeColor="White"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="permission" OnClick="permission_Click" CssClass="btn btn-primary" runat="server" Text="Ok" />
                        <%-- <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
