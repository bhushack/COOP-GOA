<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemorandumOfAssociation.aspx.cs" Inherits="GoaSocietyRegistration.User.MemorandumOfAssociation" %>

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
    <title>Memorandum of Association</title>
   <link href="../assets/css/bootstrap.min.css" rel="stylesheet" /> 
    <link href="../Login_Assets/css/font-awesome.min.css" rel="stylesheet" />
    <link rel="shortcut icon" href="../assets/images/favicon.ico" />
     <script>
         function printContent(el)
         {
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
                        <asp:Label ID="Label1" runat="server" Font-Bold="true" Font-Size="Large" Text=" Memorandum of Association "></asp:Label><br />
                        <asp:Label ID="Label2" runat="server" Font-Bold="true" Font-Size="Large" Text=" of "></asp:Label><br />
                        <asp:Label ID="lblsocname" runat="server" Font-Size="Large" Text=". . . . . . . . . . . . . . . . . . . . . . . "></asp:Label><br />
                    </div>
                   
                    <br />
                    <div class="col-12" style="font-size:large; margin-top:20px">
                        
                        <asp:Label ID="Label3" runat="server" Font-Bold="true" Text="1. Name of the Society :- "></asp:Label><asp:Label ID="lblsocname1" runat="server" Text="____________________________"></asp:Label><br />
                        <asp:Label ID="Label4" runat="server" Font-Bold="true" Text="2. Address of the Society :- "></asp:Label><asp:Label ID="lblsocaddr" runat="server" Text="____________________________"></asp:Label><br />
                        <asp:Label ID="Label5" runat="server" Font-Bold="true" Text="3. Object of the Society :- "></asp:Label><br />
                         <asp:GridView ID="gv_objective" runat="server" CellPadding="3"  AutoGenerateColumns="false" 
                              CssClass=" ml-3" EmptyDataText="No Record Found." ShowHeader="false" Width="95%" GridLines="None" >                         
                           
                            <Columns>
                                <asp:TemplateField ItemStyle-VerticalAlign="Top">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex+1 %>)
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="objective" />   
                            </Columns>
                            <EmptyDataTemplate>
                                <div align="center">No records found.</div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                            <br />
                         <asp:Label ID="Label6" runat="server" Font-Bold="true" Text="4. Names, Designations, Occupations, Addresses of the members of the Managing Committee"></asp:Label><br />
                     
                        <asp:GridView ID="gv_mangcomm" runat="server" CellPadding="3"  AutoGenerateColumns="false" 
                             CssClass="ml-3 mt-4" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="95%">
                           
                           <Columns>
                                <asp:TemplateField HeaderText="Sr. No">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex+1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>                                
                                <asp:BoundField HeaderText="Name" DataField="fname" />                                  
                                <asp:BoundField HeaderText="Designation" DataField="designtaion" />
                                <asp:BoundField HeaderText="Occupation" DataField="occupation" />
                                <asp:BoundField HeaderText="Address" DataField="address" />                                      
                                </Columns>
                            <EmptyDataTemplate>
                                <div align="center">No records found.</div>
                            </EmptyDataTemplate>
                        </asp:GridView>

                        <br />
                        <hr />
                         <asp:Label ID="Label7" runat="server" Font-Bold="true" Text="5.We, the following persons, being desirous of forming ourselves into a Society under the Societies Registration Act, 1860 have subscribed our      names to this Memorandum on dated "></asp:Label>
                        <asp:Label ID="lbldate" runat="server" Text=""></asp:Label><br />
                     
                        <asp:GridView ID="gv_members" runat="server" CellPadding="3"  AutoGenerateColumns="false" 
                            CssClass="ml-3 mt-4" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="95%" RowStyle-Height="70px" >                        
                          <Columns>
                                <asp:TemplateField HeaderText="Sr. No"  ItemStyle-Width="70px">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex+1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:BoundField HeaderText="Name" DataField="fname" />                                  
                                <asp:BoundField HeaderText="Signature"/>
                                
                                    
                                </Columns>
                            <EmptyDataTemplate>
                                <div align="center">No records found.</div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                    
                  

                     
                   
                </div> <hr />
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
