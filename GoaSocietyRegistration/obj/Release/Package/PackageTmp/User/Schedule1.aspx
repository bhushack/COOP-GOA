<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Schedule1.aspx.cs" Inherits="GoaSocietyRegistration.User.Schedule1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <title>SCHEDULE I</title>
   <link href="../assets/css/bootstrap.min.css" rel="stylesheet" />
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
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
                        <asp:Label ID="Label1" runat="server" Font-Bold="true" Font-Size="Large" Text=" SCHEDULE I "></asp:Label><br />
                        <asp:Label ID="Label2" runat="server" Font-Size="Large" Text="(See Rule 6) "></asp:Label><br />                       
                    </div>
                   
                    <br />
                    <div class="col-12" style="font-size:large; margin-top:20px">
                        <asp:Label ID="Label8" runat="server" Text="Statement of annual list of the persons referred to in Section 4 of Societies Registration act, 1860"></asp:Label><br /><br />
                        <asp:Label ID="Label3" runat="server" Font-Bold="true" Text="Name & Address of the Society :- "></asp:Label><asp:Label ID="lblsocname1" runat="server" Text="____________________________"></asp:Label><br />
                        <asp:Label ID="Label4" runat="server" Font-Bold="true" Text="Address of the Society :- " Visible="false"></asp:Label><asp:Label ID="lblsocaddr" runat="server" Text="____________________________" Visible="false"></asp:Label>
                        <asp:Label ID="Label9" runat="server" Font-Bold="true" Text="Registration No. under the Societies Registration Act, 1860:- "></asp:Label><asp:Label ID="lblregno" runat="server" Text="____________________________"></asp:Label><br />
                         <asp:Label ID="Label5" runat="server" Font-Bold="true"  Text="Date of Election and period for which elected:- "></asp:Label><asp:Label ID="lbldateofelec" runat="server" Text=""></asp:Label><br />  
                          <asp:Label ID="Label12" runat="server" Font-Bold="true"  Text="Period for which the list is filed:- "></asp:Label><asp:Label ID="lblperiod" runat="server" Text=""></asp:Label><br />                            
                     
                       
                         <asp:GridView ID="gv_mangcomm" runat="server" CellPadding="3"  AutoGenerateColumns="false" 
                             CssClass="ml-3 mt-4" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="95%" ItemStyle-Wrap="true" >                          
                            <Columns>
                                <asp:TemplateField HeaderText="Sr. No">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Name" DataField="fname" />  
                               <asp:BoundField HeaderText="Address" DataField="address" /> 
                                <asp:BoundField HeaderText="Occupation" DataField="occupation" />
                                <asp:BoundField HeaderText="Age" DataField="age" />                                  
                                <asp:BoundField HeaderText="Designation" DataField="designtaion" />
                               <asp:BoundField HeaderText="Remarks" DataField="remarks" />                                
                            </Columns>
                             <EmptyDataTemplate>
                                <div align="center">No records found.</div>
                            </EmptyDataTemplate>
                        </asp:GridView>


                        <br />
                         <asp:Label ID="Label7" runat="server" Font-Bold="true" Text="NOTE: A copy of the resolution of the General Body electing the members shall accompany this statement."></asp:Label>


                        <div class="row mt-5">
                            <div class="col-6" style="text-align:left">
                            <asp:Label ID="Label6" runat="server"  Text="Date:-"></asp:Label><asp:Label ID="lbldate" runat="server"  Text="_________"></asp:Label>
                                </div>

                            <div class="col-6" style="text-align: right;font-size:large; margin-top:15px;">
                                    <asp:Label ID="Label15" runat="server" Text="____________________"></asp:Label><br />
                                    <asp:Label ID="lblappname" runat="server" Text="  "></asp:Label><br />
                                    <asp:Label ID="lbldesign" runat="server" Text="      President/Secretary     "></asp:Label><br />
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
