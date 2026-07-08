<%@ Page Title="" Language="C#" MasterPageFile="~/User/Society.Master" AutoEventWireup="true" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" CodeBehind="ViewApplicantDetails.aspx.cs" Inherits="GoaSocietyRegistration.ViewApplicantDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <script src="../Scripts/jquery-3.5.0.js"></script>
    <style type="text/css">
        .uppercase {
            text-transform: uppercase;
        }

        .dis {
            cursor: not-allowed;
        }

        .a {
            width: 30%;
        }

        .b {
            width: 70%;
        }

        .modal-lg {
            max-width: 950px !important;
        }
    </style>
    <script type="text/javascript">
        function openInNewTab() {
            window.document.forms[0].target = '_blank';
            setTimeout(function () { window.document.forms[0].target = ''; }, 0);
        }
    </script>
    <%-- <script type="text/javascript">
        window.onload = function () {
            noBack();
        }
        function noBack() {
            window.history.forward();
        }
    </script>--%>
    
    <script type="text/javascript">
        function noBack() {
            window.history.forward();
        }
        noBack();
        window.onload = noBack;
        window.onpageshow = function (evt) {
            if (evt.persisted)
                noBack();
        };
        window.onunload = function () {
            void (0);
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
       
        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div class="card" style="width: 100%"><br />
                     <header align="center"><h3>Society Registration Form</h3></header>
                    <div class="modal-body">
                        <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                            <div class="panel-title" style="color: white; font-weight: bold;">Applicant Details</div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-md-6 col-xs-12">
                                <label class="control-label" for="street">Name of The Applicant  <span style="color: red">*</span></label>
                                <asp:TextBox ID="ViewAppName" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>
                                <asp:HiddenField ID="HiddenField1" runat="server" />

                            </div>
                            <div class="col-md-6 col-xs-12">
                                <label class="control-label" for="street">Designation <span style="color: red">*</span></label>
                                <asp:TextBox ID="appdesignation" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                            </div>

                        </div>
                        <br />
                        <div class="row">
                            <div class="col-md-12 col-xs-12">
                                <label class="control-label" for="street">Address: </label>
                                <asp:TextBox ID="appaddress" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-md-4 col-xs-12">
                                <label class="control-label" for="street">District:<span style="color: red">*</span></label>
                                <asp:TextBox ID="appdistrict" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                            </div>
                            <div class="col-md-4 col-xs-12 ">
                                <label class="control-label" for="street">Mobile No:<span style="color: red"></span></label>
                                <asp:TextBox ID="appmobileno" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-4 col-xs-12 ">
                                <label class="control-label" for="street">Email:<span style="color: red"></span></label>
                                <asp:TextBox ID="appemail" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                            </div>

                        </div>
                        <br />
                        <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                            <div class="panel-title" style="color: white; font-weight: bold;">Society Details</div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-md-6 col-xs-12">
                                <label class="control-label" for="street">Society Type:<span style="color: red">*</span></label>
                                <asp:TextBox ID="value_society_type" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>


                            </div>
                            <div class="col-md-6 col-xs-12">
                                <label class="control-label" for="street">Society Name:<span style="color: red">*</span></label>
                                <asp:TextBox ID="value_society_name" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                            </div>

                        </div>
                        <br />
                        <div class="row">
                            <div class="col-md-12 col-xs-12">
                                <label class="control-label" for="street">Society Address:</label>
                                <asp:TextBox ID="value_society_address" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-md-6 col-xs-12">
                                <label class="control-label" for="street">District: <span style="color: red">*</span></label>
                                <asp:TextBox ID="value_society_district" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>


                            </div>
                            <div class="col-md-6 col-xs-12">
                                <label class="control-label" for="street">Taluka:<span style="color: red">*</span></label>
                                <asp:TextBox ID="value_taluka" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                            </div>

                        </div>
                        <br />
                        <div class="row">
                            <div class="col-md-4 col-xs-12">
                                <label class="control-label" for="street">Registration Fee:<span style="color: red">*</span></label>
                                <asp:TextBox ID="value_registration_fee" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                            </div>
                            <div class="col-md-4 col-xs-12 ">
                                <label class="control-label" for="street">Proccessing Fee<span style="color: red">*</span></label>
                                <asp:TextBox ID="value_processing_fee" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-4 col-xs-12 ">
                                <label class="control-label" for="street">Total Fee to be paid<span style="color: red">*</span></label>
                                <asp:TextBox ID="value_total_fee" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                            </div>

                        </div>
                        <br />
                        <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                            <div class="panel-title" style="color: white; font-weight: bold;">Managing Committee/Member Details</div>
                        </div>
                        
                             
                        <div class="table-responsive table-bordered " style="text-align: center; width: 100%; margin: auto">
                            <asp:GridView runat="server" ID="grvMemberDetails" HeaderStyle-CssClass="header" RowStyle-CssClass="rows" BackColor="White" BorderColor="#DEDFDE"
                                BorderStyle="None" BorderWidth="2px" CellPadding="4" ForeColor="Black" GridLines="Horizontal" AutoGenerateColumns="false"
                                CssClass="mygrdContent rows header pager" AllowPaging="false" Width="100%">
                                <AlternatingRowStyle BackColor="White" />
                                <HeaderStyle BackColor="#d8ddf5" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr. No." ItemStyle-Width="70">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Name" DataField="fname" />
                                    <asp:BoundField HeaderText="Designation" DataField="designtaion" />
                                    <asp:BoundField HeaderText="Occupation" DataField="occupation" />
                                    <asp:BoundField HeaderText="Address" DataField="address" />
                                    <asp:BoundField HeaderText="ID Proof" DataField="proofname" />
                                    <asp:BoundField HeaderText="Managing Committee" DataField="mangcomm" />
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton3" runat="server" OnCommand="ImageButton3_Command"  ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" />
                                            <asp:HiddenField ID="hfdocid" Value='<%# Eval("doc_id") %>' runat="server" />
                                            <asp:HiddenField ID="hfdmongodoc" Value='<%# Eval("document_mongoentry") %>' runat="server" />
                                            <asp:HiddenField ID="hfmemberid" Value='<%# Eval("member_id") %>' runat="server" />
                                            <asp:HiddenField ID="hdnPDF" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                   
                                </Columns>
                            </asp:GridView>
                        </div>
                       
                        <br />
                        <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                            <div class="panel-title" style="color: white; font-weight: bold;">Total Members</div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-xs-12">
                                <label class="control-label" for="street">Managing Committee/Members:<span style="color: red">*</span></label>
                                <asp:TextBox ID="value_total_members" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <br />
                         <div class="form-inline row">
                        <div class="col-md-6 col-xs-6">
                            <label class="control-label" style="justify-content:flex-start">Members List:</label>
                            
                        </div>
                        <div class="col-md-6 col-xs-6">
                            
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton10_Click" />

                        </div>
                    </div>
                        <div>
                             <asp:Label ID="listexist" runat="server" Text=""  style="color:red"></asp:Label>
                        </div>
                        <br />
                        <div id="div_employee" runat="server" visible="false">
                        <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                            <div class="panel-title" style="color: white; font-weight: bold;">Employee Details</div>
                        </div>
                        
                             
                        <div class="table-responsive table-bordered " style="text-align: center; width: 100%; margin: auto">
                            <asp:GridView runat="server" ID="gv_employee" HeaderStyle-CssClass="header" RowStyle-CssClass="rows" BackColor="White" BorderColor="#DEDFDE"
                                BorderStyle="None" BorderWidth="2px" CellPadding="4" ForeColor="Black" GridLines="Horizontal" AutoGenerateColumns="false"
                                CssClass="mygrdContent rows header pager" AllowPaging="false" Width="100%">
                                <AlternatingRowStyle BackColor="White" />
                                <HeaderStyle BackColor="#d8ddf5" />
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
                                     <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton_employee" runat="server" OnCommand="ImageButton_employee_Command"  ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" />                                            
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                       
                                       
                                    </Columns>

                            </asp:GridView>
                        </div>

                            <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                            <div class="panel-title" style="color: white; font-weight: bold;">Total Employees</div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-xs-12">
                                <label class="control-label" for="street">Total Paid Employees</label>
                                <asp:TextBox ID="value_total_employee" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                            </div>
                       


                        <br />
                        <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                            <div class="panel-title" style="color: white; font-weight: bold;">Document Uploaded</div>
                        </div>
                       
                        <div class="row" id="documents" runat="server">
                            <div class="table-responsive" style="margin:0px 15px;">
                                <div class="table table-bordered table-hover">
                                    <table style="width: 100%">
                                        <thead>
                                            <tr>
                                                <th>
                                                    <asp:Label ID="lbsrno" runat="server" Text="Sr. No."></asp:Label>
                                                </th>
                                                <th>
                                                    <asp:Label ID="lbheader" runat="server" Text="Name of Document"></asp:Label>
                                                </th>

                                                <th>
                                                    <asp:Label ID="lbStatus" runat="server" Text="View"></asp:Label>
                                                </th>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lb1" runat="server" Text="1"></asp:Label>
                                                </td>
                                                <td>
                                                    <label class="control-label" for="street">Application for Registration<span style="color: red">*</span></label>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="../assets/images/pdf.png" OnClick="ImageButton7_Click" Width="30px" Height="30px"/>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label1" runat="server" Text="2"></asp:Label>
                                                </td>
                                                <td>
                                                    <label class="control-label" for="street">Memorandum of Association<span style="color: red">*</span></label>

                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton7_Click" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label2" runat="server" Text="3"></asp:Label>
                                                </td>
                                                <td>
                                                    <label class="control-label" for="street">Rules And Regulation<span style="color: red">*</span></label>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton7_Click"/>

                                                </td>
                                            </tr>
                                    <%--        <tr>
                                                <td>
                                                    <asp:Label ID="Label3" runat="server" Text="4"></asp:Label>
                                                </td>
                                                <td>
                                                    <label class="control-label" for="street">Certificate<span style="color: red">*</span></label>
                                                </td>
                                                <td>

                                                    <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton7_Click" />
                                                </td>
                                            </tr>--%>
                                            <tr id="docone" runat="server">   
                                                  <td>
                                                    <asp:Label ID="Label4" runat="server" Text="4"></asp:Label>
                                                </td>                                           
                                                <td>
                                                    <asp:Label ID="valsocietyotherdoc1" runat="server" ></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton7_Click" />
                                                </td>
                                            </tr>
                                            <tr id="doctwo" runat="server">
                                                  <td>
                                                    <asp:Label ID="Label5" runat="server" Text="5"></asp:Label>
                                                </td>
                                                
                                                <td>
                                                      <asp:Label ID="valsocietyotherdoc2" runat="server" ></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton7_Click" />
                                                </td>
                                            </tr>
                                        </thead>
                                    </table>
                                </div>
                            </div>
                        </div>

                         <div class="row" id="Documents_Renewal" runat="server">
                            <div class="table-responsive" style="margin:0px 15px;" >
                                <div class="table table-bordered table-hover">
                                    <table style="width: 100%">
                                        <thead>
                                            <tr>
                                                <th>
                                                    <asp:Label ID="Label3" runat="server" Text="Sr. No."></asp:Label>
                                                </th>
                                                <th>
                                                    <asp:Label ID="Label6" runat="server" Text="Name of Document"></asp:Label>
                                                </th>

                                                <th>
                                                    <asp:Label ID="Label7" runat="server" Text="View"></asp:Label>
                                                </th>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label8" runat="server" Text="1"></asp:Label>
                                                </td>
                                                <td>
                                                    <label class="control-label" for="street">Application for Renewal<span style="color: red">*</span></label>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="../assets/images/pdf.png" OnClick="ImageButton7_Click" Width="30px" Height="30px"/>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label9" runat="server" Text="2"></asp:Label>
                                                </td>
                                                <td>
                                                    <label class="control-label" for="street">Schedule I/ Managing Committee Details<span style="color: red">*</span></label>

                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton7_Click" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label10" runat="server" Text="3"></asp:Label>
                                                </td>
                                                <td>
                                                    <label class="control-label" for="street">Schedule VI/ Member Details<span style="color: red">*</span></label>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton7_Click"/>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label11" runat="server" Text="4"></asp:Label>
                                                </td>
                                                <td>
                                                    <label class="control-label" for="street">Schedule II/ Employee Details<span style="color: red">*</span></label>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton7_Click"/>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label12" runat="server" Text="5"></asp:Label>
                                                </td>
                                                <td>
                                                    <label class="control-label" for="street">Schedule IV/ Income and Expenditure Details<span style="color: red">*</span></label>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="ImageButton12" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton7_Click"/>

                                                </td>
                                            </tr>
                                    
                                            
                                        </thead>
                                    </table>
                                </div>
                            </div>
                        </div>

                        <br />
                        <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                            <div class="panel-title" style="color: white; font-weight: bold;">Additional Documents Uploaded</div>
                        </div>
                        

                          <div style="text-align: center;margin:auto">
                                <asp:GridView ID="grvAdditionalDocs" runat="server" CellPadding="5" AutoGenerateColumns="false" style="width: 100%;" AllowPaging="true" PageSize="10" CssClass="Grid" AlternatingRowStyle-CssClass="alt"
                                             PagerStyle-CssClass="pgr" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" BorderStyle="None" BorderColor="#DEDFDE" ForeColor="Black">
                                <AlternatingRowStyle BackColor="White" />                                                        
                                <HeaderStyle BackColor="#d8ddf5" Font-Bold="True" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                                        
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr. No">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Document Name" DataField="docname" />
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:BoundField HeaderText="ObjectID" DataField="object_id">
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                                            
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LbView" OnClick="LbView_AddDocs_Click"  CausesValidation="false" runat="server" ><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton>
                                            <asp:HiddenField ID="hfobjectID" Value='<%# Eval("object_id") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div align="center">No records found.</div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                                                    </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Button trigger modal -->
    <%-- <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#exampleModalLong">
        Launch demo modal
    </button>--%>
    <!-- Modal -->
    <%--  <div class="modal fade" id="exampleModalLong" tabindex="-1" role="dialog" aria-labelledby="exampleModalLongTitle" aria-hidden="true">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLongTitle">Form Preview</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Applicant Details</div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-6 col-xs-12">
                            <label class="control-label" for="street">Name of The Applicant  <span style="color: red">*</span></label>
                            <asp:TextBox ID="ViewAppName" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>


                        </div>
                        <div class="col-md-6 col-xs-12">
                            <label class="control-label" for="street">Designation <span style="color: red">*</span></label>
                            <asp:TextBox ID="appdesignation" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                        </div>

                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-12 col-xs-12">
                            <label class="control-label" for="street">Address: </label>
                            <asp:TextBox ID="appaddress" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-4 col-xs-12">
                            <label class="control-label" for="street">District:<span style="color: red">*</span></label>
                            <asp:TextBox ID="appdistrict" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                        </div>
                        <div class="col-md-4 col-xs-12 ">
                            <label class="control-label" for="street">Mobile No:<span style="color: red"></span></label>
                            <asp:TextBox ID="appmobileno" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-4 col-xs-12 ">
                            <label class="control-label" for="street">Email:<span style="color: red"></span></label>
                            <asp:TextBox ID="appemail" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                        </div>

                    </div>
                    <br />
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Society Details</div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-6 col-xs-12">
                            <label class="control-label" for="street">Society Type:<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_society_type" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>


                        </div>
                        <div class="col-md-6 col-xs-12">
                            <label class="control-label" for="street">Society Name:<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_society_name" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                        </div>

                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-12 col-xs-12">
                            <label class="control-label" for="street">Society Address:</label>
                            <asp:TextBox ID="value_society_address" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-6 col-xs-12">
                            <label class="control-label" for="street">District: <span style="color: red">*</span></label>
                            <asp:TextBox ID="value_society_district" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>


                        </div>
                        <div class="col-md-6 col-xs-12">
                            <label class="control-label" for="street">Taluka:<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_taluka" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                        </div>

                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-4 col-xs-12">
                            <label class="control-label" for="street">Registration Fee:<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_registration_fee" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                        </div>
                        <div class="col-md-4 col-xs-12 ">
                            <label class="control-label" for="street">Proccessing Fee<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_processing_fee" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-4 col-xs-12 ">
                            <label class="control-label" for="street">Total Fee to be paid<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_total_fee" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                        </div>

                    </div>
                    <br />
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Member Details</div>
                    </div>

                    <asp:GridView runat="server" ID="grvMemberDetails" HeaderStyle-CssClass="header" RowStyle-CssClass="rows" BackColor="White" BorderColor="#DEDFDE"
                        BorderStyle="None" BorderWidth="2px" CellPadding="4" ForeColor="Black" GridLines="Horizontal" AutoGenerateColumns="false"
                        CssClass="mygrdContent rows header pager" AllowPaging="false">
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>
                            <asp:TemplateField HeaderText="Sr. No." ItemStyle-Width="70">
                                <ItemTemplate>
                                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="Name" DataField="fname" />
                            <asp:BoundField HeaderText="Designation" DataField="designtaion" />
                            <asp:BoundField HeaderText="Occupation" DataField="occupation" />
                            <asp:BoundField HeaderText="Address" DataField="address" />
                            <asp:BoundField HeaderText="ID Proof" DataField="proofname" />
                            <asp:BoundField HeaderText="Managing Committee" DataField="mangcomm" />
                            <asp:TemplateField HeaderText="View">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButton3" runat="server" OnCommand="ImageButton3_Command"  ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" />
                                    <asp:HiddenField ID="hfdocid" Value='<%# Eval("doc_id") %>' runat="server" />
                                    <asp:HiddenField ID="hfdmongodoc" Value='<%# Eval("document_mongoentry") %>' runat="server" />
                                    <asp:HiddenField ID="hfmemberid" Value='<%# Eval("member_id") %>' runat="server" />
                                    <asp:HiddenField ID="hdnPDF" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:BoundField HeaderText="documentid" DataField="doc_id" />
                                    <asp:BoundField HeaderText="mongoid" DataField="document_mongoentry" />
                                    <asp:BoundField HeaderText="member_id" DataField="member_id" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <br />
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Total Members</div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-xs-12">
                            <label class="control-label" for="street">Total Members:<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_total_members" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <br />
                  <%--  <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Document Upload</div>
                    </div>--%>
    <%-- </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary">Save changes</button>
                </div>
            </div>
        </div>
    </div>--%>
    
    <div id="pdfModal" class="modal fade" role="dialog">
        <div class="modal-dialog modal-lg">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">

                    <embed id="embed1" runat="server" frameborder="0" width="100%" height="500px" />
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>
