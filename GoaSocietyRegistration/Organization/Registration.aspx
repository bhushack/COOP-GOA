<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" EnableEventValidation="false"  Inherits="GoaSocietyRegistration.Registration" %>

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
    <title>Society Registration Certificate</title>
    
    <link href="../assets/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="shortcut icon" href="../assets/images/favicon.ico" />
    <style>
        .box {
            background-color: lightgrey;
            width: auto;
            border: 3px solid #7460ee;
            padding: 5px;
        }

        .myborder {
        }

        .mydiv {
            float: right !important;
        }

        p {
            /*word-spacing:5px;*/
            text-align: justify;
        }

        .namecenter {
            margin: 30px;
        }

        .alert {
            font-size: 14px;
            width: 100%;
        }
     

        myclass {
            padding: 50px;
        }

    </style>
     <script>
        function Popup(url) {
            window.open(url, "myWindow", "status = 1, height = 600, width = 800, resizable = 0")
        }
    </script>
    <script>
        function printContent(el) {
            var restorepage = $('body').html();
            var printcontent = $('#' + el).clone();
            $('body').empty().html(printcontent);
            window.print();
            $('body').html(restorepage);
        }
    </script>
     <script type="text/javascript">
  $(document).ready(function() {
      window.history.pushState(null, "", window.location.href);        
      window.onpopstate = function() {
          window.history.pushState(null, "", window.location.href);
      };
  });
