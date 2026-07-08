<%@ Page Title="" Language="C#" MasterPageFile="~/User/Society.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="DocumentUpload.aspx.cs" Inherits="GoaSocietyRegistration.DocumentUpload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
  <%--  <script src="../Scripts/jquery-3.5.0.js"></script>
    <script src="../assets/js/jquery.min.js"></script>--%>
    <style type="text/css">
        .uppercase {
            text-transform: uppercase;
        }

        .dis {
            cursor: not-allowed;
        }

        .bs-example {
            margin: 20px;
        }

        .a {
            width: 30%;
        }

        .b {
            width: 70%;
        }

        .modal-lg {
            max-width: 950px !important;
        }
       /*a {
    color: #0d6efd!important;
    text-decoration: underline;
}*/
    </style>
    <script>
        function Popup(url) {
            window.open(url, "myWindow", "status = 1, height = 600, width = 800, resizable = 0")
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

        <link href="../assets/css/StyleSheet.css" rel="stylesheet" />
    <script src="../Scripts/Script.js"></script>

    <script type="text/javascript">
        function openInNewTab() {
            window.document.forms[0].target = '_blank';
            setTimeout(function () { window.document.forms[0].target = ''; }, 0);
        }    
			
		</script>

     <script type="text/javascript">
        function CoverClickLK(val) {
            if (val == "BtnBack") {
                document.getElementById("<%=BtnBack.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "Button1") {
                document.getElementById("<%=Button1.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "Button2") {
                document.getElementById("<%=Button2.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "BtnMemo") {
                document.getElementById("<%=BtnMemo.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Application_Upload") {
                document.getElementById("<%=LB_Application_Upload.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Application_Delete") {
                document.getElementById("<%=LB_Application_Delete.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "btnAdd") {
                document.getElementById("<%=btnAdd.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "btnSubmitfianl") {
                document.getElementById("<%=btnSubmitfianl.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "gotohomepage") {
                document.getElementById("<%=gotohomepage.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "btnrenewalform_view") {
                document.getElementById("<%=btnrenewalform_view.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LkAddDocs_renew") {
                document.getElementById("<%=LkAddDocs_renew.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "btnSubmit") {
                document.getElementById("<%=btnSubmit.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "LB_Application_View") {
                document.getElementById("<%=LB_Application_View.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Memorandom_Upload") {
                document.getElementById("<%=LB_Memorandom_Upload.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Memorandum_Delete") {
                document.getElementById("<%=LB_Memorandum_Delete.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "LB_Bylaws_Upload") {
                document.getElementById("<%=LB_Bylaws_Upload.ClientID %>").style.display = "none";
                    CoverClick1();
            }else if (val == "LB_Bylaws_Delete") {
                document.getElementById("<%=LB_Bylaws_Delete.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Bylaws_View") {
                document.getElementById("<%=LB_Bylaws_View.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LkAddDocs_reg") {
                document.getElementById("<%=LkAddDocs_reg.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "Button3") {
                document.getElementById("<%=Button3.ClientID %>").style.display = "none";
                    CoverClick1();
            }else if (val == "Lb_RenewalApplication_Upload") {
                document.getElementById("<%=Lb_RenewalApplication_Upload.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "Lb_RenewalApplication_Delete") {
                document.getElementById("<%=Lb_RenewalApplication_Delete.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "Lb_RenewalApplication_View") {
                document.getElementById("<%=Lb_RenewalApplication_View.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "Lb_GenerateRenewalApplication") {
                document.getElementById("<%=Lb_GenerateRenewalApplication.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "LB_GenerateSchedule1") {
                document.getElementById("<%=LB_GenerateSchedule1.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Schedule1_Upload") {
                document.getElementById("<%=LB_Schedule1_Upload.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Schedule1_Delete") {
                document.getElementById("<%=LB_Schedule1_Delete.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "LB_Schedule1_View") {
                document.getElementById("<%=LB_Schedule1_View.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "LB_GenerateSchedule6") {
                document.getElementById("<%=LB_GenerateSchedule6.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Schedule6_Upload") {
                document.getElementById("<%=LB_Schedule6_Upload.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Schedule6_View") {
                document.getElementById("<%=LB_Schedule6_View.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "LB_Schedule6_Delete") {
                document.getElementById("<%=LB_Schedule6_Delete.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "LB_GenerateSchedule2") {
                document.getElementById("<%=LB_GenerateSchedule2.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Schedule2_Upload") {
                document.getElementById("<%=LB_Schedule2_Upload.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Schedule2_View") {
                document.getElementById("<%=LB_Schedule2_View.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "LB_Schedule2_Delete") {
                document.getElementById("<%=LB_Schedule2_Delete.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "LB_GenerateSchedule4") {
                document.getElementById("<%=LB_GenerateSchedule4.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Schedule4_Upload") {
                document.getElementById("<%=LB_Schedule4_Upload.ClientID %>").style.display = "none";
                CoverClick1();
            }
            else if (val == "LB_Schedule4_View") {
                document.getElementById("<%=LB_Schedule4_View.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "LB_Schedule4_Delete") {
                document.getElementById("<%=LB_Schedule4_Delete.ClientID %>").style.display = "none";
                    CoverClick1();
            }
            else if (val == "LB_Memorandum_View") {
                document.getElementById("<%=LB_Memorandum_View.ClientID %>").style.display = "none";
                    CoverClick1();
            }
}
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <div class="row">
            <div class="col-lg-12  col-md-12  col-sm-12 col-xs-12">
                <div class="card" style="width: 100%;"> 
                      <h4 class="card-header"><strong>Document Upload :: </strong></h4>                  
                    <div class="card-body">                      
                        <div class="form-group">
                            <asp:HiddenField ID="HiddenField1" runat="server" />
                            <asp:Label ID="Label3" runat="server"> NOTE - दस्तावेज अपलोड करने के लिए गूगल क्रोम या मोज़िला फ़ायरफ़ॉक्स का नवीनतम संस्करण का उपयोग करें/</asp:Label><br />
                            <asp:Label ID="Label9" runat="server">Please Use Latest Version Of Google Chrome or Mozilla Firefox To Upload Documents</asp:Label>
                        </div>
                        <div class="row" runat="server">
                            <div class="table-responsive">
                                <div class="table table-bordered table-hover">
                                    <table style="width: 100%">
                                        <tr>
                                            <td align="right">File Type Allowed
                                            </td>
                                            <td>
                                                <b style="color: red">Only PDF File
                                                </b>
                                            </td>
                                            <td align="right">Maximum File Size Allowed
                                            </td>
                                            <td>
                                                <b style="color: red">2 MB</b>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class="row" runat="server">
                            <div class="table-responsive">
                                <div id="documents" class="table table-bordered table-hover" runat="server">
                                    <table style="width: 100%">
                                        <thead>
                                            <tr>
                                                <th>
                                                    <asp:Label ID="lbsrno" runat="server" Text="Sr. No."></asp:Label></th>
                                                <th>
                                                    <asp:Label ID="lblchecklist" runat="server" Text="CheckList"></asp:Label></th>
                                                <th>
                                                    <asp:Label ID="lbheader" runat="server" Text="Name of Document"></asp:Label></th>
                                                <th>
                                                    <asp:Label ID="LBUpload" runat="server" Text="Document Upload"></asp:Label></th>
                                                <th>
                                                    <asp:Label ID="Label12" runat="server" Text=""></asp:Label></th>
                                                <th>
                                                    <asp:Label ID="Label13" runat="server" Text=""></asp:Label></th>
                                                <th>
                                                    <asp:Label ID="Labelpdf" runat="server" Text=""></asp:Label></th>
                                                <th>
                                                    <asp:Label ID="lbStatus" runat="server" Text="Status"></asp:Label></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr><td colspan="4" style="color:orangered"> Note:- Generate the Application for Registration. Self Attest and Upload the Same.</td><td colspan="4"> <asp:Button ID="Button2" runat="server" CssClass="btn btn-primary" OnClientClick="CoverClickLK('Button2');" OnClick="Button2_Click" Text="Generate the Application for Registration" /></td></tr>
                                            <tr><td colspan="4" style="color:orangered"> Note:- Generate the Memorandum of Association. Self Attest and Upload the Same. </td><td colspan="4"><asp:Button ID="BtnMemo"  runat="server" CssClass="btn btn-primary" OnClientClick="CoverClickLK('BtnMemo');" OnClick="BtnMemo_Click" Text="Generate the Memorandum of Association" /></td></tr>
                                            <tr><td colspan="4" style="color:orangered"> Note:- Download Rules & Regulations, Fill the form and Upload. </td><td colspan="4"><a href="../Download/ByLaws.pdf" class="btn btn-primary" attributes-list download ="ByLaws.pdf" ><i class="fa fa-download" aria-hidden="true"></i>&nbsp;Download</a></td></tr>
                                            <%--<a href ="../Download/ByLaws.pdf" attributes-list download ="optional-value" > <img src="../assets/images/pdf.png" /> </a>--%>
                                            <tr>                                                                                               
                                                <td>
                                                    <asp:Label ID="lb1" runat="server" Text="1"></asp:Label></td>
                                                <td>
                                                    <asp:CheckBox ID="CheckBox1" runat="server" Text=" " Enabled="false" Checked="true" AutoPostBack="true" /></td>
                                                <td>
                                                    <asp:Label ID="Label1" runat="server" Text="Application for Registration"></asp:Label><span class="text-danger">*</span></td>

                                                <td>
                                                    <asp:FileUpload ID="FileUpload1" runat="server" /></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Application_Upload" runat="server" OnClientClick="CoverClickLK('LB_Application_Upload');"  ToolTip="Upload File" OnClick="LB_Application_Upload_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Application_Delete" runat="server" OnClientClick="CoverClickLK('LB_Application_Delete');" ToolTip="Delete File" OnClick="LB_Application_Delete_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Application_View" runat="server" OnClientClick="CoverClickLK('LB_Application_View');" OnClick="LB_Application_View_Click"  ToolTip="View File" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:RegularExpressionValidator ID="revfileupload1" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload1" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbfu1status" runat="server"></asp:Label></td>
                                            </tr>
                                            
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lb2" runat="server" Text="2"></asp:Label></td>
                                                <td>
                                                    <asp:CheckBox ID="chkDoc1" runat="server" Text=" " Checked="true" /></td>
                                                <td>
                                                    <asp:Label ID="Label4" runat="server" Text="Memorandum of Association"></asp:Label><span class="text-danger">*</span></td>
                                                <td>
                                                    <asp:FileUpload ID="FileUpload2" runat="server" /></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Memorandom_Upload" runat="server" OnClientClick="CoverClickLK('LB_Memorandom_Upload');" ToolTip="Upload File" OnClick="LB_Memorandom_Upload_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Memorandum_Delete" runat="server" OnClientClick="CoverClickLK('LB_Memorandum_Delete');" ToolTip="Delete File" OnClick="LB_Memorandum_Delete_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Memorandum_View" runat="server" OnClientClick="CoverClickLK('LB_Memorandum_View');" OnClick="LB_Memorandum_View_Click"  ToolTip="View File" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:RegularExpressionValidator ID="revfileupload2" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload2" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbfu2status" runat="server"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lb3" runat="server" Text="3"></asp:Label></td>
                                                <td>
                                                    <asp:CheckBox ID="chkDoc2" runat="server" Text=" " Checked="true" /></td>
                                                <td>
                                                    <asp:Label ID="Label6" runat="server" Text="Rules And Regulation/Constitution of Association/By-laws"></asp:Label><span class="text-danger">*</span></td>
                                                <td>
                                                    <asp:FileUpload ID="FileUpload3" runat="server" /></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Bylaws_Upload" runat="server" OnClientClick="CoverClickLK('LB_Bylaws_Upload');" ToolTip="Upload File" OnClick="LB_Bylaws_Upload_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Bylaws_Delete" runat="server" OnClientClick="CoverClickLK('LB_Bylaws_Delete');" ToolTip="Delete File" OnClick="LB_Bylaws_Delete_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Bylaws_View" runat="server" OnClientClick="CoverClickLK('LB_Bylaws_View');" Enabled="false" OnClick="LB_Bylaws_View_Click"  ToolTip="View File"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:RegularExpressionValidator ID="revfileupload3" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload3" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbfu3status" runat="server"></asp:Label></td>
                                            </tr>
                                       <%--     <tr>
                                                <td>
                                                    <asp:Label ID="lb4" runat="server" Text="4"></asp:Label></td>
                                                <td>
                                                    <asp:CheckBox ID="chkDoc5" runat="server" Text=" " Checked="true" /></td>
                                                <td>
                                                    <asp:Label ID="Label8" runat="server" Text="Certificate"></asp:Label><span class="text-danger">*</span></td>
                                                <td>
                                                    <asp:FileUpload ID="FileUpload7" runat="server" /></td>
                                                <td>
                                                    <asp:LinkButton ID="LinkButton3" runat="server" ToolTip="Upload File" OnClick="LinkButton3_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LinkButton4" runat="server" ToolTip="Delete File" OnClick="LinkButton4_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LinkButton10" runat="server" OnClick="LinkButton10_Click"  ToolTip="View File" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:RegularExpressionValidator ID="revfileupload4" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload4" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbfu4status" runat="server"></asp:Label></td>
                                            </tr>--%>
                                           <%-- <tr>
                                                <td>
                                                    <asp:Label ID="Label2" runat="server" Text="4"></asp:Label></td>
                                                <td>
                                                    <asp:CheckBox ID="chkDoc3" runat="server" Text=" " AutoPostBack="true" OnCheckedChanged="chkDoc3_CheckedChanged" /></td>
                                                <td>
                                                    <asp:TextBox runat="server" ID="txtDoc3" Width="45%" MaxLength="50"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="revfirstname" runat="server" Display="Dynamic" ControlToValidate="txtDoc3" ForeColor="Red" ValidationExpression="[\sa-zA-Z]*$" ErrorMessage="Invalid Name" /><br />
                                                    </td>
                                                <td>
                                                    <asp:FileUpload ID="FileUpload4" runat="server" /></td>
                                                <td>
                                                    <asp:LinkButton ID="LinkButton5" runat="server" ToolTip="Upload File" Enabled="false" OnClick="LinkButton5_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LinkButton6" runat="server" ToolTip="Delete File" Enabled="false" OnClick="LinkButton6_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LinkButton11" runat="server" OnClick="LinkButton11_Click"  Enabled="false" ToolTip="View File" ><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload4" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="Label5" runat="server"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label7" runat="server" Text="5"></asp:Label></td>
                                                <td>
                                                    <asp:CheckBox ID="chkDoc4" AutoPostBack="true" OnCheckedChanged="chkDoc4_CheckedChanged" runat="server" Text=" " /></td>

                                                <td>
                                                    <asp:TextBox runat="server" ID="txtDoc4" Width="45%" MaxLength="50"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" Display="Dynamic" ControlToValidate="txtDoc4" ForeColor="Red" ValidationExpression="[\sa-zA-Z]*$" ErrorMessage="Invalid Name" /><br />
                                                    </td>
                                                <td>
                                                    <asp:FileUpload ID="FileUpload5" runat="server" /></td>
                                                <td>
                                                    <asp:LinkButton ID="LinkButton7" runat="server" ToolTip="Upload File" OnClick="LinkButton7_Click" Enabled="false"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LinkButton8" runat="server" ToolTip="Delete File" Enabled="false" OnClick="LinkButton8_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LinkButton12" runat="server" OnClick="LinkButton12_Click"  Enabled="false" ToolTip="View File"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload4" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="Label10" runat="server"></asp:Label></td>
                                            </tr>--%>
                                           
                                           
                                            <tr id="tr_adddocbtn1" runat="server" >
                                                      
                                                    <td colspan="8" style="text-align: center">
                                                        <asp:LinkButton ID="LkAddDocs_reg" CssClass="btn btn-info" Style="text-align: center" OnClientClick="CoverClickLK('LkAddDocs_reg');" OnClick="LkAddDocs_Click" runat="server"><i class="fas fa-plus"></i>&nbsp;Add Additional Documents</asp:LinkButton>
                                                        <br />
                                                        <asp:Label Font-Bold="true" runat="server"  style="color:orangered; text-align:left" Text="If any additional documents are to be uploaded, Please add them here."></asp:Label>
                                                    </td>

                                                </tr>
                                            
                                            <tr id="tr_regadddocs" runat="server" visible="false">
                                                <td colspan="8">
                                                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 ">
                                                    <h3 class="text-center">Additional Documents</h3>
                                                    <div style="text-align: center;margin:auto">
                                                        <asp:GridView ID="GridViewAdditionalDocs_reg" runat="server" CellPadding="5" OnRowDataBound="GridViewAdditionalDocs_reg_RowDataBound" AutoGenerateColumns="false" style="width: 100%;" AllowPaging="true" PageSize="10" CssClass="Grid" AlternatingRowStyle-CssClass="alt" PagerStyle-CssClass="pgr" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true">
                                                        <AlternatingRowStyle BackColor="White" />
                                                        <FooterStyle BackColor="#CCCC99" />
                                                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                                        <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                                        <RowStyle BackColor="#e2def7" />
                                                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                                        <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                                        <SortedAscendingHeaderStyle BackColor="#848384" />
                                                        <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                                        <SortedDescendingHeaderStyle BackColor="#575357" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Sr. No">
                                                                <ItemTemplate>
                                                                    <%#Container.DataItemIndex+1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField HeaderText="Document Name" DataField="docname" />
                                                            <asp:TemplateField Visible="false">
                                                                <ItemTemplate>
                                                                    <asp:BoundField HeaderText="ObjectID" DataField="object_id">
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Delete">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="LBDelete" OnClick="LBDelete_Click"  CausesValidation="false" runat="server"><i class="fa fa-trash"></i></asp:LinkButton>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="View">
                                                                <ItemTemplate>
                                                                     <asp:LinkButton ID="LbView2" runat="server" OnClick="LbView_Click" ToolTip="View File"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                                    <asp:HiddenField ID="hfobjectID" Value='<%# Eval("object_id") %>' runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataTemplate>
                                                            <div align="center">No records found.</div>
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                    </div>
                                                        </div>
                                                </td>
                                            </tr>

                                            <tr style="border:none;border-color:none">
                                                <td colspan="8">
                                                    <asp:Label runat="server" ID="lblError" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label><br />
                                                    <asp:Label ID="Label11" runat="server" Text="The information filled in the application form is verified by me & is found to be correct as per my knowledge & verify the same personally. Only those documents which can are uploaded by the candidate as above shall be considered."> </asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="left">
                                                    <asp:Button ID="Button3" runat="server" OnClientClick="CoverClickLK('Button3');" OnClick="Button3_Click" Text="Back" CssClass="btn btn-warning" />
                                                </td>
                                                <td colspan="2" align="center">
                                                    <!-- Button trigger modal -->
                                                    <asp:Button ID="Button1" runat="server"  OnClientClick="CoverClickLK('Button1');" Text="View Form" OnClick="Button1_Click" CssClass="btn btn-primary" />
                                                    <%--<button type="button" class="btn btn-primary" data-toggle="modal" data-target="#exampleModalLong" onclick="viewform();">
                                                        View Form
                                                    </button>--%>

                                                </td>
                                                <td colspan="4" align="right">
                                                  
                                                    <asp:Button ID="btnSubmit" runat="server" OnClientClick="CoverClickLK('btnSubmit');" OnClick="btnconfirmsubmit_Click" Text="Submit" CssClass="btn btn-primary" />
                                                </td>
                                            </tr>

                                             
                                
                                        </tbody>
                                    </table>
                                </div>

                                  <div id="renewaldocuments" class="table table-bordered table-hover" runat="server">
                                      <asp:Label Font-Bold="true" ForeColor="OrangeRed" CssClass="ml-2" Text="Generate each Application corresponding to the name of Document, Self Attest and Upload back the same in PDF format" runat="server"></asp:Label>
                                    <table style="width: 100%">
                                        <thead>
                                            <tr style="background-color:#fdefd9">
                                                <th>
                                                    <asp:Label ID="Label8" runat="server" Text="Sr. No."></asp:Label></th>
                                               <%-- <th >
                                                    <asp:Label ID="Label22" runat="server" Text="CheckList" ></asp:Label></th>--%>
                                                <th>
                                                    <asp:Label ID="Label25" runat="server" Text="Name of Document"></asp:Label></th>
                                                 <th>
                                                    <asp:Label ID="Label2" runat="server" Text="Generate Application"></asp:Label></th>
                                                <th>
                                                    <asp:Label ID="Label26" runat="server" Text="Document Upload"></asp:Label></th>
                                                <th>
                                                    <asp:Label ID="Label27" runat="server" Text=""></asp:Label></th>
                                                <th>
                                                    <asp:Label ID="Label28" runat="server" Text=""></asp:Label></th>
                                                <th>
                                                    <asp:Label ID="Label29" runat="server" Text=""></asp:Label></th>
                                                <th>
                                                    <asp:Label ID="Label30" runat="server" Text="Status"></asp:Label></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                           <%-- <tr><td colspan="8" style="color:orangered"> Note:- Download the Application for Renewal. After that Self Attest and Upload the Same. <asp:Button ID="Btn_RenewAppl" runat="server" CssClass="btn btn-primary" OnClick="Btn_RenewAppl_Click" Text="Download" /></td></tr>--%>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label31" runat="server" Text="1"></asp:Label></td>
                                                <%--<td>
                                                    <asp:CheckBox ID="CheckBox8" runat="server" Text=" " Enabled="false" Checked="true" AutoPostBack="true"/></td>--%>
                                                <td>
                                                    <asp:Label ID="Label32" runat="server" Text="Application for Renewal"></asp:Label><span class="text-danger">*</span></td>
                                                <td>
                                                    <asp:LinkButton ID="Lb_GenerateRenewalApplication" OnClientClick="CoverClickLK('Lb_GenerateRenewalApplication');"  CssClass="btn btn-primary" runat="server" ToolTip="Upload File" OnClick="Lb_GenerateRenewalApplication_Click" ><i class="fa fa-download" aria-hidden="true"></i>&nbsp;Generate</asp:LinkButton></td>

                                                <td>
                                                    <asp:FileUpload ID="RenewalFileUpload1" runat="server" /></td>
                                                
                                                <td>
                                                    <asp:LinkButton ID="Lb_RenewalApplication_Upload" OnClientClick="CoverClickLK('Lb_RenewalApplication_Upload');" runat="server" ToolTip="Upload File" OnClick="Lb_RenewalApplication_Upload_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="Lb_RenewalApplication_Delete" OnClientClick="CoverClickLK('Lb_RenewalApplication_Delete');" runat="server" ToolTip="Delete File" OnClick="Lb_RenewalApplication_Delete_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="Lb_RenewalApplication_View" OnClientClick="CoverClickLK('Lb_RenewalApplication_View');" runat="server" OnClick="Lb_RenewalApplication_View_Click"  ToolTip="View File" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="RenewalFileUpload1" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbrfu1status" runat="server"></asp:Label></td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label50" runat="server" Text="2"></asp:Label></td>
                                                <%--<td>
                                                    <asp:CheckBox ID="CheckBox9" runat="server" Text=" " Enabled="false" Checked="true" Visible="false"/></td>--%>
                                                <td>
                                                    <asp:Label ID="Label51" runat="server" Text=" Schedule I/ Managing Committee Details"></asp:Label><span class="text-danger">*</span></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_GenerateSchedule1" OnClientClick="CoverClickLK('LB_GenerateSchedule1');" runat="server" CssClass="btn btn-primary" ToolTip="Upload File" OnClick="LB_GenerateSchedule1_Click" ><i class="fa fa-download" aria-hidden="true"></i>&nbsp;Generate</asp:LinkButton></td>
                                                <td>
                                                    <asp:FileUpload ID="RenewalFileUpload2" runat="server" /></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Schedule1_Upload" OnClientClick="CoverClickLK('LB_Schedule1_Upload');" runat="server" ToolTip="Upload File" OnClick="LB_Schedule1_Upload_Click" ><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Schedule1_Delete" OnClientClick="CoverClickLK('LB_Schedule1_Delete');" runat="server" ToolTip="Delete File" OnClick="LB_Schedule1_Delete_Click" ><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Schedule1_View" OnClientClick="CoverClickLK('LB_Schedule1_View');" runat="server" ToolTip="View File" OnClick="LB_Schedule1_View_Click" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="RenewalFileUpload2" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbrfu2status" runat="server"></asp:Label></td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label5" runat="server" Text="3"></asp:Label></td>
                                                <%--<td>
                                                    <asp:CheckBox ID="CheckBox10" runat="server" Text=" " Enabled="false" Checked="true" Visible="false"/></td>--%>
                                                <td>
                                                    <asp:Label ID="Label7" runat="server" Text="Schedule VI/ Member Details"></asp:Label><span class="text-danger">*</span></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_GenerateSchedule6" OnClientClick="CoverClickLK('LB_GenerateSchedule6');"  runat="server" CssClass="btn btn-primary" ToolTip="Upload File" OnClick="LB_GenerateSchedule6_Click" ><i class="fa fa-download" aria-hidden="true"></i>&nbsp;Generate</asp:LinkButton></td>
                                                <td>
                                                    <asp:FileUpload ID="RenewalFileUpload3" runat="server" /></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Schedule6_Upload" OnClientClick="CoverClickLK('LB_Schedule6_Upload');"  runat="server" ToolTip="Upload File" OnClick="LB_Schedule6_Upload_Click" ><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Schedule6_Delete" OnClientClick="CoverClickLK('LB_Schedule6_Delete');"  runat="server" ToolTip="Delete File" OnClick="LB_Schedule6_Delete_Click" ><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Schedule6_View" OnClientClick="CoverClickLK('LB_Schedule6_View');"  runat="server" ToolTip="View File" OnClick="LB_Schedule6_View_Click" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="RenewalFileUpload3" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbrfu3status" runat="server"></asp:Label></td>
                                            </tr>
                                            
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label34" runat="server" Text="3"></asp:Label></td>
                                                <%--<td>
                                                    <asp:CheckBox ID="CheckBox11" runat="server" Text=" " Enabled="false" Checked="true" Visible="false"/></td>--%>
                                                <td>
                                                    <asp:Label ID="Label35" runat="server" Text="Schedule II/ Employee Details"></asp:Label><span class="text-danger">*</span></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_GenerateSchedule2" OnClientClick="CoverClickLK('LB_GenerateSchedule2');" runat="server" CssClass="btn btn-primary" ToolTip="Upload File" OnClick="LB_GenerateSchedule2_Click" ><i class="fa fa-download" aria-hidden="true"></i>&nbsp;Generate</asp:LinkButton></td>
                                                <td>
                                                    <asp:FileUpload ID="RenewalFileUpload4" runat="server" /></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Schedule2_Upload" OnClientClick="CoverClickLK('LB_Schedule2_Upload');" runat="server" ToolTip="Upload File" OnClick="LB_Schedule2_Upload_Click" ><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Schedule2_Delete" OnClientClick="CoverClickLK('LB_Schedule2_Delete');" runat="server" ToolTip="Delete File" OnClick="LB_Schedule2_Delete_Click" ><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Schedule2_View" OnClientClick="CoverClickLK('LB_Schedule2_View');" runat="server" ToolTip="View File" OnClick="LB_Schedule2_View_Click" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="RenewalFileUpload4" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbrfu4status" runat="server"></asp:Label></td>
                                            </tr>                                           


                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label37" runat="server" Text="5"></asp:Label></td>
                                                <%--<td>
                                                    <asp:CheckBox ID="CheckBox12" runat="server" Text=" " Enabled="false" Checked="true" Visible="false"/></td>--%>
                                                <td>
                                                    <asp:Label ID="Label38" runat="server" Text="Schedule IV/Income and Expenditure Account"></asp:Label><span class="text-danger">*</span></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_GenerateSchedule4" OnClientClick="CoverClickLK('LB_GenerateSchedule4');"  runat="server" CssClass="btn btn-primary" ToolTip="Upload File" Visible="false"><i class="fa fa-download" aria-hidden="true"></i>&nbsp;Generate</asp:LinkButton></td>
                                                <td>
                                                    <asp:FileUpload ID="RenewalFileUpload5" runat="server" /></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Schedule4_Upload" OnClientClick="CoverClickLK('LB_Schedule4_Upload');"  runat="server" ToolTip="Upload File" OnClick="LB_Schedule4_Upload_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Schedule4_Delete" OnClientClick="CoverClickLK('LB_Schedule4_Delete');"  runat="server" ToolTip="Delete File" OnClick="LB_Schedule4_Delete_Click" ><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:LinkButton ID="LB_Schedule4_View" OnClientClick="CoverClickLK('LB_Schedule4_View');"  runat="server" Enabled="false" ToolTip="View File" OnClick="LB_Schedule4_View_Click" ><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                <td>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="RenewalFileUpload5" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbrfu5status" runat="server"></asp:Label></td>
                                            </tr>
                                      
                                          
                                            <tr></tr>
                                            <tr id="tr_adddocbtn2" runat="server" >
                                                      
                                                    <td colspan="9" style="text-align: center">
                                                         <asp:Label Font-Bold="true" runat="server"  style="color:orangered; text-align:left" Text="If any additional documents are to be uploaded, Please add them here."></asp:Label>
                                                        <br />
                                                        <asp:LinkButton ID="LkAddDocs_renew" OnClientClick="CoverClickLK('LkAddDocs_renew');" CssClass="btn btn-info" Style="text-align: center" OnClick="LkAddDocs_Click" runat="server"><i class="fas fa-plus"></i>&nbsp;Add Additional Documents</asp:LinkButton>

                                                       
                                                    </td>


                                                </tr>
                                            <tr id="tr_adddocsgridview" runat="server" visible="false">
                                                <td colspan="9">
                                                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 ">
                                                    <h3 class="text-center">Additional Documents</h3>
                                                    <div style="text-align: center;margin:auto">
                                                        <asp:GridView ID="GrideViewAdditionalDocs" runat="server" CellPadding="5" OnRowDataBound="GrideViewAdditionalDocs_RowDataBound" AutoGenerateColumns="false" style="width: 100%;" AllowPaging="true" PageSize="10" CssClass="Grid" AlternatingRowStyle-CssClass="alt" PagerStyle-CssClass="pgr" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true">
                                                        <AlternatingRowStyle BackColor="White" />
                                                        <FooterStyle BackColor="#CCCC99" />
                                                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                                        <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                                        <RowStyle BackColor="#e2def7" />
                                                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                                        <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                                        <SortedAscendingHeaderStyle BackColor="#848384" />
                                                        <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                                        <SortedDescendingHeaderStyle BackColor="#575357" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Sr. No">
                                                                <ItemTemplate>
                                                                    <%#Container.DataItemIndex+1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField HeaderText="Document Name" DataField="docname" />
                                                            <asp:TemplateField Visible="false">
                                                                <ItemTemplate>
                                                                    <asp:BoundField HeaderText="ObjectID" DataField="object_id">
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Delete">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="LBDelete" OnClick="LBDelete_Click" CausesValidation="false" runat="server"><i class="fa fa-trash"></i></asp:LinkButton>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="View">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="LbView" OnClick="LbView_Click"  CausesValidation="false" runat="server" ><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton>
                                                                    <asp:HiddenField ID="hfobjectID" Value='<%# Eval("object_id") %>' runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EmptyDataTemplate>
                                                            <div align="center">No records found.</div>
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                    </div>
                                                        </div>
                                                </td>
                                            </tr>
                                
                                           
                                            <tr>
                                                <td colspan="9">
                                                    <asp:Label runat="server" ID="Label44" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label><br />
                                                    <asp:Label ID="Label45" runat="server" Text="The information filled in the application form is verified by me & is found to be correct as per my knowledge & verify the same personally. Only those documents which are uploaded by the candidate as above shall be considered."> </asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="left">
                                                   <asp:Button ID="BtnBack" runat="server" OnClientClick="CoverClickLK('BtnBack');" OnClick="BtnBack_Click" Text="Back" CssClass="btn btn-warning" />
                                                </td>
                                                <td colspan="2" align="center">
                                                    <asp:Button ID="btnrenewalform_view" OnClientClick="CoverClickLK('btnrenewalform_view');" runat="server" Text="View Form" OnClick="btnrenewalform_view_Click" CssClass="btn btn-primary mr-4" />

                                                </td>
                                                <td colspan="4" align="right">
                                                  
                                                        <asp:Button ID="btnrenewaldocssubmit" OnClientClick="CoverClickLK('btnrenewaldocssubmit');" runat="server" OnClick="btnrenewaldocssubmit_Click" Text="Submit" CssClass="btn btn-primary" />
                                                </td>
                                            </tr>
                                             
                                        </tbody>
                                    </table>
                                </div>

                                 <%--<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12 " runat="server" id="AdditionalDocs">
                                    <div class="card" style="width: 100%;">
                                        <h5 class="card-header"></h5>
                                        <div class="card-body">

                                           
                                                
                                            
                                        </div>

                                    </div>
                                </div>--%>

                            </div>
                        </div>

                        <%--div for renewal documents--%>

                         <%--<div class="row"  runat="server">
                            <div class="table-responsive">
                              
                            </div>
                        </div>--%>

                         <div class="row">
                               
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
                Height="50%" Width="20%" Style="vertical-align: middle;" />
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
        <div id="myModal1" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Status" runat="server" Text="" ForeColor="Green"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--    <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                        <asp:Button ID="gotohomepage" OnClientClick="CoverClickLK('gotohomepage');" CssClass="btn btn-primary" runat="server" OnClick="gotohomepage_Click" Text="Ok" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="errormodal" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label15" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal -->
    <div class="modal fade" id="exampleModalLong" tabindex="-1" role="dialog" aria-labelledby="exampleModalLongTitle" aria-hidden="true">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLongTitle">Form Preview</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Applicant Details</div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-6 col-xs-12">
                            <label class="control-label" for="street">Name of The Applicant  <span style="color: red">*</span></label>
                            <asp:TextBox ID="ViewAppName" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>


                        </div>
                        <div class="col-md-6 col-xs-12">
                            <label class="control-label" for="street">Designation <span style="color: red">*</span></label>
                            <asp:TextBox ID="appdesignation" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                        </div>

                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-12 col-xs-12">
                            <label class="control-label" for="street">Address: </label>
                            <asp:TextBox ID="appaddress" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-4 col-xs-12">
                            <label class="control-label" for="street">District:<span style="color: red">*</span></label>
                            <asp:TextBox ID="appdistrict" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                        </div>
                        <div class="col-md-4 col-xs-12 ">
                            <label class="control-label" for="street">Mobile No:<span style="color: red"></span></label>
                            <asp:TextBox ID="appmobileno" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-4 col-xs-12 ">
                            <label class="control-label" for="street">Email:<span style="color: red"></span></label>
                            <asp:TextBox ID="appemail" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                        </div>

                    </div>
                    <br />
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Society Details</div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-6 col-xs-12">
                            <label class="control-label" for="street">Society Type:<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_society_type" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>


                        </div>
                        <div class="col-md-6 col-xs-12">
                            <label class="control-label" for="street">Society Name:<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_society_name" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                        </div>

                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-12 col-xs-12">
                            <label class="control-label" for="street">Society Address:</label>
                            <asp:TextBox ID="value_society_address" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-6 col-xs-12">
                            <label class="control-label" for="street">District: <span style="color: red">*</span></label>
                            <asp:TextBox ID="value_society_district" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>


                        </div>
                        <div class="col-md-6 col-xs-12">
                            <label class="control-label" for="street">Taluka:<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_taluka" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                        </div>

                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-4 col-xs-12">
                            <label class="control-label" for="street">Registration Fee:<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_registration_fee" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>

                        </div>
                        <div class="col-md-4 col-xs-12 ">
                            <label class="control-label" for="street">Proccessing Fee<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_processing_fee" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-4 col-xs-12 ">
                            <label class="control-label" for="street">Total Fee to be paid<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_total_fee" runat="server" type="text" disabled="disabled" CssClass="form-control"></asp:TextBox>
                        </div>

                    </div>
                    <br />
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Managing Committee/Member Details</div>
                    </div>
                    <br />
                    <asp:GridView runat="server" ID="grvMemberDetails" HeaderStyle-CssClass="header" RowStyle-CssClass="rows" BackColor="White" BorderColor="#DEDFDE"
                        BorderStyle="none" BorderWidth="2px" CellPadding="4" ForeColor="Black" GridLines="Horizontal" AutoGenerateColumns="false"
                        CssClass="mygrdContent rows header pager" AllowPaging="false">
                        <AlternatingRowStyle BackColor="White" />
                        <HeaderStyle BackColor="#d7dbf1" Font-Bold="true" />
                        <Columns>
                            <asp:TemplateField HeaderText="Sr. No." ItemStyle-Width="70">
                                <ItemTemplate>
                                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="Name" DataField="fname" />
                            <asp:BoundField HeaderText="Designation" DataField="designtaion" />
                            <asp:BoundField HeaderText="Occupation" DataField="occupation" />
                            <asp:BoundField HeaderText="Address" DataField="address" />
                            <asp:BoundField HeaderText="ID Proof" DataField="proofname" />
                            <asp:BoundField HeaderText="Managing Committee" DataField="mangcomm" />
                            <asp:TemplateField HeaderText="View">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButton3" runat="server" OnCommand="ImageButton3_Command"  ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" />
                                    <asp:HiddenField ID="hfdocid" Value='<%# Eval("doc_id") %>' runat="server" />
                                    <asp:HiddenField ID="hfdmongodoc" Value='<%# Eval("document_mongoentry") %>' runat="server" />
                                    <asp:HiddenField ID="hfmemberid" Value='<%# Eval("member_id") %>' runat="server" />
                                    <asp:HiddenField ID="hdnPDF" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>                            
                        </Columns>
                    </asp:GridView>
                    <br />
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Total Managing Committee/Members</div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-xs-12">
                            <label class="control-label" for="street">Total Members:<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_total_members" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Other Members</div>
                    </div>
                    <br />
                    <div class="form-inline row">
                        <div class="col-md-6 col-xs-6">
                            <label class="control-label" style="justify-content:flex-start">Members List:</label>
                            
                        </div>
                        <div class="col-md-6 col-xs-6">
                            
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton4_Click"/>

                        </div>
                         <div class="col-md-6 col-xs-6">
                             <asp:Label ID="listexist" runat="server" Text=""  style="color:red;justify-content:flex-start"></asp:Label>
                        </div>
                    </div>

                    <br />
                    <div id ="div_employee" runat="server">
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Employee Details</div>
                    </div>
                    <br />
                    <asp:GridView runat="server" ID="gv_employee" HeaderStyle-CssClass="header" RowStyle-CssClass="rows" BackColor="White" BorderColor="#DEDFDE"
                        BorderStyle="none" BorderWidth="2px" CellPadding="4" ForeColor="Black" GridLines="Horizontal" AutoGenerateColumns="false"
                        CssClass="mygrdContent rows header pager" AllowPaging="false">
                        <AlternatingRowStyle BackColor="White" />
                        <HeaderStyle BackColor="#d7dbf1" Font-Bold="true" />
                         <Columns>
                                <asp:TemplateField HeaderText="Sr. No">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Name and Designation of the employee" DataField="name_desig" />
                                <asp:BoundField HeaderText="Present pay scale" DataField="present_pay_scale" />
                                <asp:BoundField HeaderText="Nature of Appointment" DataField="temporary_permanent" />
                                <asp:BoundField HeaderText="Present pay per month" DataField="present_pay" />
                                <asp:BoundField HeaderText="Dearness allowance per month" DataField="dearness_allowance" />
                                <asp:BoundField HeaderText="Special Pay" DataField="special_pay" />
                                <asp:BoundField HeaderText="Other Allowances" DataField="other_allowance" />
                                <asp:BoundField HeaderText="Provident Fund" DataField="provident_fund" />
                                <asp:BoundField HeaderText="Other benefits" DataField="other_benefits" />
                                <asp:TemplateField HeaderText="View">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButton_employee" runat="server" OnCommand="ImageButton_employee_Command"  ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" />                                            
                                </ItemTemplate>
                            </asp:TemplateField>                                     
                                       
                            </Columns>
                    </asp:GridView>
                    <br />
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Total Employees</div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-xs-12">
                            <label class="control-label" for="street">Total Paid Employees:<span style="color: red">*</span></label>
                            <asp:TextBox ID="value_total_employee" runat="server" type="text" disabled="disabled" Style="text-align: justify" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                        </div>
                    
                    <br />
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Documents Uploaded</div>
                    </div>
                    <br />
                   
                    <div class="row" id="Div1" runat="server">
                        <div class="table-responsive">
                            <div class="table table-bordered table-hover">
                                <table style="width: 100%">
                                    <thead>
                                        <tr>
                                            <th>
                                                <asp:Label ID="Label16" runat="server" Text="Sr. No."></asp:Label>
                                            </th>
                                            <th>
                                                <asp:Label ID="Label17" runat="server" Text="Name of Document"></asp:Label>
                                            </th>

                                            <th>
                                                <asp:Label ID="Label18" runat="server" Text="View"></asp:Label>
                                            </th>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label19" runat="server" Text="1"></asp:Label>
                                            </td>
                                            <td>
                                                <label class="control-label" for="street">Application for Registration<span style="color: red">*</span></label>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="../assets/images/pdf.png" OnClick="LB_Application_View_Click" Width="30px" Height="30px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label20" runat="server" Text="2"></asp:Label>
                                            </td>
                                            <td>
                                                <label class="control-label" for="street">Memorandum of Association<span style="color: red">*</span></label>

                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="LB_Memorandum_View_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label21" runat="server" Text="3"></asp:Label>
                                            </td>
                                            <td>
                                                <label class="control-label" for="street">Rules And Regulation<span style="color: red">*</span></label>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="LB_Bylaws_View_Click" />

                                            </td>
                                        </tr>
                                    <%--    <tr>
                                            <td>
                                                <asp:Label ID="Label22" runat="server" Text="4"></asp:Label>
                                            </td>
                                            <td>
                                                <label class="control-label" for="street">Certificate<span style="color: red">*</span></label>
                                            </td>
                                            <td>

                                                <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton7_Click" />
                                            </td>
                                        </tr>--%>
                                        <%--<tr id="docone" runat="server">
                                             <td>
                                                <asp:Label ID="Label23" runat="server" Text="4"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="valsocietyotherdoc1" runat="server" ></asp:Label>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton7_Click" />
                                            </td>
                                        </tr>
                                       <tr id="doctwo" runat="server">
                                            <td>
                                                <asp:Label ID="Label24" runat="server" Text="5"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="valsocietyotherdoc2" runat="server" ></asp:Label>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="ImageButton7_Click" />
                                            </td>
                                        </tr>--%>
                                        
                                    </thead>
                                </table>
                            </div>
                        </div>
                    </div>                   
                    

                     <div class="row" id="Div2" runat="server">
                        <div class="table-responsive">
                            <div class="table table-bordered table-hover">
                                <table style="width: 100%">
                                    <thead>
                                        <tr>
                                            <th>
                                                <asp:Label ID="Label33" runat="server" Text="Sr. No."></asp:Label>
                                            </th>
                                            <th>
                                                <asp:Label ID="Label36" runat="server" Text="Name of Document"></asp:Label>
                                            </th>

                                            <th>
                                                <asp:Label ID="Label39" runat="server" Text="View"></asp:Label>
                                            </th>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label40" runat="server" Text="1"></asp:Label>
                                            </td>
                                            <td>
                                                <label class="control-label" for="street">Application for Renewal<span style="color: red">*</span></label>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="../assets/images/pdf.png" OnClick="Lb_RenewalApplication_View_Click" Width="30px" Height="30px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label41" runat="server" Text="2"></asp:Label>
                                            </td>
                                            <td>
                                                <label class="control-label" for="street">Schedule 1/ Managing Committee Details<span style="color: red">*</span></label>

                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="LB_Schedule1_View_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label42" runat="server" Text="3"></asp:Label>
                                            </td>
                                            <td>
                                                <label class="control-label" for="street">Schedule VI/ Member Details<span style="color: red">*</span></label>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="LB_Schedule6_View_Click" />

                                            </td>
                                        </tr>
                                         <tr>
                                            <td>
                                                <asp:Label ID="Label10" runat="server" Text="3"></asp:Label>
                                            </td>
                                            <td>
                                                <label class="control-label" for="street">Schedule II/ Employee Details<span style="color: red">*</span></label>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="LB_Schedule2_View_Click" />

                                            </td>
                                        </tr>
                                         <tr>
                                            <td>
                                                <asp:Label ID="Label52" runat="server" Text="3"></asp:Label>
                                            </td>
                                            <td>
                                                <label class="control-label" for="street">Schedule IV/ Income and Expenditure Details<span style="color: red">*</span></label>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="ImageButton12" runat="server" ImageUrl="../assets/images/pdf.png" Width="30px" Height="30px" OnClick="LB_Schedule4_View_Click" />

                                            </td>
                                        </tr>
                                        
                                      
                                    </thead>
                                </table>
                            </div>
                        </div>
                    </div>

                     <br />
                    <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 10px 15px;">
                        <div class="panel-title" style="color: white; font-weight: bold;">Additional Documents Uploaded</div>
                    </div>

                    <br />
                    <%--<asp:GridView ID="grvAdditionalDocs" runat="server" CellPadding="5" AutoGenerateColumns="false" style="width: 100%;" AllowPaging="true" PageSize="10" CssClass="Grid" AlternatingRowStyle-CssClass="alt"
                                             PagerStyle-CssClass="pgr"  BorderStyle="None" BorderColor="#DEDFDE"--%>
                    <div style="text-align:center">
                    <asp:GridView runat="server" ID="grvAddDocs" style="width: 100%;" HeaderStyle-CssClass="header" RowStyle-CssClass="rows" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None"
                         BorderWidth="2px" CellPadding="5" ForeColor="Black" GridLines="Horizontal" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true"
                        CssClass="pager" AllowPaging="false">
                        <AlternatingRowStyle BackColor="White" />
                        <HeaderStyle BackColor="#d7dbf1" Font-Bold="true" />
                        <Columns>
                            <asp:TemplateField HeaderText="Sr. No">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex+1 %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="Document Name" DataField="docname" />
                            
                            <asp:TemplateField HeaderText="View">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LbView1" OnClick="LbView_Click"  CausesValidation="false" runat="server" ><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton>
                                    <asp:HiddenField ID="hfobjectID" Value='<%# Eval("object_id") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                        </div>

                           


                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    <%--  <asp:Button ID="Button2" runat="server" OnClick="btnconfirmsubmit_Click" Text="Submit" CssClass="btn btn-primary" />--%>
                </div>
            </div>
        </div>
    </div>


      <div class="modal" id="otherdocumentuploads">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Other Documents For Society</h4>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="table-responsive" style="width: 100%">
                        <div class="table table-bordered table-hover">
                            <table style="width: 100%">
                                <thead>
                                    <tr>
                                        <td class="a">
                                            <asp:Label runat="server" ID="lbl_DocName" Text="Name of Document" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                        </td>
                                        <td class="b">
                                            <asp:TextBox runat="server" ID="txtbx_DocName" MaxLength="50" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" Display="Dynamic" ControlToValidate="txtbx_DocName" ForeColor="Red" ValidationExpression="[\sa-zA-Z]*$" ErrorMessage="Invalid Name" /><br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="a">
                                            <asp:Label ID="Label43" runat="server" CssClass="control-label" Text="Document Upload" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                        </td>
                                        <td class="b">
                                            <asp:FileUpload ID="FileUpload9" runat="server" /><br />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload9" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><br />
                                            <asp:Label runat="server" ID="Label46" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label>
                                            <asp:Label runat="server" ID="Label49" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:Button ID="btnAdd" OnClientClick="CoverClickLK('btnAdd');" CausesValidation="false" OnClick="btnAdd_Click" CssClass="btn btn-primary" runat="server" Text="Add" />
                                        </td>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                </div>

            </div>
        </div>
    </div>

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="finalmodal" class="modal fade">
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
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="btnSubmitfianl"  OnClientClick="CoverClickLK('btnSubmitfianl');" CssClass="btn btn-primary" runat="server" OnClick="btnSubmit_Click" Text="Ok" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal -->
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
                        <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
