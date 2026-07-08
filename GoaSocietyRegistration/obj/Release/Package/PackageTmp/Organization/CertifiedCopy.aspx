<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="CertifiedCopy.aspx.cs" Inherits="GoaSocietyRegistration.Organization.CertifiedCopy" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
      <style>
        .society_list {
            background-color: #6c53dc !important;
            color: #ffffff;
            padding: 8px;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            window.history.pushState(null, "", window.location.href);
            window.onpopstate = function () {
                window.history.pushState(null, "", window.location.href);
            };
        });
    </script>
     <script>
        function Popup(url) {
            window.open(url, "myWindow", "status = 1, height = 600, width = 800, resizable = 0")
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
     <div class="container-fluid">
        <div class="row">
            <div class="col-12">
                <div class="society_list">Certified Copy</div>
                <asp:HiddenField ID="HiddenField1" runat="server" />
                <div style="border: solid 1px #ddd">
                    <div class="card-header tab-card-header">
                        <ul class="nav nav-tabs card-header-tabs" id="myTab" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link" id="zero-tab" data-toggle="tab" href="#zero" role="tab" aria-controls="Zero" aria-selected="true">New Application (<asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>) </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" id="one-tab" data-toggle="tab" href="#one" role="tab" aria-controls="One" aria-selected="true">Archive (<asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>) </a>
                            </li>
                        </ul>
                    </div>
                    <div class="tab-content table-responsive text-center" id="myTabContent">
                        <div class="tab-pane fade show active p-3" id="zero" role="tabpanel" aria-labelledby="zero-tab">

                            <asp:GridView ID="newappln" runat="server" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
                                <AlternatingRowStyle BackColor="White" />
                                <FooterStyle BackColor="#CCCC99" />
                                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                <RowStyle BackColor="#c8caf1" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                <SortedAscendingHeaderStyle BackColor="#848384" />
                                <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                <SortedDescendingHeaderStyle BackColor="#575357" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr. No">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LbApplicationID" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                            <asp:Label ID="LBGuid" runat="server" Text='<%# Eval("cert_guid") %>'></asp:Label>
                                            <asp:Label ID="Lbreceipt" runat="server" Text='<%# Eval("echallan_rcpt_cross_entry") %>'></asp:Label>                                        
                                          
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="app_id" HeaderText="Application ID" />
                                    <asp:BoundField DataField="socname" HeaderText="Society Name" />
                                     <asp:BoundField DataField="socregid" HeaderText="Society Registration No" />                                                                  
                                    <asp:BoundField DataField="appliedon" HeaderText="Applied On" DataFormatString="{0:dd/MM/yyyy}"/>                                                                  
                                   <%-- <asp:BoundField DataField="echallan_no" HeaderText="Echallan No" />--%>
                                    <asp:TemplateField HeaderText="Receipt">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LkReceipt"  CssClass="btn btn-info" CausesValidation="false" runat="server" OnClick="LkReceipt_Click"><i class="fa fa-inr" aria-hidden="true"></i></asp:LinkButton>
                                            <asp:HiddenField ID="hdreceipt" Value='<%# Eval("echallan_rcpt_cross_entry") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>                               
                                    
                                    <asp:TemplateField HeaderText="Download">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="Lkredirect" CssClass="btn btn-secondary" OnClick="Lkredirect_Click" CausesValidation="false" runat="server" ><i class="fa fa-arrow-right"></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>                                                             
                                </Columns>
                            </asp:GridView>  

                        </div>
                        <div class="tab-pane fade show  p-3" id="one" role="tabpanel" aria-labelledby="one-tab">
                             <asp:GridView ID="gv_certcopyarchive" OnDataBound="gv_certcopyarchive_DataBound" runat="server" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
                                <AlternatingRowStyle BackColor="White" />
                                <FooterStyle BackColor="#CCCC99" />
                                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                <RowStyle BackColor="#c8caf1" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />                                
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr. No">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Guid" Visible="false">
                                        <ItemTemplate>
                                              <asp:Label ID="LBGuid" runat="server" Text='<%# Eval("cert_guid") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>                                   
                                    <asp:BoundField DataField="app_id" HeaderText="Application ID" />
                                    <asp:BoundField DataField="socname" HeaderText="Society Name" />
                                     <asp:BoundField DataField="socregid" HeaderText="Society Registration No" />                                                                  
                                    <asp:BoundField DataField="appliedon" HeaderText="Applied On" DataFormatString="{0:dd/MM/yyyy}"/>  
                                      <asp:BoundField DataField="docname" HeaderText="Document Name"/> 
                                                                                                          
                                                    
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
                           <%-- <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    <div id="pdfModal" class="modal fade" role="dialog">
        <div class="modal-dialog modal-lg">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">

                    <embed id="embed1" runat="server" frameborder="0" width="100%" height="500px" />
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>

            </div>
        </div>
    </div>

</asp:Content>
