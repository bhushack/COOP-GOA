<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginModule.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="GoaSocietyRegistration.LoginModule"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Society Registration Portal-Login</title>
    <link rel="icon" href="../assets/images/favicon.ico" type="image/x-icon" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" /> 
    <script src="../assets/js/jquery-3.5.0.min.js"></script> 
    <link href="../Login_Assets/vendor/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <link href="../Login_Assets/css/style.default.css" rel="stylesheet" />
    <link href="../Login_Assets/css/custom.css" rel="stylesheet" />
    <link href="../Login_Assets/css/fontastic.css" rel="stylesheet" />
    <link href="../Login_Assets/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%--   <script src="../assets/js/jquery.min.js"></script>--%><!--comment this jquery and uncomment top two  -->
    <%--    <script src="../Scripts/jquery.min.js"></script>--%>
    <script src="../Scripts/popper.min.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
    <%--<script src="../Login_Assets/js/front.js"></script>--%>
    <script src="../Scripts/Script.js"></script>

    <script src="../Scripts/aes.js"></script>
    <script src="../Scripts/encrypt.js"></script>
    <link href="../assets/css/StyleSheet.css" rel="stylesheet" />

    <script type="text/javascript">
        function CoverClick() {
            if (Page_ClientValidate("LoginModule")) {
                document.getElementById("<%=lkbtnotp.ClientID %>").style.display = "none";
               
                CoverClick1();
            }
        }
    </script>
    <script type="text/javascript">
        function CoverClickLK() {
            if (Page_ClientValidate("LoginModule")) {
                document.getElementById("<%=lkbtnsubmit.ClientID %>").style.display = "none";
                CoverClick1();
            }
        }
    </script>
    <script type="text/javascript">
        function load() {
            if (Page_ClientValidate("LoginModule")) {
                document.getElementById("<%=smsresend.ClientID %>").style.display = "none";
                CoverClick1();
            }
        }
    </script>

    
 <%-- <script >

  
      jQuery(document).ready(function ($) {
          $('#<%=lkbtnotp.ClientID %>').click(function () {
              alert("1");
              var name = "test";
              $.ajax({
                  type: 'POST',
                  url: 'LoginModule.aspx/MyMethod',
                  data: JSON.stringify({ name: name }),
                  contentType: "application/json; charset=utf-8",
                  dataType: "json",
                  success: function (response) {
                      if (isNaN(response.d)) {
                          alert(response.d);
                      }
                      else {
                          alert("2")
                      }
                  },
                  failure: function (response) {
                      alert("failure");
                  },
                  error: function (response) {
                      alert("5");
                  }
              });
          });
      });

  </script>--%>

    <link href="../AssestsLogin/CSS/base.css" rel="stylesheet" media='all' />
    <link href="../AssestsLogin/CSS/extra.css" rel="stylesheet" media='all' />
    <style>
        .govBranding, #topBar .govBranding ul li a {
            padding: 6px 10px !important;
        }

        .abc, .abc:after, .abc:hover {
            background-color: #3aef50 !important;
        }
        .myoverlayLoader {
            visibility: hidden;
            position: absolute;
            left: 0px;
            top: 10%;
            width: 100%;
            height: 100%;
            text-align: center;
            z-index: 1000;
        }
    </style>
    <%--   
    <script type="text/javascript">
        if (document.layers) {
            //Capture the MouseDown event.
            document.captureEvents(Event.MOUSEDOWN);

            //Disable the OnMouseDown event handler.
            document.onmousedown = function () {
                return false;
            };
        }
        else {
            //Disable the OnMouseUp event handler.
            document.onmouseup = function (e) {
                if (e != null && e.type == "mouseup") {
                    //Check the Mouse Button which is clicked.
                    if (e.which == 2 || e.which == 3) {
                        //If the Button is middle or right then disable.
                        return false;
                    }
                }
            };
        }

        //Disable the Context Menu event.
        document.oncontextmenu = function () {
            return false;
        };
    </script>--%>
