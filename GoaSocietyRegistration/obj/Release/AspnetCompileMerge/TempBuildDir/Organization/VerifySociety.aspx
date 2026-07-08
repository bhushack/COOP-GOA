<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerifySociety.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="GoaSocietyRegistration.Admin.VerifySoceity" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <title>View Application</title>
    <link href="../assets/css/font-awesome.min.css" rel="stylesheet" /> 
    <link href="assets/bootstrap.min.css" rel="stylesheet" />
    <link href="assets/font-awesome.css" rel="stylesheet" />
    <link href="assets/style.css" rel="stylesheet" />
     
     <script src="../Scripts/jquery-3.5.0.min.js"></script>
     

    <style>
          .bs-example {
            margin: 50px;
        }

          .mymodal1 {
            max-width: 1240px !important;
        }

        .box {
            border: 1px solid #8080801a;
        }

        .border {
            border: 1px solid #8080801a;
        }

        .special {
            width: 97% !important;
        }

        .mydiv {
            padding-top: 1%;
        }

        .right {
            float: right;
        }
        .modify {
        height:700px !important;
        }
        .left {
            float: left;
        }

        .center {
            margin: auto;
            width: 60%;
            border: 3px solid #73AD21;
            padding: 10px;
        }

        .tempclass {
            display: block;
            padding: .5rem 1rem;
            font-size: 16px;
        }

        .tempclass1 {
            display: block;
            padding: .2rem 0.5rem !important;
            font-size: 16px;
        }


        .btn1 {
            background-color: transparent !important;
            color: #7376df !important;
            font-size: 0.97rem !important;
        }

        .alpha {
            padding-right: 15%;
        }

        .menucolor {
            color: white;
            background: #09152f;
        }

        .menuwhite {
            color: white !important;
        }

        ul li:hover {
            background: #37f;
            font-weight: 200;
        }

        .fees:hover {
     background-color: white !important;
 }

        .a{
            width:40%;
        }

  



    </style>
     <script type="text/javascript">
  $(document).ready(function() {
      window.history.pushState(null, "", window.location.href);        
      window.onpopstate = function() {
          window.history.pushState(null, "", window.location.href);
      };
  });
</script>



