<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="ApproveDSC.aspx.cs" Inherits="GoaSocietyRegistration.Organization.ApproveDSC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .card {
            background-color: #ffffff;
            border: 1px solid rgba(0, 34, 51, 0.1);
            box-shadow: 2px 4px 10px 0 rgba(0, 34, 51, 0.05), 2px 4px 10px 0 rgba(0, 34, 51, 0.05);
            border-radius: 0.15rem;
        }

        .noc {
            padding: 8px;
            background-color: #00a65a !important;
            color: #ffffff;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            var tabName = $("[id*=TabName]").val() != "" ? $("[id*=TabName]").val() : "one";
            $('#myTab a[href="#' + tabName + '"]').tab('show');
            $("#myTab a").click(function () {
                $("[id*=TabName]").val($(this).attr("href").replace("#", ""));
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row ">
            <div class="col-12">
                <br />
                <asp:HiddenField ID="HiddenField1" runat="server" />
                <%--  <div class="card mt-3 tab-card">--%>
                <div style="border: solid 1px #ddd" class="shadow ">
                    <div class="noc">DSC Manage</div>
                    <div class="card-header tab-card-header ">
                        <ul class="nav nav-tabs card-header-tabs" id="myTab" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link" id="one-tab" data-toggle="tab" href="#one" role="tab" aria-controls="One" aria-selected="true">Approve DSC (<asp:Label ID="Label1" runat="server" Text=""></asp:Label>)</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" id="two-tab" data-toggle="tab" href="#two" role="tab" aria-controls="Two" aria-selected="false">Approved DSC (<asp:Label ID="Label2" runat="server" Text=""></asp:Label>)</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" id="three-tab" data-toggle="tab" href="#three" role="tab" aria-controls="Three" aria-selected="false">Rejected DSC (<asp:Label ID="Label3" runat="server" Text=""></asp:Label>)</a>
                            </li> 
                            <li class="nav-item">
                                <a class="nav-link" id="four-tab" data-toggle="tab" href="#four" role="tab" aria-controls="Four" aria-selected="false">DSC Expired (<asp:Label ID="Label6" runat="server" Text=""></asp:Label>)</a>
                            </li>
                        </ul>
                    </div>

                    <div class="tab-content table-responsive" id="myTabContent">
                        <div class="tab-pane fade show active p-3" id="one" role="tabpanel" aria-labelledby="one-tab">
                            <asp:GridView ID="dscRegister" runat="server" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
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
                                            <asp:Label ID="dsckey" runat="server" Text='<%# Eval("dsc_publicketstring") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="dsc_name" HeaderText="DSC Holder" />
                                    <asp:BoundField DataField="dsc_notafter" HeaderText="DSC Expiry" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField DataField="dsc_reg_ip" HeaderText="DSC Registered from" />
                                    <asp:BoundField DataField="dsc_reg_at" HeaderText="DSC Registered on" />
                                    <asp:BoundField DataField="DistrictName" HeaderText="District" />
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lkaccept" CssClass="btn btn-success" OnClick="lkaccept_Click" CausesValidation="false" runat="server"><i class="fa fa-share"></i>&nbsp;Accept</asp:LinkButton>
                                            <asp:LinkButton ID="lkreject" CssClass="btn btn-danger" OnClick="lkreject_Click" CausesValidation="false" runat="server"><i class="fa fa-ban" aria-hidden="true"></i>&nbsp;Reject</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                                </Columns>
                            </asp:GridView>
                            <asp:HiddenField ID="TabName" runat="server" />
                        </div>
                        <div class="tab-pane fade p-3" id="two" role="tabpanel" aria-labelledby="two-tab">
                            <asp:GridView ID="DscApproved" runat="server" CellPadding="5" OnRowDataBound="DscApproved_RowDataBound" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
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
                                            <asp:Label ID="dsckeyapproved" runat="server" Text='<%# Eval("dsc_publicketstring") %>'></asp:Label>
                                            <asp:Label ID="active" runat="server" Text='<%# Eval("active") %>'></asp:Label>
                                            <asp:Label ID="status" runat="server" Text='<%# Eval("status") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="dsc_name" HeaderText="DSC Holder" />
                                    <asp:BoundField DataField="dsc_notafter" HeaderText="DSC Expiry" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField DataField="dsc_reg_ip" HeaderText="DSC Registered from" />
                                    <asp:BoundField DataField="dsc_reg_at" HeaderText="DSC Registered on" />
                                    <asp:BoundField DataField="DistrictName" HeaderText="District" />
                                    <asp:BoundField DataField="approved_by_name" HeaderText="DSC Approved by" />
                                    <asp:BoundField DataField="approved_at" HeaderText="DSC Approved at" />
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lkactivate" OnClick="lkactivate_Click" Visible="false" CssClass="btn btn-success" CausesValidation="false" runat="server"><i class="fa fa-share"></i>&nbsp;Activate</asp:LinkButton>
                                            <asp:LinkButton ID="lkdeactivate" OnClick="lkdeactivate_Click" Visible="false" CssClass="btn btn-danger" CausesValidation="false" runat="server"><i class="fa fa-ban" aria-hidden="true"></i>&nbsp;Deactivate</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="tab-pane fade p-3" id="three" role="tabpanel" aria-labelledby="three-tab">
                            <asp:GridView ID="dscrejected" runat="server" CellPadding="5" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
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
                                    <asp:BoundField DataField="dsc_name" HeaderText="DSC Holder" />
                                    <asp:BoundField DataField="dsc_notafter" HeaderText="DSC Expiry" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField DataField="dsc_reg_ip" HeaderText="DSC Registered from" />
                                    <asp:BoundField DataField="dsc_reg_at" HeaderText="DSC Registered on" />
                                    <asp:BoundField DataField="DistrictName" HeaderText="District" />
                                    <asp:BoundField DataField="reason_for_reject" HeaderText="Reject Reason" />
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="tab-pane fade p-3" id="four" role="tabpanel" aria-labelledby="four-tab">
                            <asp:GridView ID="gvExpiredDSC" runat="server" CellPadding="5" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
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
                                    <asp:BoundField DataField="dsc_name" HeaderText="DSC Holder" />
                                    <asp:BoundField DataField="dsc_notafter" HeaderText="DSC Expiry" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField DataField="dsc_reg_ip" HeaderText="DSC Registered from" />
                                    <asp:BoundField DataField="dsc_reg_at" HeaderText="DSC Registered on" />
                                    <asp:BoundField DataField="DistrictName" HeaderText="District" />
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
        <div id="msgmodal" class="modal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label49" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label50" runat="server" ForeColor="red"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="RejectReason" class="modal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label4" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">

                            <label for="exampleFormControlInput1">Remarks</label>
                            <asp:TextBox ID="Txtbxremarks"  CssClass="form-control" MaxLength="200" TextMode="MultiLine" runat="server"></asp:TextBox>
                        </div>
                        <asp:HiddenField ID="hfkey" runat="server" />

                        <br />
                        <asp:Label ID="Label5" Visible="false" ForeColor="Red" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="Button1" OnClick="Button1_Click" CssClass="btn btn-danger" runat="server" Text="Reject" />
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
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
                            <asp:Label ID="Label47" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label26" runat="server" ForeColor="White"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="permission" CausesValidation="false" CssClass="btn btn-primary" runat="server" Text="Ok" />

                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
