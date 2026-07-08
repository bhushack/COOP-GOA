<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="oldCertificate.aspx.cs" Inherits="GoaSocietyRegistration.Organization.oldCertificate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <script type="text/javascript">
        function openInNewTab() {
            window.document.forms[0].target = '_blank';
            setTimeout(function () { window.document.forms[0].target = ''; }, 0);
        }    
			
		</script>
     <style>
        .society_list {
            background-color: #6c53dc !important;
            color: #ffffff;
            padding: 8px;
        }

        
    </style>
     <script type="text/javascript">
  $(document).ready(function() {
      window.history.pushState(null, "", window.location.href);        
      window.onpopstate = function() {
          window.history.pushState(null, "", window.location.href);
      };
  });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <br />
    <div class="container-fluid">
        <div class="row">
        <div class="col-12">
            <div class="society_list">Old Certificate</div>
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <div style="border: solid 1px #ddd">
                <div class="card-header tab-card-header">
                    <ul class="nav nav-tabs card-header-tabs" id="myTab" role="tablist">
                        <li class="nav-item">
                            <a class="nav-link" id="one-tab" data-toggle="tab" href="#one" role="tab" aria-controls="One" aria-selected="true">Generate Certificate (<asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>) </a>
                        </li>
                    </ul>
                </div>
                <div class="tab-content" id="myTabContent">
                    <div class="tab-pane fade show active p-3 table-responsive" id="one" role="tabpanel" aria-labelledby="one-tab">
                        <asp:GridView runat="server" ID="OldCertificate" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%">
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
                                <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="100">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRowNum" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Application ID" DataField="app_id" />
                                <asp:BoundField HeaderText="Name" DataField="applicant_name" />
                                <asp:BoundField HeaderText="Designation" DataField="DesignationName" />
                                <asp:BoundField HeaderText="Society Name" DataField="socname" />
                                <asp:BoundField HeaderText="Society Type" DataField="societyType" />
                                <asp:TemplateField HeaderText="Edit" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="LbApp_id" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                        <asp:Label ID="mongoentryFINAL" runat="server" Text='<%# Eval("final_certificate_mongo_entry") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="View">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="oldcertificate" runat="server" CssClass="btn btn-outline-info" OnClick="oldcertificate_Click"  Style="font-weight: 700; font-size: 8pt;" ><i class="fa fa-check" style="font-size:20px"></i>&nbsp;View Certificate</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>


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
