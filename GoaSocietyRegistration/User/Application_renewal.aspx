<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Application_renewal.aspx.cs" Inherits="GoaSocietyRegistration.User.Application_renewal" %>

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
    <title>Society Renewal Application</title>
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
            <fieldset style="width: 100%; margin-top: 20px;">
                <div class="row">
                    <div class="col-12" style="text-align: left;">
                        <asp:Label ID="Label1" runat="server" Font-Bold="true" Font-Size="Large" Text=" Application for RENEWAL OF CERTIFICATE OF REGISTRATION under SubSec.(2) of Section 3(B) of the Societies Registration Act, 1860(CENTRAL ACT 21 of 1860) as applicable to the State of Goa."></asp:Label><br />
                        <br />
                    </div>
                    <div class="col-12" style="text-align: left; margin-top:20px; font-size:medium">
                        <asp:Label ID="Label3" runat="server" Font-Bold="true" Text="Name of the Society:- "></asp:Label><asp:Label ID="lblsocname" runat="server" Text="____________________________"></asp:Label><br />
                        <asp:Label ID="Label4" runat="server" Font-Bold="true" Text="Registration No.:- "></asp:Label><asp:Label ID="lblregno" runat="server" Text="____________________________"></asp:Label><br />
                        <asp:Label ID="Label5" runat="server" Font-Bold="true" Text="Date of Registration:- "></asp:Label><asp:Label ID="lblregdate" runat="server" Text="__/__/__"></asp:Label><br />
                        <asp:Label ID="Label6" runat="server" Font-Bold="true" Text="Represented by:- "></asp:Label><asp:Label ID="lblrepname" runat="server" Text="____________________"></asp:Label><br />
                        
                    </div>
                    <br />
                    <div class="col-12" style="text-align: left; margin-top:20px; font-size:medium">
                        <asp:Label ID="Label7" runat="server" Text="To,"></asp:Label><br />
                        <asp:Label ID="Label8" runat="server" Text="THE INSPECTOR GENERAL,"></asp:Label><br />
                        <asp:Label ID="Label9" runat="server" Text="Societies Registration Act,1860"></asp:Label><br />
                       
                        <asp:Label ID="Label11" runat="server" Text="State of Goa,"></asp:Label><asp:Label ID="lbDistrict" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="lbPlace" runat="server" Text=""></asp:Label><asp:Label ID="Label12" runat="server" Text="Goa"></asp:Label>
                        
                    </div>
                    <br />
                    <div class="col-12" style="font-size:medium; margin-top:10px">
                        
                            <p>Sir,</p>
                        
                            <asp:Label ID="Label17" runat="server" Text="The applicant hereby applied for Renewal of Certificate of Registration of the above referred Society."></asp:Label><br /><br />
                            <asp:Label ID="Label18" runat="server" Text="(1) It is enclosed herewith:-"></asp:Label><br />
                            <p style="margin-left:22px">(a) The list of members of the Managing Body in Schedule 1 with its annexure.<br />
                            (b) The Certificate of Registration sought to be renewed.</p>
                            <asp:Label ID="Label19" runat="server" Text="(2) The applicant hereby pays alongwith this application the requisite fees."></asp:Label><br /><br /> 
                            <asp:Label ID="Label20" runat="server" Text="(3) It is hereby declared that:-"></asp:Label><br />                
                            <p style="margin-left:22px">
                            (a) All the activities of the above named society have been and are conducted strictly within the scope of the objects permitted
                            under the Societies Registration Act, 1860.<br />
                            (b) All the Registers specified under the said Act and Rules framed thereunder have been duly maintained.<br />
                            (c) All the specified statutory statements have been duly filed at the specified time.<br />
                            (d) The accounts of the society have been regularly audited and proper record maintained.<br />
                            </p>
                     
                    </div>
                    
                    <div class="col-12" style="text-align: right; font-size:medium; margin-top:15px;">
                        <asp:Label ID="Label13" runat="server" Text="____________________"></asp:Label><br />
                        <asp:Label ID="lbappName1" runat="server" Text=""></asp:Label><br />                        
                        <asp:Label ID="lblDesignations" runat="server" Text="President/Secretary"></asp:Label><br />
                    </div>

                     <div class="col-12" style="font-size:medium; margin-top:20px">
                        <p>
                            Verification:-<br />
                             I,<asp:Label ID="lblheadname" runat="server" Font-Bold="true" Text="____________________________"></asp:Label> of "
                             <asp:Label ID="lblstyname" Font-Bold="true" runat="server" Text="____________________________"></asp:Label> " do hereby solemnly affirm that whatever has been stated
                             above is true to the best of my knowledge and belief.<br /><br />
                             Panaji, dated:-<asp:Label ID="lbldate" runat="server" Text="____________________"></asp:Label><br />
                        </p>
                     </div>

                    <div class="col-12" style="text-align: right;font-size:medium; margin-top:15px;">
                        <asp:Label ID="Label15" runat="server" Text="____________________"></asp:Label><br />
                        <asp:Label ID="lbappName" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="lbldesign" runat="server" Text="      President     "></asp:Label><br />
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