</head>
<body>
    <form id="form1" runat="server">
        <nav class="navbar navbar-expand-lg navbar-dark menucolor">
            <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarTogglerDemo03" aria-controls="navbarTogglerDemo03" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <img src="../assets/images/logo-nic.png" width="150px" />
            <div class="collapse navbar-collapse menubar" id="navbarTogglerDemo03">
                <ul class="navbar-nav mr-auto mt-2 mt-lg-0">
                   
                    <li class="nav-item active ">
                        <asp:LinkButton ID="dashboard" OnClick="dashboard_Click" CssClass=" nav-link menuwhite " runat="server"><i class="fa fa-dashboard"></i>&nbsp;Dashboard</asp:LinkButton>
                    </li>
                    <li class="nav-item ">
                        <asp:LinkButton ID="nav_society" OnClick="nav_society_Click" CssClass=" nav-link menuwhite" runat="server" Text="Society Details"></asp:LinkButton>
                    </li>
                    <li class="nav-item">
                        <asp:LinkButton ID="nav_members" OnClick="nav_members_Click" CssClass=" nav-link menuwhite" runat="server" Text="Member Details"></asp:LinkButton>
                    </li>
                    <li class="nav-item">
                        <asp:LinkButton ID="nav_documents" OnClick="nav_documents_Click" CssClass=" nav-link menuwhite" runat="server" Text="Documents"></asp:LinkButton>
                    </li>
                    <li class="nav-item">
                        <asp:LinkButton ID="addtempobservation" OnClick="addtempobservation_Click" CssClass=" nav-link menuwhite" runat="server" Visible="false" Text="Add Observation"></asp:LinkButton>
                    </li>
                    <li class="nav-item">
                        <asp:LinkButton ID="oldobservation" OnClick="oldobservation_Click" CssClass=" nav-link menuwhite" runat="server" Text="Last Observation"></asp:LinkButton>
                    </li>
                     <li class="nav-item">
                        <asp:LinkButton ID="histobservation" OnClick="histobservation_Click" CssClass=" nav-link menuwhite" runat="server" Text="Observation History"></asp:LinkButton>
                    </li>
                    
                    <li class="nav-item">
                        <asp:LinkButton ID="nav_amendment" OnClick="nav_amendment_Click" CssClass=" nav-link menuwhite" runat="server" Text="Amendment"></asp:LinkButton>
                    </li>
                    <li class="nav-item">
                        <asp:LinkButton ID="lookuptable" OnClick="lookuptable_Click" CssClass=" nav-link menuwhite" runat="server" Text="Look Up"></asp:LinkButton>
                    </li>
                </ul>
                <asp:LinkButton ID="view_applicant_logout" OnClick="view_applicant_logout_Click" CssClass="nav-link menuwhite my-0 my-sm-0 " runat="server"><em class="fa fa-power-off mr-1"></em>Logout</asp:LinkButton>

            </div>

        </nav>
        <div class="container-fluid special">
            <div class="row">
                <div class="col-6 border">
                    <br />
                    <div runat="server" id="Renewallabel" visible="true" class="text-center" style="background-color:#c8d8e6">
                        <asp:Label ID="Label22" runat="server" Font-Bold="true" ForeColor="#484ba5" Font-Size="Large" Text="RENEWAL" ></asp:Label>
                     </div>
                    <div runat="server" id="Applicant" visible="true">
                        <div class="table-responsive">
                            <div class="table table-bordered table-hover">
                                <table style="width: 100%">
                                    <tbody>
                                           
                                        <tr>
                                            <td colspan="2">
                                                <div class="box card-header mydiv">
                                                    <strong>Applicant Details</strong> <asp:LinkButton ID="LinkButton6" OnClick="nav_society_Click" CssClass=" right" runat="server"><i class="fa fa-refresh" aria-hidden="true"></i></asp:LinkButton>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_application_id" runat="server" Text="Application Id:"></asp:Label>

                                            </td>
                                            <td class="b">
                                                <asp:Label ID="value_application_id" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_applicant_name" runat="server" Text="Applicant Name:"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="value_applicant_name" runat="server" Style="text-transform: uppercase"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_designation" runat="server" Text="Designation:"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="value_desigantion" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_mobileno" runat="server" Text="Mobile No:"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="value_mobileno" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                              <td class="a">
                                                <asp:Label ID="lbl_email" runat="server" Text="Email Id:"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="value_email" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_address" runat="server" Text="Address:"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="value_address" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <%--   <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_district" runat="server" Text="District:"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="value_district" runat="server"></asp:Label>
                                            </td>
                                        </tr>--%>
                                        <tr>
                                            <td colspan="2">
                                                <div class="box card-header mydiv">
                                                    <strong>Society Details</strong>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_society_type" runat="server" Text="Society Type:"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="value_society_type" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_society_name" runat="server" Text="Society Name:"></asp:Label></td>
                                            <td>
                                                <asp:Label ID="value_society_name" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_society_address" runat="server" Text="Society Address:"></asp:Label></td>
                                            <td>
                                                <asp:Label ID="value_society_address" runat="server"></asp:Label></td>
                                        </tr>
                                        <%--     <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_society_district" runat="server" Text="District:"></asp:Label></td>
                                            <td>
                                                <asp:Label ID="value_society_district" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl_taluka" runat="server" Text="Taluka:"></asp:Label></td>
                                            <td>
                                                <asp:Label ID="value_taluka" runat="server"></asp:Label></td>
                                        </tr>--%>
                                      
                                        <tr id="tr_regfee" runat="server">
                                            <td class="a">
                                                <asp:Label ID="lbl_registration_fee" runat="server" Text="Registration Fee:"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="value_registration_fee" runat="server"></asp:Label></td>

                                        </tr>
                                        <tr id="tr_processfee" runat="server">
                                            <td class="a">
                                                <asp:Label ID="lbl_processing_fee" runat="server" Text="Proccessing Fee"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="value_processing_fee" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr id="tr_totalfee" runat="server">
                                            <td class="a">
                                                <asp:Label ID="lbl_total_fee" runat="server" Text="Total Fee to be paid"></asp:Label></td>
                                            <td>
                                                <asp:Label ID="value_total_fee" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <div class="box card-header mydiv">
                                                    <strong>Society Objectives</strong>
                                                </div>
                                            </td>
                                        </tr>   
                                        <tr>                                       
                                            <td colspan="2">
                                                <asp:GridView ID="gv_objective" runat="server" CellPadding="5" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
                                                    <Columns>    
                                                        <asp:TemplateField HeaderText="Sr. No">
                                                                    <ItemTemplate>
                                                                       <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>                               
                                                        <asp:BoundField DataField="objective" HeaderText="Objective" />                                                                                     
                                    
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                     <tr>
                                            <td colspan="2">
                                                <asp:Button ID="btn_gotomembers" runat="server" OnClick="btn_gotomembers_Click" CssClass="btn btn-info right" Text="Next" CausesValidation="false" />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                    <!-- Member Division Starts -->
                    <div runat="server" id="Members" visible="false"  >
                        <div class="table-responsive">
                            <div class="table-bordered table-hover">
                                <table style="width: 100%;">
                                    <tbody>                                      
                                        <tr>
                                            <td colspan="2">
                                                <div class="box card-header mydiv">
                                                    <strong>Managing Committee Details</strong>
                                                </div>
                                            </td>
                                        </tr>
                                       <tr>
                                           <td colspan="2">
                                                <asp:GridView runat="server" ID="gv_mangcomm" OnRowDataBound="gv_mangcomm_RowDataBound" HeaderStyle-CssClass="header" RowStyle-CssClass="rows" Font-Size="Small" BackColor="White" BorderColor="#DEDFDE"
                                                    BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Horizontal" AutoGenerateColumns="false"
                                                    CssClass="mygrdContent rows header pager" AllowPaging="false">
                                                    <AlternatingRowStyle BackColor="White" />
                                                    <FooterStyle BackColor="#CCCC99" />
                                                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                                    <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                                    <RowStyle BackColor="#c8caf1" />
                                                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />                                                    
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Sr. No." ItemStyle-Width="70">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField HeaderText="Salutation" DataField="salutation" />
                                                        <asp:BoundField HeaderText="Name" DataField="fname" />
                                                        <asp:BoundField HeaderText="Gender" DataField="gender" />
                                                        <asp:BoundField HeaderText="Age" DataField="age" />
                                                        <asp:BoundField HeaderText="Designation" DataField="designtaion" />
                                                        <asp:BoundField HeaderText="Occupation" DataField="OccupationName" />                                                        
                                                        <asp:BoundField HeaderText="Address" DataField="address" />
                                                        <asp:BoundField HeaderText="ID Proof" DataField="proofname" />
                                                        <asp:BoundField HeaderText="Managing Committee" DataField="mangcomm" Visible="false" />
                                                        <asp:BoundField HeaderText="Remarks" DataField="remarks" />
                                                        <asp:BoundField HeaderText="Date of Admission" DataField="dateofadmission" DataFormatString="{0:dd/MM/yyyy}" />
                                                        <asp:TemplateField Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="LbOccupOthers" runat="server" Text='<%# Eval("occupation_others") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="View">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="ImageButton3" runat="server" OnClientClick="openInNewTab();" OnClick="ImageButton3_Click" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" />
                                                                <asp:HiddenField ID="hfdocid" Value='<%# Eval("doc_id") %>' runat="server" />
                                                                <asp:HiddenField ID="hfdmongodoc" Value='<%# Eval("document_mongoentry") %>' runat="server" />
                                                                <asp:HiddenField ID="hfmemberid" Value='<%# Eval("member_id") %>' runat="server" />                                                                
                                                                <asp:HiddenField ID="hdnPDF" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        
                                                    </Columns>
                                                </asp:GridView>
                                                <br />
                                           </td>
                                       </tr>
                                         <tr>
                                            <td >
                                                <asp:Label ID="lbl_mangcomm_members"  runat="server" Text="Total Managing Committee Members:" CssClass="mr-2"></asp:Label>
                                               
                                                </td>
                                            <td>
                                                 <asp:Label ID="value_mangcomm_members" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                           <tr>
                                            <td colspan="2">
                                                <div class="box card-header mydiv">
                                                    <strong>Mandatory Documents</strong>
                                                </div>
                                            </td>
                                        </tr>
                                         <tr id="Tr1" runat="server" > 
                                            <td class="a">
                                                <asp:Label ID="Label48" runat="server" Font-Bold="true" Text=" 1 ) Memorandum of Association"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton1" CssClass="float-right" runat="server" OnClick="ImgBtnViewPdf2_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <div class="box card-header mydiv">
                                                    <strong>Member Details</strong>
                                                </div>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td colspan="2">                                               
                                                <asp:GridView runat="server" ID="grvMemberDetails" OnRowDataBound="grvMemberDetails_RowDataBound" HeaderStyle-CssClass="header" Font-Size="Small" RowStyle-CssClass="rows" BackColor="White" BorderColor="#DEDFDE"
                                                    BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Horizontal" AutoGenerateColumns="false"
                                                    CssClass="mygrdContent rows header pager" AllowPaging="false">
                                                    <AlternatingRowStyle BackColor="White" />
                                                    <FooterStyle BackColor="#CCCC99" />
                                                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                                    <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                                    <RowStyle BackColor="#c8caf1" />
                                                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                                    <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                                    <SortedAscendingHeaderStyle BackColor="#848384" />
                                                    <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                                    <SortedDescendingHeaderStyle BackColor="#575357" />
                                                   
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Sr. No." ItemStyle-Width="70">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField HeaderText="Salutation" DataField="salutation" />
                                                        <asp:BoundField HeaderText="Name" DataField="fname" />
                                                        <asp:BoundField HeaderText="Gender" DataField="gender" />
                                                        <asp:BoundField HeaderText="Age" DataField="age" />
                                                        <asp:BoundField HeaderText="Designation" DataField="designtaion" />
                                                        <asp:BoundField HeaderText="Occupation" DataField="OccupationName" />                                                        
                                                        <asp:BoundField HeaderText="Address" DataField="address" />
                                                        <asp:BoundField HeaderText="ID Proof" DataField="proofname" />
                                                        <asp:BoundField HeaderText="Managing Committee" DataField="mangcomm" Visible="false" />
                                                         <asp:BoundField HeaderText="Remarks" DataField="remarks" />
                                                        <asp:BoundField HeaderText="Date of Admission" DataField="dateofadmission" DataFormatString="{0:dd/MM/yyyy}" />
                                                        <asp:TemplateField Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="LbOccupOthers" runat="server" Text='<%# Eval("occupation_others") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="View">
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="ImageButton3" runat="server" OnClientClick="openInNewTab();" OnClick="ImageButton3_Click" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" />
                                                                <asp:HiddenField ID="hfdocid" Value='<%# Eval("doc_id") %>' runat="server" />
                                                                <asp:HiddenField ID="hfdmongodoc" Value='<%# Eval("document_mongoentry") %>' runat="server" />
                                                                <asp:HiddenField ID="hfmemberid" Value='<%# Eval("member_id") %>' runat="server" />
                                                                
                                                                <asp:HiddenField ID="hdnPDF" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                       
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:Label ID="LblOccupationOthers" runat="server" Text="" Visible="false"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>                                              <td colspan="2">
                                               
                                                    <strong style="color:red">* Members who belongs to Govt Service are highlighted with Orange Color </strong>
                                                
                                            </td>
                                        </tr>
                                         <%--<tr>
                                            <td colspan="2">
                                                <div class="box card-header mydiv">
                                                    <strong>Total Members</strong>
                                                </div>
                                            </td>

                                        </tr>--%>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lbl_total_members"  runat="server" Text="Total Members:" CssClass="mr-2"></asp:Label>
                                                
                                            </td>
                                            <td>
                                                <asp:Label ID="value_total_members" runat="server" ></asp:Label>
                                            </td>
                                            
                                        </tr>
                                       

                                        <tr id="row1" runat="server" visible="false">
                                            <td colspan="2">
                                                <div class="box card-header mydiv">
                                                    <strong>Other Members List</strong>
                                                </div>
                                            </td>
                                        </tr>


                                        <tr id="row2" runat="server" visible="false">
                                            <td>
                                                <asp:Label ID="Label20" runat="server" Text="View Members List"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="ImgBtnViewPdf7" runat="server" OnClick="ImgBtnViewPdf7_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                        <tr id="row3" runat="server" visible="false">   <td colspan="2">
                                               
                                                    <asp:Label ID="listexist" runat="server" Text=""  style="color:red"></asp:Label></td>
                                                
                                            
                                        </tr>

                                         <tr >
                                            <td colspan="2">
                                                <asp:Button ID="btn_gotosociety" runat="server" OnClick="btn_gotosociety_Click" CssClass="btn btn-info left" Text="Back" CausesValidation="false" />
                                                <asp:Button ID="btn_gotodocumentoremployee" runat="server" OnClick="btn_gotodocumentoremployee_Click" CommandArgument="1" CssClass="btn btn-success right" Text="Next" CausesValidation="false" />
                                            </td>
                                        </tr>
                                    
                                         
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <br />
                        <br />
                    </div>


                    <!-- Employee Division Starts -->
                     <div runat="server" id="Employees" visible="false"  >
                        <div class="table-responsive">
                            <div class="table-bordered table-hover">
                                <table style="width: 100%;">
                                    <tbody>
                                                                                
                                        <tr>
                                            <td colspan="8">
                                                <div class="box card-header mydiv">
                                                    <strong>Employee Details</strong>
                                                </div>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td colspan="8">                                               
                                                <asp:GridView runat="server" ID="gv_employeedetails" HeaderStyle-CssClass="header" Font-Size="Small" RowStyle-CssClass="rows" BackColor="White" BorderColor="#DEDFDE"
                                                    BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" GridLines="Horizontal" AutoGenerateColumns="false"
                                                    CssClass="mygrdContent rows header" AllowPaging="false">
                                                    <AlternatingRowStyle BackColor="White" />
                                                    <FooterStyle BackColor="#CCCC99" />
                                                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />                                                  
                                                    <RowStyle BackColor="#c8caf1" />
                                                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                                   
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
                                                                
                                                            </Columns>
                                                </asp:GridView>
                                                <asp:Label ID="Label34" runat="server" Text="" Visible="false"></asp:Label>
                                            </td>
                                        </tr>
                                        
                                         <tr>
                                            <td colspan="8">
                                                <div class="box card-header mydiv">
                                                    <strong>Total Employee</strong>
                                                </div>
                                            </td>

                                        </tr>                                        
                                        <tr>
                                            <td colspan="4">
                                                <asp:Label ID="Label37"  runat="server" Text="Total:" CssClass="mr-2"></asp:Label>
                                               
                                                </td>
                                            <td>
                                                 <asp:Label ID="tot_employee" runat="server"></asp:Label>
                                            </td>
                                        </tr>

                                       
                                      
                                         <tr>
                                            <td colspan="8">
                                                <asp:Button ID="btn_gotomembers_renewal" runat="server" OnClick="btn_gotomembers_Click" CssClass="btn btn-info left" Text="Back" CausesValidation="false" />
                                                <asp:Button ID="btn_gotodocument_renewal" runat="server" OnClick="btn_gotodocument_Click" CommandArgument="2" CssClass="btn btn-success right" Text="Next" CausesValidation="false" />
                                            </td>
                                        </tr>
                                    
                                         
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <br />
                        <br />
                    </div>

                   

                    <div runat="server" id="Documents" visible="false" >
                        <div class="table-responsive">
                            <div class="table table-bordered table-hover">
                                <table style="width: 100%">
                                    <tbody>
                                          <tr>
                                            <td colspan="2">
                                                <asp:Button ID="goback_tomembersoremployee" runat="server" OnClick="goback_tomembersoremployee_Click" CssClass="btn btn-info left" Text="Back" CausesValidation="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <div class="box card-header mydiv">
                                                    <strong>Mandatory Documents</strong>
                                                </div>
                                            </td>
                                        </tr>

                                        <%-- Case of Registration --%>
                                        <tr id="regdoc1" runat="server" visible="false">
                                            <td class="a">
                                                <asp:Label ID="Label1" runat="server" Text="1) Application for Registration"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="ImgBtnViewPdf1" runat="server" OnClick="ImgBtnViewPdf1_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                        <tr id="regdoc2" runat="server" visible="false"> 
                                            <td class="a">
                                                <asp:Label ID="Label2" runat="server" Text="2) Memorandum of Association"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="ImgBtnViewPdf2" runat="server" OnClick="ImgBtnViewPdf2_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                        <tr id="regdoc3" runat="server" visible="false">
                                            <td class="a">
                                                <asp:Label ID="Label3" runat="server" Text="3) Rules And Regulation/Constitution of Association/By-laws"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="ImgBtnViewPdf3" runat="server" OnClick="ImgBtnViewPdf3_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                   
                                         <%-- Case of Renewal --%>
                                          <tr id="renewdoc1" runat="server" visible="false">
                                            <td class="a">
                                                <asp:Label ID="lbl1" runat="server" Text="1) Application for Renewal"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="ImgBtn_renewalAppln_ViewPdf" runat="server" OnClick="ImgBtn_renewalAppln_ViewPdf_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                        <tr id="renewdoc2" runat="server" visible="false">
                                            <td class="a">
                                                <asp:Label ID="lbl2" runat="server" Text="2) Schedule 1/ Managing Committee Details"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="ImgBtn_Schedule1_ViewPdf" runat="server" OnClick="ImgBtn_Schedule1_ViewPdf_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                        <tr id="renewdoc3" runat="server" visible="false">
                                            <td class="a">
                                                <asp:Label ID="lbl3" runat="server" Text="3) Schedule VI/ Member Details"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="ImgBtn_Schedule6_ViewPdf" runat="server" OnClick="ImgBtn_Schedule6_ViewPdf_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                         <tr id="renewdoc4" runat="server" visible="false">
                                            <td class="a">
                                                <asp:Label ID="Label25" runat="server" Text="4) Schedule II/ Employee Details"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="ImgBtn_Schedule2_ViewPdf" runat="server" OnClick="ImgBtn_Schedule2_ViewPdf_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                         <tr id="renewdoc5" runat="server" visible="false">
                                            <td class="a">
                                                <asp:Label ID="Label26" runat="server" Text="5) Schedule IV/ Income and Expenditure Account"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="ImgBtn_Schedule4_ViewPdf" runat="server" OnClick="ImgBtn_Schedule4_ViewPdf_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                   


                                       

                                         <tr id="header_adddocs" runat="server" visible="true">
                                            <td colspan="3">
                                                <div class="box card-header">
                                                    <strong>Additional Documents</strong>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr id="soc_add_docs" runat="server" visible="true">
                                            <td colspan="5" style="text-align:center;">
                                                <asp:GridView ID="GridView_AddDocs" Width="100%" runat="server" CellPadding="5" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" CssClass="Grid" PagerStyle-CssClass="pgr" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" BackColor="White"> <%-- AlternatingRowStyle-CssClass="alt"--%>
                                                    <%--<AlternatingRowStyle BackColor="White" />--%>
                                                    <FooterStyle BackColor="#CCCC99" />
                                                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                                    <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                                    <%--<RowStyle BackColor="#F7F7DE" />--%>
                                                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                                    <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                                    <SortedAscendingHeaderStyle BackColor="#848384" />
                                                    <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                                    <SortedDescendingHeaderStyle BackColor="#575357" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Sr. No">
                                                            <ItemTemplate>
                                                                <%#Container.DataItemIndex+1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField HeaderText="Document Name" DataField="docname" />                                                       
                                                        <asp:TemplateField HeaderText="View">
                                                            <ItemTemplate>
                                                                <%--<asp:LinkButton ID="LbView" CausesValidation="false" runat="server" OnClick="LbView_Click"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton>--%>
                                                                 <asp:ImageButton ID="View_adddocs" runat="server" OnClick="LbView_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>                                                               
                                                                <asp:HiddenField ID="hfobjectID" Value='<%# Eval("object_id") %>' runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <EmptyDataTemplate>
                                                        <div align="center">No Documents found.</div>
                                                    </EmptyDataTemplate>
                                                </asp:GridView>
                                            </td>
                                        </tr>

                                       
                                              <tr id="heading" runat="server"> 
                                        <td colspan="2">
                                            <div class="box card-header mydiv">
                                                 
                                                <strong>Observation</strong>
                                            </div>
                                              <asp:Label ID="Label5" runat="server" ForeColor="Red" Text="(Alphabet A-Z, a-z, 0-9 and Special Characters -,._()/:@ only are allowed)"></asp:Label>
                                        </td>
                                    </tr>

                                  <%--  <tr id="gobsv" runat="server">
                                        <td style="width:50%">

                                            <div class="box card-header mydiv">
                                                <strong>Society Observation</strong>
                                            </div>

                                        </td>
                                        <td> 
                                            <textarea id="outputtext" name="outputtext" maxlength="1000" runat="server" rows="2" style="width: 100%"></textarea>

                                        </td>
                                    </tr>--%>
                                    <tr id="bobsv"  runat="server" >
                                      <%--  <td style="width:50%">

                                            <div class="box card-header mydiv">
                                                <strong>Members Observation</strong>
                                            </div>

                                        </td>--%>
                                        <td colspan="2">
                                            <textarea id="outputtext1" name="outputtext1"  maxlength="1000" runat="server" rows="2" style="width: 100%"></textarea>
                                        </td>
                                    </tr>
                                              <tr runat="server" id="hidefordh"> 
                                        <td colspan="2" >
                                       
                                            <div class="box card-header mydiv">
                                                <strong>Remarks<span class="text-danger">*</span></strong>
                                            </div>
                                                 <asp:Label ID="Label4" runat="server" ForeColor="Red" Text="(Alphabet A-Z, a-z, 0-9 and Special Characters -,._()/:@ only are allowed)"></asp:Label>
                                        </td>
                                    </tr>
                                            <tr>
                                                 
                                        <td colspan="2">
                                            <textarea id="outputtextremarks" name="outputtextremarks" visible="false" maxlength="1000" runat="server" rows="3" style="width: 100%"></textarea>
                                        </td>
                                    </tr>
                                        <%-- <tr id="checkcompulsory" runat="server">
                                            <td class="a">
                                                <asp:CheckBox ID="chkverified" AutoPostBack="true" runat="server" />
                                            </td>
                                            <td>
                                                <asp:Label ID="Label5" runat="server" >All Data is Verified</asp:Label></td>
                                        </tr>--%>

                                        <!--mot used remarks division-->
                                        <tr id="remarks" runat="server" visible="false">
                                            <td class="a">
                                                <asp:Label ID="Label7" runat="server" class="col-sm-3 col-form-label">Remarks</asp:Label><span class="text-danger">*</span>
                                            </td>
                                            <td>
                         
                                                <asp:TextBox ID="txtRemarks" TextMode="MultiLine" MaxLength="1000" Rows="3" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rvfremarks" Display="Dynamic" runat="server" ControlToValidate="txtRemarks" CssClass="text-danger" ErrorMessage="Please Enter Remarks"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        
                                        <tr id="tr_renewfeesheading" runat="server" visible="false">
                                            <td colspan="2">
                                                <div class="box card-header mydiv">
                                                    <strong>Renewal Fees</strong>
                                                </div>
                                            </td>
                                        </tr>

                                        
                                        <tr id="tr_feescalc" class="fees" runat="server" visible="false">
                                            <td colspan="2">
                                                <table id="feestable" style="width:100%" class="table table-borderless">
                                                    <tbody>                                                        
                                                        <tr>
                                                            <td class="a"><asp:Label ID="Label28" runat="server" class="col-sm-3 col-form-label">Registration Date</asp:Label><span class="text-danger">*</span></td>
                                                            <td><asp:TextBox ID="TxtBxRegistrationDate" CssClass="ml-3" type="date" Enabled="false" runat="server" AutoCompleteType="Disabled"></asp:TextBox></td>
                                                         </tr>
                                                        <tr>
                                                            <td  class="a"><asp:Label ID="Label29" runat="server" class="col-sm-3 col-form-label">Last Date for Renewal</asp:Label><span class="text-danger">*</span></td>
                                                            <td><asp:TextBox ID="TxtBxRenewalDate" type="date" CssClass="ml-3" runat="server"  AutoCompleteType="Disabled"></asp:TextBox></td>
                                                         </tr>
                                                         <tr>
                                                            <td  class="a"><asp:Label ID="Label30" runat="server" class="col-sm-3 col-form-label">Due Date</asp:Label><span class="text-danger">*</span></td>
                                                            <td><asp:TextBox ID="TxtbxDueDate" type="date" CssClass="ml-3" runat="server" AutoCompleteType="Disabled"></asp:TextBox></td>
                                                         </tr>
                                                        <tr>
                                                            <td  class="a"><asp:Label ID="Label27" runat="server" class="col-sm-3 col-form-label">Processing Fee</asp:Label><span class="text-danger">*</span></td>
                                                            <td><asp:Label ID="Label36" runat="server" Text="₹ "></asp:Label><asp:TextBox ID="TxtBxProcessFee" Enabled="false" runat="server" Text="75"  AutoCompleteType="Disabled"></asp:TextBox></td>
                                                         </tr>
                                                        <tr>
                                                            <td  class="a"><asp:Label ID="Label31" runat="server" class="col-sm-3 col-form-label">Penalty</asp:Label><span class="text-danger">*</span></td>
                                                            <td><asp:Label ID="Label35" runat="server" Text="₹ "></asp:Label><asp:TextBox ID="TxtBxPenalty" Enabled="false" runat="server" Text=""  AutoCompleteType="Disabled"></asp:TextBox></td>
                                                         </tr>

                                                         
                                                         <tr>
                                                            <td class="a"><asp:Label ID="Label32" runat="server" class="col-sm-3 col-form-label">Total Fees to be paid</asp:Label><span class="text-danger">*</span></td>
                                                            <td><asp:Label ID="Label38" runat="server" Text="₹ "></asp:Label><asp:TextBox ID="txtbxtotalfees" runat="server" Text=""  AutoCompleteType="Disabled"></asp:TextBox>
                                                                <asp:LinkButton ID="LkCalcFees" OnClick="LkCalcFees_Click" runat="server" CssClass="btn btn-outline-info ml-3" Text="Calculate Fees"></asp:LinkButton>
                                                                <asp:HiddenField ID="hffees" runat="server" />


                                                            </td>
                                                         </tr>
                                                         <tr>
                                                            <td class="a"><asp:Label ID="Label33" runat="server" class="col-sm-3 col-form-label">Fees Remarks</asp:Label><span class="text-danger">*</span></td>
                                                             <td>
                                                                <textarea id="txtareafeesremarks" name="feesremarks" maxlength="500" runat="server" rows="3" style="width: 100%"></textarea>
                                                            </td>
                                                           
                                                         </tr>
                                                        <tr>
                                                           
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                 <asp:LinkButton ID="lkSaveFees" OnClick="lkSaveFees_Click" CssClass="btn btn-info" runat="server"><span class="btn-label"><i class="fa fa-save"></i></span>&nbsp Save Fees</asp:LinkButton>
                                                            </td>
                                                           
                                                         </tr>
                                                    </tbody>
                                                </table>
                                                
                                               
                                                
                                            </td> 
                                        </tr>
                                        
                                        <tr>
                                            <td class="a">
                                                 <asp:LinkButton ID="saveobservation" OnClick="saveobservation_Click" CssClass="btn btn-success" Visible="false" runat="server"><span class="btn-label"><i class="fa fa-save"></i></span>&nbsp Save Observation and Forward</asp:LinkButton>
                                                <asp:LinkButton ID="pull_application_from_dh" OnClick="pull_application_from_dh_Click" CssClass="btn btn-warning" Visible="false" runat="server"><span class="btn-label"><i class="fa fa-pull"></i></span>Pull Application</asp:LinkButton>
                                                <%--<asp:LinkButton ID="reject" OnClick="reject_Click" CssClass="btn btn-danger" Visible="false" runat="server"><span class="btn-label"><i class="fa fa-times"></i></span>Reject Application</asp:LinkButton> <%--without upload--%>
                                             <asp:LinkButton ID="sendobservation" OnClick="sendobservation_Click" CssClass="btn btn-warning" Visible="false" runat="server"><span class="btn-label"><i class="fa fa-times"></i></span>Send Observation</asp:LinkButton></td>
                                            <td>
                                                <asp:LinkButton ID="accepted" OnClick="accepted_Click" CssClass="btn btn-primary" Visible="false" runat="server"><span class="btn-label"><i class="fa fa-check"></i></span>Accept</asp:LinkButton>
                                                <asp:LinkButton ID="LkReject" OnClick="LkReject_Click" CssClass="btn btn-danger" Visible="false" runat="server"><span class="btn-label"><i class="fa fa-ban" aria-hidden="true"></i></span>Reject</asp:LinkButton><%--with upload--%>

                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                   

                </div>
                <div class="col-6 border" runat="server" id="framedivision">

                    <br />
                    <iframe id="pdfiframe" src="ViewPdf.aspx" name="pdfiframe" style="height: 95%; width: 100%" runat="server"></iframe>
                </div>
            </div>
        </div>



        
        <div class="bs-example">
        <!-- Modal HTML -->
        <div id="RejectUploadModal" class="modal fade">
            <div class="modal-dialog  modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Upload</h4>
                    </div>
                    <div class="modal-body">
                         <div class="row" runat="server">
                            <div class="table-responsive">
                                <div class="table table-bordered table-hover">
                                    <table style="width: 100%">
                                        <tr>
                                            <td align="right">File Type Allowed
                                            </td>
                                            <td>
                                                <b style="color: red">Only PDF File
                                                </b>
                                            </td>
                                            <td align="right">Maximum File Size Allowed
                                            </td>
                                            <td>
                                                <b style="color: red">2 MB</b>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <asp:HiddenField ID="AppIDHD" runat="server" />
                        <div class="form-group">
                            <asp:Label ID="Label43" runat="server" Text="Application ID : "></asp:Label>
                            <asp:Label ID="Label44" runat="server" Text=""></asp:Label>
                            <br />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label47" runat="server" Text="Remarks : "></asp:Label>
                            <asp:TextBox ID="TextBox1" CssClass="form-control" TextMode="MultiLine" Rows="3" runat="server"></asp:TextBox>
                            <br />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label45" CssClass="control-label" runat="server" Text="Upload" ForeColor="Black"></asp:Label>
                            <asp:FileUpload ID="FileUpload2" CssClass="form-control" runat="server" />
                        </div>
                        <asp:Label ID="Label46" ForeColor="Red" Visible="false" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                         <asp:Button ID="btnUpload" OnClick="btnUpload_Click" CssClass="btn btn-primary" runat="server" Text="Upload" />
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>



        <div id="remarkshistory" class="modal fade" role="dialog">
             <div class="modal-dialog modal-xl mymodal1">
                    <div class="modal-content ">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label21" runat="server" Text="Application Observation History" ForeColor="White"></asp:Label>
                            </h4>
                          </div>
                        <div class="modal-body">                           
                             <div class="table-responsive">                                   
                                 <asp:GridView ID="gvhistory" runat="server" EditRowStyle-HorizontalAlign="Left" AutoGenerateColumns="false" OnRowDataBound="gvhistory_RowDataBound" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%" CellPadding="5">
                                     <Columns>
                                        <asp:TemplateField HeaderText="Edit" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lbAppID" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
