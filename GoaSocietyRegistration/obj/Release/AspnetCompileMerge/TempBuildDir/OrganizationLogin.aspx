<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrganizationLogin.aspx.cs" Inherits="GoaSocietyRegistration.OrganizationLogin" %>

<!DOCTYPE html>

<html lang="en-US" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
     <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />

    <link href="assets/css/loginstyle.css" rel="stylesheet" />
    <link href="assets/css/font-awesome.min.css" rel="stylesheet" />
    <link href="Login_Assets/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="assets/js/jquery-3.5.0.min.js"></script>
    <%--  <script src="../assets/js/slick.js"></script>--%>

    <%--    <script src="Login_Assets/vendor/jquery/jquery.min.js"></script>--%>
    <script src="Login_Assets/vendor/popper.js/popper.min.js"></script>
    <script src="Login_Assets/vendor/bootstrap/js/bootstrap.min.js"></script>
    <link rel="icon" href="favicon.ico" type="image/gif" sizes="16x16" />
    <title>Organization Login</title>
   

  <%--  <script>
        $('.message a').click(function () {
            $('form').animate({ height: "toggle", opacity: "toggle" }, "slow");
        });
    </script>--%>

    <link href="AssestsLogin/CSS/base.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/extra.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/fontawesome.min.css" rel="stylesheet" media='all' />
    <style>
        a {
            text-decoration: none !important;
        }
        .wrap {
        	background: #50a3a2;
background: -webkit-linear-gradient(top left, #50a3a2 0%, #53e3a6 100%);
background: -moz-linear-gradient(top left, #50a3a2 0%, #53e3a6 100%);
background: -o-linear-gradient(top left, #50a3a2 0%, #53e3a6 100%);
background: linear-gradient(to bottom right, #50a3a2 0%, #53e3a6 100%);
        }

        .myclass {
            padding: 0px !important;
        }

       
        .bg-bubbles {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 1;
}
.bg-bubbles li {
  position: absolute;
  list-style: none;
  display: block;
  width: 40px;
  height: 40px;
  background-color: rgba(255, 255, 255, 0.15);
  bottom: -160px;
  -webkit-animation: square 25s infinite;
  animation: square 25s infinite;
  -webkit-transition-timing-function: linear;
  transition-timing-function: linear;
}
.bg-bubbles li:nth-child(1) {
  left: 10%;
}
.bg-bubbles li:nth-child(2) {
  left: 20%;
  width: 80px;
  height: 80px;
  -webkit-animation-delay: 2s;
          animation-delay: 2s;
  -webkit-animation-duration: 17s;
          animation-duration: 17s;
}
.bg-bubbles li:nth-child(3) {
  left: 25%;
  -webkit-animation-delay: 4s;
          animation-delay: 4s;
}
.bg-bubbles li:nth-child(4) {
  left: 40%;
  width: 60px;
  height: 60px;
  -webkit-animation-duration: 22s;
          animation-duration: 22s;
  background-color: rgba(255, 255, 255, 0.25);
}
.bg-bubbles li:nth-child(5) {
  left: 70%;
}
.bg-bubbles li:nth-child(6) {
  left: 80%;
  width: 120px;
  height: 120px;
  -webkit-animation-delay: 3s;
          animation-delay: 3s;
  background-color: rgba(255, 255, 255, 0.2);
}
.bg-bubbles li:nth-child(7) {
  left: 32%;
  width: 160px;
  height: 160px;
  -webkit-animation-delay: 7s;
          animation-delay: 7s;
}
.bg-bubbles li:nth-child(8) {
  left: 55%;
  width: 20px;
  height: 20px;
  -webkit-animation-delay: 15s;
          animation-delay: 15s;
  -webkit-animation-duration: 40s;
          animation-duration: 40s;
}
.bg-bubbles li:nth-child(9) {
  left: 25%;
  width: 10px;
  height: 10px;
  -webkit-animation-delay: 2s;
          animation-delay: 2s;
  -webkit-animation-duration: 40s;
          animation-duration: 40s;
  background-color: rgba(255, 255, 255, 0.3);
}
.bg-bubbles li:nth-child(10) {
  left: 90%;
  width: 160px;
  height: 160px;
  -webkit-animation-delay: 11s;
          animation-delay: 11s;
}
@-webkit-keyframes square {
  0% {
    -webkit-transform: translateY(0);
            transform: translateY(0);
  }
  100% {
    -webkit-transform: translateY(-700px) rotate(600deg);
            transform: translateY(-700px) rotate(600deg);
  }
}
@keyframes square {
  0% {
    -webkit-transform: translateY(0);
            transform: translateY(0);
  }
  100% {
    -webkit-transform: translateY(-700px) rotate(600deg);
            transform: translateY(-700px) rotate(600deg);
  }
}

    </style>
    <script type="text/javascript">
        window.addEventListener('keydown', function (e) { if (e.keyIdentifier == 'U+000A' || e.keyIdentifier == 'Enter' || e.keyCode == 13) { if (e.target.nodeName == 'INPUT' && e.target.type == 'text') { e.preventDefault(); return false; } } }, true);
    </script>
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
    </script>
    <script type="text/javascript">

        $(document).ready(function () {
            //Disable mouse right click
            $(document).bind("contextmenu", function (e) {
                return false;
            });

            $("form input").keypress(function (e) {
                if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
                    $('#<%=lkbtnorglogin.ClientID %>').click();

	                return false;
	            } else {
	                return true;
	            }
            });
        });
    </script>

    <script src="Scripts/aes.js"></script>
    <script src="Scripts/encrypt.js"></script>
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
                        <a href="Default.aspx" aria-label="Go to home" class="emblem" rel="home">
                            <img class="site_logo" height="100" id="logo" src="AssestsLogin/img/goa.png" alt="Goa State Emblem" />

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
                                <a href="Default.aspx" class="test0">Home</a>
                            </li>
                            <li id="menu-item-2659" class="menu-item menu-item-type-custom menu-item-object-custom current-menu-item current_page_item menu-item-home menu-item-2658">
                                <a href="aboutus.html">About Us</a></li>


                            <li id="menu-item-2494" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2494"><a href="contactus.html">Contact Us</a></li>
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
        <div class="container-fluid wrap">
            <div class="row">
                <div class="col-12 ">

                    <br />  


                        <div class="form" style="border: 1px solid; padding: 10px; box-shadow: 5px 10px 8px #3c3636;">
                            <p class="text-center"><i class="fa fa-users" aria-hidden="true" style="font-size: 35px;"></i></p>
                            <h3 class="text-center" style="margin: 0">Organization</h3>

                            <asp:TextBox ID="txtbxusername" ValidationGroup="b" AutoCompleteType="Disabled" CssClass="input-material" runat="server" ToolTip="Login Name" placeholder="Login Name"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rvf1" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtbxusername" ErrorMessage="Enter Login Name"> </asp:RequiredFieldValidator>
                            <asp:regularexpressionvalidator id="revtxtbxusername" runat="server" controltovalidate="txtbxusername" forecolor="red" validationexpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-za-z]{2,4}|[0-9]{1,3})(\]?)$" display="dynamic" errormessage="please enter login name" />

                            <asp:TextBox ID="txtbxPassword" ValidationGroup="b" AutoCompleteType="Disabled" CssClass="input-material" TextMode="Password" runat="server" ToolTip="Password" placeholder="Password"></asp:TextBox>
                            <asp:HiddenField runat="server" ID="HDShaPass" />
                            <div style="display: inline">
                                <asp:Image ID="image" CssClass="imgclass" runat="server" />
                                <asp:LinkButton ID="lkrefreshCaptcha" CssClass="btn btn-mini " OnClick="lkrefreshCaptcha_Click" runat="server" CausesValidation="false" Style="background-color: #6699ff; color: white"><i class="fa fa-refresh" aria-hidden="true"></i></asp:LinkButton>
                                
                            </div>
                            <div><br /> </div>
                            <asp:TextBox ID="TextBox1" ValidationGroup="a" MaxLength="6" AutoCompleteType="Disabled" CssClass="input-material" runat="server" ToolTip="Captcha" placeholder="Captcha"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="TextBox1" ErrorMessage="Enter Captcha"> </asp:RequiredFieldValidator>

                            <asp:LinkButton ID="lkbtnorglogin" ValidationGroup="b" CssClass="btn-success btn" OnClick="lkbtnorglogin_Click" OnClientClick="disable()" runat="server"><i class="fa fa-sign-in" aria-hidden="true"></i>&nbsp;Login</asp:LinkButton>
                            <br />
                            <asp:Label ID="lberror" runat="server" CssClass="text-danger"></asp:Label>
                            <br />
                            <asp:LinkButton ID="generate_pass" OnClick="generate_pass_Click" CssClass="btnclass btn btn-info" CausesValidation="false" runat="server"><i class="fa fa-key" aria-hidden="true"></i>&nbsp;Generate Password</asp:LinkButton>
                      
                        </div>


                    
