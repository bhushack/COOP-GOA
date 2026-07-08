<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="ViewProfile.aspx.cs" Inherits="GoaSocietyRegistration.Organization.ViewProfile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
      <script type="text/javascript">
       $(function () {
            $("[id*=TxtBxDobs]").datepicker({
                maxDate: 0,
                showAnim: "",
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
            });
        });  
    </script>
    <style>
        .right {
            float: right;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="container">
        <div class="row">
            <div class="col-12">
                <br />
                <div class="card shadow">
                    <div class="card-body" id="groom" runat="server">
                        <div class="box card-header">
                            <strong>User Profile </strong><br />
                            <asp:Label ID="Label2" runat="server" Text="Your Login ID : "></asp:Label><asp:Label ID="Label1" runat="server" Text=""></asp:Label><asp:HiddenField ID="HiddenField1" runat="server" />
                        </div>
                        <br />
                        <div class="form-group row">
                            <label id="labelFirstName" class="col-sm-3 col-xs-12 form-group">First Name</label>
                            <div class="col-sm-3 col-xs-12">
                                <asp:TextBox ID="TxtBxFirstName" CssClass="form-control form-group" ToolTip="First Name" placeholder="First Name" MaxLength="50" autocomplete="off" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvfname" Display="Dynamic" runat="server" CssClass="text-danger" ControlToValidate="TxtBxFirstName" ErrorMessage="First Name is blank!" SetFocusOnError="True" />
                                <asp:RegularExpressionValidator ID="revfname" runat="server" ControlToValidate="TxtBxFirstName" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\sa-zA-Z]*$" ErrorMessage="No special characters allowed." />
                            </div>
                            <div class="col-sm-3 col-xs-12">
                                <asp:TextBox ID="TxtBxMiddlename" CssClass="form-control form-group" ToolTip="Middle Name" placeholder="Middle Name" MaxLength="50" autocomplete="off" runat="server"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="revmiddlename" runat="server" ControlToValidate="TxtBxMiddlename" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\sa-zA-Z]*$" ErrorMessage="No special characters allowed." />
                            </div>
                            <div class="col-sm-3 col-xs-12">
                                <asp:TextBox ID="TxtBxLastName" CssClass="form-control form-group" ToolTip="Last Name" placeholder="Last Name" MaxLength="50" autocomplete="off" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvlname" Display="Dynamic" runat="server" CssClass="text-danger" ControlToValidate="TxtBxLastName" ErrorMessage="Last Name is blank!" SetFocusOnError="True" />
                                <asp:RegularExpressionValidator ID="revlname" runat="server" ControlToValidate="TxtBxLastName" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\sa-zA-Z ]*$" ErrorMessage="No special characters allowed." />
                            </div>
                        </div>
                        <div class="form-group row">
                        <%--    <label id="labeldateofBirth" class="col-sm-3 col-xs-12">Date of Birth</label>--%>
                           <%-- <div class="col-sm-3 col-xs-12">
                                <asp:TextBox ID="TxtBxDobs" ToolTip="Date of Birth"  placeholder="Date of Birth" autocomplete="off" CssClass="form-control" runat="server"></asp:TextBox>                                
                            </div>--%>
                            <div class="col-sm-3 col-xs-12">
                                <label id="labelbrdmartialstatus">Designation</label>
                            </div>
                            <div class="col-sm-3 col-xs-12">
                                <asp:DropDownList ID="ddldesgination" Enabled="false" CssClass="form-control custom-select mr-sm-2" runat="server"></asp:DropDownList>                                
                            </div>
                        </div>
                        <div class="form-group row">
                            <label id="lbMobileNo" class="col-sm-3 col-xs-12">Mobile No</label>
                            <div class="col-sm-3 col-xs-12">
                                <asp:TextBox ID="TxtBxMobile" ToolTip="Mobile No" MaxLength="10" placeholder="Mobile No" autocomplete="off" CssClass="form-control" runat="server"></asp:TextBox> 
                                 <asp:RegularExpressionValidator ID="rev" runat="server" CssClass="text-danger" Display="Dynamic" ControlToValidate="TxtBxMobile" ErrorMessage="Invalid Mobile No"  ValidationExpression="[0-9]{10}"></asp:RegularExpressionValidator> 
                            <asp:RequiredFieldValidator runat="server" id="reqMobile" CssClass="text-danger" Display="Dynamic" controltovalidate="TxtBxMobile" errormessage="Please enter new Mobile no!" />
                        
                            </div>
                            
                        </div>
                        <asp:Label ID="Lberror" runat="server" ForeColor="Red" Visible="false" Text="Label"></asp:Label>
                        <br />                        
                        <asp:LinkButton ID="btn_edit"  CssClass="btn btn-info" OnClick="btn_edit_Click" CausesValidation="false" Visible="true" runat="server"><i class="fa fa-edit"></i>&nbsp;Edit</asp:LinkButton> 
                        <asp:LinkButton ID="btn_cancel"  CssClass="btn btn-danger" OnClick="btn_cancel_Click" CausesValidation="false" Visible="false" runat="server"><i class="fa fa-window-close"></i>&nbsp;Cancel</asp:LinkButton>                                               
                        <asp:LinkButton ID="btnUpdate"  Visible="false" OnClick="btnUpdate_Click" CssClass="btn btn-info right" runat="server"><i class="fa fa-download"></i>&nbsp;Update</asp:LinkButton>
                        
                    </div>
                </div>
            </div>
        </div>
    </div>

     <div class="bs-example">
        <div id="ConfirmationModal" class="modal fade">
            <div class="modal-dialog ">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label5" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">                           
                            <asp:Label ID="Label8" CssClass="control-label" runat="server" Text="" ForeColor="Black"></asp:Label><br />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="redirect" OnClick="redirect_Click" CssClass="btn btn-success" runat="server" Text="Ok" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