</head>
<body>
    <form id="form1" runat="server">
        <header>
            <div id="topBar" class="wrapper">
                <div class="container">

                    <div class="push-left">
                        <div class="govBranding">
                            <ul>
                                <li><a href="https://www.goa.gov.in/">गोयें सरकार </a></li>
                                <li><a lang="grt" href="https://www.goa.gov.in/">Government of Goa</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
            <div class="wrapper header-wrapper">
                <div class="container header-container">
                    <div class="logo">
                        <a href="../Default.aspx" aria-label="Go to home" class="emblem" rel="home">
                            <img class="site_logo" height="100" id="logo" src="../AssestsLogin/img/goa.png" alt="Goa State Emblem" />

                            <div class="logo-text">
                                <strong lang="grt" class="site_name_regional" style="font-weight: 700!important">Registration Of Societies</strong>
                                <h1 class="site_name_english">Department of Registration ,Goa</h1>

                            </div>
                        </a>
                    </div>

                    <%--    <div class="header-right clearfix">
                        <div class="right-content clearfix">
                            <div class="float-element">
                                <a aria-label="Digital India - External site that opens in a new window" href="http://digitalindia.gov.in/" target="_blank" title="Digital India">
                                    <img class="sw-logo" height="95" src="AssestsLogin/img/digital-india.png" alt="Digital India">
                                </a>
                            </div>
                        </div>
                    </div>--%>
                    <a class="menuToggle" href="javascript:void(0);" aria-label="Mobile Menu"><span class="icon-menu"></span><span class="tcon">Menu Toggle</span></a>
                </div>
            </div>
            <div class="menuWrapper">
                <div class="menuMoreText hide">More</div>
                <div class="container">
                    <nav class="menu">
                        <ul id="menu-header-en" class="nav clearfix">
                            <li id="menu-item-26" class="menu-item menu-item-type-custom menu-item-object-custom current-menu-item current_page_item menu-item-home menu-item-2658">
                                <a href="../Default.aspx">Home</a>
                            </li>
                            <li id="menu-item-2659" class="menu-item menu-item-type-custom menu-item-object-custom current-menu-item current_page_item menu-item-home menu-item-2658">
                                <a href="../aboutus.html">About Us</a></li>


                            <li id="menu-item-2494" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2494"><a href="../contactus.html">Contact Us</a></li>
                        </ul>
                    </nav>
                </div>
            </div>
            <div class="clearfix"></div>
            <div id="overflowMenu">
                <div class="ofMenu">
                    <ul>
                    </ul>
                </div>
                <a title="Close" href="javascript:void(0);" class="closeMenu"><span class="icon-close" aria-hidden="true"></span>Close</a>
            </div>
        </header>
        <div id="preloader">
                <div id="status">
                    <div class="status-mes"></div>
                </div>
            </div>
            <div id="cover" style="display: none;"></div>
            <div class="row">
                <div id="CoverDoubleClick" class="opac_divLoader overlayLoader" style="display: none;">                    
                    <asp:Image ID="wait" runat="server" ImageUrl="../assets/images/loading.gif" AlternateText="w a i t"
                         Style="vertical-align: middle;height:250px" />
                </div>
            </div>

        <div class="container-fluid" style="background-image: url(../Login_Assets/img/bg.jpg)">
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"  EnablePartialRendering="true" >
</asp:ScriptManager>     
            <div class="col-md-12">
                <br />
                <br />
                <br />
                <div class="col-md-4 mx-auto" style="margin-bottom: 0px !important;">
                    <div class="card border-secondary shadow">
                        <div class="card-header">
                            <h4 class="mb-0 my-2">Login for Applicant</h4>
                        </div>
                        <div class="card-body">

                            <h4>Login Id <span class="text-danger">*</span></h4>
                            <div class="form-group">
                                <%-- <label for="login-username" runat="server" CssClass="label-material">User Name</label>--%>
                                <asp:TextBox ID="txtbxcitiusername" AutoCompleteType="Disabled" CssClass="input-material" runat="server" ToolTip="Login Name" placeholder="Login Id"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvloginame" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtbxcitiusername" ErrorMessage="Enter Login Id"> </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revfirstname" runat="server" Display="Dynamic" ControlToValidate="txtbxcitiusername" ForeColor="Red" ValidationExpression="[\sa-zA-Z0-9]*$" ErrorMessage="Invalid Login Id" />
                            </div>

                            <div class="form-group">
                                <asp:Image ID="ImgCaptcha" runat="server" />
                                <%-- <label for="login-username" runat="server" CssClass="label-material">User Name</label>--%>
                                <asp:LinkButton ID="lkrefreshCaptcha" CssClass="btn btn-mini " OnClick="lkrefreshCaptcha_Click" runat="server" CausesValidation="false" Style="background-color: #6699ff; color: white"><i class="fa fa-refresh" aria-hidden="true"></i></asp:LinkButton>
                            </div>
                            <div class="form-group">
                                <asp:TextBox ID="txtbxcaptcha" MaxLength="6" AutoCompleteType="Disabled" CssClass="input-material" runat="server" ToolTip="Captcha" placeholder="Captcha"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvcaptcha" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtbxcaptcha" ErrorMessage="Enter Captcha"> </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Display="Dynamic" ControlToValidate="txtbxcaptcha" ForeColor="Red" ValidationExpression="[\sa-zA-Z0-9]*$" ErrorMessage="Invalid Captcha" />
                            </div>

                            <asp:LinkButton ID="lkbtnotp" CssClass="btn btn-primary" OnClick="lkbtnotp_Click" OnClientClick="CoverClick()" CausesValidation="false" runat="server"><i class="fa fa-sign-in" aria-hidden="true"></i>&nbsp;Get OTP</asp:LinkButton>
                            <%--  <asp:LinkButton ID="registerpage" OnClick="registerpage_Click" CssClass="btn btn-primary" CausesValidation="false" runat="server"><i class="fa fa-sign-in" aria-hidden="true"></i>&nbsp;Register</asp:LinkButton>--%>
                            <div class="form-group">
                                <%-- <label for="login-username" runat="server" CssClass="label-material">User Name</label>--%>
                                <asp:TextBox ID="txtenterotp" Visible="false" AutoCompleteType="Disabled" CssClass="input-material" runat="server" MaxLength="8" ToolTip="OTP" placeholder="OTP"></asp:TextBox>
                                <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtenterotp" ValidationExpression="[\s0-9]*$" ErrorMessage="Enter OTP"> </asp:RequiredFieldValidator>--%>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" Display="Dynamic" ControlToValidate="txtenterotp" ForeColor="Red" ValidationExpression="[0-9]*$" ErrorMessage="Invalid OTP" />
                            </div>
                            <asp:LinkButton ID="lkbtnsubmit" CausesValidation="false" CssClass="btn btn-primary" OnClick="lkbtnsubmit_Click" OnClientClick="CoverClickLK()" Visible="false" runat="server">Submit</asp:LinkButton>
                            <asp:LinkButton ID="lkbtnresend" CssClass="btn btn-primary" CausesValidation="false" OnClick="lkbtnresend_Click" Visible="false" runat="server">Re send OTP</asp:LinkButton>
                            <asp:LinkButton ID="LkforgotToken" CausesValidation="false" OnClick="LkforgotToken_Click" runat="server">Forgot Token No?</asp:LinkButton>


                        </div>
                        <asp:Label ID="lbstatus" CssClass="alert-danger" runat="server" Text=""></asp:Label>
                    </div>
                </div>
            </div>
            <br />
            <br />
            <br />
        </div>

        <div class="bs-example">
            <div class="modal" id="forgottoken" runat="server">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" >Forgot Token No</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">

                            <div class="form-group">
                                <asp:Label ID="Label2" runat="server" CssClass="col-form-label" Text="Mobile No"></asp:Label>
                                <asp:TextBox ID="TxtBxMobileNo" CssClass="form-control" MaxLength="10" AutoCompleteType="Disabled" Placeholder="Mobile Number" runat="server"></asp:TextBox>
                                <asp:HiddenField ID="HDForgotmobile" runat="server" />
                            </div>
                            <div class="form-group"">

                                <asp:Label ID="Label5" runat="server" CssClass="col-form-label" Text="Captcha"></asp:Label>
                                <asp:Image ID="ImgCaptcha1" runat="server" />
                                <asp:LinkButton ID="LinkButton4" CssClass="btn btn-mini text-center" OnClick="LinkButton4_Click" runat="server" CausesValidation="false" Style="background-color: #6699ff; color: white"><i class="fa fa-refresh" aria-hidden="true"></i></asp:LinkButton>
                                <asp:TextBox ID="TxtBxCapt" CssClass="form-control" MaxLength="6" AutoCompleteType="Disabled" Placeholder="Captcha" runat="server" style="margin-top:10px;"></asp:TextBox>


                            </div>
                            <div class="form-group">
                                <asp:LinkButton ID="LkGetOTP" CausesValidation="false" CssClass="btn btn-primary mybtn" OnClientClick="coconuts();" OnClick="LkGetOTP_Click" runat="server">Get Token No</asp:LinkButton>
                                <div id="otptoken" runat="server" visible="false">
                                    <asp:Label ID="Label3" runat="server" CssClass="col-form-label" Text="Enter OTP"></asp:Label>
                                    <asp:TextBox ID="TxtBxotpforgot" CssClass="form-control" MaxLength="8" AutoCompleteType="Disabled" Placeholder="OTP" runat="server"></asp:TextBox>
                                </div>

                            </div>

                            <div class="form-group">
                                <asp:Label ID="Label4" runat="server" CssClass="col-form-label" Visible="false" Text=""></asp:Label>
                            </div>

                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>


      <%--  <div class="bs-example">
            <div class="modal" id="" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                  <div class="modal-dialog" role="document">
                    <div class="modal-content">
                      <div class="modal-header" style="background-color:#5cb85c; color:#ffffff">                          
                        <h5 class="modal-title" style="margin-left:40%">Token No</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                          <span aria-hidden="true">&times;</span>
                        </button>
                      </div>
                      <div class="modal-body">
                          <div class="table-responsive">
                        <asp:GridView ID="gridforgottoken" runat="server" OnRowDataBound="gridforgottoken_RowDataBound" CellPadding="5" AutoGenerateColumns="false" CssClass="Grid" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sr. No">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Token No" DataField="login_id" />
                                        <asp:BoundField HeaderText="Mobile No" DataField="applicant_mobile_no" />
                                    </Columns>
                                </asp:GridView>
                              </div>
                      </div>
                      <div class="modal-footer text-center">
                        <button type="button" class="btn btn-success" data-dismiss="modal">OK</button>
                       
                      </div>
                    </div>
                  </div>
                </div>
            </div>--%>
        
        
        <div id="forgottokenlist" class="modal">

            <div class="modal-dialog modal-confirm"  role="document">
                <div class="modal-content">
                    <div class="modal-header" style="background-color:#5cb85c; color:#ffffff">                        
                        <h4 class="modal-title" >Token No</h4>
                    </div>
                    <div class="modal-body">
                        <p class="text-center">
                            <asp:GridView ID="gridforgottoken" runat="server" OnRowDataBound="gridforgottoken_RowDataBound" CellPadding="5" AutoGenerateColumns="false" CssClass="Grid" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%" >
                                <Columns>
                                   <asp:TemplateField HeaderText="Sr. No">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Token No" DataField="login_id" />
                                        <asp:BoundField HeaderText="Mobile No" DataField="applicant_mobile_no" />
                                </Columns>
                            </asp:GridView>
                        </p>
                    </div>
                    <div class="modal-footer">
                        <button id="button1" runat="server" class="btn btn-success btn-block abc" data-dismiss="modal">OK</button>
                    </div>
                </div>
            </div>
        </div>         



        <div class="bs-example">
            <!-- Modal HTML -->
            <div id="smsmodal" class="modal">
                <div class="modal-dialog  modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title">Confirmation</h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label1" runat="server" Text="Do you want to send OTP again. Click Confirm to Proceed" ForeColor="Green"></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="Button2" runat="server" class="btn btn-primary" CausesValidation="false" data-dismiss="modal" Text="Cancel"></asp:Button>
                            <asp:Button ID="smsresend" OnClick="smsresend_Click" OnClientClick="load()" CausesValidation="false" CssClass="btn btn-primary" runat="server" Text="Proceed" />
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

                    <li id="menu-item-2501" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a href="../website-policies.html">Website Policies</a></li>
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
                        <img src="../AssestsLogin/img/makeinindia.png" style="width: 127px; height: 45px" alt="Make IN India opens a new window"/></a>
                    <a href="http://www.nic.in/">
                        <img src="../AssestsLogin/img/nicLogo.png"  alt="National Informatics Centre opens a new window"/></a>
                    <a href="http://www.digitalindia.gov.in/">
                        <img src="../AssestsLogin/img/digitalIndia.png" alt="Digital India opens a new window"/></a>
                    <!-- <a href="#" class="stqc-logo"><img src="/common_utility/images/STQC-approved.png"  alt="STQC"></a> -->
                </div>
            </div>
        </div>
    </footer>
