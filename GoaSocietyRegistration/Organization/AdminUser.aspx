<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="AdminUser.aspx.cs" Inherits="GoaSocietyRegistration.Organization.AdminUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
     <style>
        .bs-example {
            margin: 20px;
        }

        .adminuser {
            background-color: #6c53dc !important;
            color: #ffffff;
            padding: 8px;
        }
        .mybtn{
            background-color:#E8E8E8;
            color:#000;
        }
        /*.btn,.btn:hover{
             color:#fff;
         }*/
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <br />
    <div class="container-fluid">
        <div class="row">
            <div class="col-12">
                <div class="adminuser">User Sheet</div>
                <div style="border: solid 1px #ddd">
                    <div class="card-header tab-card-header">
                        <ul class="nav nav-tabs card-header-tabs" id="myTab" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link" id="one-tab" data-toggle="tab" href="#one" role="tab" aria-controls="One" aria-selected="true">All User (<asp:Label ID="Label1" runat="server" Text=""></asp:Label>)</a>
                            </li>
                        </ul>
                    </div>
                    <div class="tab-content table-responsive" id="myTabContent">
                        <asp:HiddenField ID="HiddenField1" runat="server" />
                        <div class="tab-pane fade show active p-3 text-center" id="one" role="tabpanel" aria-labelledby="one-tab">
                            <asp:GridView ID="GridViewUser" runat="server"  PageSize="10" AllowPaging="true" OnRowDataBound="GridViewUser_RowDataBound" OnPageIndexChanging="GridViewUser_PageIndexChanging"  CellPadding="5" AutoGenerateColumns="false" CssClass="Grid" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%">
                                <%--<AlternatingRowStyle BackColor="White" />
                                <FooterStyle BackColor="#CCCC99" />
                                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                <RowStyle BackColor="#c8caf1" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                <SortedAscendingHeaderStyle BackColor="#848384" />
                                <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                <SortedDescendingHeaderStyle BackColor="#575357" />--%>
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr. No">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LbUsername" runat="server" Text='<%# Eval("user_name") %>'></asp:Label>
                                            <asp:Label ID="LbLoginName" runat="server" Text='<%# Eval("username") %>'></asp:Label>
                                            <asp:Label ID="LbDesignation" runat="server" Text='<%# Eval("user_designation") %>'></asp:Label>
                                            <asp:Label ID="LbMobiles" runat="server" Text='<%# Eval("usermobileno") %>'></asp:Label>
                                            <asp:Label ID="Lbactive" runat="server" Text='<%# Eval("active") %>'></asp:Label>
                                            <asp:Label ID="LbTaluka" runat="server" Text='<%# Eval("district_id") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="User Name" DataField="user_name" />
                                    <asp:BoundField HeaderText="User Login ID" DataField="username" />
                                    <asp:BoundField HeaderText="Designation" DataField="user_designation" />
                                    <asp:BoundField HeaderText="Mobile No." DataField="usermobileno" />
                                    <asp:TemplateField HeaderText="Update Mobile">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LbMobile" OnClick="LbMobile_Click" CssClass="btn mybtn"  CausesValidation="false" runat="server"><i class="fa fa-phone" aria-hidden="true"></i>&nbsp;Update</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Activate Account">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LbActivate" OnClick="LbActivate_Click" CssClass="btn btn-success" Enabled="false" CausesValidation="false" runat="server"><i class="fa fa-check" aria-hidden="true"></i>&nbsp;</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Deactivate Account">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LbDeactivate" OnClick="LbDeactivate_Click"  Enabled="false" CssClass="btn btn-danger" CausesValidation="false" runat="server"><i class="fa fa-exclamation-triangle" aria-hidden="true"></i>&nbsp;</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Reset Password">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LbResetPassword" OnClick="LbResetPassword_Click"  CssClass="btn btn-warning" CausesValidation="false" runat="server"><i class="fa fa-refresh" aria-hidden="true"></i>&nbsp; </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Status" Visible="false" DataField="" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

     <div class="bs-example">
        <div id="UpdateMobile" class="modal fade">
            <div class="modal-dialog ">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label18" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <asp:Label ID="Label19" runat="server" Text="User ID : "></asp:Label><asp:Label ID="Label20" runat="server" Text=""></asp:Label><br />
                            <asp:Label ID="Label22" runat="server" Text="Old Mobile No : "></asp:Label><asp:Label ID="Label23" runat="server" Text=""></asp:Label><br />
                            <asp:Label ID="Label21" CssClass="control-label" runat="server" Font-Bold="true" Text="Kindly click confirm to Update the mobile no." ForeColor="Black"></asp:Label><br />
                            <asp:Label ID="Label24" runat="server"  Text="New Mobile No : "></asp:Label><asp:TextBox ID="TxtBxNewMobile" AutoCompleteType="Disabled" MaxLength="10" runat="server"></asp:TextBox><br />
                            <asp:RegularExpressionValidator ID="rev" runat="server" CssClass="text-danger" Display="Dynamic" ControlToValidate="TxtBxNewMobile" ErrorMessage="Invalid Mobile No"  ValidationExpression="[0-9]{10}"></asp:RegularExpressionValidator> 
                            <asp:RequiredFieldValidator runat="server" id="reqMobile" CssClass="text-danger" Display="Dynamic" controltovalidate="TxtBxNewMobile" errormessage="Please enter new Mobile no!" />
                        </div>
                        <asp:Label ID="error" Visible="false" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="LkUpdate" OnClick="LkUpdate_Click"  CssClass="btn btn-success" runat="server">Confirm</asp:LinkButton>
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>



    <div class="bs-example">
        <div id="ModalActivate" class="modal fade">
            <div class="modal-dialog ">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label12" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <asp:Label ID="Label2" runat="server" Text="User ID : "></asp:Label><asp:Label ID="Label3" runat="server" Text="Label"></asp:Label><br />
                            <asp:Label ID="Label4" CssClass="control-label" runat="server" Text="Kindly click confirm to ACTIVATE the account." ForeColor="Black"></asp:Label><br />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="LkConfirm"  OnClick="LkConfirm_Click" CssClass="btn btn-success" CausesValidation="false" runat="server">Confirm</asp:LinkButton>
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="bs-example">
        <div id="ModalDeactivate" class="modal fade">
            <div class="modal-dialog ">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label9" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <asp:Label ID="Label10" runat="server" Text="User ID : "></asp:Label><asp:Label ID="Label11" runat="server" Text=""></asp:Label><br />
                            <asp:Label ID="Label13" CssClass="control-label" runat="server" Text="Kindly click confirm to DEACTIVATE the account." ForeColor="Red"></asp:Label><br />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="LkconfirmDeactivate" OnClick="LkconfirmDeactivate_Click"  CssClass="btn btn-success" CausesValidation="false" runat="server">Confirm</asp:LinkButton>
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

     <div class="bs-example">
        <div id="ModalReset" class="modal fade">
            <div class="modal-dialog ">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label14" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <asp:Label ID="Label15" runat="server" Text="User ID : "></asp:Label><asp:Label ID="Label16" runat="server" Text=""></asp:Label><br />
                            <asp:Label ID="Label17" CssClass="control-label" runat="server" Text="Kindly click confirm to Reset password of the account." ForeColor="Red"></asp:Label><br />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="LkResetPassword" OnClick="LkResetPassword_Click"  CssClass="btn btn-success" CausesValidation="false" runat="server">Confirm</asp:LinkButton>
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
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
                            <asp:Label ID="Label6" runat="server" Text="User ID : "></asp:Label><asp:Label ID="Label7" runat="server" Text="Label"></asp:Label><br />
                            <asp:Label ID="Label8" CssClass="control-label" runat="server" Text="" ForeColor="Black"></asp:Label><br />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
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
                                <asp:Label ID="Label25" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                           <asp:Label ID="Label26" runat="server" ForeColor="White"></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="permission" OnClick="permission_Click"  CausesValidation="false"  CssClass="btn btn-primary" runat="server" Text="Ok" />
                         
                        </div>
                    </div>
                </div>
            </div>
        </div>
</asp:Content>
