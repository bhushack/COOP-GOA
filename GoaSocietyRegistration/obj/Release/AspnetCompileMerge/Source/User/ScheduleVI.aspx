<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScheduleVI.aspx.cs" Inherits="GoaSocietyRegistration.User.ScheduleVI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <title>SCHEDULE VI</title>
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
                        <asp:Label ID="Label1" runat="server" Font-Bold="true" Font-Size="Large" Text=" SCHEDULE VI " Font-Underline="true"></asp:Label><br />                                            
                    </div>
                   
                    <br />
                    <div class="col-12" style="font-size:large; margin-top:20px">
                        <asp:Label ID="Label8" runat="server" Text="List of Members to be maintained by the governing body of a Society"></asp:Label><br /><br />
                        <asp:Label ID="Label3" runat="server"  Font-Bold="true"  Text="Name of the Society :- "></asp:Label><asp:Label ID="lblsocname1" runat="server" Text="____________________________"></asp:Label><br />
                        <asp:Label ID="Label4" runat="server"  Font-Bold="true"  Text="Address of the Society :- " Visible="false"></asp:Label><asp:Label ID="lblsocaddr" runat="server" Text="____________________________" Visible="false"></asp:Label>
                        <asp:Label ID="Label12" runat="server" Font-Bold="true"  Text="Registration No. of the Society under the Societies Registration Act, 1860:- "></asp:Label><asp:Label ID="lblregno" runat="server" Text="____________________________"></asp:Label>  <br />                          
                      
                       <asp:GridView ID="gv_members" runat="server" CellPadding="3"  AutoGenerateColumns="false" 
                            CssClass="ml-3 mt-4" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="95%" RowStyle-Height="70px"> 
                          
                            <Columns>
                                <asp:TemplateField HeaderText="Sr. No">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Name" DataField="fname" />
                               <asp:BoundField HeaderText="Address" DataField="address" />                                    
                                <asp:BoundField HeaderText="Date of Admission" DataField="dateofadmission" DataFormatString="{0:dd/MM/yyyy}"/>
                                <asp:BoundField HeaderText="Signature of the member" ItemStyle-Width="300px"  />
                              <asp:BoundField HeaderText="Remarks" DataField="remarks"  /> 
                                                              
                            </Columns>
                            <EmptyDataTemplate>
                                <div align="center">No records found.</div>
                            </EmptyDataTemplate>

                        </asp:GridView>


                        <br />
                         <asp:Label ID="Label7" runat="server" Text="Order and in the name of the Administrator of Goa, Daman and Diu."></asp:Label>
                        
                     
                       
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