<%--                                        <asp:BoundField DataField="groomname" HeaderText="Groom Name" />
                                        <asp:BoundField DataField="bridename" HeaderText="Bride Name" /> --%>                                   
                                        <asp:BoundField DataField="observation_by_dh" HeaderText=" Society Remarks by the Dealing Hand  " />
                                        <%--<asp:BoundField HeaderText=" Members Remarks by the Dealing Hand  " />--%>
                                        <asp:BoundField DataField="remarks_sendobservation" HeaderText=" Observation by DRO " />                                        
                                        <asp:BoundField DataField="submit_time_remarkssendobservation" HeaderText="Observation Made at" />                                        
                                    </Columns>
                                </asp:GridView>
                             </div>

                                <br />
                        
                         </div>
                        <div class="modal-footer">                    
                            <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
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
                            <asp:Button ID="btnRedirect" OnClick="btnRedirect_Click" CssClass="btn btn-primary" runat="server" Text="Confirm" />
                     <%--       <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                        </div>
                    </div>
                </div>
            </div>
        </div>
            <div class="bs-example">
            <!-- Modal HTML -->
            <div id="saveobs_confirmation_modal" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label6" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label8" runat="server" Text="Are you sure to save the observation. If yes click on Confirm"></asp:Label>
                            </div>
                        <asp:Label ID="Label9" CssClass="alert-danger" runat="server" Text=""></asp:Label>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-danger left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="save_obs_modal_confirm_button" OnClick="save_obs_modal_confirm_button_Click"  CssClass="right btn btn-success" runat="server">Confirm</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
          <div class="bs-example">
            <!-- Modal HTML -->
            <div id="sendobs_confirmation_modal" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label10" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label11" runat="server" Text="Are you sure to send the observation to applicant for correction. If yes click on Confirm"></asp:Label>
                            </div>
                        <asp:Label ID="Label12" CssClass="alert-danger" runat="server" Text=""></asp:Label>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-danger left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="send_obs_modal_confirm_button" OnClick="send_obs_modal_confirm_button_Click"  CssClass="right btn btn-success" runat="server">Confirm</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
                <div class="bs-example">
            <!-- Modal HTML -->
            <div id="pull_application_confirmation_modal" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label16" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label17" runat="server" Text="Are you sure to pull the applicant from Dealing Hand. If yes click on Confirm"></asp:Label>
                            </div>
                        <asp:Label ID="Label18" CssClass="alert-danger" runat="server" Text=""></asp:Label>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-danger left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="modal_pull_application_button" OnClick="modal_pull_application_button_Click"  CssClass="right btn btn-success" runat="server">Confirm</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="bs-example">
            <!-- Modal HTML -->
            <div id="reject_modal" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label13" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label14" runat="server" Text="Are you sure to reject the application. If yes click on Confirm"></asp:Label>
                            </div>
                        <asp:Label ID="Label15" CssClass="alert-danger" runat="server" Text=""></asp:Label>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-danger left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="reject_modal_confirm_button" CausesValidation="false" OnClick="reject_modal_confirm_button_Click"  CssClass="right btn btn-success" runat="server">Confirm</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
           <div class="bs-example">
            <!-- Modal HTML -->
            <div id="AskForPayment" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label70" runat="server" Text="Payment" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label79" runat="server" Text="Are you sure to accept the application. If yes click on Confirm"></asp:Label>
                            </div>
                        <asp:Label ID="lberror" CssClass="alert-danger" runat="server" Text=""></asp:Label>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-danger left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="accecptapplicationmodalclick" OnClick="accecptapplicationmodalclick_Click"  CssClass="right btn btn-success" runat="server">Confirm</asp:LinkButton>
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
                            <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
         <div class="bs-example">
            <!-- Modal HTML -->
            <div id="errorModal" class="modal">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label19" runat="server" Text="Observation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <textarea id="inputtext" name="inputtext" runat="server" cols="50" rows="2"></textarea>

                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="addobservationtotextarea" OnClick="addobservationtotextarea_Click" CssClass="btn btn-primary" runat="server" Text="Add" />
                            <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
         <div class="bs-example">
            <!-- Modal HTML -->
            <div id="showoldobsv" class="modal fade">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label57" runat="server" Text="Previous Observation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <div class="table table-bordered table-hover">
                                <table style="width: 100%">
                                    <tbody>
                       <%--                 <tr id="soc_modal" runat="server">
                                            <td>

                                                <div class="box card-header mydiv">
                                                    <strong>Society Observation</strong>
                                                </div>

                                            </td>
                                            <td>
                                                <asp:Label ID="socmodal" runat="server" Text="Label"></asp:Label>


                                            </td>
                                        </tr>--%>
                                        <tr id="members_modal" runat="server">
                                            <td>

                                                <div class="box card-header mydiv">
                                                    <strong>Observation</strong>
                                                </div>

                                            </td>
                                            <td>
                                                <asp:Label ID="membersmodal" runat="server" Text="Label"></asp:Label>

                                            </td>
                                        </tr>     
                                        <tr id="old_remarks_modal" runat="server" visible="false">
                                            <td>

                                                <div class="box card-header mydiv">
                                                    <strong>Observation Remarks by Registrar<span class="text-danger">*</span></strong>
                                                </div>
                                            </td>

                                            <td>
                                                <asp:Label ID="remarksmodal" runat="server" Text="Label"></asp:Label>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>

                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

           <div class="bs-example">
            <!-- Modal HTML -->            
                <div id="lookupModal" class="modal fade">
                <div class="modal-dialog modal-lg" style="max-width: 1200px !important">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label165" runat="server" Text="Search Result" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body  table-responsive ">
                            
                            <div runat="server" id="Div2" visible="true" class="text-center" style="background-color:#fbf8a2">
                                <asp:Label ID="Label24" runat="server" Font-Bold="true" ForeColor="#484ba5" Font-Size="Large" Text="SOCIETIES REGISTERED ONLINE" ></asp:Label>
                                </div>
                            <asp:GridView ID="gvLookup" runat="server" OnRowDataBound="gvLookup_RowDataBound" CellPadding="2" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
                                      
                                <Columns>
                                    <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LbSearchApp_id" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>                                                   
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="login_id" HeaderText="Token ID" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="app_id" HeaderText="Application ID" HeaderStyle-Width="15%" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="socname" HeaderText="Society Name" HeaderStyle-Width="25%" />
                                    <asp:BoundField DataField="socregid" HeaderText="Registration ID" HeaderStyle-Width="12%" />
                                    <asp:BoundField DataField="regdate" HeaderText="Registration Date" HeaderStyle-Width="10%" DataFormatString="{0:dd/MM/yyyy}" />                                            
                                    <asp:BoundField DataField="DistrictName" HeaderText="Registered At" HeaderStyle-Width="8%" />
                                    <asp:BoundField DataField="status_id" HeaderText="Status" HeaderStyle-Width="15%" />
                                    <asp:TemplateField HeaderText="View Details" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LkviewLookup" CssClass="btn btn-info" OnClick="LkviewLookup_Click" CausesValidation="false" runat="server"><i class="fa fa-search" aria-hidden="true"></i></asp:LinkButton>
                                                    
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <br />
                            <div runat="server" id="Div1" visible="true" class="text-center" style="background-color:#fbf8a2">
                                <asp:Label ID="Label23" runat="server" Font-Bold="true" ForeColor="#484ba5" Font-Size="Large" Text="SOCIETIES REGISTERED OFFLINE" ></asp:Label>
                             </div>
                             <asp:GridView ID="gvLookup_offline" runat="server" OnRowDataBound="gvLookup_offline_RowDataBound" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" ShowHeader="true" Width="100%">
                                    <Columns>
                                            <asp:BoundField HeaderText="Token ID" ShowHeader="false" HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center"  />
                                            <asp:BoundField  HeaderText="Application ID" ShowHeader="false" HeaderStyle-Width="15%" ItemStyle-HorizontalAlign="Center"/>
                                            <asp:BoundField DataField="socname" HeaderText="Society Name" ShowHeader="false" HeaderStyle-Width="25%"  />
                                            <asp:BoundField DataField="socregid" HeaderText="Registration ID" ShowHeader="false" HeaderStyle-Width="15%" />
                                            <asp:BoundField DataField="reg_date" HeaderText="Registration Date" ShowHeader="false" HeaderStyle-Width="10%" DataFormatString="{0:dd/MM/yyyy}"  />                                            
                                            <asp:BoundField DataField="DistrictName" HeaderText="Registered At" ShowHeader="false" HeaderStyle-Width="10%" />
                                            <asp:BoundField  HeaderText="Status" ShowHeader="false" HeaderStyle-Width="15%" ItemStyle-HorizontalAlign="Center"  />
                                            <asp:TemplateField HeaderText="View Details" HeaderStyle-Width="5%" Visible="false">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="LkviewLookup1" Enabled="false" CssClass="btn btn-info" CausesValidation="false" runat="server"><i class="fa fa-search" aria-hidden="true"></i></asp:LinkButton>
                                                    
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField Visible="false">
                                             <ItemTemplate>
                                                                                  
                                                   <asp:Label ID="LbDate" runat="server" Text='<%# Eval("regdate") %>'></asp:Label>                                           
                                                    <asp:HiddenField ID="hfdatemodify" Value='<%# Eval("datemodified") %>' runat="server" />
                                                    
                                             </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                
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
            <div id="feesconfirmation" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label39" runat="server" Text="Payment" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label40" runat="server" Text="Are you sure to save the Fees? If yes click on Confirm"></asp:Label>
                            </div>
                        <asp:Label ID="Label41" CssClass="alert-danger" runat="server" Text=""></asp:Label>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-danger left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="LkSaveFees_modalbtn" OnClick="LkSaveFees_modalbtn_Click"  CssClass="right btn btn-success" runat="server">Confirm</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <div class="bs-example">
        <!-- Modal HTML -->
        <div id="successmodal" class="modal fade">
            <div class="modal-dialog  modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Success</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label42" runat="server" Text="" ForeColor="Green"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
     <script src="../Scripts/popper.min.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
   
</html>
