<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerifyAmendment.aspx.cs" EnableEventValidation="false" Inherits="GoaSocietyRegistration.Organization.VerifyAmendment" %>

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
    <title>View Amendment</title>
    <link href="../assets/css/font-awesome.min.css" rel="stylesheet" />
    <script src="../Scripts/jquery-3.5.0.min.js"></script>
    <script src="../Scripts/popper.min.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
   <%-- <link href="assets/adminstyle/vendor/bootstrap/css/bootstrap.css" rel="stylesheet" />--%>
    <link href="assets/bootstrap.min.css" rel="stylesheet" />
    <link href="assets/font-awesome.css" rel="stylesheet" />
    <link href="assets/style.css" rel="stylesheet" />
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
        <nav class="navbar navbar-expand-lg navbar-dark menucolor fixed-top">
            <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarTogglerDemo03" aria-controls="navbarTogglerDemo03" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <img src="../assets/images/logo-nic.png" width="150px" />
            <div class="collapse navbar-collapse menubar" id="navbarTogglerDemo03">
                <ul class="navbar-nav mr-auto mt-2 mt-lg-0">
                   
                    <li class="nav-item ">
                        <asp:LinkButton ID="dashboard" OnClick="dashboard_Click" CssClass=" nav-link menuwhite " runat="server"><i class="fa fa-dashboard"></i>&nbsp;Dashboard</asp:LinkButton>
                    </li>     
                              
                    <li class="nav-item active">
                        <asp:LinkButton ID="nav_documents" OnClick="nav_documents_Click" CssClass=" nav-link menuwhite" runat="server" Text="Amendment"></asp:LinkButton>
                    </li>                  
                   
                     <li class="nav-item">
                        <asp:LinkButton ID="histobservation" OnClick="histobservation_Click" CssClass=" nav-link menuwhite" runat="server" Text="Observation History"></asp:LinkButton>
                    </li>
                     

                    <li class="nav-item">
                        <asp:LinkButton ID="gobacktosociety" OnClick="gobacktosociety_Click" CssClass=" nav-link menuwhite" runat="server" Text="Society Details"></asp:LinkButton>
                    </li>
                     <li class="nav-item">
                        <asp:LinkButton ID="nav_applications" OnClick="nav_applications_Click" CssClass=" nav-link menuwhite" runat="server" Text="Amendment Applications"></asp:LinkButton>
                    </li>   
                </ul>
                <asp:LinkButton ID="view_applicant_logout" OnClick="view_applicant_logout_Click" CssClass="nav-link menuwhite my-0 my-sm-0 " runat="server"><em class="fa fa-power-off mr-1"></em>Logout</asp:LinkButton>

            </div>

        </nav>
        <br />
        <div class="container-fluid special mt-5">
            <div class="row">
                <div class="col-6 border">
                    <br />
                   

                    <div runat="server" id="Documents" visible="true" >

                         <div class="card" style="width: 100%;">
                         <h6 class="card-header" style="font-weight:bold">Society Details</h6>
                         <div class="card-body" style="font-size:15px;">
                             <asp:Label runat="server" ID="lbl_registration_id" CssClass="control-label" Text="Registration No:" Font-Bold="true"></asp:Label>
                              <asp:Label runat="server" ID="value_registration_id" CssClass="control-label" Text=""></asp:Label><br />
                             <asp:Label runat="server" ID="lbl_registration_date" CssClass="control-label" Text="Registration Date" Font-Bold="true"></asp:Label>
                             <asp:Label runat="server" ID="value_registration_date" CssClass="control-label" Text=""></asp:Label><br />
                             <asp:Label runat="server" ID="lbl_society_name" CssClass="control-label" Text="Society Name:" Font-Bold="true"></asp:Label>
                              <asp:Label runat="server" ID="value_society_name" CssClass="control-label" Text=""></asp:Label><br />
                               <asp:Label runat="server" ID="lbl_society_address" CssClass="control-label" Text="Society Address:" Font-Bold="true"></asp:Label>
                               <asp:Label runat="server" ID="value_society_address" CssClass="control-label" Text=""></asp:Label><br />
                       
                       
                    </div>
                </div>
                        <br />
                        <div class="table-responsive">
                            <div class="table table-bordered table-hover">
                                <table style="width: 100%; font-size:14px;">
                                    <tbody>                                       
                                       
                                        <tr>
                                            <td colspan="2">
                                                <div class="box card-header mydiv">
                                                    <strong>Mandatory Documents</strong>
                                                </div>
                                            </td>
                                        </tr>

                                       
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="Label1" runat="server" Text="1) Original Bye-Laws"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="Amend_ImgBtnViewPdf1" runat="server" OnClick="Amend_ImgBtnViewPdf1_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                        <tr> 
                                            <td class="a">
                                                <asp:Label ID="Label2" runat="server" Text="2) Statement of Changes"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="Amend_ImgBtnViewPdf2" runat="server" OnClick="Amend_ImgBtnViewPdf2_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="Label3" runat="server" Text="3) AGM Notice"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="Amend_ImgBtnViewPdf3" runat="server" OnClick="Amend_ImgBtnViewPdf3_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                   
                                       
                                          <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl1" runat="server" Text="4) Support of AGM Resolution"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="Amend_ImgBtnViewPdf4" runat="server" OnClick="Amend_ImgBtnViewPdf4_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl2" runat="server" Text="5) Amendment Bye-Laws"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="Amend_ImgBtnViewPdf5" runat="server" OnClick="Amend_ImgBtnViewPdf5_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="lbl3" runat="server" Text="6) Revised Version"></asp:Label></td>
                                            <td>
                                                <asp:ImageButton ID="Amend_ImgBtnViewPdf6" runat="server" OnClick="Amend_ImgBtnViewPdf6_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
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
                                                  <asp:GridView ID="GridView_AddAmendDocs" Width="100%" runat="server" CellPadding="5" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" CssClass="Grid" PagerStyle-CssClass="pgr" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" BackColor="White">
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
                                                        <asp:TemplateField Visible="false">
                                                            <ItemTemplate>
                                                                <asp:BoundField HeaderText="ObjectID" DataField="object_id">
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="View">
                                                            <ItemTemplate>
                                                                <%--<asp:LinkButton ID="LbView" CausesValidation="false" runat="server" OnClick="LbView_Click"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton>--%>
                                                                 <asp:ImageButton ID="View_adddocs" runat="server" OnClick="View_adddocs_Click2" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>
                                                                 <%--<asp:LinkButton ID="LbView" CausesValidation="false" runat="server" OnClick="LbView_Click"><i class="fa fa-file-pdf" aria-hidden="true"></i>VIEW</asp:LinkButton>--%>
                                    
                                                               
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
                                            <textarea id="outputtextremarks" name="outputtextremarks" visible="true" maxlength="1000" runat="server" rows="3" style="width: 100%"></textarea>
                                        </td>
                                    </tr>
                                      

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
                                        <tr>
                                            <td colspan="2">
                                                  <asp:LinkButton ID="sendobservation" OnClick="sendobservation_Click" CssClass="btn btn-warning" Visible="false" runat="server"><span class="btn-label"><i class="fa fa-times"></i></span>Send Observation</asp:LinkButton>
                                                 <asp:LinkButton ID="Amendaccepted" OnClick="Amendaccepted_Click" CssClass="btn btn-primary" Visible="false" runat="server" Style="float:right"><span class="btn-label"><i class="fa fa-check"></i></span>Accept</asp:LinkButton>
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
                                 <asp:GridView ID="gvhistory" runat="server" EditRowStyle-HorizontalAlign="Left" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%" CellPadding="5">
                                     <Columns>
                                        <asp:TemplateField HeaderText="Edit" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lbAppID" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
