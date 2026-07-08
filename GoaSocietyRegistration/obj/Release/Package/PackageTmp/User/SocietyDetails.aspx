<%@ Page Title="" Language="C#" MasterPageFile="~/User/Society.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" EnableEventValidation="true" CodeBehind="SocietyDetails.aspx.cs" Inherits="GoaSocietyRegistration.SocietyDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <%--   <script src="../Scripts/jquery.min.js"></script>--%>
    <%-- <script src="../Scripts/jquery-3.5.0.js"></script>   --%>
     <link href="../assets/css/StyleSheet.css" rel="stylesheet" />
    <script src="../Scripts/Script.js"></script>
    <style type="text/css">
        .uppercase {
            text-transform: uppercase;
        }

        a.disabled:hover {
            cursor: not-allowed;
        }

        .a {
            width: 30%;
        }

        .b {
            width: 70%;
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
        function CoverClickLK(val) {
            if (val == "btnBack") {
                document.getElementById("<%=btnBack.ClientID %>").style.display = "none";
                CoverClick1();
            }

            else if (val == "SocietyDetailsBtn") {
                document.getElementById("<%=SocietyDetailsBtn.ClientID %>").style.display = "none";
                CoverClick1();
           }
            else if (val == "btnedit") {
                document.getElementById("<%=btnedit.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "btnupdate") {
                document.getElementById("<%=btnupdate.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "btnnext") {
                document.getElementById("<%=btnnext.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "confirmmodalbutton") {
                document.getElementById("<%=confirmmodalbutton.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "btn_addobjec") {
                document.getElementById("<%=btn_addobjec.ClientID %>").style.display = "none";
                CoverClick1();
            }
        } 
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div class="container">
        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div class="card" style="width: 100%">
                    <h4 class="card-header"><strong>Society details :: </strong></h4>
                    <div class="card-body">
                        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="false" />
                        <div id="societydetails">
                            <div class="form-group">
                                <asp:Label ID="Label6" runat="server" ForeColor="Red">The fields marked with (*) are mandatory.</asp:Label>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblChkSoc" runat="server" Text="chkSocNam" Visible="false"></asp:Label>
                            </div>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                                <ContentTemplate>
                                    <asp:HiddenField ID="HiddenField1" runat="server" />
                                </ContentTemplate>

                            </asp:UpdatePanel>
                            <div class="row">
                                <div class="table-responsive">
                                    <div class="table table-bordered table-hover">
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <table style="width: 100%">
                                                    <thead>
                                                        <tr>
                                                            <td class="a">
                                                                <asp:Label runat="server" ID="lblSocType" Text="Society Type" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span></td>
                                                            <td class="b">
                                                                <asp:DropDownList runat="server" ID="ddlSocType" CssClass="form-control" OnSelectedIndexChanged="ddlSocType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                <asp:RequiredFieldValidator InitialValue="-1" ID="rfvddlsoctype" Display="Dynamic" runat="server" ControlToValidate="ddlSocType" CssClass="text-danger" ErrorMessage="Select Society Type"></asp:RequiredFieldValidator></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="a">
                                                                <asp:Label runat="server" ID="lblSocName" Text="Society Name" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                            </td>
                                                            <td class="b">
                                                                <asp:TextBox runat="server" ID="txtSocName" MaxLength="250" AutoPostBack="true" autocomplete="off" AutoCompleteType="Disabled" OnTextChanged="txtSocName_TextChanged" CssClass="form-control uppercase"> </asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvfsocname" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtSocName" ErrorMessage="Enter Society Name"> </asp:RequiredFieldValidator><br />
                                                                <asp:RegularExpressionValidator ID="revfirstname" runat="server" Display="Dynamic" ControlToValidate="txtSocName" ForeColor="Red" ValidationExpression="[\sa-zA-Z0-9-',.`_()-]+$" ErrorMessage="Alphabet A-Z, a-z , 0-9 and Special Characters -,._()` only are allowed. & can be written as AND" /><br />
                                                                <asp:Label ID="Label8" runat="server" Font-Size="Small" ForeColor="OrangeRed">(कृपया इस क्षेत्र में गावं, शेहर, जिला का नाम नहीं लिखे /Please do not write Village,Taluka,District name in this field).</asp:Label>
                                                                <br />
                                                                <asp:Label runat="server" ID="errorforsociety" Visible="false" CssClass="control-label" Font-Bold="true"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="a">
                                                                <asp:Label runat="server" ID="lblSocAddress" Text="Address of Society" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span></td>
                                                            <td class="b">
                                                                <asp:TextBox runat="server" ID="txtSocAddress" MaxLength="200" CssClass="form-control" autocomplete="off" AutoCompleteType="Disabled"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtSocAddress" ErrorMessage="Enter Society Address"> </asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtSocAddress" ForeColor="Red" SetFocusOnError="true" Display="Dynamic" ErrorMessage="No special characters allowed" ID="rfvname" ValidationExpression="^[\sa-zA-Z0-9()-,._/:@-]+$" />
                                                                <br />
                                                                <asp:Label ID="Label1" runat="server" Font-Size="Small" ForeColor="OrangeRed">(कृपया इस क्षेत्र में शेहर, जिला का नाम नहीं लिखे /Please do not write Taluka,district name in this field).</asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="a">
                                                                <asp:Label ID="lblSocDistrict" runat="server" Text="District" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span></td>
                                                            <td class="b">
                                                                <asp:DropDownList ID="ddlSocDistrict" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSocDistrict_SelectedIndexChanged" CssClass="form-control"></asp:DropDownList>
                                                                <asp:RequiredFieldValidator InitialValue="-1" ID="Req_ID" Display="Dynamic" runat="server" ControlToValidate="ddlSocDistrict" CssClass="text-danger" ErrorMessage="Please Select District"></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="a">
                                                                <asp:Label ID="lblTaluka" runat="server" Text="Taluka:" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span></td>
                                                            <td class="b">
                                                                <asp:DropDownList ID="ddlSocTaluka" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                <asp:RequiredFieldValidator InitialValue="-1" ID="rfvddlsoctaluka" Display="Dynamic" runat="server" ControlToValidate="ddlSocTaluka" CssClass="text-danger" ErrorMessage="Please Select Taluka "></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="a">
                                                                <asp:Label ID="lbPincode" runat="server" Text="Pincode:" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span></td>
                                                            <td class="b">
                                                                <asp:TextBox runat="server" ID="TxtbxPincode" MaxLength="6" CssClass="form-control" ToolTip="Pincode" autocomplete="off" AutoCompleteType="Disabled"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="TxtbxPincode" ErrorMessage="Enter Pincode"> </asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator runat="server" ID="rexNumber" ControlToValidate="TxtbxPincode" CssClass="text-danger" ValidationExpression="^[0-9]{6}$" ErrorMessage="Please enter a 6 digit number!" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="a">
                                                                <asp:Label ID="Label9" runat="server" Text="Object of the Society" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                            </td>
                                                            <td>
                                                                <div id="addobjec_div" class="row" runat="server">
                                                                    <asp:TextBox runat="server" ID="txtbx_object" MaxLength="500" ToolTip="Objective" CssClass="form-control ml-3" autocomplete="off" AutoCompleteType="Disabled" Width="75%"></asp:TextBox>
                                                                    <asp:Button ID="btn_addobjec" OnClientClick="CoverClickLK('btn_addobjec');"  Visible="true" runat="server" Text="Add Objective" OnClick="btn_addobjec_Click" CssClass="btn btn-primary  ml-3" CausesValidation="false" />
                                                                    <br />
                                                                    <asp:Label ID="Label10" runat="server" Font-Size="Small" CssClass="ml-3" ForeColor="OrangeRed">(Please add one point at a time)</asp:Label>
                                                                </div>

                                                                <br />

                                                                <asp:GridView ID="gv_objective" Visible="true" ShowFooter="false" runat="server" CellPadding="5" Style="width: 95%; margin-top: 5px;" AutoGenerateColumns="false" CssClass="Grid" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true">
                                                                    <RowStyle BackColor="White" />
                                                                    <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                                                                    <HeaderStyle BackColor="#bdcbd7" Font-Bold="True" ForeColor="Black" />
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Sr. No" ItemStyle-Width="10%">
                                                                            <ItemTemplate>
                                                                                <%#Container.DataItemIndex+1 %>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Objective">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblobjective" runat="server" Text='<%# Eval("objective") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Delete">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton ID="LBDelete" OnClick="LBDelete_Click" CausesValidation="false" runat="server"><i class="fa fa-trash"></i></asp:LinkButton>
                                                                                <asp:HiddenField ID="hfrowID" Value='<%# Eval("row_id") %>' runat="server" />
                                                                            </ItemTemplate>

                                                                        </asp:TemplateField>

                                                                    </Columns>

                                                                    <EmptyDataTemplate>
                                                                        <div align="center">No records found.</div>
                                                                    </EmptyDataTemplate>
                                                                </asp:GridView>

                                                            </td>
                                                        </tr>


                                                        <tr id="tr_regfee" runat="server">
                                                            <td class="a">
                                                                <asp:Label ID="lblRegFee" runat="server" Text="Registration Fee" CssClass="control-label" Font-Bold="true"></asp:Label></td>
                                                            <td class="b"><a class="disabled" data-toggle="modal" data-target="#notAvailable">
                                                                <asp:Label ID="Label5" runat="server" Text="₹"></asp:Label>
                                                                <asp:Label ID="lblAmt" Font-Bold="true" runat="server" Enabled="false"></asp:Label></a>
                                                                <asp:HiddenField ID="hfregfee" runat="server" />
                                                            </td>

                                                        </tr>
                                                        <tr id="tr_processfee" runat="server">
                                                            <td class="a">
                                                                <asp:Label ID="lblProcFee" runat="server" Text="Proccessing Fee" CssClass="control-label" Font-Bold="true"></asp:Label></td>
                                                            <td class="b"><a class="disabled" data-toggle="modal" data-target="#notAvailable">
                                                                <asp:Label ID="Label3" runat="server" Text="₹"></asp:Label>
                                                                <asp:Label ID="lblProcAmt" Font-Bold="true" runat="server" Enabled="false"></asp:Label></a>
                                                                <asp:HiddenField ID="hfprocfee" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr id="tr_totfee" runat="server">
                                                            <td>
                                                                <asp:Label ID="lblTotFee" runat="server" Text="Total Fee to be paid" CssClass="control-label" Font-Bold="true"></asp:Label></td>
                                                            <td><a class="disabled" data-toggle="modal" data-target="#notAvailable">
                                                                <asp:Label ID="Label7" runat="server" Text="₹ "></asp:Label><asp:Label ID="lblTotAmt" Font-Bold="true" runat="server" ForeColor="Green" Enabled="false"></asp:Label></a>
                                                            </td>
                                                        </tr>
                                                        <tr>

                                                            <td colspan="2" class="text-center">
                                                                <asp:Button ID="btnBack" Visible="false" OnClientClick="CoverClickLK('btnBack');" runat="server" Text="Back" Style="margin-right: -22%" OnClick="btnBack_Click" CssClass="btn btn-warning" CausesValidation="false" />
                                                                <asp:Label runat="server" ID="lblError" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                                <asp:Button ID="SocietyDetailsBtn" OnClientClick="CoverClickLK('SocietyDetailsBtn');" runat="server" OnClick="SocietyDetailsBtn_Click" Text="Save and Next" CssClass="btn btn-primary" CausesValidation="true" />
                                                                <asp:Button ID="btnedit" OnClientClick="CoverClickLK('btnedit');" Visible="false" runat="server" Text="Edit" OnClick="btnedit_Click" CssClass="btn btn-info" CausesValidation="false" />
                                                                <asp:Button ID="btnupdate" OnClientClick="CoverClickLK('btnupdate');" Visible="false" runat="server" Text="Update" OnClick="btnupdate_Click" CssClass="btn btn-info" CausesValidation="false" />

                                                                <asp:Button ID="btnnext" OnClientClick="CoverClickLK('btnnext');"  Visible="false" runat="server" Text="Next" OnClick="btnnext_Click" CssClass="btn btn-primary" CausesValidation="false" />


                                                                <%--   <asp:Button ID="btnedit" Visible="false" runat="server" Text="Edit" OnClick="btnedit_Click" CssClass="btn btn-primary"/>
                                                    <asp:Button ID="btnupdate" Visible="false" runat="server" Text="Update" OnClick="btnupdate_Click" CssClass="btn btn-primary" />--%>
                                                            </td>
                                                        </tr>
                                                    </thead>
                                                </table>
                                            </ContentTemplate>
                                            <Triggers>
                                                <%--<asp:AsyncPostBackTrigger ControlID="ddlSocDistrict" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ddlSocType" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="txtSocName" EventName="TextChanged" />--%>
                                                <asp:PostBackTrigger ControlID="SocietyDetailsBtn" />
                                                <asp:PostBackTrigger ControlID="btnupdate" />
                                                <asp:PostBackTrigger ControlID="btnedit" />
                                                <asp:PostBackTrigger ControlID="btnnext" />
                                                <asp:PostBackTrigger ControlID="btnBack" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>

                            </div>

                        </div>

                        <div id="DivNote" runat="server">

                            <asp:Label ID="Label4" runat="server" Font-Bold="true" ForeColor="Red" BackColor="lightyellow" Text="NOTE : The Society Name is Subject to Approval from the Department."></asp:Label>

                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>


    <div id="cover" style="display: none;"></div>
    <div class="row">
        <div id="CoverDoubleClick" class="opac_divLoader overlayLoader" style="display: none;">
            <asp:Image ID="wait" runat="server" ImageUrl="../assets/images/loading.gif" AlternateText="w a i t"
                Height="45%" Width="20%" Style="vertical-align: middle;" />
        </div>
    </div>



    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="confirmModal" class="modal fade">
            <div class="modal-dialog  modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label2" runat="server" Text="Please confirm the Society Details you entered. It will not be edited or changed later on. Click Save to proceed" ForeColor="Green"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="confirmmodalbutton" runat="server" OnClientClick="CoverClickLK('confirmmodalbutton');"  OnClick="confirmmodalbutton_Click" Text="Save" CssClass="btn btn-primary" CausesValidation="true" />
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="redirectModal" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label64" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label65" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--   <asp:Button ID="btnRedirect" OnClick="btnRedirect_Click" CssClass="btn btn-primary" runat="server" Text="Confirm" />--%>
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