</div>

                </div>
     <div class="row">
	<ul class="bg-bubbles">
		<li></li>
		<li></li>
		<li></li>
		<li></li>
		<li></li>
		<li></li>
		<li></li>
		<li></li>
	
	</ul>
</div>
        </div>

        <!-- Modal -->
        <div class="bs-example">
            <!-- Modal HTML -->
            <div id="errorModal" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label47" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label48" CssClass="control-label" runat="server"></asp:Label>

                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="redirection" CssClass="btn btn-primary" CausesValidation="false" OnClick="redirection_Click" Text="Ok" />
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
                        <li id="menu-item-2502" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a target="_blank" href="Download/faq.pdf">FAQ&nbsp;<i class="fa fa-star-o fa-spin" aria-hidden="true"></i></a></li>
                       
                        <li id="menu-item-2504" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a target="_blank" href="Download/user_manual.pdf">User Manual</a></li>
                        <li id="menu-item-2506" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2506"><a href="contactus.html">Contact Us</a></li>


                </ul>
            </div>
            <div class="copyRights">
                <div class="pd-bottom5 color-white ctnt-ownd-dis">Content Owned by Registration Department, Government of Goa</div>
                <div class="copyRightsText">
                    <p>
                        Developed and hosted by <a rel="noopener noreferrer" href="https://nicgoa.nic.in/" target="_blank">National Informatics Centre, Goa</a>,<br>
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
</body>
<script>
    $(document).ready(function () {

        $('#<%=txtbxusername.ClientID %>').click(function () {
                     this.select();
                 });

                 $('#<%=txtbxusername.ClientID %>').keyup(function (e) {
                     var code = e.keyCode || e.which;
                     if (code != '16' && code != '9')
                         $('#<%=txtbxusername.ClientID%>').attr('type', 'text');
            });



                 $('#<%=txtbxPassword.ClientID %>').click(function () {
                     this.select();
                 });

                 $('#<%=txtbxPassword.ClientID %>').change(function () {
                     var txtpassword = document.getElementById("<%=txtbxPassword.ClientID %>").value.trim();
                var errPass = $('#<%=lberror.ClientID %>');
                if (txtpassword == "") errPass.html("Required !");
                else {
                    errPass.html("");
                    EncryptUserIdentity("PW");
                }
            });
             });//End ready()

</script>
<script>
    function disable() {
        document.getElementById("<%=txtbxPassword.ClientID %>").disabled = true;
           }//End ready()

</script>
<script type="text/javascript">

    function EncryptUserIdentity(inputIdentifier) {
        //Encrypt Credentials
        var key = CryptoJS.enc.Utf8.parse('<%=Session["Enc_Random"]%>');
            var iv = CryptoJS.enc.Utf8.parse('<%=Session["Enc_Vector"]%>');
            if (inputIdentifier == "PW") {
                var txtpassword = document.getElementById("<%=txtbxPassword.ClientID %>").value.trim();
               $('#<%=HDShaPass.ClientID %>').val(SHA512('<%=Session["Sess_rndNo"]%>' + SHA512(txtpassword)));
               var rndP = Math.random().toString(36) + Math.random().toString(36) + Math.random().toString(36) + Math.random().toString(36);
               document.getElementById("<%=txtbxPassword.ClientID %>").value = rndP;
               document.getElementById("lkbtnorglogin").disabled = false;
           }
       }
</script>
    <script src='AssestsLogin/JS/menu.js'></script>
</html>
