<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="GoaSocietyRegistration.Organization.CreateUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <style>
        .right {
            float: right;
        }
    
        .society_list {
            background-color: #6c53dc !important;
            color: #ffffff;
            padding: 8px;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            window.history.pushState(null, "", window.location.href);
            window.onpopstate = function () {
                window.history.pushState(null, "", window.location.href);
            };
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <div class="container-fluid">
        <section class="row">
            <div class="col-sm-12">

                <section class="row">
                    <asp:HiddenField ID="HiddenField1" runat="server" />
                    <div class="col-lg-12 mb-12 bg-default">
                        <div class="society_list">User Details</div>
                        <div class="card">
                            <div class="card-block">
                                <div class="form-group row">
                                    <label class="col-md-3 col-form-label">First Name<span class="text-danger">*</span></label>
                                    <div class="col-md-9">
                                        <asp:TextBox ID="TxtBxFirstname" CssClass="form-control" ToolTip="First Name" placeholder="First Name" autocomplete="off" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-md-3 col-form-label">Middle Name</label>
                                    <div class="col-md-9">
                                        <asp:TextBox ID="TxtBxMiddleName" CssClass="form-control" ToolTip="Middle Name" placeholder="Middle Name" autocomplete="off" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-md-3 col-form-label">Last Name<span class="text-danger">*</span></label>
                                    <div class="col-md-9">
                                        <asp:TextBox ID="TxtbxLastName" CssClass="form-control" ToolTip="Last Name" placeholder="Last Name" autocomplete="off" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-md-3 col-form-label">Email ID / User ID<span class="text-danger">*</span></label>
                                    <div class="col-md-9">
                                        <asp:TextBox ID="TxtBxEmailid" CssClass="form-control" ToolTip="Email ID" placeholder="Email ID" autocomplete="off" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-md-3 col-form-label">Mobile No<span class="text-danger">*</span></label>
                                    <div class="col-md-9">
                                        <asp:TextBox ID="TxtBxMobileNo" CssClass="form-control" ToolTip="Mobile No" MaxLength="10" placeholder="Mobile No" autocomplete="off" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-md-3 col-form-label">User Designation<span class="text-danger">*</span></label>
                                    <div class="col-md-9">
                                        <asp:DropDownList ID="ddlDesignationtype" CssClass="form-control form-group" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                <asp:LinkButton ID="lkbtnsubmit" OnClick="lkbtnsubmit_Click" CssClass="btn btn-success right" runat="server">Submit</asp:LinkButton>
                            </div>
                            <asp:Label ID="label1" Visible="false" CssClass="text-danger" runat="server" Text="Error::"></asp:Label><asp:Label ID="lberror" runat="server" Text=""></asp:Label>
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
