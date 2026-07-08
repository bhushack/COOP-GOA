<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="ApproveSociety.aspx.cs" Inherits="GoaSocietyRegistration.Organization.ApproveSociety" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <style>
        .verify_society {
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
                <div class="verify_society">Verify Society</div>
                <div style="border: solid 1px #ddd">
                    <div class="card-header tab-card-header">
                        <ul class="nav nav-tabs card-header-tabs" id="myTab1" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link" id="verify_tab" data-toggle="tab" href="#verifysociety" role="tab" aria-controls="VerifySociety" aria-selected="true">Verify Society (
                                <asp:Label ID="Label3" runat="server" Text=""></asp:Label><asp:HiddenField ID="HiddenField1" runat="server" />
                                    )</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" id="obs_tab" data-toggle="tab" href="#obssociety" role="tab" aria-controls="ObsSociety" aria-selected="false">Verify Observation Society (
                                <asp:Label ID="Lbcount" runat="server" Text=""></asp:Label>
                                    )</a>
                            </li>

                            <li class="nav-item">
                                <a class="nav-link" id="verifyrenewal_tab" data-toggle="tab" href="#verifyrenewalsociety" role="tab" aria-controls="VerifyRenewalSociety" aria-selected="true">Verify Renewal Society (
                                <asp:Label ID="Lbrenew_count" runat="server" Text=""></asp:Label>
                                    )</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" id="obsrenewal_tab" data-toggle="tab" href="#obsrenewalsociety" role="tab" aria-controls="ObsRenewalSociety" aria-selected="false">Verify Observation Renewal Society (
                                <asp:Label ID="lbobsrenew_count" runat="server" Text=""></asp:Label>
                                    )</a>
                            </li>
                        </ul>
                    </div>
                    <div class="tab-content" id="societyverifytabcontent">
                        <div class="tab-pane fade show active p-3 table-responsive" id="verifysociety" role="tabpanel" aria-labelledby="verify_tab">
                            <asp:GridView runat="server" ID="grvApplicantDetails" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%">
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
                                    <%-- <asp:BoundField HeaderText="Mobile No" DataField="applicant_mobile_no" />--%>
                                    <asp:BoundField HeaderText="Society Name" DataField="socname" />
                                    <asp:BoundField HeaderText="Society Type" DataField="societyType" />
                                       <asp:BoundField HeaderText="Submission Time" DataField="application_submission_time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                    <%-- <asp:HyperLinkField HeaderText="View Details Information" DataNavigateUrlFields="App_ID" DataNavigateUrlFormatString="VerifyForm.aspx?" Text="View Profile"></asp:HyperLinkField>--%>
                                    <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LbApp_id" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="ENameLinkBtn" runat="server" Style="font-weight: 700; font-size: 8pt;" OnClick="ENameLinkBtn_Click"><i class="fa fa-arrow-circle-right" style="font-size:20px;"></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>

                            </asp:GridView>
                        </div>
                        <div class="tab-pane fade show  p-3 table-responsive" id="obssociety" role="tabpanel" aria-labelledby="obs_tab">
                            <asp:GridView runat="server" ID="grvobservation_society" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%">
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
                                            <asp:Label ID="lblRowNum_obs" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Application ID" DataField="app_id" />
                                    <asp:BoundField HeaderText="Name" DataField="applicant_name" />
                                    <asp:BoundField HeaderText="Designation" DataField="DesignationName" />
                                    <%-- <asp:BoundField HeaderText="Mobile No" DataField="applicant_mobile_no" />--%>
                                    <asp:BoundField HeaderText="Society Name" DataField="socname" />
                                    <asp:BoundField HeaderText="Society Type" DataField="societyType" />
                                       <asp:BoundField HeaderText="Submission Time" DataField="application_submission_time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                    <%-- <asp:HyperLinkField HeaderText="View Details Information" DataNavigateUrlFields="App_ID" DataNavigateUrlFormatString="VerifyForm.aspx?" Text="View Profile"></asp:HyperLinkField>--%>
                                    <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LbApp_id_obs" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="ENameLinkBtn_obs" runat="server" Style="font-weight: 700; font-size: 8pt;" OnClick="ENameLinkBtn_obs_Click"><i class="fa fa-arrow-circle-right" style="font-size:20px;"></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>

                            </asp:GridView>
                        </div>

                        <div class="tab-pane fade show p-3 table-responsive" id="verifyrenewalsociety" role="tabpanel" aria-labelledby="verifyrenewal_tab">
                            <asp:GridView runat="server" ID="grvRenewalApplications" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%">
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
                                    <%-- <asp:BoundField HeaderText="Mobile No" DataField="applicant_mobile_no" />--%>
                                    <asp:BoundField HeaderText="Society Name" DataField="socname" />
                                    <asp:BoundField HeaderText="Society Type" DataField="societyType" />
                                       <asp:BoundField HeaderText="Submission Time" DataField="application_submission_time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                    <%-- <asp:HyperLinkField HeaderText="View Details Information" DataNavigateUrlFields="App_ID" DataNavigateUrlFormatString="VerifyForm.aspx?" Text="View Profile"></asp:HyperLinkField>--%>
                                    <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LbApp_id" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="ENameLinkBtnRenewal" runat="server" Style="font-weight: 700; font-size: 8pt;" OnClick="ENameLinkBtnRenewal_Click"><i class="fa fa-arrow-circle-right" style="font-size:20px;"></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>

                            </asp:GridView>
                        </div>
                        <div class="tab-pane fade show  p-3 table-responsive" id="obsrenewalsociety" role="tabpanel" aria-labelledby="obsrenewal_tab">
                            <asp:GridView runat="server" ID="grvrenewal_obs" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%">
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
                                            <asp:Label ID="lblRowNum_obs" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Application ID" DataField="app_id" />
                                    <asp:BoundField HeaderText="Name" DataField="applicant_name" />
                                    <asp:BoundField HeaderText="Designation" DataField="DesignationName" />
                                    <%-- <asp:BoundField HeaderText="Mobile No" DataField="applicant_mobile_no" />--%>
                                    <asp:BoundField HeaderText="Society Name" DataField="socname" />
                                    <asp:BoundField HeaderText="Society Type" DataField="societyType" />
                                       <asp:BoundField HeaderText="Submission Time" DataField="application_submission_time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                    <%-- <asp:HyperLinkField HeaderText="View Details Information" DataNavigateUrlFields="App_ID" DataNavigateUrlFormatString="VerifyForm.aspx?" Text="View Profile"></asp:HyperLinkField>--%>
                                    <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LbApp_id_obs" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="ENameLinkBtnrenewal_obs" runat="server" Style="font-weight: 700; font-size: 8pt;" OnClick="ENameLinkBtnrenewal_obs_Click"><i class="fa fa-arrow-circle-right" style="font-size:20px;"></i></asp:LinkButton>
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
</asp:Content>