</body>


<%--<script type="text/javascript">
    $(document).ready(function () {
 
        var Firstname= "a";
        var Lastname="m";
 
        $.ajax({
            type:"GET",
            url:"LoginModule.aspx/encrypt",
            data:' {FirstName:"'+Firstname+'",LastName:"'+Lastname+'"}',
            ContentType:"application/json;  charset=utf-8",
            datatype:"json",
            async: "true",
            cache: "false",
            success: function (msg) {
                // On success      
             
                alert('<%=Session["bony"] %>');
            },
            Error: function (x, e) {
                // On Error
                alert("eror");
            }
        });
    }  )
</script>--%>
<script>
    function coconut() {

        var mobs;

        var uname = document.getElementById("<%=TxtBxMobileNo.ClientID %>").value.trim();
        var errPass = $('#<%=lbstatus.ClientID %>');
        if (uname == "") errPass.html(" Mobile No Required !");
        else {
            errPass.html("");
            mobs = CheckMobileNo("MOB");
            if (Boolean(mobs)) {


                document.getElementById("<%=TxtBxMobileNo.ClientID %>").disabled = true;

                } else {
                    enablefields();
                }
            }
        }//End ready()

</script>
<script>
    function coconuts() {

        var mobs;

        var uname = document.getElementById("<%=TxtBxMobileNo.ClientID %>").value.trim();
        var errPass = $('#<%=lbstatus.ClientID %>');
        if (uname == "") errPass.html(" Mobile No Required !");
        else {
            errPass.html("");
            mobs = CheckMobileNo("MOB");
            if (Boolean(mobs)) {


                document.getElementById("<%=TxtBxMobileNo.ClientID %>").disabled = true;

                } else {
                    enablefields();
                }
            }
        }//End ready()

</script>
<script type="text/javascript">

    function CheckMobileNo(inputIdentifier) {
        //Encrypt Credentials

        if (inputIdentifier == "MOB") {

            var key = CryptoJS.enc.Utf8.parse('<%=Session["Enc_Random"]%>');
            var iv = CryptoJS.enc.Utf8.parse('<%=Session["Enc_Vector"]%>');
            var mobileno = document.getElementById("<%=TxtBxMobileNo.ClientID %>").value.trim();
            var encryptedlogin = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(mobileno), key,
           {
               keySize: 128 / 8,
               iv: iv,
               mode: CryptoJS.mode.CBC,
               padding: CryptoJS.pad.Pkcs7
           });

            if (encryptedlogin != null && encryptedlogin != "") {

                $('#<%=HDForgotmobile.ClientID %>').val(encryptedlogin);
                    return true;
                } else {

                    return false;
                }

            }


        }
</script>
</html>
