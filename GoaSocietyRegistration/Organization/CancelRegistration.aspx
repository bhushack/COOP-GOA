<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="CancelRegistration.aspx.cs" Inherits="GoaSocietyRegistration.Organization.CancelRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <style>
        .declaration {
            background-color: #f70b4c !important;
            color: #ffffff;
            padding: 8px;
            font-weight: bold;
        }

        .bs-example {
            margin: 20px;
        }

        .left {
            float: left;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />

    <div class="container-fluid">
        <div class="row">
            <div class="col-lg-12 col-xs-12">
                <asp:HiddenField ID="HiddenField1" runat="server" />
                <div class="declaration">Cancel Registration</div>
                <div class="card-header tab-card-header table-responsive">
                    <div class="row">
                        <div class="form-inline">
                            <div class="form-group mb-2">
                                <label for="staticEmail2" class="sr-only">Enter Registration No</label>
                                <input type="text" readonly class="form-control-plaintext" id="staticEmail2" value="Enter Registration No">
                            </div>
                            <div class="form-group mx-sm-3 mb-2">
                                <label for="inputPassword2" class="sr-only">Registration No</label>
                                <asp:TextBox ID="TxtBxRegNo" CssClass="form-control" placeholder="Registration No." runat="server"></asp:TextBox> 
                            </div>
                            <asp:LinkButton ID="LinkButton1" OnClick="LinkButton1_Click" CssClass="btn btn-primary mb-2" runat="server">Get Details</asp:LinkButton> 
                        </div>
                    </div>
                    <div class="" id="data" runat="server" visible="false">
                        <br /><p><strong><h4>Society Details</h4></strong></p><br />
                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <label for="inputEmail4">Society Name</label>
                                <asp:TextBox ID="TxtBxSocName" CssClass="form-control" Enabled="false" runat="server"></asp:TextBox> 
                            </div>
                            <asp:HiddenField ID="hfAppID" runat="server" />
                            <div class="form-group col-md-6">
                                <label for="inputPassword4">Society Applicant Name</label>
                                <asp:TextBox ID="TxtAppName" Enabled="false" CssClass="form-control" runat="server"></asp:TextBox> 
                            </div>
                        </div><br />
                        <div class="form-row">
                            <div class="form-group col-md-12">
                                <label for="inputEmail4">Remarks (Reason for cancellation)</label>
                                <asp:TextBox ID="txtRemarks" CssClass="form-control" TextMode="MultiLine" Rows="5" runat="server"></asp:TextBox> 
                            </div>
                        </div>
                        <div class="form-row">
                             <asp:LinkButton ID="LinkButton2" OnClick="LinkButton2_Click" CssClass="btn btn-success mb-2" runat="server">Cancel Registartion</asp:LinkButton> 
                             <asp:LinkButton ID="LinkButton3" OnClick="LinkButton3_Click" CssClass="btn btn-danger mb-2" runat="server">Close</asp:LinkButton> 
                        </div>
                    </div>
                </div>
            </div>
        </div>
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
                        <asp:Label ID="Label5" runat="server"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="permission" OnClick="permission_Click"  CssClass="btn btn-primary" runat="server" Text="Ok" />
                        <%-- <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                    </div>
                </div>
            </div>
        </div>
    </div>

     <div class="bs-example">
        <!-- Modal HTML -->
        <div id="msgModal" class="modal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label1" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label2" runat="server" ></asp:Label>
                    </div>
                    <div class="modal-footer">
                         <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="RedModal" class="modal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label3" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label4" runat="server" ></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <a href="CancelRegistration.aspx" class="btn btn-danger">Close</a> 
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="deleteModalAlert" class="modal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label6" runat="server" Text="Alert" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label7" runat="server" ></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="lkDelete" OnClick="lkDelete_Click" CssClass="btn btn-warning" runat="server">Delete Registration</asp:LinkButton>
                         <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
