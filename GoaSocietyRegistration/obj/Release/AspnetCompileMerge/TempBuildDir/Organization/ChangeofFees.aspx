<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="ChangeofFees.aspx.cs" Inherits="GoaSocietyRegistration.Organization.ChangeofFees" %>
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
            background-color: #6c53dc !important;
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

         .a{
             width:30%;
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
                    <div class="slot">Change Fees</div>
                    <div class="card-body" runat="server">


                        <div id="search" >

                             <div class="form-group">
                                <div class="input-group-prepend">
                                    <span class="input-group-text" id="basic-addon1" style="padding: 0.6rem .75rem!important; border-radius: 0rem;">Application Id</span>
                                    <asp:TextBox ID="TxtBxAppID" aria-describedby="basic-addon1" CssClass="form-control" ToolTip="Application number" placeholder="Application No" autocomplete="off" runat="server" Style="margin-right: 20px"></asp:TextBox>
                                </div>

                            </div>
                            <div id="buttons" runat="server">
                                <asp:LinkButton ID="LkSearch" OnClick="LkSearch_Click" CssClass="btn btn-info" runat="server"><i class="fa fa-search"></i>&nbsp;Search</asp:LinkButton>
                                <asp:LinkButton ID="LkClear" OnClick="LkClear_Click" CssClass="btn btn-danger" runat="server"><i class="fa fa-refresh"></i>&nbsp;Clear</asp:LinkButton>

                            </div>  
                            
                               <br />

                    <div id="showdata" runat="server" visible="false">
                         <table id="feestable" style="width:100%" class="table table-borderless">
                                <tbody>                                                        
                                    <tr>
                                        <td class="a"><asp:Label ID="Label28" runat="server" class="col-sm-4 col-form-label">Registration Date</asp:Label><span class="text-danger">*</span></td>
                                        <td><asp:TextBox ID="TxtBxRegistrationDate" CssClass="ml-3" type="date" Enabled="false" runat="server" AutoCompleteType="Disabled"></asp:TextBox></td>
                                        </tr>
                                    <tr>
                                        <td  class="a"><asp:Label ID="Label29" runat="server" class="col-sm-4 col-form-label">Last Date for Renewal</asp:Label><span class="text-danger">*</span></td>
                                        <td><asp:TextBox ID="TxtBxRenewalDate" Enabled="false" type="date" CssClass="ml-3" runat="server"  AutoCompleteType="Disabled"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                        <td  class="a"><asp:Label ID="Label30" runat="server" class="col-sm-4 col-form-label">Due Date</asp:Label><span class="text-danger">*</span></td>
                                        <td><asp:TextBox ID="TxtbxDueDate" Enabled="false" type="date" CssClass="ml-3" runat="server" AutoCompleteType="Disabled"></asp:TextBox></td>
                                        </tr>
                                    <tr>
                                        <td  class="a"><asp:Label ID="Label27" runat="server" class="col-sm-4 col-form-label">Processing Fee</asp:Label><span class="text-danger">*</span></td>
                                        <td><asp:Label ID="Label36" runat="server" Text="₹ "></asp:Label><asp:TextBox ID="TxtBxProcessFee" Enabled="false" runat="server" Text="75"  AutoCompleteType="Disabled"></asp:TextBox></td>
                                        </tr>
                                    <tr>
                                        <td  class="a"><asp:Label ID="Label31" runat="server" class="col-sm-4 col-form-label">Penalty</asp:Label><span class="text-danger">*</span></td>
                                        <td><asp:Label ID="Label35" runat="server" Text="₹ "></asp:Label><asp:TextBox ID="TxtBxPenalty" Enabled="false" runat="server" Text=""  AutoCompleteType="Disabled"></asp:TextBox></td>
                                        </tr>

                                                         
                                        <tr>
                                        <td class="a"><asp:Label ID="Label32" runat="server" class="col-sm-3 col-form-label">Total Fees to be paid</asp:Label><span class="text-danger">*</span></td>
                                        <td><asp:Label ID="Label38" runat="server" Text="₹ "></asp:Label><asp:TextBox ID="txtbxtotalfees" Enabled="false" runat="server" Text=""  AutoCompleteType="Disabled"></asp:TextBox>
                                            <asp:LinkButton ID="LkCalcFees" Visible="false" OnClick="LkCalcFees_Click" runat="server" CssClass="btn btn-outline-info ml-3" Text="Calculate Fees"></asp:LinkButton>
                                            <asp:HiddenField ID="hffees" runat="server" />


                                        </td>
                                        </tr>
                                        <tr>
                                        <td class="a"><asp:Label ID="Label33" runat="server" class="col-sm-3 col-form-label">Fees Remarks</asp:Label><span class="text-danger">*</span></td>
                                            <td>
                                            <textarea id="txtareafeesremarks" disabled="disabled" name="feesremarks" class="ml-3" maxlength="500" runat="server" rows="3" style="width: 100%"></textarea>
                                        </td>
                                                           
                                        </tr>
                                    <tr>
                                                           
                                    </tr>
                                    <tr style="text-align:center">
                                        <td colspan="2">
                                            <asp:LinkButton ID="lkcancelchanges" Visible="false" OnClick="lkcancelchanges_Click" CssClass="btn btn-warning" runat="server" CausesValidation="false"><i class="fa fa-times">Cancel Changes</asp:LinkButton>
                                            <asp:LinkButton ID="lkeditfees" OnClick="lkeditfees_Click" CssClass="btn btn-info" runat="server" CausesValidation="false">Edit</asp:LinkButton>
                                                <asp:LinkButton ID="lkSaveFees" OnClick="lkSaveFees_Click" CssClass="btn btn-primary" Visible="false" runat="server"><span class="btn-label"><i class="fa fa-save"></i></span>&nbsp Save Changes</asp:LinkButton>
                                        </td>
                                                           
                                        </tr>
                                </tbody>
                            </table>
                    </div>                  
                          
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
                           <asp:Label ID="Label8" runat="server" ForeColor="White"></asp:Label>
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
            <div id="feesconfirmation" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label39" runat="server" Text="Payment" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label40" runat="server" Text="Are you sure to Save Changes in Fees? If yes click on Confirm"></asp:Label>
                            </div>
                        <asp:Label ID="Label41" CssClass="alert-danger" runat="server" Text=""></asp:Label>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-primary left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="LkSaveFees_modalbtn" OnClick="LkSaveFees_modalbtn_Click"  CssClass="right btn btn-success" runat="server">Confirm</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>


      <div class="bs-example">
            <!-- Modal HTML -->
            <div id="redirectModal" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label64" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label65" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnRedirect" OnClick="btnRedirect_Click" CssClass="btn btn-primary" runat="server" Text="OK" />
                     <%--       <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                        </div>
                    </div>
                </div>
            </div>
        </div>


     <div class="bs-example">
            <!-- Modal HTML -->
            <div id="MyErrorModal" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label66" runat="server" Text="Error!!" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label69" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

</asp:Content>
