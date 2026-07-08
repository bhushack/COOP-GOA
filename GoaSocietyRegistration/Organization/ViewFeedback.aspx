<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="ViewFeedback.aspx.cs" Inherits="GoaSocietyRegistration.Organization.ViewFeedback" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
     <%--<script src="../Scripts/jquery-3.5.0.js"></script>--%>
   <%-- <script src="../Scripts/jquery.min.js"></script>--%>
   <%-- <script src="../Scripts/jquery-3.5.0.min.js"></script>--%>
   
    <style>
         .bs-example {
            margin: 20px;
        }

         
        .feedback_list {
            background-color: #6c53dc !important;
            color: #ffffff;
            padding: 8px;
            font-weight:bold;
        }

        .cssPager td  
              { 
                  color:#0796fb; 
                  font-size:18px;
                  /*color:#fff;*/
                   padding-left: 4px;     
                  padding-right: 4px;    
              }    
    

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />

     <div class="container-fluid">
        <div class="row">
            <div class="col-12">
                <div class="feedback_list">Feedback List</div>
                <div style="border: solid 1px #ddd">
                    <asp:HiddenField ID="HiddenField1" runat="server" />
                    <div class="tab-content" id="societyverifytabcontent">
                       <div class="tab-pane fade show active p-3 table-responsive" id="one" role="tabpanel" aria-labelledby="one-tab">
                         <asp:GridView runat="server" ID="GridviewFeedback" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" OnPageIndexChanging="GridviewFeedback_PageIndexChanging" AllowPaging="true" PageSize="25" PagerStyle-CssClass="pgr" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%">
                            <AlternatingRowStyle BackColor="White" />
                            <FooterStyle BackColor="#CCCC99" />
                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                            <PagerStyle CssClass="cssPager" BackColor="#d4d4d4" ForeColor="Black" HorizontalAlign="Right" />
                            <PagerSettings Mode="Numeric" PageButtonCount="5" LastPageText="Last" />
                            <RowStyle BackColor="#f9f3dc"  /> <%--  f5f9dc --%>
                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                            
                            <Columns>
                              
                                    <asp:TemplateField HeaderText="Sr. No">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Name" DataField="feedback_name"  /> 
                                    <asp:BoundField HeaderText="Email ID" DataField="feedback_email" />                                  
                                    <asp:BoundField HeaderText="Feedback" DataField="feedback" />
                                   
                                </Columns>
                           
                        </asp:GridView>
                        </div>
                       
                    </div>
                </div>
            </div>
        </div>
    </div>

        <div class="bs-example">
            <!-- Modal HTML -->
            <div id="permission_error_modal" class="modal">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label19" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                           <asp:Label ID="Label5" runat="server" ForeColor="White"></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="permission" OnClick="permission_Click" CausesValidation="false"  CssClass="btn btn-primary" runat="server" Text="Ok" />
                           
                        </div>
                    </div>
                </div>
            </div>
        </div>
         
</asp:Content>
