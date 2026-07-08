<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="DisableLogin.aspx.cs" Inherits="GoaSocietyRegistration.Organization.DisableLogin" %>

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
    </style>
    <style>
        .society_list {
            background-color: #6c53dc !important;
            color: #ffffff;
            padding: 8px;
        }

        #customer {
            font-family: Arial, Helvetica, sans-serif;
            border-collapse: collapse;
            width: 100%;
        }

            #customer td, #customer th {
                border: 1px solid #ddd;
                padding: 8px;
            }

            #customer tr:nth-child(even) {
                background-color: #f2f2f2;
            }

            #customer tr:hover {
                background-color: #ddd;
            }

            #customer th {
                padding-top: 12px;
                padding-bottom: 12px;
                text-align: left;
                background-color: #04AA6D;
                color: white;
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
                        <div class="society_list">Disable Login</div>
                        <div class="card">
                            <div class="card-block">
                                <div class="form-group row">
                                    <label class="col-md-3 col-form-label">Login ID<span class="text-danger">*</span></label>
                                    <div class="col-md-3">
                                        <asp:TextBox ID="LkLogin" CssClass="form-control" ToolTip="Login ID" placeholder="Login ID" autocomplete="off" runat="server"></asp:TextBox>
                                    </div>
                                    <div class="col-md-3">
                                        <asp:LinkButton ID="Lkgetinfo" CssClass="btn btn-info" OnClick="Lkgetinfo_Click" runat="server">Get Info</asp:LinkButton>
                                    </div>
                                </div>
                                <br />
                                <div class="form-group row" id="data" runat="server" visible="false">
                                    <div class="table-responsive" id="customer">
                                        <table style="width: 100%">
                                            <tr>
                                                <th>Applicant Name</th>
                                                <th>Scoiety Name</th>
                                                <th>Scoeity Address</th>
                                                <th>Society Type</th>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label4" runat="server" Text="Label"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <br />
                                    <br />
                                    <div class="form-group row">
                                        <div class="col-md-3">
                                            <asp:LinkButton ID="lkdisable" CssClass="btn btn-info" OnClick="lkdisable_Click" runat="server">Disable Login</asp:LinkButton>
                                        </div>
                                    </div>
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
                           <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

</asp:Content>