</script>
</head>
<body style="width: 100%; overflow: auto;">
    <form id="form1" runat="server">       
      
        <div id="print" runat="server" class="container  myborder" style="text-align: center; border: none 1px black; margin-top: 4%; border-width: 5px; border-style: double;">

            <div class="row" style="padding-bottom: 5%;">

                <div align="center" runat="server">
                    <fieldset style="width: 90%; height:100%; margin-top: 20px;">

                        <br />
                        <div class="col-12 text-center" style="border: solid black 1px; border-radius: 20px; padding-top: 10px; padding-bottom: 10px;">
                            <asp:HiddenField ID="HiddenField1" runat="server" />
                            <asp:Label ID="Label1" runat="server" Style="font-size: 22px; font-family:Calibri" Text="Government of Goa"></asp:Label><br />
                            <asp:Label ID="lbofficeName" Font-Bold="true" Style="font-size: 26px; font-family:Calibri" runat="server" Text=""></asp:Label><br />
                            <asp:Label ID="lbofficeaddress" runat="server" Style="font-size: 22px; font-family:Calibri" Text=""></asp:Label><br />
                            <b style="font-size: 22px; font-family:Calibri">Off Tel No.&nbsp;</b><asp:Label ID="lbOfficeTelNo" runat="server" Style="font-size: 22px; font-family:Calibri" Text=""></asp:Label>
                            <b style="font-size: 22px; font-family:Calibri">Email:-</b><asp:Label ID="lbEmail" Font-Underline="true" Style="font-size: 22px; font-family:Calibri" runat="server" Text=""></asp:Label><br />
                            <asp:Label ID="Label2" runat="server" Style="font-size: 22px; font-family:Calibri" Text="<b>Website:-</b> www.registration.goa.gov.in"></asp:Label>

                        </div>

                        <br />
                        <br />
                        <img src="../Goa.png" class="img-fluid" style="display: inline-block; left: 50%; width: 170px" />
                        <br />
                        <br />
                        <asp:Label ID="Label3" runat="server" Style="text-align: center; font-size: 28px; font-family:Calibri; text-transform: uppercase" Font-Bold="true" Font-Underline="true" Text="certificate of registration"></asp:Label><br />
                        <asp:Label ID="Label4" runat="server" Style="font-size: 22px; font-family:Calibri" Text="(See Rule 5)"></asp:Label><br />
                        <br />
                        <asp:Label ID="Label5" runat="server" Font-Bold="true" Style="font-size: 26px; font-family:Calibri" Text="(The Societies Registration Act, 1860)"></asp:Label><br />
                        <asp:Label ID="Label6" runat="server" Style="font-size: 22px; font-family:Calibri" Text="(Central Act 21 of 1860)"></asp:Label><br />
                        <br />
                        <br />
                        <p>
                            <asp:Label ID="Label7" runat="server"  Style="font-size: 26px; font-family:Gabriola" Text="It is certified that the Society "></asp:Label>
                            <asp:Label ID="lbSocietyName" runat="server" Font-Bold="true" Style="font-size: 26px;  font-family:'Bookman Old Style'" Text=""></asp:Label>
                            <asp:Label ID="Label8" runat="server"  Style="font-size: 26px; font-family:Gabriola" Text="has this day been duly registered under the Societies Registration Act, 1860 (Central Act 21 of 1860)."></asp:Label>
                        </p>
                        <br />
                        <div class="col-12 text-center box" style="width:70%">
                            <asp:Label ID="Label9" runat="server" Font-Bold="true" Style="font-size: 34px;font-family:Calibri" Font-Underline="true" Text="Registered No."></asp:Label>
                            <asp:Label ID="lbRegistration" runat="server" Style="font-size: 34px; font-family:Calibri" Font-Bold="true" Font-Underline="true" Text=""></asp:Label>
                        </div>
                        <br />

                        <div class="col-12 " style="text-align: left !important">
                            <p>
                            <asp:Label ID="Label10" runat="server" Style="font-size: 22px; font-family:'Bookman Old Style'" Text="Given under my hand this day of "></asp:Label>
                            <asp:Label ID="Label11" runat="server" Style="font-size: 22px; font-family:'Bookman Old Style'" Font-Bold="true" Text=""></asp:Label>
                                </p>
                        </div>
                        <br />
                        <br />
                        <br />
                        <div class="row myclass">
                            <div align="right" class="col-12 mydiv">
                                <asp:Label ID="lbRegistrarName" runat="server" CssClass="namecenter text-capitalize" Font-Bold="true" Style="font-size: 22px; font-family:'Bookman Old Style'" Text=""></asp:Label>
                                <br />
                                 <asp:Label ID="Label15" runat="server" CssClass="text-center" Style="font-size: 22px; font-family:'Bookman Old Style'" Text=" Inspector General of Societies"></asp:Label>
                               
                                <br />
                                <asp:Label ID="lbDesignation" runat="server" CssClass="text-center" Style="font-size: 22px; font-family:'Bookman Old Style'" Text=""></asp:Label>
                            </div>
                        </div>
                        <br />
                        <div class="border border-dark " style="font-size: 16px; font-family:'Bookman Old Style'; text-align: center" role="alert" align="left">
                            <asp:Label ID="lbAmountPaid" runat="server" Text=""></asp:Label>
                            <asp:Label ID="Label12" runat="server" Text=" vide eChallan No. "></asp:Label>
                            <asp:Label ID="lbeChallanNo" runat="server" Text=""></asp:Label>
                            <asp:Label ID="Label13" runat="server" Text=" dated "></asp:Label>
                            <asp:Label ID="lbeChallanDate" runat="server" Text=""></asp:Label>
                            <asp:Label ID="Label14" runat="server" Text=" towards Processing and Registration fees. "></asp:Label>
                        </div>
                        <br />
                    </fieldset>
                </div>

            </div>
        </div>
        <br/><br/>
         <div class="container">
             <div class="row">
                 <asp:LinkButton ID="LinkButton1"   OnClientClick="printContent('print');" CssClass="btn-success btn-lg" runat="server"><i class="fa fa-print" aria-hidden="true"></i>&nbsp;Print</asp:LinkButton>
             </div>
         </div>
        <br/><br/>
    </form>
    <script src="../Scripts/jquery.min.js"></script>
    <script src="../Scripts/popper.min.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
</body>
</html>
