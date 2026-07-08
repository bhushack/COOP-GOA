<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Application_registration.aspx.cs" Inherits="GoaSocietyRegistration.Application_registration" %>

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
    <title>Society Registration Application</title>
   <link href="../assets/css/bootstrap.min.css" rel="stylesheet" />
   
    <link href="../Login_Assets/css/font-awesome.min.css" rel="stylesheet" />
    <link rel="shortcut icon" href="../assets/images/favicon.ico" />
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
        window.onload = function () {
            noBack();
        }
        function noBack() {
            window.history.forward();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container myborder" id="print" style="border: none ; margin-top: 4%;">
            <fieldset style="width: 100%; margin-top: 20px; font-size:18px">
                <div class="row">
                    <div class="col-12" style="text-align: right">
                        <asp:Label ID="Label1" runat="server" Text="From:"></asp:Label><asp:Label ID="lbname" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="lbDesignation" runat="server" Text="President"></asp:Label><br />
                        <asp:Label ID="Label2" runat="server" Text="Name:"></asp:Label><asp:Label ID="lbsocname" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="lbAddress" runat="server" Text="Address: "></asp:Label><asp:Label ID="lbsocAddress" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="lbDate" runat="server" Text=""></asp:Label>
                    </div>
                    <br />
                    <div class="col-12" style="text-align: left">
                        <asp:Label ID="Label3" runat="server" Text="To,"></asp:Label><br />
                        <asp:Label ID="Label4" runat="server" Text="The Inspector, General of Societies"></asp:Label><br />
                        <asp:Label ID="Label17" runat="server" Text="U/Societies Registration Act, 1860,"></asp:Label><br />
                        <asp:Label ID="Label5" runat="server" Text="District Registrar"></asp:Label>&nbsp;<asp:Label ID="lbDistrict" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="lbPlace" runat="server" Text=""></asp:Label><asp:Label ID="Label7" runat="server" Text="Goa"></asp:Label>
                    </div>
                    <br />
                    <div class="col-12" style="text-align: center">
                        <asp:Label ID="Label6" runat="server" Font-Bold="true" Text="Subject:"></asp:Label><asp:Label ID="Label8" runat="server" Text=" Application for Registration of "></asp:Label>
                        <asp:Label ID="lbsonamee" runat="server" Text=""></asp:Label><asp:Label ID="Label9" runat="server" Text=" under Societies Registration Act, 1860"></asp:Label><br />
                    </div>
                    <br />
                    <div class="col-12">
                        <asp:Label ID="Label10" runat="server" Text="Sir,"></asp:Label><br /><br />
                        <asp:Label ID="Label11" runat="server" Text="Please find enclosed herewith the under mentioned documents for the purpose of registering above mentioned Society Under Societies Regsitration Act, 1860."></asp:Label><br />
                        <asp:Label ID="Label12" runat="server" Text="1 ) Memorandum of Association"></asp:Label><br /><br />
                        <asp:Label ID="Label13" runat="server" Text="2 ) Correct copy of Rules and Regulations / Constitution"></asp:Label><br /><br />
                    </div>
                    <div class="col-12">
                        <asp:Label ID="Label14" runat="server" Text="Thanking You,"></asp:Label><br /><br />
                    </div><br /><br />
                    <div class="col-12" style="text-align: right">
                        <asp:Label ID="Label15" runat="server" Text="Yours faithfully"></asp:Label><br /><br /><br /><br />
                        <asp:Label ID="lbappName" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="lbDesignations" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="Label16" runat="server" Text="Phone No: "></asp:Label><asp:Label ID="lbphone" runat="server" Text=""></asp:Label><br />
                    </div>
                </div>
            </fieldset>
        </div>
        
        <br/><br/>
         <div class="container">
         
                  <div style='float: left;'>
                  <%-- <asp:LinkButton ID="LinkButton2"   CssClass="btn-success btn-lg" OnClick="LinkButton2_Click" runat="server"><i class="fa fa-arrow-left" aria-hidden="true"></i>&nbsp;Back</asp:LinkButton>--%>
               </div>
                  <div style='float: right;'>
                       <asp:LinkButton ID="LinkButton1"   OnClientClick="printContent('print');"  CssClass="btn-success btn-lg " runat="server"><i class="fa fa-print" aria-hidden="true"></i>&nbsp;Print</asp:LinkButton>
              </div>
             
         </div>
        <br /><br />
    </form>
    
    <script src="../assets/js/jquery.min.js"></script>
    <script src="../assets/js/popper.min.js"></script>
    <script src="../assets/js/bootstrap.min.js"></script>
</body>
</html>
