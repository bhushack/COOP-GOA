<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Schedule2.aspx.cs" Inherits="GoaSocietyRegistration.User.Schedule2" %>

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
    <title>SCHEDULE-II</title>
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
                    <div class="col-12" style="text-align: center;">
                        <asp:Label ID="Label1" runat="server" Font-Bold="true" Font-Size="Large" Text=" SCHEDULE - II " Font-Underline="true"></asp:Label><br />   
                        <asp:Label ID="Label2" runat="server" Font-Bold="true" Font-Size="Medium" Text="(See Rule 7)" Font-Underline="true"></asp:Label>                                         
                    </div>
                   
                    <br />
                    <div class="col-12" style="font-size:large; margin-top:20px">
                        <asp:Label ID="Label8" runat="server" Text="Statement relating to persons employed by he Society, their condition of employment, etc., during the year ending 31"></asp:Label><sup>st</sup><asp:Label ID="Label5" runat="server" Text=" December"></asp:Label><br /><br />
                        <asp:Label ID="Label3" runat="server" Font-Bold="true" Text="Name and Address of the Society:- "></asp:Label><asp:Label ID="lblsocname" runat="server" Text="____________________________"></asp:Label><br />
                        <asp:Label ID="Label4" runat="server"  Text="Address of the Society :- " Visible="false"></asp:Label><asp:Label ID="lblsocaddr" runat="server" Text="____________________________" Visible="false"></asp:Label>
                          <asp:Label ID="Label12" runat="server"  Font-Bold="true"  Text="Registration No. of the Society under the Societies Registration Act, 1860:- "></asp:Label><asp:Label ID="lblregno" runat="server" Text="____________________________"></asp:Label>  <br />                          
                     
                         <asp:GridView ID="gridview_employeeData" runat="server" CellPadding="3" AutoGenerateColumns="false"  
                            font-size="Medium" CssClass="ml-3 mt-4" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true">
                          
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

                        <br />
                         <asp:Label ID="Label7" runat="server" Text="Order and in the name of the Administrator of Goa, Daman and Diu."></asp:Label>


                           <div class="mt-5">
                            
                            <div style="text-align: right;font-size:large; margin-top:15px;">
                                    <asp:Label ID="Label15" runat="server" Text="____________________"></asp:Label><br />
                                    <asp:Label ID="lblappname" runat="server" Text="  "></asp:Label><br />
                                    <asp:Label ID="lbladdress" runat="server" Text="  "></asp:Label><br />
                                </div>
                        </div>
                        
                     
                       
                    </div>

                  
                    
                   

                     
                   
                </div>
            </fieldset>
        </div>
        
        <br/><br/>
         <div class="container">
         
                  <div style='float: left;'>
                  <%-- <asp:LinkButton ID="LinkButton2"   CssClass="btn-success btn-lg" OnClick="LinkButton2_Click" runat="server"><i class="fa fa-arrow-left" aria-hidden="true"></i>&nbsp;Back</asp:LinkButton>--%>
               </div>
                  <div style='float: left;'>
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
