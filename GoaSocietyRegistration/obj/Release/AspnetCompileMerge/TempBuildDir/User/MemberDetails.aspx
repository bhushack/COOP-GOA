<%@ Page Title="" Language="C#" MasterPageFile="~/User/Society.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="MemberDetails.aspx.cs" Inherits="GoaSocietyRegistration.MemberDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <%--    <script src="../Scripts/jquery-3.5.0.js"></script>--%>

    <%-- <script src="../assets/js/jquery.min.js"></script>--%>

    <%--<script src="../assets/js/jquery-1.12.4.min.js"></script>--%>
    <%--<script src="../Scripts/jquery-3.5.0.min.js"></script>--%>
    <%-- <script src="../Scripts/popper.min.js"></script>
    <script src="../assets/js/bootstrap-3.3.7.min.js"></script>--%>
    <script src="../Scripts/encrypt.js"></script> 
    <script src="../Scripts/aes.js"></script>

    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
      
    <link href="../assets/css/StyleSheet.css" rel="stylesheet" />
    <script src="../Scripts/Script.js"></script> 
    <script type="text/javascript">

        function bonya() {
            var docsno;
            var uname = document.getElementById("<%=TxtBxDocumentNo.ClientID %>").value.trim();
            var errPass = $('#<%=lblError.ClientID %>');
            if (uname == "") errPass.html(" Document No required !");
            else {
                errPass.html("");
                docsno = CheckdocumentNo();
                if (Boolean(docsno)) {
                    document.getElementById("<%=TxtBxDocumentNo.ClientID %>").disabled = true;
                    document.getElementById("<%=originalAadhar.ClientID %>").disabled = true;

                } else {
                    document.getElementById("<%=TxtBxDocumentNo.ClientID %>").disabled = false;
                    document.getElementById("<%=originalAadhar.ClientID %>").disabled = false;
                }            }
        }
    </script>




    <script type="text/javascript">

        function CheckdocumentNo() {       //Encrypt Credentials
       
             var key = CryptoJS.enc.Utf8.parse('<%=Session["Enc_Random"]%>');
            var iv = CryptoJS.enc.Utf8.parse('<%=Session["Enc_Vector"]%>');
            var documentno = document.getElementById("<%=TxtBxDocumentNo.ClientID %>").value.trim();
           var sourceValue = $("#ContentPlaceHolder1_originalAadhar").val();// $('#ContentPlaceHolder1$originalAadhar').val();
             
           var encryptedlogin = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(sourceValue), key,
           {
               keySize: 128 / 8,
               iv: iv,
               mode: CryptoJS.mode.CBC,
               padding: CryptoJS.pad.Pkcs7
           });
          

           if (encryptedlogin != null && encryptedlogin != "") {

               $('#<%=hfencryptNo.ClientID %>').val(encryptedlogin);

                return true;
            } else {

                return false;
            }

        }
    </script>

      <script>
        $(document).ready(function () {
            // Function to mask Aadhar number
            function maskAadhar(value) {
                // Check if the value is a valid Aadhar number (12 digits)
                if (/^\d{12}$/.test(value)) {
                    // Mask all but the last 4 digits
                    return 'XXXXXXXX' + value.substring(8);
                }
                return value; // Return original value if not a valid Aadhar number
            }

            // On input in Aadhar textbox   keydown keyup mousedown mouseup propertychange paste
            $('#ContentPlaceHolder1_TxtBxDocumentNo').on('input ', function () { 
                var originalValue = this.value;
               
                var maskedValue = maskAadhar(originalValue); 
                $('#ContentPlaceHolder1_TxtBxDocumentNo').val(maskedValue); // Set masked value
                $('#ContentPlaceHolder1_originalAadhar').val(originalValue); // Set original value
                 
            });

            // On click on masked Aadhar textbox
            $('#ContentPlaceHolder1_TxtBxDocumentNo').click(function () {
                var originalValue = $('#ContentPlaceHolder1_originalAadhar').val();
                $(this).val(originalValue); // Set original value to the masked Aadhar textbox 
            });
        });
    </script>
      <script>
        function enablefields() {
            document.getElementById("<%=TxtBxDocumentNo.ClientID %>").disabled = false; 
        }//End ready() 
    </script>
    <script type="text/javascript">
        function CoverClickLK(val) {
            if (val == "btnBack") {
                document.getElementById("<%=btnBack.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "Button1") {
                document.getElementById("<%=Button1.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "MemberDetailsBtn") {
                document.getElementById("<%=MemberDetailsBtn.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "btnUpdate") {
                document.getElementById("<%=btnUpdate.ClientID %>").style.display = "none";
                CoverClick1();
                bonya();
            }
            else if (val == "LB_memberslist_view") {
                document.getElementById("<%=LB_memberslist_view.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_memberslist_delete") {
                document.getElementById("<%=LB_memberslist_delete.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_memberslist_upload") {
                document.getElementById("<%=LB_memberslist_upload.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "btnAdd") {
                document.getElementById("<%=btnAdd.ClientID %>").style.display = "none";
                CoverClick1();
                bonya();
            }
            else if (val == "btnAddOld") {
                document.getElementById("<%=btnAddOld.ClientID %>").style.display = "none";
                CoverClick1();
                bonya();
            }
            else if (val == "close_modal") {
                document.getElementById("<%=close_modal.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "btn_fetchcancel") {
                document.getElementById("<%=btn_fetchcancel.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "btn_fetchconfirm") {
                document.getElementById("<%=btn_fetchconfirm.ClientID %>").style.display = "none";
                    CoverClick1();
                }
}
    </script>

    <style>
        .bs-example {
            margin: 20px;
        }

        .Grid {
            background-color: #fff;
            margin: 10px 0 10px 0;
            border: solid 2px #525252;
            border-collapse: collapse;
            color: #474747;
            font-size: 15px;
        }

            .Grid td {
                padding: 2px;
                border: solid 2px #716d6d;
            }

            .Grid th {
                padding: 10px 15px;
                color: #fff;
                background: #6365d2;
                border-left: solid 2px #525252;
                font-size: 0.9em;
            }

            .Grid .alt {
                background: #fcfcfc;
            }

            .Grid .pgr {
                background: #363670;
            }

                .Grid .pgr table {
                    margin: 3px 0;
                }

                .Grid .pgr td {
                    border-width: 0;
                    padding: 0 6px;
                    border-left: solid 1px #666;
                    font-weight: bold;
                    color: #fff;
                    line-height: 12px;
                }

                .Grid .pgr a {
                    color: Gray;
                    text-decoration: none;
                }

                    .Grid .pgr a:hover {
                        color: #525252;
                        text-decoration: none;
                    }

        a {
            color: #7460EE;
        }

        .modal-lg {
            max-width: 800px !important;
        }

        .Grid1 {
            margin: 10px 0 10px 0;
            border: solid 2px #525252;
            color: #474747;
            font-size: 15px;
        }

            .Grid1 td {
                padding: 2px;
                border: solid 2px #716d6d;
            }

            .Grid1 th {
                border: solid 2px white;
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
        function openInNewTab() {
            window.document.forms[0].target = '_blank';
            setTimeout(function () { window.document.forms[0].target = ''; }, 0);
        }
    </script>


    <script type="text/javascript">
        function name_changed(val) {

            if (val == "txt_MemName") {

                var mytext = document.getElementById("<%=txt_MemName.ClientID %>").value;
                var newText = mytext.replaceAll('\'', '`');
                document.getElementById("<%=txt_MemName.ClientID %>").value = newText;

            }

        }
    </script>

     
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="false" />
    <div class="container-fluid">
        <div class="row">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <asp:HiddenField ID="HiddenField1" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div class="card" style="width: 100%;">
                    <h4 class="card-header"><strong>Managing Committee Details </strong><strong id="renewhead1" runat="server" visible="false">(Schedule I) </strong><strong>:: </strong></h4>
                    <div class="card-body">
                        <div class="form-group">
                            <asp:Label ID="Label6" runat="server" ForeColor="Blue"></asp:Label>

                        </div>
                        <asp:HiddenField ID="HFEdit_flag" runat="server" />

                        <div class="row">
                            <div class="table-responsive">
                                <div class="table table-bordered table-hover">
                                    <table style="width: 100%; font-size: small">
                                        <thead> 
                                            <tr id="oldmembersgridview" runat="server">
                                                <td colspan="6">
                                                    <h3 class="text-center">Managing Committee Members at the time of Previous Registration</h3>
                                                    <div class="justify-content-center" style="text-align: center; width: 100%; margin: auto">
                                                        <asp:GridView ID="grvOldMemberDetails" runat="server" CellPadding="3" OnRowDataBound="grvOldMemberDetails_RowDataBound" AutoGenerateColumns="false" AllowPaging="true" PageSize="8"
                                                            Font-Size="Small" OnPageIndexChanging="grvMemberDetails_PageIndexChanging" CssClass="Grid1" AlternatingRowStyle-CssClass="alt" PagerStyle-CssClass="pgr" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true">
                                                            <AlternatingRowStyle BackColor="White" />
                                                            <FooterStyle BackColor="#CCCC99" />
                                                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                                            <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                                            <RowStyle BackColor="#F7F7DE" />
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
                                                                <asp:BoundField HeaderText="Salutation" DataField="salutation" />
                                                                <asp:BoundField HeaderText="Name" DataField="fname" />
                                                                <asp:BoundField HeaderText="Gender" DataField="gender" />
                                                                <asp:BoundField HeaderText="Age" DataField="age" />
                                                                <asp:BoundField HeaderText="Designation" DataField="designtaion" />
                                                                <asp:BoundField HeaderText="Occupation" DataField="occupation" />
                                                                <asp:BoundField HeaderText="Address" DataField="address" />
                                                                <asp:BoundField HeaderText="ID Proof" DataField="proofname" />
                                                                <%--<asp:BoundField HeaderText="Managing Committee" DataField="mangcomm" />--%>
                                                                <asp:BoundField HeaderText="Proof Document No" DataField="proof_document_no" />

                                                                <asp:TemplateField HeaderText="View">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="LbViewOld" CausesValidation="false" runat="server" OnClick="LbViewOld_Click"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton>
                                                                        <asp:HiddenField ID="hfmemIDOld" Value='<%# Eval("member_id") %>' runat="server" />
                                                                        <asp:HiddenField ID="hfobjectIDOld" Value='<%# Eval("document_mongoentry") %>' runat="server" />
                                                                        <%-- <asp:HiddenField ID="hfmcomm" Value='<%# Eval("mangcomm_id") %>' runat="server" /> --%>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Add">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="LBAdd" CausesValidation="false" runat="server" OnClick="LBAdd_Click"><i class="fa fa-plus-square"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                  <asp:TemplateField HeaderText="Edit" Visible="false">
                                                                    <ItemTemplate>

                                                                        <asp:Label ID="lbold_proof_id" runat="server" Text='<%# Eval("proofid") %>'></asp:Label>
                                                                         <asp:Label ID="lbold_memberid" runat="server" Text='<%# Eval("member_id") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </td>
                                            </tr>

                                            <tr id="Oldmembers_tr" runat="server">
                                                <td colspan="6">
                                                    <asp:Label ID="Label5" runat="server" ForeColor="Red">Note: If you want to add any Member from Previous Members to Current Members List,Please click on Add button corresponding to it in the above list.</asp:Label>
                                                </td>

                                            </tr>
                                            <tr>
                                                <td colspan="6" style="text-align: center">
                                                    <asp:Button ID="Button1" CausesValidation="false" OnClientClick="CoverClickLK('Button1');" CssClass="btn btn-primary" runat="server" OnClick="Button1_Click" Text="Add Managing Committee  " />
                                                </td>
                                            </tr>
                                            <tr runat="server" visible="false">
                                                <td colspan="6">
                                                    <h3 class="text-center">Managing Committee Details</h3>
                                                    <h3 id="renewhead2" runat="server" visible="false" class="text-center">(Schedule I)</h3>
                                                    <div class=" justify-content-center" style="text-align: center; width: 100%; margin: auto">
                                                        <asp:GridView ID="gv_managingcomm" runat="server" CellPadding="3" OnRowDataBound="gv_managingcomm_RowDataBound" AutoGenerateColumns="false" Font-Size="Small" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
                                                            <AlternatingRowStyle BackColor="White" />
                                                            <FooterStyle BackColor="#CCCC99" />
                                                            <HeaderStyle BackColor="#6B695B" Font-Bold="True" ForeColor="White" />
                                                            <RowStyle BackColor="#F7F7DE" />
                                                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Sr. No">
                                                                    <ItemTemplate>
                                                                        <asp:Label runat="server" ID="lblRowNumber1" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField HeaderText="Salutation" DataField="salutation" />
                                                                <asp:BoundField HeaderText="Name" DataField="fname" />
                                                                <asp:BoundField HeaderText="Gender" DataField="gender" />
                                                                <asp:BoundField HeaderText="Age" DataField="age" />
                                                                <asp:BoundField HeaderText="Designation" DataField="designtaion" />
                                                                <asp:BoundField HeaderText="Occupation" DataField="occupation" />
                                                                <asp:BoundField HeaderText="Address" DataField="address" />
                                                                <asp:BoundField HeaderText="ID Proof" DataField="proofname" />
                                                                <asp:BoundField HeaderText="Remarks" DataField="remarks" />
                                                                <asp:BoundField HeaderText="Proof Document No" DataField="proof_document_no" />
                                                                <asp:BoundField HeaderText="Date of Admission" DataField="dateofadmission" DataFormatString="{0:dd/MM/yyyy}" />
                                                                <asp:TemplateField HeaderText="Edit" Visible="false">
                                                                    <ItemTemplate>

                                                                        <asp:Label ID="lbmang_proof_id" runat="server" Text='<%# Eval("proofid") %>'></asp:Label>
                                                                         <asp:Label ID="lbmang_memberid" runat="server" Text='<%# Eval("member_id") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                <div align="center">No records found.</div>
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr runat="server" visible="false">
                                                <td>
                                                    <asp:Label runat="server" ID="Label9" CssClass="col-sm-3 col-form-label mylabel" Text="Total number of Managing Committee Members"></asp:Label></td>
                                                <td colspan="5">
                                                    <asp:TextBox runat="server" ID="txttotmangcomm" Enabled="false" CssClass="form-control" MaxLength="3"></asp:TextBox></td>
                                            </tr>

                                            <tr>

                                                <td colspan="6">
                                                    <h3 class="text-center">Managing Committee Details</h3>
                                                    <h3 id="renewhead3" runat="server" visible="false" class="text-center">(Schedule I)</h3>
                                                    <div class=" justify-content-center" style="text-align: center; width: 100%; margin: auto">
                                                        <asp:GridView ID="grvMemberDetails" runat="server" CellPadding="3" OnRowDataBound="grvMemberDetails_RowDataBound" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" Font-Size="Small" OnPageIndexChanging="grvMemberDetails_PageIndexChanging" CssClass="Grid" AlternatingRowStyle-CssClass="alt" PagerStyle-CssClass="pgr" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true">
                                                            <AlternatingRowStyle BackColor="White" />
                                                            <FooterStyle BackColor="#CCCC99" />
                                                            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                                            <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                                            <RowStyle BackColor="#F7F7DE" />
                                                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                                            <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                                            <SortedAscendingHeaderStyle BackColor="#848384" />
                                                            <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                                            <SortedDescendingHeaderStyle BackColor="#575357" />
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Sr. No">
                                                                    <ItemTemplate>
                                                                        <asp:Label runat="server" ID="lblRowNumber2" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField HeaderText="Salutation" DataField="salutation" />
                                                                <asp:BoundField HeaderText="Name" DataField="fname" />
                                                                <asp:BoundField HeaderText="Gender" DataField="gender" />
                                                                <asp:BoundField HeaderText="Age" DataField="age" />
                                                                <asp:BoundField HeaderText="Designation" DataField="designtaion" />
                                                                <asp:BoundField HeaderText="Occupation" DataField="occupation" />
                                                                <asp:BoundField HeaderText="Address" DataField="address" />
                                                                <asp:BoundField HeaderText="ID Proof" DataField="proofname" />
                                                                <%--<asp:BoundField HeaderText="Managing Committee" DataField="mangcomm" />--%>
                                                                <asp:BoundField HeaderText="Proof Document No" DataField="proof_document_no" />
                                                                <asp:BoundField HeaderText="Date of admission" DataField="dateofadmission" DataFormatString="{0:dd/MM/yyyy}" />
                                                                <asp:BoundField HeaderText="Remarks" DataField="remarks" />

                                                                <asp:TemplateField HeaderText="Edit">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="LBUpdate" CausesValidation="false" runat="server" OnClick="LBUpdate_Click"><i class="far fa-edit" ></i></asp:LinkButton>
                                                                        <asp:HiddenField ID="hfdesignid" Value='<%# Eval("design") %>' runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Delete">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="LBDelete" CausesValidation="false" runat="server" OnClick="LBDelete_Click"><i class="fa fa-trash"></i></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="View">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="LbView" CausesValidation="false" runat="server" OnClick="LbView_Click"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton>
                                                                        <asp:HiddenField ID="hfmemID" Value='<%# Eval("member_id") %>' runat="server" />
                                                                        <asp:HiddenField ID="hfobjectID" Value='<%# Eval("document_mongoentry") %>' runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                  <asp:TemplateField HeaderText="Edit" Visible="false">
                                                                    <ItemTemplate>

                                                                        <asp:Label ID="lbupdate_proof_id" runat="server" Text='<%# Eval("proofid") %>'></asp:Label>
                                                                         <asp:Label ID="lbupdate_memberid" runat="server" Text='<%# Eval("member_id") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                <div align="center">No records found.</div>
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label runat="server" ID="lblTotMembers" CssClass="col-sm-3 col-form-label mylabel" Text="Total number of Managing Committee Members"></asp:Label></td>
                                                <td colspan="5">
                                                    <asp:TextBox runat="server" ID="txtTotMember" Enabled="false" CssClass="form-control" MaxLength="3"></asp:TextBox></td>
                                            </tr>


                                            <tr>
                                                <asp:Label ID="Status" runat="server" Text=""></asp:Label>
                                            </tr>

                                            <tr runat="server" visible="false">


                                                <td>
                                                    <asp:Label ID="Label4" runat="server" Text="Add Members List" Font-Bold="true"></asp:Label></td>

                                                <td>
                                                    <asp:FileUpload ID="FileUpload1" runat="server" /></td>

                                                <td id="listupload" runat="server">
                                                    <asp:LinkButton ID="LB_memberslist_upload" OnClientClick="CoverClickLK('LB_memberslist_upload');" CausesValidation="false" runat="server" ToolTip="Upload File" OnClick="LB_memberslist_upload_Click"><i class="fa fa-upload" ></i></asp:LinkButton></td>
                                                <td id="listdelete" runat="server">
                                                    <asp:LinkButton ID="LB_memberslist_delete" OnClientClick="CoverClickLK('LB_memberslist_delete');" CausesValidation="false" runat="server" ToolTip="Delete File" OnClick="LB_memberslist_delete_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_memberslist_view" OnClientClick="CoverClickLK('LB_memberslist_view');" CausesValidation="false" runat="server" OnClick="LB_memberslist_view_Click" ToolTip="View File"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:RegularExpressionValidator ID="revfileupload1" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload1" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File." Display="Dynamic" />
                                                    <asp:Label ID="lbfu1status" runat="server"></asp:Label>
                                                </td>

                                            </tr>
                                            <tr>
                                                <td colspan="6">
                                                    <asp:Button ID="btnBack" OnClientClick="CoverClickLK('btnBack');" runat="server" Text="Back" CssClass="btn btn-warning" OnClick="btnBack_Click" CausesValidation="false" />
                                                    <asp:Button ID="MemberDetailsBtn" OnClientClick="CoverClickLK('MemberDetailsBtn');" runat="server" Text="Next" Style="margin-left: 75%" CssClass="btn btn-primary" OnClick="MemberDetailsBtn_Click" CausesValidation="false" /></td>

                                            </tr>
                                        </thead>
                                    </table>
                                </div>
                            </div>
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
                Height="63%" Width="20%" Style="vertical-align: middle;" />
        </div>
    </div>

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="myModal" class="modal fade">
            <div class="modal-dialog  modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label1" runat="server" Text="" ForeColor="Green"></asp:Label>
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
        <div id="addmembersmodal" class="modal">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Managing Committee Details</h4>
                    </div>
                    <div class="modal-body">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                            <ContentTemplate>

                                <div class="table-responsive">
                                    <div class="table table-bordered table-hover">
                                        <table style="width: 100%">
                                            <thead>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="lbl_salutation" runat="server" Text="Salutation" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:DropDownList ID="ddl_salutation" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator7" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="ddl_salutation" ErrorMessage="Please Select Salutation"> </asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label runat="server" ID="lbl_MemName" Text="Full Name" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox runat="server" ID="txt_MemName" MaxLength="50" CssClass="form-control" autocomplete="off" AutoCompleteType="Disabled" onkeyup="name_changed('txt_MemName');"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvfullname" Display="Dynamic" runat="server" ControlToValidate="txt_MemName" CssClass="text-danger" ErrorMessage="Enter Full Name"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revfirstname" runat="server" Display="Dynamic" ControlToValidate="txt_MemName" ForeColor="Red" ValidationExpression="[\sa-zA-Z`]*$" ErrorMessage="Invalid Name" /><br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="lbl_gender" runat="server" Text="Gender" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:RadioButtonList ID="Rdbtngender" RepeatDirection="Horizontal" OnSelectedIndexChanged="Rdbtngender_SelectedIndexChanged" Width="100%" runat="server" AutoPostBack="true">
                                                            <asp:ListItem Value="M">Male</asp:ListItem>
                                                            <asp:ListItem Value="F">Female</asp:ListItem>
                                                            <asp:ListItem Value="T">Transgender</asp:ListItem>

                                                        </asp:RadioButtonList>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" CssClass="text-danger" Display="Dynamic" ControlToValidate="Rdbtngender" ErrorMessage="Select atleast One"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label runat="server" ID="lbl_age" Text="Age" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox runat="server" ID="txtbx_age" MaxLength="3" CssClass="form-control" autocomplete="off" AutoCompleteType="Disabled"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" Display="Dynamic" runat="server" ControlToValidate="txtbx_age" CssClass="text-danger" ErrorMessage="Enter Age"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td class="a">
                                                        <asp:Label runat="server" ID="lbl_DesignMem" Text="Designation" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:DropDownList ID="ddl_DesignMem" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_DesignMem_SelectedIndexChanged" CssClass="form-control"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator2" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="ddl_DesignMem" ErrorMessage="Please Select Designation"> </asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr id="row_DesignOthers" runat="server" visible="false">
                                                    <td class="a">
                                                        <asp:Label runat="server" ID="lbl_DesignOthers" Text="Designation(Others)" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox ID="TxtBx_DesignOthers" CssClass="form-control" runat="server" autocomplete="off" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" Display="Dynamic" runat="server" ControlToValidate="TxtBx_DesignOthers" CssClass="text-danger" ErrorMessage="Enter Designation"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="TxtBx_DesignOthers" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\sa-zA-Z]*$" ErrorMessage="No special characters allowed." />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label runat="server" ID="lbl_MemOccup" Text="Occupation" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:DropDownList ID="ddl_MemOccup" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_MemOccup_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator1" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="ddl_MemOccup" ErrorMessage="Please Select Profession"> </asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr id="row_OccupOthers" runat="server" visible="false">
                                                    <td class="a">
                                                        <asp:Label runat="server" ID="lbl_OccupOthers" Text="Occupation(Others)" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox ID="TxtBx_OccupOthers" CssClass="form-control" runat="server" MaxLength="100" autocomplete="off" AutoCompleteType="Disabled"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" Display="Dynamic" runat="server" ControlToValidate="TxtBx_OccupOthers" CssClass="text-danger" ErrorMessage="Enter Other Occupation Name"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="TxtBx_OccupOthers" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\sa-zA-Z]*$" ErrorMessage="No special characters allowed." />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="lbl_MemAddress" runat="server" Text="Address:" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox ID="txt_MemAddress" CssClass="form-control" runat="server" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" Display="Dynamic" runat="server" ControlToValidate="txt_MemAddress" CssClass="text-danger" ErrorMessage="Enter Full Address"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revbuilding" runat="server" ControlToValidate="txt_MemAddress" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\sa-zA-Z0-9()-,._/:@-]+$" ErrorMessage="No special characters allowed." />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="lbl_MemDocType" runat="server" Text="ID Proof" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>

                                                    </td>
                                                    <td class="b">
                                                        <asp:DropDownList ID="ddl_MemDocType" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddl_MemDocType_SelectedIndexChanged" CssClass="form-control"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator4" Display="Dynamic" runat="server" ControlToValidate="ddl_MemDocType" CssClass="text-danger" ErrorMessage="Please Select ID Proof "></asp:RequiredFieldValidator><br />
                                                        <asp:Label ID="Label3" runat="server" Text="Please avoid uploading Aadhaar Card as ID Proof." CssClass="control-label" ForeColor="OrangeRed"></asp:Label>

                                                    </td>
                                                </tr>
                                                <tr id="trvisible" runat="server" >
                                                    <td class="a">
                                                        <asp:Label ID="Label2" runat="server" Text="" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>

                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox runat="server" ID="TxtBxDocumentNo" autocomplete="off" AutoCompleteType="Disabled" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" Display="Dynamic" runat="server" ControlToValidate="TxtBxDocumentNo" CssClass="text-danger" ErrorMessage="Enter Proof No"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="TxtBxDocumentNo" CssClass="text-danger" Display="Dynamic" ValidationExpression="[a-zA-Z0-9]*$" ErrorMessage="No special characters allowed." />
                                                        <asp:HiddenField ID="hfencryptNo" runat="server" />
                                                         
                                                        <asp:HiddenField ID="originalAadhar" runat="server" /> 
                                                         <%--   <div class="input-group-append">
                                                <button id="show_password" class="btn btn-primary" type="button">
                                                    <span class="fa fa-eye-slash icon"></span>
                                                </button>
                                            </div>--%> </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="lbl_ManComm" runat="server" Text="Managing Committee" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:DropDownList ID="ddlMcom" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddlMcom_SelectedIndexChanged" CssClass="form-control" AppendDataBoundItems="True">
                                                            <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                                            <%--          <asp:ListItem Text="No" Value="2"></asp:ListItem>--%>
                                                        </asp:DropDownList>
                                                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator5" Display="Dynamic" runat="server" ControlToValidate="ddlMcom" CssClass="text-danger" ErrorMessage="Please Select part of Managing Committee "></asp:RequiredFieldValidator>
                                                        <div class="alert alert-info" role="alert">
                                                            Part of Managing Committee
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr id="tr_dateofadmission" runat="server" visible="false">
                                                    <td class="a">
                                                        <asp:Label ID="Label10" runat="server" Text="Date of Admission" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <%--<input  name="party" min="2017-04-01" max="2017-04-20" >--%>
                                                        <asp:TextBox ID="txtbxdateadmission" type="date" ToolTip="Date of Admission" placeholder="Date of Admission" autocomplete="off" CssClass="form-control" runat="server"></asp:TextBox>

                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator12" Display="Dynamic" runat="server" CssClass="text-danger" ControlToValidate="txtbxdateadmission" ErrorMessage="Date of Admission is blank!" SetFocusOnError="True" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="Label7" runat="server" CssClass="control-label" Text="Remarks" Font-Bold="true"></asp:Label>
                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox ID="TxtBxRemarks" CssClass="form-control" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="Label8" runat="server" CssClass="control-label" Text="Upload ID" Font-Bold="true"></asp:Label><span id="upload" class="text-danger" runat="server">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:FileUpload ID="FileUpload6" runat="server" /><br />
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload6" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File." Display="Dynamic" /><br />
                                                        <asp:Label runat="server" ID="lbupload" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                        <asp:Label runat="server" ID="lblError" Text="" Width="100%" ForeColor="Red" CssClass="alert-danger" Font-Bold="true"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:Button ID="btnUpdate" CssClass="btn btn-primary" OnClientClick="CoverClickLK('btnUpdate');" Visible="false" runat="server" OnClick="btnUpdate_Click" Text="Update" />
                                                        <%--CausesValidation="false"--%>
                                                        <asp:Button ID="btnAdd" CssClass="btn btn-primary" OnClientClick="CoverClickLK('btnAdd');" runat="server" OnClick="btnAdd_Click" Text="Add" /><%--CausesValidation="false"--%>
                                                        <asp:Button ID="btnAddOld" CssClass="btn btn-info" OnClientClick="CoverClickLK('btnAddOld');" runat="server" OnClick="btnAddOld_Click" Text="Add" /><%--CausesValidation="false"--%>
                                              
                                                    </td>
                                                </tr>
                                                
                                                <tr>
                                                    <td style="color: orangered" colspan="2">* Only PDF File Type & Maximum File Size Allowed is 2 MB 
                                                    </td>

                                                </tr>

                                            </thead>
                                        </table>
                                    </div>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <%--<asp:AsyncPostBackTrigger ControlID="ddl_DesignMem" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ddl_MemOccup" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ddl_MemDocType" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="TxtBxDocumentNo" EventName="TextChanged" />--%>
                                <asp:PostBackTrigger ControlID="btnUpdate" />
                                <asp:PostBackTrigger ControlID="btnAdd" />
                                <asp:PostBackTrigger ControlID="btnAddOld" />

                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="close_modal" OnClick="close_modal_Click" OnClientClick="CoverClickLK('close_modal');" CausesValidation="false" CssClass="btn btn-danger" runat="server" Text="Close" />
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

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="errorModal" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label47" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label48" CssClass="control-label" runat="server" Text=""></asp:Label>
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
        <div id="fetchmembersconfirm" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label14" runat="server" Text="" ForeColor="OrangeRed"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--    <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                        <asp:Button ID="btn_fetchcancel" OnClientClick="CoverClickLK('btn_fetchcancel');" CssClass="btn btn-danger" runat="server" Text="No" CausesValidation="false" />
                        <asp:Button ID="btn_fetchconfirm" OnClientClick="CoverClickLK('btn_fetchconfirm');" CssClass="btn btn-primary" runat="server" Text="Confirm" CausesValidation="false" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%--  <link href="../Admin/datepicker/jquery-ui.css" rel="stylesheet" />
        <script src="../Admin/datepicker/jquery-1.10.2.js"></script>--%>
    <%-- <script src="../admin/datepicker/jquery-ui.js"></script>--%>
</asp:Content>
