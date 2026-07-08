<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="SocietyList.aspx.cs" Inherits="GoaSocietyRegistration.Organization.SocietyList" %>

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
        $.expr[":"].containsNoCase = function (el, i, m) {
            var search = m[3];
            if (!search) return false;
            return eval("/" + search + "/i").test($(el).text());
        };

        $(document).ready(function () {
            $('#txtSearch').keyup(function () {
                if ($('#txtSearch').val().length > 1) {
                    $('#ContentPlaceHolder1_GridviewAccepted tr').hide();
                    $('#ContentPlaceHolder1_GridviewAccepted tr:first').show();
                    $('#ContentPlaceHolder1_GridviewAccepted tr td:containsNoCase(\'' + $('#txtSearch').val() + '\')').parent().show();
                }
                else if ($('#txtSearch').val().length == 0) {
                    resetSearchValue();
                }

                if ($('#ContentPlaceHolder1_GridviewAccepted tr:visible').length == 1) {
                    $('.norecords').remove();
                    $('#ContentPlaceHolder1_GridviewAccepted').append('<tr class="norecords"><td colspan="6" class="Normal" style="text-align: center">No records were found</td></tr>');
                }
            });

            $('#txtSearch').keyup(function (event) {
                if (event.keyCode == 27) {
                    resetSearchValue();
                }
            });
        });

        function resetSearchValue() {
            $('#txtSearch').val('');
            $('#ContentPlaceHolder1_GridviewAccepted tr').show();
            $('.norecords').remove();
            $('#txtSearch').focus(); location.reload();
        }

    </script>

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
            <div class="society_list">All Socities</div>
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <div style="border: solid 1px #ddd">
                <div class="card-header tab-card-header">
                    <ul class="nav nav-tabs card-header-tabs" id="myTab" role="tablist">
                        <li class="nav-item">
                            <a class="nav-link" id="one-tab" data-toggle="tab" href="#one" role="tab" aria-controls="One" aria-selected="true">Accepted Societies (<asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>) </a>
                        </li>
                        <li class="nav-item">
                            
                            <a class="nav-link" id="two-tab" data-toggle="tab" href="#two" role="tab" aria-controls="Two" aria-selected="false">Rejected Societies (<asp:Label ID="Label4" runat="server" Text="Label"></asp:Label>) </a>
                        </li>
                    </ul>
                </div>
                <div class="tab-content" id="myTabContent">
                    <div class="tab-pane fade show active p-3 table-responsive" id="one" role="tabpanel" aria-labelledby="one-tab">
                        <div class="input-group input-group-sm mb-3">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="basic-addon1">Find</span>
                            </div>
                            <input type="text" class="form-control" id="txtSearch" name="txtSearch" placeholder="Find Match" maxlength="50">
                        </div>

                        <asp:GridView runat="server" ID="GridviewAccepted" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%" OnRowDataBound="GridviewAccepted_RowDataBound">
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
                                <asp:BoundField DataField="new_or_renewal" HeaderText="New/Renewed" />     
                                   <asp:BoundField HeaderText="Submission Time" DataField="application_submission_time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />                            
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


                    <div class="tab-pane fade p-3 table-responsive" id="two" role="tabpanel" aria-labelledby="two-tab">
                        <asp:GridView runat="server" ID="gridviewRejected" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%" OnRowDataBound="gridviewRejected_RowDataBound">
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
                                        <asp:Label ID="Label1" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Application ID" DataField="app_id" />
                                <asp:BoundField HeaderText="Name" DataField="applicant_name" />
                                <asp:BoundField HeaderText="Designation" DataField="DesignationName" />
                                <asp:BoundField HeaderText="Society Name" DataField="socname" />
                                <asp:BoundField HeaderText="Society Type" DataField="societyType" />
                                <asp:BoundField DataField="new_or_renewal" HeaderText="New/Renewed" />   
                                   <asp:BoundField HeaderText="Submission Time" DataField="application_submission_time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                <asp:TemplateField HeaderText="Edit" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="View">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" Style="font-weight: 700; font-size: 8pt;" OnClick="LinkButton1_Click"><i class="fa fa-arrow-circle-right" style="font-size:20px;"></i></asp:LinkButton>
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
                            <asp:Button ID="permission" OnClick="permission_Click"  CssClass="btn btn-primary" runat="server" Text="Ok" />                         
                        </div>
                    </div>
                </div>
            </div>
        </div>
</asp:Content>