<%--                                        <asp:BoundField DataField="groomname" HeaderText="Groom Name" />
                                        <asp:BoundField DataField="bridename" HeaderText="Bride Name" /> --%>                                   
                                        <asp:BoundField DataField="obsremarks_bydro" HeaderText=" Society Remarks by the Registrar  " />
                                        <%--<asp:BoundField HeaderText=" Members Remarks by the Dealing Hand  " />--%>
                                       <%-- <asp:BoundField DataField="remarks_sendobservation" HeaderText=" Observation by DRO " />--%>                                        
                                        <asp:BoundField DataField="submittime_obsremarks" HeaderText="Observation Made at" />                                        
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
                            <button type="button" class="btn btn-primary left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="save_obs_modal_confirm_button" CssClass="right btn btn-success" runat="server">Confirm</asp:LinkButton>
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
                            <button type="button" class="btn btn-primary left" data-dismiss="modal">Close</button>
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
                            <button type="button" class="btn btn-primary left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="modal_pull_application_button"  CssClass="right btn btn-success" runat="server">Confirm</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
       
           <div class="bs-example">
            <!-- Modal HTML -->
            <div id="AmendAccepted" class="modal fade">
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
                            <button type="button" class="btn btn-primary left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="acceptapplicationmodalclick" OnClick="acceptapplicationmodalclick_Click"  CssClass="right btn btn-success" runat="server">Confirm</asp:LinkButton>
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
                                <asp:Label ID="Label66" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
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
                            <asp:Button ID="addobservationtotextarea" CssClass="btn btn-primary" runat="server" Text="Add" />
                            <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
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
                                        <tr id="soc_modal" runat="server">
                                            <td>

                                                <div class="box card-header mydiv">
                                                    <strong>Society Observation</strong>
                                                </div>

                                            </td>
                                            <td>
                                                <asp:Label ID="socmodal" runat="server" Text="Label"></asp:Label>


                                            </td>
                                        </tr>
                                        <tr id="members_modal" runat="server">
                                            <td>

                                                <div class="box card-header mydiv">
                                                    <strong>Member Observation</strong>
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
                                <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
