<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="EchallanStatus.aspx.cs" Inherits="GoaSocietyRegistration.Organization.EchallanStatus" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <style>
        .right{
            float:right;
        }

         .bs-example {
            margin: 20px;
        }

         .slot {
            padding: 8px;
            background-color: #ebb434 !important;
            color: #ffffff;
            font-weight:bold;
        }

        .left{
            float:left;
        }
         #search{
             margin-top:30px;
             margin-left:40px;
         }
         #TxtBxechallanno{
             border-radius: 0rem!important;
         }
         #CheckStatus
         {
             margin:20px!important;
         }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
     <div class="container-fluid">
        <div class="row">
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <div class="col-12">               
                <div class="card shadow">
                    <div class="slot">Get Echallan Status</div>
                    <div class="card-body" runat="server">


                        <div id="search" >

                            <div class="form-group">
                                <div class="input-group-prepend">
                                    <span class="input-group-text" id="basic-addon1" style="padding:0.6rem .75rem!important;border-radius:0rem;">eChallan No</span>
                                </div>
                                <asp:TextBox ID="TxtBxechallanno" aria-describedby="basic-addon1" CssClass="form-control"  ToolTip="Echallan number" placeholder="Echallan No" autocomplete="off" runat="server" Style="margin-right: 20px"></asp:TextBox>
                     
                            </div>
                            <div class="form-group">
                                <asp:LinkButton ID="CheckStatus" OnClick="CheckStatus_Click" CssClass="btn btn-info left " runat="server"><i class="fa fa-search"></i>&nbsp;Get Status</asp:LinkButton>
                                <asp:LinkButton ID="Lkreset" OnClick="Lkreset_Click" CausesValidation="false" CssClass="btn btn-warning right" runat="server"><i class="fa fa-refresh"></i>&nbsp;Reset</asp:LinkButton>

                            </div>
                            <br />
                             <br /> <br /> <br />
                            <div class="form-group">
                        <asp:Label ID="textdisplay" runat="server" Text=""></asp:Label>
                             </div>
                        </div>
                         
                        <br />
                        

                        <asp:Table ID="statustable" CssClass="table-bordered" runat="server" CellPadding="5" Width="50%" Style="margin-left: 55px; margin-top: 15px; text-align: center">
                            <asp:TableHeaderRow Font-Bold="false" BackColor="#c0c0c0" runat="server">
                                <asp:TableHeaderCell Text="PartyName"></asp:TableHeaderCell>
                                <asp:TableHeaderCell Text="Amount"></asp:TableHeaderCell>
                                <asp:TableHeaderCell Text="Status"></asp:TableHeaderCell>
                            </asp:TableHeaderRow>

                            <asp:TableRow>
                                <asp:TableCell ID="echallanr1cf1" runat="server"></asp:TableCell>
                                <asp:TableCell ID="echallanr1c2" runat="server"></asp:TableCell>
                                <asp:TableCell ID="echallanr1c3" runat="server"></asp:TableCell>
                            </asp:TableRow>



                        </asp:Table>
                            
                     
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
                           <asp:Label ID="Label5" runat="server" ForeColor="White"></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="permission" OnClick="permission_Click" CausesValidation="false"  CssClass="btn btn-primary" runat="server" Text="Ok" />
                           <%-- <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="msgmodal" class="modal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label49" runat="server" Text="Alert" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label50" runat="server" ForeColor="red"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
