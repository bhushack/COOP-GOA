<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeneratePassword.aspx.cs" Inherits="GoaSocietyRegistration.Organization.GeneratePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Generate Password</title>
    <!-- fonr awesome and bootstrap css -->
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <link href="../assets/css/bootstrap.min.css" rel="stylesheet" media="all" />
    <link href="../assets/css/font-awesome.min.css" rel="stylesheet" media="all" />
    <link href="assets/css/style.css" rel="stylesheet"media="all"/>
  <link href="assets/style.css" rel="stylesheet" />
    <script src="../Scripts/jquery-3.5.0.min.js"></script>
    <script src="../Scripts/popper.min.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
    <script src="../Scripts/aes.js"></script>
    <script src="../Scripts/encrypt.js"></script>
    <style type="text/css">
        .color {
        color:white !important;
        }
    </style>
    <script type='text/javascript'>
        $(function () {
            $("#TextBox1").focus();
        });
    </script>
      <link href="../AssestsLogin/CSS/base.css" rel="stylesheet" media='all' />
    <link href="../AssestsLogin/CSS/extra.css" rel="stylesheet" media='all' />
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
                     <a href="../../Default.aspx" aria-label="Go to home" class="emblem" rel="home">
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
                            <li id="menu-item-26" class="menu-item menu-item-type-custom menu-item-object-custom current-menu-item current_page_item menu-item-home menu-item-2658 ">
                                <a href="../Default.aspx" >Home</a>
                            </li>
                            <li id="menu-item-2659" class="menu-item menu-item-type-custom menu-item-object-custom current-menu-item current_page_item menu-item-home menu-item-2658">
                                <a href="../aboutus.html" >About Us</a></li>


                            <li id="menu-item-2494" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2494"><a href="../contactus.html" >Contact Us</a></li>
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
        <br />
        <div class="container" style="min-height:400px">
            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <h5 class="card-header">Generate Password</h5>
                        <br />
                        <div class="card-body">
                            <div class="form-group row">
                                <label for="colFormLabelSm" style="font-size:15px" class="col-sm-2 col-form-label col-form-label-sm">User ID/ Login Name</label>
                                <div class="col-sm-5 col-xs-12">
                                    <asp:TextBox ID="TextBox1" Focus="true" CssClass="col-sm-12 col-xs-12 form-control form-group" runat="server" AutoCompleteType="Disabled" Placeholder="email@example.com"></asp:TextBox>                                
                                     <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TextBox1" ForeColor="Red" ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"
                                        Display="Dynamic" ErrorMessage="Invalid UserId/Login Name address" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox1" ForeColor="Red" Display="Dynamic" ErrorMessage="User ID is blank" />
                                </div>
                                <asp:LinkButton ID="resetbutton" OnClick="resetbutton_Click"  CssClass="btn btn-default col-sm-2" CausesValidation="false" runat="server" Text="Reset"><i class="fa fa-refresh" aria-hidden="true"></i>&nbsp;Reset</asp:LinkButton>
                            </div>
                            <div class="form-group row">
                                <label for="colFormCatpcha" style="font-size:15px" class="col-sm-2 col-form-label col-form-label-sm">Captcha</label>
                                <div class="col-sm-6 col-xs-12">
                                    <asp:Image ID="image1" runat="server" />
                                    <asp:LinkButton ID="LkBtnCaptchaRefresh" CssClass="btn btn-mini text-center btn-secondary" OnClick="LkBtnCaptchaRefresh_Click" runat="server" CausesValidation="false"><i class="fa fa-refresh" aria-hidden="true"></i></asp:LinkButton>
                                </div>

                            </div>
                            <div class="form-group row">                                 
                                    <label id="lblcapthca" style="font-size:15px" class="col-sm-2 col-form-label col-form-label-sm"> Enter Captcha<span class="text-danger">*</span></label>
                                    <div class="col-sm-3 col-xs-12">
                                        <asp:TextBox ID="TextBxCaptcha" MaxLength="6" CssClass="col-sm-12 col-xs-12 form-control form-group" AutoCompleteType="Disabled" Placeholder="Captcha" runat="server"></asp:TextBox>
                                        <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBxCaptcha" ForeColor="Red" Display="Dynamic" ErrorMessage="User ID is blank" />--%>
                                    </div>
                                      
                           
                                <asp:LinkButton ID="LkBtnSearch" OnClick="LkBtnSearch_Click"  CssClass="btn btn-default col-sm-2" runat="server"><i class="fa fa-search" aria-hidden="true"></i>&nbsp;Search</asp:LinkButton>
                                
                                </div>

                            <div id="showdata" runat="server" visible="false">
                                <div class="form-group row">
                                    <label for="colFormLabelSm" class="col-sm-2 col-form-label col-form-label-sm">Name</label>
                                    <div class="col-sm-3 col-xs-12">
                                        <asp:Label ID="LbName" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label id="lbMobile" class="col-sm-2 col-form-label col-form-label-sm">Mobile No.</label>
                                    <div class="col-sm-3 col-xs-12">
                                        <asp:Label ID="LbMobileNo" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label id="NewPassword" class="col-sm-2 col-form-label col-form-label-sm">New Password</label>

                                    <div class="col-sm-3 col-xs-12">
                                        <div class="input-group">
                                            <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" AutoCompleteType="Disabled" Placeholder="New Password" CssClass="form-control"></asp:TextBox>
                                          
                                            <asp:HiddenField runat="server" ID="HDShaPass" />
                                       <%--     <div class="input-group-append">
                                                <button id="show_password" class="btn btn-primary" type="button">
                                                    <span class="fa fa-eye-slash icon"></span>
                                                </button>
                                            </div>--%>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group row">
                                    <label id="ConfirmPassword" class="col-sm-2 col-form-label col-form-label-sm">Confirm Password</label>

                                    <div class="col-sm-3 col-xs-12">
                                        <asp:TextBox ID="TxtBxConfirmPass" CssClass="col-sm-12 col-xs-12 form-control form-group" TextMode="Password" AutoCompleteType="Disabled" Placeholder="Confirm Password" runat="server"></asp:TextBox>
                                     
                                        <asp:HiddenField runat="server" ID="HDShaCnfPass" />
                                    </div>
                                    <span id="cphMain_lblNote">Password should contians<br />
                                        At least one upper case letter (A – Z)<br />
                                        At least one lower case letter: (a - z)<br />
                                        At least one number: (0 - 9)<br />
                                        At least one Special Characters: ( @ ! # $ % &amp; ^ + = * ( ) - . : ? [ ] _ | { } ~ )<br />
                                        Optimum length is between 8 and 15<br />
                                        Consecutive Digits are not allowed

                                    </span>
                                    <br />
                                </div>
                                <div class="form-group row">
                                    <asp:LinkButton ID="submit_password" OnClick="submit_password_Click" OnClientClick="disables();" CssClass="btn btn-primary" runat="server" style="margin-left:25px;" >Submit</asp:LinkButton><br />
                                </div>
                            </div>

                            <asp:Label ID="Label1" ForeColor="Red" runat="server" Text=""></asp:Label>

                        </div>
                    </div>

                </div>
            </div>
        </div>
        <br /><br />
             <script type="text/javascript">
            function CheckPassword(inputtxt) {
                var passw = /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@!#$%&^+=*()-.:?[\]_|{}~])[a-zA-Z0-9@!#$%&^+=*()-.:?[\]_|{}~]{8,15}$/;
                if (inputtxt.match(passw)) {
                    if (hasConsecutiveDigits(inputtxt)) {
                        alert('Password should not contain 2 consecutive digits');
                        return false;
                    }
                    else {
                        return true;
                    }
                }
                else {
                    alert('Password should contians At least 1 upper case, 1 lower case, 1 special character and 1 number required. Optimum length is between 8 and 15');
                    return false;
                }
            };
            function isSequential(digits) {
                for (let i = 0; i < digits.length - 1; i++) {
                    if (parseInt(digits[i + 1]) !== parseInt(digits[i]) + 1) {
                        return false;
                    }
                }
                return true;
            }
            function hasConsecutiveDigits(password) {
                // Regular expression to match two consecutive digits
                const regex = /\d{2}/;
                let match;

                while ((match = regex.exec(password)) !== null) {
                    let digits = match[0];

                    // Check if the digits are sequential
                    if (isSequential(digits)) {
                        return true;
                    }
                    else {
                        return false;
                    }

                }
            }
        </script>
          <script>
           function disables() {
            
               document.getElementById("<%=txtPassword.ClientID %>").disabled = true;   
               document.getElementById("<%=TxtBxConfirmPass.ClientID %>").disabled = true;
                 
        }//End ready()

    </script>
        <script>
            $(document).ready(function () {

                $('#<%=TextBox1.ClientID %>').click(function () {
                this.select();
            });

            $('#<%=TextBox1.ClientID %>').keyup(function (e) {
                var code = e.keyCode || e.which;
                if (code != '16' && code != '9')
                    $('#<%=TextBox1.ClientID%>').attr('type', 'text');
            });

         

            $('#<%=txtPassword.ClientID %>').click(function () {
                this.select();
            });

            $('#<%=txtPassword.ClientID %>').change(function () {
                var txtpassword = document.getElementById("<%=txtPassword.ClientID %>").value.trim();
                var errPass = $('#<%=Label1.ClientID %>');
                if (txtpassword == "") errPass.html("Required !");
                else {
                    errPass.html("");
                    EncryptUserIdentity("PW");
                }
            });
            $('#<%=TxtBxConfirmPass.ClientID %>').click(function () {
                this.select();
            });

            $('#<%=TxtBxConfirmPass.ClientID %>').change(function () {
                var txtpassword = document.getElementById("<%=TxtBxConfirmPass.ClientID %>").value.trim();
                var errPass = $('#<%=Label1.ClientID %>');
                if (txtpassword == "") errPass.html("Required !");
                else {
                    errPass.html("");
                    EncryptUserIdentity("CPW");
                }
            });
        });//End ready()

        </script>
        <script type="text/javascript">

            function EncryptUserIdentity(inputIdentifier) {
                //Encrypt Credentials
                var key = CryptoJS.enc.Utf8.parse('<%=Session["Enc_Random"]%>');
            var iv = CryptoJS.enc.Utf8.parse('<%=Session["Enc_Vector"]%>');
     
             if (inputIdentifier == "PW") {
                 var txtpassword1 = document.getElementById("<%=txtPassword.ClientID %>").value.trim();
                 var temp = CheckPassword(txtpassword1);
                 if (Boolean(temp)) {
                     $('#<%=HDShaPass.ClientID %>').val(SHA512(txtpassword1));

                     var rndP = Math.random().toString(36) + Math.random().toString(36) + Math.random().toString(36) + Math.random().toString(36);
                     document.getElementById("<%=txtPassword.ClientID %>").value = rndP;
                 } else
                 {
                     document.getElementById("<%=txtPassword.ClientID %>").value = "";
                 }
            }
            else if (inputIdentifier == "CPW") {
                var txtpassword1 = document.getElementById("<%=TxtBxConfirmPass.ClientID %>").value.trim();
                var temp1 = CheckPassword(txtpassword1);
                if (Boolean(temp1)) {
                    $('#<%=HDShaCnfPass.ClientID %>').val(SHA512(txtpassword1));

                    var rndC = Math.random().toString(36) + Math.random().toString(36) + Math.random().toString(36) + Math.random().toString(36);
                    document.getElementById("<%=TxtBxConfirmPass.ClientID %>").value = rndC;
                } else {

                    document.getElementById("<%=TxtBxConfirmPass.ClientID %>").value = "";
                }
            }
}
        </script>
          <div class="bs-example">
            <!-- Modal HTML -->
            <div id="myModal" class="modal fade">
                <div class="modal-dialog  modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title">Confirmation</h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label2" runat="server" Text="" ForeColor="Green"></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="redirect_to_login" OnClick="redirect_to_login_Click" runat="server" CssClass="btn btn-success" Text="Proceed" />
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

                   
                        <%-- <li id="menu-item-2500" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a href="Feedback.aspx">Feedback</a></li>--%>
                        <li id="menu-item-2501" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a href="../website-policies.html">Website Policies</a></li>
                        <li id="menu-item-2502" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a target="_blank" href="Download/faq.pdf">FAQ&nbsp;<i class="fa fa-star-o fa-spin" aria-hidden="true"></i></a></li>
                        <%-- <li id="menu-item-2503" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a href="Guidelines.html">Guidelines</a></li>--%>
                        <li id="menu-item-2504" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a target="_blank" href="Download/user_manual.pdf">User Manual</a></li>
                        <li id="menu-item-2506" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2506"><a href="../contactus.html" >Contact Us</a></li>


                </ul>
            </div>
            <div class="copyRights">
                <div class="pd-bottom5 color-white ctnt-ownd-dis">Content Owned by Registration Department, Government of Goa</div>
                <div class="copyRightsText">
                    <p>
                        Developed and hosted by <a href="https://nicgoa.nic.in/" target="_blank">National Informatics Centre, Goa</a>,<br>
                        <a href="http://meity.gov.in/" target="_blank">Ministry of Electronics & Information Technology</a>, Government of India
                    </p>

                </div>
                <div class="copyRightsLogos">
                    <a href="#">
                        <img src="../AssestsLogin/img/makeinindia.png" style="width: 127px; height: 45px" alt="Make IN India opens a new window " /></a>
                    <a href="http://www.nic.in/">
                        <img src="../AssestsLogin/img/nicLogo.png" alt="National Informatics Centre opens a new window" /></a>
                    <a href="http://www.digitalindia.gov.in/">
                        <img src="../AssestsLogin/img/digitalIndia.png" alt="Digital India opens a new window" /></a>
                    <!-- <a href="#" class="stqc-logo"><img src="/common_utility/images/STQC-approved.png"  alt="STQC"></a> -->
                </div>
            </div>
        </div>
    </footer>  
</body>
</html>
