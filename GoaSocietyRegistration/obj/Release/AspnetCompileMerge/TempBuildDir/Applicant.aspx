<%@ Page Language="C#" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="Applicant.aspx.cs" Inherits="GoaSocietyRegistration.Applicant" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Society Registartion</title>
   <link rel="icon" href="assets/images/favicon.ico" type="image/x-icon" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <link href="assets/css/bootstrap.min.css" rel="stylesheet" />
   <%-- <link href="assets/css/font-awesome/css/all.css" rel="stylesheet" />--%>
    <link href="assets/css/style.css" rel="stylesheet" />
    <link href="assets/style.css" rel="stylesheet" />
   <script src="assets/js/jquery.min.js"></script>
 
    <script src="Scripts/Script.js"></script>
    
    <link href="assets/css/StyleSheet.css" rel="stylesheet" />

    <link href="AssestsLogin/CSS/base.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/extra.css" rel="stylesheet" media='all' />
    
     <link href="Login_Assets/vendor/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
     <link href="Login_Assets/css/custom.css" rel="stylesheet" />
     


    <script type="text/javascript">

        //$(document).ready(function()
        $(document).ready(function () {
            if ("<%=IsPostBack%>" == "False") {
                $('#myModal').modal({ backdrop: 'static' });
            }
        });
    </script>
       <script type="text/javascript">
        function load1() {
            if (Page_ClientValidate("Applicant")) {
                document.getElementById("<%=btnGenOTP.ClientID %>").style.display = "none";
                CoverClick1();
            }
        }
    </script>
    <script type="text/javascript">
        function load() {
            if (Page_ClientValidate("Applicant")) {
                document.getElementById("<%=btnOTPSubmit.ClientID %>").style.display = "none";
                CoverClick1();
            }
        }
    </script>
    <style type="text/css">
        .bs-example {
            margin: 20px;
        }

        .a {
            width: 30% !important; 
        }

        .b {
            width: 70%;
        }
        .imga{
            width:150px;
            margin-top:2%
        }

 
    </style>

     <script type="text/javascript">
        function name_changed(val) {
          
            if (val == "txtAppName") {                
                
                var mytext=document.getElementById("<%=txtAppName.ClientID %>").value;
                var newText = mytext.replaceAll('\'', '`');           
                 document.getElementById("<%=txtAppName.ClientID %>").value = newText;
               
            }         
           
             
        }
    </script>

</head>

