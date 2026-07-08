<%@ Page Title="" Language="C#" MasterPageFile="~/User/Society.Master" AutoEventWireup="true" CodeBehind="PaidEmployee.aspx.cs" Inherits="GoaSocietyRegistration.User.PaidEmployee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
<%--    <script src="../assets/js/jquery-1.12.4.min.js"></script>
    <script src="../Scripts/popper.min.js"></script>
    <script src="../assets/js/bootstrap-3.3.7.min.js"></script>--%>

    <%-- <script src="../Scripts/jquery-3.5.0.js"></script>
    <script src="../assets/js/jquery.min.js"></script>
    --%>
    <style>
        .bs-example {
            margin: 20px;
        }

        .Grid {
            background-color: #fff;
            margin: 10px 0 10px 0;
            border: solid 2px #525252;
            border-collapse: collapse;
            color: #474747;
            font-size: 15px;
        }

            .Grid td {
                padding: 2px;
                border: solid 2px #716d6d;
            }

            .Grid th {
                padding: 10px 15px;
                color: #fff;
                background: #6365d2;
                border-left: solid 2px #525252;
                font-size: 0.9em;
            }

            .Grid .alt {
                background: #fcfcfc;
            }

            .Grid .pgr {
                background: #363670;
            }

                .Grid .pgr table {
                    margin: 3px 0;
                }

                .Grid .pgr td {
                    border-width: 0;
                    padding: 0 6px;
                    border-left: solid 1px #666;
                    font-weight: bold;
                    color: #fff;
                    line-height: 12px;
                }

                .Grid .pgr a {
                    color: Gray;
                    text-decoration: none;
                }

                    .Grid .pgr a:hover {
                        color: #363670;
                        text-decoration: none;
                    }

        a {
            color: #7460EE;
        }

        .modal-lg {
            max-width: 800px !important;
        }

        .Grid1 {
            margin: 10px 0 10px 0;
            border: solid 2px #525252;
            color: #474747;
            font-size: 15px;
        }

            .Grid1 td {
                padding: 2px;
                border: solid 2px #716d6d;
            }

            .Grid1 th {
                border: solid 2px white;
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
    <script type="text/javascript">
        function openInNewTab() {
            window.document.forms[0].target = '_blank';
            setTimeout(function () { window.document.forms[0].target = ''; }, 0);
        }
    </script>


    <script type="text/javascript">
        function name_changed(val) {

            if (val == "txt_MemName") {

                var mytext = document.getElementById("<%=txt_MemName.ClientID %>").value;
                var newText = mytext.replaceAll('\'', '`');
                document.getElementById("<%=txt_MemName.ClientID %>").value = newText;

            }

        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div class="card" style="width: 100%;">
                    <h4 class="card-header"><strong>Schedule - II Details  ::</strong></h4>
                    <div class="card-body">
                        <div class="form-group">
                            <asp:Label ID="Label6" runat="server" ForeColor="Blue"></asp:Label>
                            <asp:HiddenField ID="HiddenField1" runat="server" />
                        </div>

                        <div class="row">
                            <div class="table-responsive">
                                <div class="table table-bordered table-hover">
                                    <table style="width: 100%; font-size: small">
                                        <thead>
                                            <tr id="oldmembersgridview" runat="server">
                                                <td colspan="6">
                                                    <h5 class="text-center">Statement Relating to persons employed by the Society, their condition of employement, etc, during the year ending 31<sup>st</sup> December.</h5>
                                                    <div class="row justify-content-center" style="text-align: center; width: 100%; margin: auto">
                                                        <asp:GridView ID="gridview_employeeData" runat="server" CellPadding="3" AutoGenerateColumns="false" OnRowDataBound="gridview_employeeData_RowDataBound"
                                                            Font-Size="Small" AlternatingRowStyle-CssClass="alt" PagerStyle-CssClass="pgr" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true">
                                                            <AlternatingRowStyle BackColor="White" />
                                                            <FooterStyle BackColor="#CCCC99" />
                                                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                                            <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                                            <RowStyle BackColor="#F7F7DE" />
                                                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                                            <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                                            <SortedAscendingHeaderStyle BackColor="#848384" />
                                                            <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                                            <SortedDescendingHeaderStyle BackColor="#575357" />
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Sr. No">
                                                                    <ItemTemplate>
                                                                        <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField HeaderText="Name and Designation of the employee" DataField="name_desig" />
                                                                <asp:BoundField HeaderText="Present pay scale" DataField="present_pay_scale" />
                                                                <asp:BoundField HeaderText="Nature of Appointment" DataField="temporary_permanent" />
                                                                <asp:BoundField HeaderText="Present pay per month" DataField="present_pay" />
                                                                <asp:BoundField HeaderText="Dearness allowance per month" DataField="dearness_allowance" />
                                                                <asp:BoundField HeaderText="Special Pay" DataField="special_pay" />
                                                                <asp:BoundField HeaderText="Other Allowances" DataField="other_allowance" />
                                                                <asp:BoundField HeaderText="Provident Fund" DataField="provident_fund" />
                                                                <asp:BoundField HeaderText="Other benefits" DataField="other_benefits" />
                                                                <asp:TemplateField Visible="false">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lbAppid" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                                                        <asp:Label ID="lbEmployeeID" runat="server" Text='<%# Eval("employee_id") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Edit">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="LkEdit" CssClass="btn btn-primary" CausesValidation="false" OnClick="LkEdit_Click" runat="server"><i class="fa fa-edit"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Delete">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="LkDelete" CssClass="btn btn-primary" CausesValidation="false" OnClick="LkDelete_Click" runat="server"><i class="fa fa-trash" aria-hidden="true"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="6" style="text-align: center">
                                                    <asp:Button ID="btnAddEmployee" OnClick="btnAddEmployee_Click" CausesValidation="false" CssClass="btn btn-primary" runat="server" Text="Add Employee Details" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label runat="server" ID="lblTotMembers" CssClass="col-sm-3 col-form-label mylabel" Text="Total number of Employees"></asp:Label></td>
                                                <td colspan="5">
                                                    <asp:TextBox runat="server" ID="txtTotMember" Enabled="false" CssClass="form-control" MaxLength="3"></asp:TextBox></td>
                                            </tr>
                                        </thead>
                                    </table>
                                    <table style="width: 100%">
                                        <tr>
                                            <td><asp:Button ID="btnBack" OnClick="btnBack_Click" CssClass="btn btn-warning"  CausesValidation="false" runat="server" Text="Back" /></td>
                                            <td> <asp:Button ID="Button1" OnClick="Button1_Click" CssClass="btn btn-primary" Style="margin-left: 88%;"  CausesValidation="false" runat="server" Text="Next" /></td>
                                            
                                        </tr>
                                    </table>
                                   
                                    

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="deleteMsgModal" class="modal fade">
            <div class="modal-dialog  modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hdEmployeeID" runat="server" />
                        <asp:Label ID="lblMSG1" runat="server" Text="Please click yes to delete" ForeColor="Green"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="btnConfirm" OnClick="btnConfirm_Click" CssClass="btn btn-primary" CausesValidation="false" runat="server">Yes</asp:LinkButton>
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="AddEmployeeData" class="modal">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Employee Details</h4>
                    </div>
                    <div class="modal-body">
                        <div class="table-responsive">
                            <div class="table table-bordered table-hover">
                                <table style="width: 100%">
                                    <thead>
                                        <asp:HiddenField ID="HfEmployyeIDEDIT" runat="server" />
                                        <tr>
                                            <td class="a">
                                                <asp:Label runat="server" ID="lbl_MemName" Text="Employee Name" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                            </td>
                                            <td class="b">
                                                <asp:TextBox runat="server" ID="txt_MemName" MaxLength="200" CssClass="form-control" AutoCompleteType="Disabled" onkeyup="name_changed('txt_MemName');"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvfullname" Display="Dynamic" runat="server" ControlToValidate="txt_MemName" CssClass="text-danger" ErrorMessage="Enter Full Name"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="revfirstname" runat="server" Display="Dynamic" ControlToValidate="txt_MemName" ForeColor="Red" ValidationExpression="[\sa-zA-Z`]*$" ErrorMessage="Invalid Name" /><br />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="a">
                                                <asp:Label runat="server" ID="Label1" Text="Employee Designation" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                            </td>
                                            <td class="b">
                                                <asp:TextBox runat="server" ID="TxtDesignation" MaxLength="50" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator12" Display="Dynamic" runat="server" ControlToValidate="TxtDesignation" CssClass="text-danger" ErrorMessage="Enter Employee Designation"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" Display="Dynamic" ControlToValidate="TxtDesignation" ForeColor="Red" ValidationExpression="[\sa-zA-Z`]*$" ErrorMessage="Invalid Employee Designation" /><br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_gender" runat="server" Text="Present Payscale" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                            </td>
                                            <td class="b">
                                                <asp:TextBox runat="server" ID="TxtBxPayScale" MaxLength="50" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" Display="Dynamic" runat="server" ControlToValidate="TxtBxPayScale" CssClass="text-danger" ErrorMessage="Enter Employee Payscale"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" Display="Dynamic" ControlToValidate="TxtBxPayScale" ForeColor="Red" ValidationExpression="[\sa-zA-Z`]*$" ErrorMessage="Invalid Employee Payscale" /><br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label runat="server" ID="lbl_age" Text="Temporary or Permanent / full time or part time" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                            </td>
                                            <td class="b">
                                                <asp:TextBox runat="server" ID="txtData" MaxLength="50" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" Display="Dynamic" runat="server" ControlToValidate="txtData" CssClass="text-danger" ErrorMessage="Enter Employee Details"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" Display="Dynamic" ControlToValidate="txtData" ForeColor="Red" ValidationExpression="[\sa-zA-Z`]*$" ErrorMessage="Invalid Employee Details" /><br />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="a">
                                                <asp:Label runat="server" ID="lbl_DesignMem" Text="Present pay per month" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                            </td>
                                            <td class="b">
                                                <asp:TextBox runat="server" ID="TxtPayMonth" MaxLength="10" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" Display="Dynamic" runat="server" ControlToValidate="TxtPayMonth" CssClass="text-danger" ErrorMessage="Enter Employee Present pay per month"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" Display="Dynamic" ControlToValidate="TxtPayMonth" ForeColor="Red" ValidationExpression="[\s0-9`]*$" ErrorMessage="Invalid Employee Present pay per month" /><br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label runat="server" ID="lbl_MemOccup" Text="Dearness Allowance per month" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                            </td>
                                            <td class="b">
                                                <asp:TextBox ID="TxtBxDa" CssClass="form-control" runat="server" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Display="Dynamic" runat="server" ControlToValidate="TxtBxDa" CssClass="text-danger" ErrorMessage="Enter Dearness Allowance"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" ControlToValidate="TxtBxDa" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\s0-9]*$" ErrorMessage="No special characters allowed." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label runat="server" ID="lbl_DesignOthers" Text="Special Pay if any" CssClass="control-label" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td class="b">
                                                <asp:TextBox ID="TxtBxspcialPay" CssClass="form-control" runat="server" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="TxtBxspcialPay" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\s0-9a-zA-Z]*$" ErrorMessage="No special characters allowed." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label runat="server" ID="lbl_OccupOthers" Text="Other Allowances if any(House Rent,Medical Conveyance)" CssClass="control-label" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td class="b">
                                                <asp:TextBox ID="TxtBxotherallowance" CssClass="form-control" runat="server" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="TxtBxotherallowance" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\s0-9a-zA-Z]*$" ErrorMessage="No special characters allowed." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_MemAddress" runat="server" Text="Provident Fund benefits if any" CssClass="control-label" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td class="b">
                                                <asp:TextBox ID="txtprovident" CssClass="form-control" runat="server" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator10" runat="server" ControlToValidate="txtprovident" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\s0-9a-zA-Z]*$" ErrorMessage="No special characters allowed." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_MemDocType" runat="server" Text="Other benefits and amenities provided by the society, if any" CssClass="control-label" Font-Bold="true"></asp:Label>

                                            </td>
                                            <td class="b">
                                                <asp:TextBox ID="txtothers" CssClass="form-control" runat="server" MaxLength="10" AutoCompleteType="Disabled"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator11" runat="server" ControlToValidate="txtothers" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\s0-9a-zA-Z]*$" ErrorMessage="No special characters allowed." />
                                            </td>
                                        </tr>

                                        <%--<tr>
                                            <td>

                                            </td>
                                            <td>                                               
                                                

                                            </td>
                                        </tr>--%>
                                    </thead>
                                    <asp:Label ID="Label2" Visible="false" ForeColor="Red" runat="server" Text="Label"></asp:Label>
                                </table>
                            </div>
                        </div>


                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnAdd" CssClass="btn btn-primary" OnClick="btnAdd_Click" runat="server" Text="Add" />
                        <asp:Button ID="BtnUpdate" Visible="false" CssClass="btn btn-primary" OnClick="BtnUpdate_Click" runat="server" Text="Update" />
                        <asp:Button ID="close_modal" CausesValidation="false" CssClass="btn btn-danger" runat="server" Text="Close" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
