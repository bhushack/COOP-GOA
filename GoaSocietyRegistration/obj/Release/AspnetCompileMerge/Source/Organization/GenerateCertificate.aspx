<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="GenerateCertificate.aspx.cs" Inherits="GoaSocietyRegistration.Organization.GenerateCertificate" %>

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
     <script type="text/javascript">
        $(function () {
            var tabName = $("[id*=TabName]").val() != "" ? $("[id*=TabName]").val() : "one";
            $('#myTab a[href="#' + tabName + '"]').tab('show');
            $("#myTab a").click(function () {
                $("[id*=TabName]").val($(this).attr("href").replace("#", ""));
            });
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
                <div class="society_list">Generate Certificate</div>
                <asp:HiddenField ID="HiddenField1" runat="server" />
                <div style="border: solid 1px #ddd">
                    <div class="card-header tab-card-header">
                        <ul class="nav nav-tabs card-header-tabs" id="myTab" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link" id="zero-tab" data-toggle="tab" href="#zero" role="tab" aria-controls="Zero" aria-selected="true">Approve Society (<asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>) </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" id="one-tab" data-toggle="tab" href="#one" role="tab" aria-controls="One" aria-selected="true">Generate Certificate (<asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>) </a>
                            </li>
                        </ul>
                    </div>
                    <div class="tab-content table-responsive text-center" id="myTabContent">
                        <div class="tab-pane fade show active p-3" id="zero" role="tabpanel" aria-labelledby="zero-tab">

                            <asp:GridView runat="server" ID="gvApprove" CssClass="Grid" OnRowDataBound="gvApprove_RowDataBound" AutoGenerateColumns="false" CellPadding="5" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Font-Size="14px" Width="100%">
                                <AlternatingRowStyle BackColor="White" />
                                <FooterStyle BackColor="#CCCC99" />
                                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                <%--<PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                <RowStyle BackColor="#c8caf1" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                <SortedAscendingHeaderStyle BackColor="#848384" />
                                <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                <SortedDescendingHeaderStyle BackColor="#575357" />--%>
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="100">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRowNum" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="App ID" DataField="app_id" />
                                    <asp:BoundField HeaderText="Name" DataField="applicant_name" />
                                    <asp:BoundField HeaderText="Designation" DataField="DesignationName" />
                                    <asp:BoundField HeaderText="Society Name" DataField="socname" />
                                    <asp:BoundField HeaderText="Society Type" DataField="societyType" />
                                    <asp:BoundField DataField="new_or_renewal" HeaderText="New/Renewal" />
                                    <asp:TemplateField HeaderText="eChallan Receipt">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton2_approve" runat="server" OnCommand="ImageButton2_approve_Command" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" />
                                            <asp:HiddenField ID="hfechallanreceipt_approve" Value='<%# Eval("echallan_rcpt_cross_entry") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LbApp_id_approve" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Approve Society">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LkApprove" OnClick="LkApprove_Click" runat="server" CssClass="btn btn-outline-success" Style="font-weight: 700; font-size: 8pt;"><i class="fa fa-check" aria-hidden="true"></i>&nbsp;Approve</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                              <asp:HiddenField ID="TabName" runat="server" />
                        </div>
                        <div class="tab-pane fade show  p-3" id="one" role="tabpanel" aria-labelledby="one-tab">
                            <asp:GridView runat="server" ID="gridViewGenerateCertificate" OnRowDataBound="gridViewGenerateCertificate_RowDataBound" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Font-Size="14px" Width="100%">
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
                                    <asp:BoundField HeaderText="App ID" DataField="app_id" />
                                    <asp:BoundField HeaderText="Name" DataField="applicant_name" />
                                    <asp:BoundField HeaderText="Designation" DataField="DesignationName" />
                                    <asp:BoundField HeaderText="Society Name" DataField="socname" />
                                    <asp:BoundField HeaderText="Society Type" DataField="societyType" />
                                    <asp:BoundField DataField="new_or_renewal" HeaderText="New/Renewal" />
                                    <asp:TemplateField HeaderText="eChallan Pdf">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton1" runat="server" OnCommand="ImageButton1_Command" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" />
                                            <asp:HiddenField ID="hfechallanpdf" Value='<%# Eval("echallan_pdf_cross_entry") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="eChallan Receipt">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton2" runat="server" OnCommand="ImageButton2_Command" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" />
                                            <asp:HiddenField ID="hfechallanreceipt" Value='<%# Eval("echallan_rcpt_cross_entry") %>' runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                                    <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LbApp_id" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                            <asp:Label ID="mongoentryFINAL" runat="server" Text='<%# Eval("final_certificate_mongo_entry") %>'></asp:Label>
                                            <asp:Label ID="lbrenewal" runat="server" Text='<%# Eval("new_or_renewal") %>'></asp:Label>
                                            <asp:Label ID="LbSocietyName" runat="server" Text='<%# Eval("socname") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Generate">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LkGenerateCertificate" runat="server" CssClass="btn btn-outline-info" OnClick="LkGenerateCertificate_Click" Style="font-weight: 700; font-size: 8pt;"><i class="fa fa-check" style="font-size:20px"></i>&nbsp;Certificate</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Digital Sign" Visible="false">
                                        <ItemTemplate>
                                           <%-- OnClick="lkDigitalSign_Click" 
                                            <asp:LinkButton ID="lkDigitalSign"  runat="server" CssClass="btn btn-outline-warning" Style="font-weight: 700; font-size: 8pt;"><i class="fa fa-pen" style="font-size:20px"></i>&nbsp;Sign</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Upload">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LkUpload" OnClick="LkUpload_Click" runat="server" CssClass="btn btn-primary" Style="font-weight: 700; font-size: 8pt;"><i class="fa fa-upload" style="font-size:20px"></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LkView" OnClick="LkView_Click" runat="server" CssClass="btn btn-outline-secondary" Style="font-weight: 700; font-size: 8pt;"><i class="fa fa-eye" style="font-size:20px"></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LkDelete" OnClick="LkDelete_Click" runat="server" CssClass="btn btn-outline-danger" Style="font-weight: 700; font-size: 8pt;"><i class="fa fa-trash-o" style="font-size:20px"></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Confirm">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkconfirm" CssClass="check-input" OnCheckedChanged="chkconfirm_CheckedChanged" AutoPostBack="true" runat="server" />
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


    <%--Upload file modal--%>
    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="UploadModal" class="modal fade">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Upload Society Certificate</h4>
                    </div>
                    <div class="modal-body">
                        <div class="table-responsive">
                            <div class="table table-bordered table-hover">
                                <table style="width: 100%">
                                    <thead>
                                        <tr>
                                            <td class="a">
                                                <asp:Label runat="server" ID="Label1" Text="Application ID" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                            </td>
                                            <td class="b">
                                                <asp:TextBox runat="server" ID="TxtBxAppID" MaxLength="50" Enabled="false" CssClass="form-control"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                <asp:Label ID="Label26" runat="server" CssClass="control-label" Text="Document Upload" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                            </td>
                                            <td class="b">
                                                <asp:FileUpload ID="FileUpload1" runat="server" /><br />
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator7" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload1" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File." Display="Dynamic" /><br />
                                                <asp:Label runat="server" ID="Label29" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                <asp:Label runat="server" ID="Label30" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>
                                                <asp:Button ID="UploadDocument" OnClick="UploadDocument_Click" CausesValidation="false" CssClass="btn btn-primary" runat="server" Text="Upload Certificate" /></td>
                                        </tr>
                                    </thead>
                                </table>
                                <asp:Label ID="LbError" Visible="false" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
        <br />
    </div>
    <%--    --------------------------------------------------------------------------------------------------------------------------------------------------%>
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
    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="DeleteConfirm" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label47" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hdAppID" runat="server" />
                        <asp:Label ID="Label48" CssClass="control-label" runat="server" Text="Are you sure, you want to delete this file. Click Yes to Confirm"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                        <asp:Button ID="BtnYes" CssClass="btn btn-primary" OnClick="BtnYes_Click" runat="server" Text="Yes" />
                        <asp:Button ID="BtnProceed" CssClass="btn btn-success" OnClick="BtnProceed_Click" Visible="false" runat="server" Text="Procceed" />
                    </div>
                </div>
            </div>
        </div>
    </div>


    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="ApproveSociety" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label4" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="HiddenField2" runat="server" />
                        <asp:CheckBox ID="CheckBox1" runat="server"></asp:CheckBox>&nbsp;&nbsp;I approve this Society
                    </div>
                    <asp:Label ID="Label5" runat="server" Visible="false" ForeColor="Red" Text=""></asp:Label>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                        <asp:Button ID="BtnApproveModal" CssClass="btn btn-success" OnClick="BtnApproveModal_Click" runat="server" Text="Approve" />
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


</asp:Content>