<body>
    <form id="form1" runat="server">
        <div class="container-fluid" style="background-color: #ffffff; color: #000; border-bottom: 1px solid #C0C0C0;">
            <nav class="navbar navbar-expand-sm navbar-light bg-faded">

                <div class="col-xs-12 col-lg-3 col-md-3 col-sm-3 text-center" style="color: #000">
                    <a class="navbar-brand" href="#">
                        <img src="assets/images/Goa.png" style="width: 85px" /></a>
                </div>
                <div class="col-xs-12 col-lg-6 col-md-6 col-sm-6 text-center" style="color: #000">
                    <h2>Registration Of Societies</h2>
                    <h5>(The Societies Registration Act, 1860)</h5>
                    <h6>(Central Act 21 of 1860)</h6>
                </div>
                <div class="col-xs-12 col-lg-3 col-md-3 col-sm-3 text-center" style="color: #000">
                   <%-- <img src="assets/images/make-in-india-header.png" />--%>
                </div>
            </nav>

        </div>
        <div class="menuWrapper">
                <div class="menuMoreText hide">More</div>
                <div class="container">
                    <nav class="menu">
                        <ul id="menu-header-en" class="nav clearfix">
                            <li id="menu-item-26" class="menu-item menu-item-type-custom menu-item-object-custom current-menu-item current_page_item menu-item-home menu-item-2658" style="float:left">
                                <a href="Default.aspx"><i class="fa fa-home" aria-hidden="true"></i>&nbsp Home</a>
                            </li>
                            <li id="menu-item-2659" class="menu-item menu-item-type-custom menu-item-object-custom current-menu-item current_page_item menu-item-home menu-item-2658" style="float:left">
                                <a href="User/LoginModule.aspx"><i class="fa fa-sign-in" aria-hidden="true"></i>&nbsp Login</a></li>

                            <li id="menu-item-2659" class="menu-item menu-item-type-custom menu-item-object-custom current-menu-item current_page_item menu-item-home menu-item-2658">
                                <a href="aboutus.html">About Us</a></li>


                            <li id="menu-item-2494" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2494"><a href="contactus.html">Contact Us</a></li>


                            
                        </ul>
                    </nav>
                </div>
            </div>

        
        <br />
        <br />

        <div class="container">
            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card" style="width: 100%">
                        <h4 class="card-header">Applicant Details ::</h4>
                        <div class="card-body" style="min-height:300px !important">
                            <div class="form-group">
                                <asp:Label ID="lblimpnotice" runat="server" ForeColor="Red"> महत्वपूर्ण लेख /Important Note: [*] किये हुये क्षेत्र अनिवार्य है</asp:Label><br />
                                <asp:Label ID="Label6" runat="server" ForeColor="Red">The fields marked with (*) are mandatory.</asp:Label>
                            </div>                           
                          
                            <div class="row" id="fetchdataforrenewal" runat="server">
                                <div class="table-responsive">
                                    <div class="table table-bordered">
                                        <table style="width: 100%">
                                            <tbody>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="Label1" runat="server" Text="Select Application Type" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span></td>
                                                    <td class="b">
                                                       <asp:RadioButtonList ValidationGroup="a" ID="Rdbtn_neworrenew" RepeatDirection="Horizontal" Width="100%" OnSelectedIndexChanged="Rdbtn_neworrenew_SelectedIndexChanged" AutoPostBack="true" runat="server" Font-Size="Medium">
                                                            <asp:ListItem Value="1" Text="New" Selected="True"></asp:ListItem>
                                                          <%--<asp:ListItem Value="2" Text="Renewal"></asp:ListItem>--%>
                                                        </asp:RadioButtonList>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="Rdbtn_neworrenew" ErrorMessage="Please Select an Option"> </asp:RequiredFieldValidator><br />
                                                      </td>
                                                </tr>
                                               
                                                
                                                <tr id="regnodetails" runat="server" visible="false">
                                                    <td class="a">
                                                       <asp:Label ID="lblregno" runat="server" Text="Previous Registration No" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                     </td>
                                                    <td class="b">
                                                       <asp:TextBox ID="TxtBxOldRegNo" CssClass="form-control form-group uppercase" ToolTip="Previous Registration Number" placeholder="Previous Registration No." MaxLength="50" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                                                        
                                                    </td>
                                                </tr>
                                                 <tr id="regdistdetails" runat="server" visible="false">
                                                    <td class="a">
                                                       <asp:Label ID="lbldistrict" runat="server" Text="Registered District" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                     </td>
                                                    
                                                    <td class="b">
                                                        <asp:DropDownList ID="ddlregdistrict" runat="server" ValidationGroup="a" AutoPostBack="true" CssClass="form-control"></asp:DropDownList>
                                                       <asp:RequiredFieldValidator InitialValue="0" ID="RequiredFieldValidator5" Display="Dynamic" runat="server" ControlToValidate="ddlregdistrict" CssClass="text-danger" ErrorMessage="Please Select District"></asp:RequiredFieldValidator>
                                                   </td>
                                                </tr>
                                                 <tr id="regsocnamedetails" runat="server" visible="false">
                                                    <td class="a">
                                                       <asp:Label ID="Label13" runat="server" Text="Registered Society Name" CssClass="control-label" Font-Bold="true" ></asp:Label><span class="text-danger"></span>
                                                     </td>
                                                    
                                                    <td class="b">
                                                        <asp:TextBox ID="TxtBxOldSocName" CssClass="form-control form-group" MaxLength="50" autocomplete="off" runat="server" Enabled="false"></asp:TextBox>
                                                     </td>
                                                </tr>
                                                <tr id="regdatedetails" runat="server" visible="false">
                                                    <td class="a">
                                                       <asp:Label ID="Label14" runat="server" Text="Registered Date" CssClass="control-label" Font-Bold="true" ></asp:Label><span class="text-danger">*</span>
                                                     </td>
                                                    
                                                    <td class="b">
                                                        <asp:TextBox ID="TxtBxOldRegDate" CssClass="form-control form-group" MaxLength="50" autocomplete="off" runat="server" Enabled="false"></asp:TextBox>
                                                     </td>
                                                </tr>
                                                   
                                                <tr id="regdetailsbtn" visible="false" runat="server">
                                                    <td></td>
                                                    <td>
                                                        
                                                        <asp:LinkButton ID="BtnRegDetails"  ValidationGroup="a"  CssClass="btn btn-primary" runat="server" CausesValidation="false" OnClick="BtnRegDetails_Click" Visible="false"> Fetch</asp:LinkButton>
                                                        <asp:LinkButton ID="BtnProceed"  ValidationGroup="a"  CssClass="btn btn-primary" runat="server" CausesValidation="false" OnClick="BtnProceed_Click" Visible="false"> Proceed</asp:LinkButton>
                                                        <asp:LinkButton ID="btnClear"  ValidationGroup="a"  CssClass="btn btn-danger" runat="server" CausesValidation="false" OnClick="btnClear_Click" Visible="false"> Clear</asp:LinkButton>
                                                                                                            
                                                        <br />
                                                        <asp:Label runat="server" ID="lblRegdetailsError" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                    </td>
                                                </tr>

                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                                    
                                
                            <div class="row" id="applicant" runat="server" visible="true">
                                <div class="table-responsive">
                                    <div class="table table-bordered">
                                        <table style="width: 100%">
                                            <tbody> 
                                                                                   
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="LBName" runat="server" Text="Applicant Name:" CssClass="control-label" Font-Bold="true" ></asp:Label><span class="text-danger">*</span></td>
                                                    <td class="b">
                                                        <asp:TextBox ID="txtAppName" ValidationGroup="a" runat="server" autocomplete="off" AutoCompleteType="Disabled" placeholder="Full Name" MaxLength="50" CssClass="form-control" ToolTip="Applicant Name" onkeyup="name_changed('txtAppName');"> </asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvfname" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtAppName" ErrorMessage="Enter Applicant Name"> </asp:RequiredFieldValidator><br />
                                                          <asp:RegularExpressionValidator ID="revfirstname" runat="server" Display="Dynamic" ControlToValidate="txtAppName" ForeColor="Red" ValidationExpression="[\sa-zA-Z`]*$" ErrorMessage="Invalid Name" /><br />
                                                        <asp:Label ID="Label3" runat="server" Font-Bold="true" Font-Size="Smaller" Text="Name should be entered as per any Valid Proof/Certificate." CssClass="control-label"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbDesignation" runat="server" Text="Applicant Designation" CssClass="control-label" Font-Bold="true" ></asp:Label><span class="text-danger">*</span></td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlapplicantdesign" ValidationGroup="a" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator2" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="ddlapplicantdesign" ErrorMessage="Please Select Designation"> </asp:RequiredFieldValidator>
                                                        <asp:Label ID="Label15" runat="server" Font-Bold="true" Font-Size="Smaller" Text="The Selected Applicant Designation will be the Signing Authority" CssClass="control-label"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbAddress" runat="server" Text="Applicant Address" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtAppAddress" runat="server" ValidationGroup="a" autocomplete="off" AutoCompleteType="Disabled" placeholder="Address" MaxLength="200" CssClass="form-control" ToolTip="Address"> </asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RfvAddress" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtAppAddress" ErrorMessage="  Enter Address"> </asp:RequiredFieldValidator>
                                                     <asp:RegularExpressionValidator ID="revbuilding" runat="server" ControlToValidate="txtAppAddress" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\sa-zA-Z0-9()-,._/:@-]+$" ErrorMessage="No special characters allowed." />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbDistrict" runat="server" Text="Applicant District:" CssClass="control-label" Font-Bold="true" ></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlAppDistrict" runat="server" ValidationGroup="a"  CssClass="form-control"></asp:DropDownList>
                                                        <asp:HiddenField ID="HFDistrictID" runat="server" />
                                                        <asp:RequiredFieldValidator InitialValue="-1" ID="rfvddlapplicant" Display="Dynamic" runat="server" ControlToValidate="ddlAppDistrict" CssClass="text-danger" ErrorMessage="Please Select District"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbMobile" runat="server" Text="Applicant Mobile No:" CssClass="control-label" Font-Bold="true" ></asp:Label><span class="text-danger">*</span></td>
                                                    <td>
                                                        <asp:TextBox ID="txtAppMobileNo" runat="server" ValidationGroup="a" autocomplete="off" AutoCompleteType="Disabled" placeholder="Mobile No" MaxLength="10" CssClass="form-control" ToolTip="Enter Mobile No without +91 and 0"> </asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvpmobile" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtAppMobileNo" ErrorMessage="Enter Mobile No"> </asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revpmobile" runat="server" ControlToValidate="txtAppMobileNo" CssClass="text-danger" ErrorMessage="Please Enter Correct Mobile No." ValidationExpression="[6-9][0-9]{9}"></asp:RegularExpressionValidator><br />
                                                        <asp:Label ID="Label5" runat="server" Font-Bold="true" Font-Size="Smaller" Text="Applicant should enter his/her personal mobile number correctly since all communication related to application" CssClass="control-label"></asp:Label>
                                                        <asp:Label ID="Label4" runat="server" Font-Bold="true" Font-Size="Smaller" Text="shall be sent on this mobile number only." CssClass="control-label"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbEmail" runat="server" Text="Applicant Email:" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span></td>
                                                    <td>
                                                        <asp:TextBox ID="TxtBxEmailaddress" runat="server" ValidationGroup="a" autocomplete="off" AutoCompleteType="Disabled" placeholder="Email" MaxLength="100" CssClass="form-control" ToolTip="Email"> </asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvemail" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="TxtBxEmailaddress" ErrorMessage="Enter Email"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="reemail" runat="server" ControlToValidate="TxtBxEmailaddress" ForeColor="Red" ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" Display="Dynamic" ErrorMessage="Invalid email address" />
                                                       <br /> <asp:Label ID="Label2" runat="server" Font-Bold="true" Font-Size="Smaller" Text="Applicant should enter his/her personal Email address correctly since all communication related to application" CssClass="control-label"></asp:Label>
                                                        <asp:Label ID="Label12" runat="server" Font-Bold="true" Font-Size="Smaller" Text="shall be sent on this email only." CssClass="control-label"></asp:Label>
                                                    </td>
                                                </tr>
                                                 <tr>
                                                    <td class="a">
                                                        <asp:Label ID="Label16" runat="server" Text="Is Government Soceity" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span></td>
                                                    <td class="b">
                                                       <asp:RadioButtonList ValidationGroup="a" ID="Rdbtn_govtornot" RepeatDirection="Horizontal" Width="100%"  runat="server" Font-Size="Medium">
                                                            <asp:ListItem Value="0" Text="No" Selected="True"></asp:ListItem>
                                                          <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="Rdbtn_neworrenew" ErrorMessage="Please Select an Option"> </asp:RequiredFieldValidator><br />
                                                      </td>
                                                </tr>
                                                <tr>
                                                    <td> 
                                                        <asp:Image ID="ImgCaptcha" runat="server" />
                                                         <asp:LinkButton ID="lkrefreshCaptcha"  CssClass="btn btn-mini " OnClick="lkrefreshCaptcha_Click" runat="server" CausesValidation="false" Style="background-color: #6699ff; color: white"><i class="fa fa-refresh" aria-hidden="true"></i></asp:LinkButton>
                                                        <%--<asp:ImageButton ID="btnrefresh" OnClick="btnrefresh_Click" CausesValidation="False" runat="server" ImageUrl="~/assets/images/refresh.png" Width="30px" Height="30px" />--%>
                                                    </td>
                                                    <td><asp:TextBox ID="txtbxcaptcha" ValidationGroup="a" AutoCompleteType="Disabled" MaxLength="6"  CssClass="form-control" runat="server" ToolTip="Captcha" placeholder="Captcha"></asp:TextBox>                                        
                                                    <asp:RequiredFieldValidator ID="rfvcaptcha" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtbxcaptcha" ErrorMessage="Enter Captcha"> </asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>                                                        
                                                        <asp:LinkButton ID="btnGenOTP" ValidationGroup="a" CssClass="btn btn-primary" runat="server" CausesValidation="false" OnClick="btnGenOTP_Click"> Generate OTP</asp:LinkButton>
                                                        <asp:Label runat="server" ID="lblError" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                    </td>
                                                </tr>

                                                <tr id="divOTP" visible="false" runat="server">
                                                    <td>
                                                        <asp:Label ID="Label11" runat="server" CssClass="control-label" Font-Bold="true" Text="Enter OTP:" ></asp:Label><span class="text-danger">*</span></td>
                                                    <td>
                                                        <asp:TextBox ID="txtOTP" ValidationGroup="b" runat="server" MaxLength="8"  autocomplete="off" AutoCompleteType="Disabled" placeholder="OTP" CssClass="form-control" ToolTip="OTP"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" CssClass="text-danger pure-form-message-inline" ControlToValidate="txtOTP" ErrorMessage="Enter OTP"> </asp:RequiredFieldValidator>

                                                    </td>
                                                </tr>
                                                
                                                <tr id="divOTPSubmit" visible="false" runat="server">
                                                    <th></th>
                                                    <th>
                                                        <asp:LinkButton ID="btnOTPSubmit" ValidationGroup="b" CssClass="btn btn-primary" runat="server" CausesValidation="false" OnClick="btnOTPSubmit_Click" Visible="false">Submit</asp:LinkButton>
                                                                                                               
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:Label ID="lbstatus" ForeColor="Red" runat="server" Text=""></asp:Label>
                                                    </td>
                                                </tr>

                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>                                   


                            <div id="info" runat="server" class="form-group" visible="false">
                                <asp:Label ID="Label7" runat="server" ForeColor="Red"> Note: आवेदकों से अनुरोध हें कि ऑनलाइन आवेदन केवल अंग्रेजी में भरे.</asp:Label><br />
                                <asp:Label ID="Label8" runat="server" ForeColor="Red">Applicants are requested to please fill all the information in online application form in English Only.</asp:Label><br />
                                <asp:Label ID="Label9" runat="server" ForeColor="Red">प्रदान की गई एसएमएस सुविधा आवेदको के लिए अतिरिक्त सुविधा होगी और एसएमएस की डिलीवरी / रसीद आश्वासित नहीं है</asp:Label><br />
                                <asp:Label ID="Label10" runat="server" ForeColor="Red">The SMS facility provided shall be additional facility for the benefit of the applicant and does not assure SMS delivery/non-receipt.</asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
     <br />
        <br />
        <div class="bs-example">
            <!-- Modal HTML -->
            <div id="myModal" class="modal fade">

                <div class="modal-dialog modal-lg modal-dialog-centered">
                    <div class="modal-content" style="border: 8px solid #d4d1d1a6;">
                        <div class="modal-header text-center">
                            <%-- <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>--%>
                            <h5 class="modal-title" style="margin: 0 auto;">IMPORTANT GUIDELINES</h5>
                        </div>
                        <div class="modal-body">
                            <div style="width: 100%; text-align: center; padding: 0px 0px 10px 0px; font-size: 15px;">
                                <p style="font-weight: bold; color: #c73232;">BEFORE YOU PROCEED FOR REGISTRATION OF SOCIETY CONFIRM THE FOLLOWING &colon; </p>
                            </div>
                            <div style="width: 85%; padding-left: 50px;">
                                <ol>
                                    <%-- <li style="padding: 0px 0px 5px 0px; font-size: 14px;">Select the Marriage Registrar Office of your jurisdiction within Goa
                                            </li>
                                            <li style="padding: 0px 0px 5px 0px; font-size: 14px;">The Acts of Marriages are Known(It can be changed by your Marriage Officer only)
                                            </li>--%>
                                    <li style="padding: 0px 0px 5px 0px; font-size: 14px;">Please ready with the following Documents at the time of filling Form &colon;
                                            <ul style="padding: 3px 0px 0px 10px; list-style-type: square; font-size: 13px;">
                                                <li style="color: #124586; padding: 0px 0px 2px 0px;">Scanned Copy of Application for Registration.
                                                </li>
                                                <li style="color: #124586; padding: 0px 0px 2px 0px;">Scanned Copy of Memorandum of Association.
                                                </li>
                                                <li style="color: #124586; padding: 0px 0px 2px 0px;">Scanned Copy of Rules And Regulation.
                                                </li>
                                                <li style="color: #124586; padding: 0px 0px 2px 0px;">Scanned Copy of any ID Proof issued by State/Centre Govt of Committee Members. 
                                                </li>
                                            </ul>
                                    </li>
                                    <li style="padding: 0px 0px 5px 0px; font-size: 14px;">Do not share <b>Password</b> or <b>OTP</b> with anyone
                                    </li>
                                </ol>
                            </div>
                        </div>
                        <div class="modal-footer" style="text-align: center;">
                            <button type="button" class="btn btn-primary btn-lg" data-dismiss="modal">Proceed</button>
                        </div>
                    </div>
                </div>
            </div>

        </div>
        <div class="bs-example">
            <!-- Modal HTML -->
            <div id="myModal1" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title">Confirmation</h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Status" runat="server" Text="" ForeColor="Green"></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <%--    <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                            <asp:LinkButton ID="RedirecttoLoginBtn" CssClass="btn btn-primary" runat="server" CausesValidation="false" OnClick="RedirecttoLoginBtn_Click">Proceed</asp:LinkButton>
                            
                        </div>
                    </div>
                </div>
            </div>
        </div>

          <%--  <div class="bs-example">
            <!-- Modal HTML -->
            <div id="OTP" class="modal fade">
                <div class="modal-dialog  modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title">Confirmation</h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label1" runat="server" Text="OTP was sent on your mobile no. Enter Captcha and try again" ForeColor="Green"></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="Button2" runat="server" class="btn btn-primary" data-dismiss="modal"></asp:Button>
                            <asp:Button ID="confirmOTP" OnClick="confirmOTP_Click" CssClass="btn btn-primary" runat="server" />

                        </div>
                    </div>
                </div>
            </div>
        </div>--%>

        <div id="cover" style="display: none;"></div>
            <div class="row">
                <div id="CoverDoubleClick" class="opac_divLoader overlayLoader" style="display: none;">
                    <asp:Image ID="wait" runat="server" ImageUrl="assets/images/loading.gif" AlternateText="w a i t"
                        Height="40%" Width="40%" Style="vertical-align: middle;" />
                </div>
            </div>
        <div class="bs-example">
            <!-- Modal HTML -->
            <div id="myModal2" class="modal fade">
                <div class="modal-dialog  modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title">Confirmation</h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="lblMSG1" runat="server" Text="" ForeColor="Green"></asp:Label>
                        </div>
                        <div class="modal-footer">
                             <asp:LinkButton ID="btnCancel" CssClass="btn btn-primary" runat="server" CausesValidation="false" data-dismiss="modal"></asp:LinkButton>
                             <asp:LinkButton ID="Button1" CssClass="btn btn-primary" runat="server" CausesValidation="false" OnClick="RedirecttoLoginBtn_Click"></asp:LinkButton>
                           
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
        <footer id="footer" class="footer-home">
        <div class="container">
            <div class="footerMenu">
                <ul id="menu-footer-en" class="menu">

                    <li id="menu-item-2501" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a href="website-policies.html">Website Policies</a></li>
                     <li id="menu-item-2500" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a href="Feedback.aspx">Feedback</a></li>
                    <li id="menu-item-2502" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a target="_blank" href="../Download/faq.pdf">FAQ&nbsp;<i class="fa fa-star-o fa-spin" aria-hidden="true"></i></a></li>

                    <li id="menu-item-2504" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a target="_blank" href="../Download/user_manual.pdf">User Manual</a></li>
                    <li id="menu-item-2506" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2506"><a href="../contactus.html">Contact Us</a></li>

                </ul>
            </div>
            <div class="copyRights">
                <div class="pd-bottom5 color-white ctnt-ownd-dis">Content Owned by Registration Department, Government of Goa</div>
                <div class="copyRightsText">
                    <p>
                        Developed and hosted by <a href="https://nicgoa.nic.in/" rel="noopener noreferrer" target="_blank">National Informatics Centre, Goa</a>,<br>
                        <a href="http://meity.gov.in/" rel="noopener noreferrer" target="_blank">Ministry of Electronics & Information Technology</a>, Government of India
                    </p>

                </div>
                <div class="copyRightsLogos">
                    <a href="#">
                        <img src="AssestsLogin/img/makeinindia.png" style="width: 127px; height: 45px" alt="Make IN India opens a new window"/></a>
                    <a href="http://www.nic.in/">
                        <img src="AssestsLogin/img/nicLogo.png" alt="National Informatics Centre opens a new window"/></a>
                    <a href="http://www.digitalindia.gov.in/">
                        <img src="AssestsLogin/img/digitalIndia.png" alt="Digital India opens a new window"/></a>
                    <!-- <a href="#" class="stqc-logo"><img src="/common_utility/images/STQC-approved.png"  alt="STQC"></a> -->
                </div>
            </div>
        </div>
    </footer>
    <script src="assets/js/jquery-3.2.1.slim.min.js"></script>
    <script src="assets/js/popper.min.js"></script>
    <script src="assets/js/bootstrap.min.js"></script>
</body>

</html>
