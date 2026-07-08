<%@ Page Title="" Language="C#" MaintainScrollPositionOnPostback="true" MasterPageFile="~/User/Society.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="GoaSocietyRegistration.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <%--  <script src="../assets/js/jquery.min.js"></script>--%>
    <%--    <script src="../Scripts/jquery-3.5.0.min.js"></script>--%>
    <link href="../assets/css/StyleSheet.css" rel="stylesheet" />
    <script src="../Scripts/Script.js"></script>
    <style type="text/css">
        .a {
            width: 50%;
        }

        .card-header {
            height: 45px;
        }

        .anv {
            text-align: justify;
        }

        .form-group {
            margin-bottom: 0px !important;
        }

        .border-primary {
            border-color: #7460ee !important;
        }

        .padd {
            padding: .75rem .75rem !important;
        }

        .bs-example {
            margin: 20px;
        }

        .mya, .mya:hover, mya:active {
            color: #67757c;
        }

        @media only screen and (min-width: 426px) and (max-width: 768px) {
            .mobhide {
                display: none !important;
            }

            .padd {
                text-align: center !important;
            }
        }

        .loader {
            position: fixed;
            left: 0px;
            top: 0px;
            width: 100%;
            height: 100%;
            z-index: 9999;
            background: url('../assets/images/loading.gif') 50% 50% no-repeat rgb(249,249,249);
            opacity: .8;
        }

        .marginer {
            margin: auto;
        }
    </style>
    <script type="text/javascript">
        function CoverClickLK(val) {
            if (val == "editapplicationbtn") {
                document.getElementById("<%=editapplicationbtn.ClientID %>").style.display = "none";
                CoverClick1();
            }

           else if (val == "LkDiscard") {
                document.getElementById("<%=LinkButton1.ClientID %>").style.display = "none";
                CoverClick1();
           }
           else if (val == "btnpaymentstatus") {
                document.getElementById("<%=btnpaymentstatus.ClientID %>").style.display = "none";
                CoverClick1();
            }
           else if (val == "LkBtnMasterRefresh") {
                document.getElementById("<%=LkBtnMasterRefresh.ClientID %>").style.display = "none";
                CoverClick1();
            }
            
}
    </script>

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

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <div class="container-fluid">
        <div class="row">
            <%-- <div class="col-lg-2 col-md-2 col-sm-12 col-xs-12">
                <div class="card ">
                    <img class="card-img-top" src="../assets/images/user.png" alt="Card image cap" />
                    <ul class="list-group list-group-flush ">
                        <li class="list-group-item padd" ><a class="mya" href="Dashboard.aspx"><i class="fa fa-home"></i>&nbsp;<span class="mobhide">Home</span></a></li>
                        <li class="list-group-item padd"><a class="mya" href="SocietyDetails.aspx"><i class="fa fa-building" aria-hidden="true"></i>&nbsp;<span class="mobhide">Society</span></a></li>
                        <li class="list-group-item padd"><a class="mya" href="MemberDetails.aspx"><i class="fa fa-users" aria-hidden="true"></i>&nbsp;<span class="mobhide">Members</span></a></li>
                        <li class="list-group-item padd" id="paidemployee" runat="server" visible="false"><a class="mya" href="PaidEmployee.aspx"><i class="fa fa-user-plus" aria-hidden="true"></i>&nbsp;<span class="mobhide">Employee Details</span></a></li>
                        <li class="list-group-item padd"><a class="mya" href="DocumentUpload.aspx"><i class="fa fa-upload" aria-hidden="true"></i>&nbsp;<span class="mobhide">Document Upload</span></a></li>
                        <li class="list-group-item padd"><a class="mya" href="ViewApplicantDetails.aspx"><i class="fa fa-wpforms" aria-hidden="true"></i>&nbsp;<span class="mobhide">View Form</span></a></li>
                        <li class="list-group-item padd">
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openInNewTab();" Enabled="false" OnClick="LinkButton1_Click"><i class="fa fa-cc-visa"></i>&nbsp;<span class="mobhide">Payment Receipt</span></asp:LinkButton></li>
                        <li class="list-group-item padd" runat="server" visible="false" id="amendment"><a class="mya" href="Amendment.aspx"><i class="fa fa-edit" aria-hidden="true"></i>&nbsp;<span class="mobhide">Amendment</span></a></li>
                        </ul>
                </div>
            </div>--%>
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div class="card  " style="width: 100%">
                    <div class="card-header rounded" style="height: auto">
                        <div class="table-responsive">
                            <div class="table-borderless">
                                <table style="width: 100%">
                                    <tr>
                                        <th class="a" style="font-weight: bold; font-size: large">
                                            <asp:Label ID="Label1" runat="server" ForeColor="green">Welcome, </asp:Label>
                                            <asp:HiddenField ID="HiddenField1" runat="server" />
                                            <asp:Label ID="lblusername" runat="server" ForeColor="Blue" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="Label5" runat="server" ForeColor="green">!!! Your Login Id is</asp:Label>
                                            <asp:Label ID="lblloginid" runat="server" ForeColor="Blue" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="Label6" runat="server" ForeColor="green" Font-Bold="true"> . Please note down Login Id for Future Login.</asp:Label><br />
                                            <asp:Label ID="Label7" runat="server" ForeColor="OrangeRed" CssClass="anv" Font-Bold="true" Font-Size="Medium" Text="IMPORTANT : APPLICANTS ARE REQUESTED TO ENSURE THE CORRECTNESS OF THE ENTERED DETAILS BEFORE SUBMISSION.
                                                KINDLY NOTE THAT THE APPLICATION WILL BE SENT BACK TO YOU IN CASE OF INCORRECT DETAILS OR INSUFFICIENT DETAILS FOR A MAXIMUM OF 5 TIMES."></asp:Label>
                                        </th>

                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card border-primary" style="width: 100%" id="edit_application" visible="true" runat="server">
                    <div class="card-header bg-primary ">
                        <h4 class="card-title text-white ">Application Status</h4>
                    </div>
                    <div class="card-body">
                        <div class="form-group">
                            <asp:Label ID="lblimpnotice" runat="server" ForeColor="Gray" Font-Size="Large" Font-Bold="true" Text="Application form has been filled partially/not yet submitted. Please click 'Edit Application Form' to continue filling the application form."></asp:Label><br />
                            <asp:Label ID="lblnotice2" runat="server" ForeColor="Red" Font-Size="Large" Font-Bold="true" Visible="false"></asp:Label>
                            <asp:Label ID="pleasedonec_changes" runat="server" ForeColor="Gray" Font-Size="Large" Visible="false" Font-Bold="true" Text="Please do the necessary changes suggested by Officer and Submit your application again."></asp:Label><br />
                            <asp:Label ID="observation_remarks" runat="server" ForeColor="Red" Font-Size="Large" Font-Bold="true" Visible="false"></asp:Label>

                            <br />
                        </div>
                        <div class="form-group text-right">
                            <asp:LinkButton ID="editapplicationbtn" OnClientClick="CoverClickLK('editapplicationbtn');" CssClass="btn btn-primary" OnClick="editapplicationbtn_Click" runat="server"></asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="card border-primary" style="width: 100%" id="Div1" visible="false" runat="server">
                    <div class="card-header bg-primary ">
                        <asp:HiddenField ID="HiddenField2" runat="server" />
                        <h4 class="card-title text-white ">Application Status</h4>
                    </div>
                    <div class="card-body">
                        <div class="form-group">
                            <div class="alert alert-success">
                                <asp:Label ID="lblremarks_app" runat="server" ForeColor="Gray" Font-Size="Large" Font-Bold="true"> Your Application has been Accepted.</asp:Label>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="lblRemarks" runat="server" ForeColor="Gray" Text="Remarks:"> </asp:Label><br />
                            <asp:Label ID="lbremarks_applicant" runat="server" ForeColor="Gray" Text="Remarks"> </asp:Label><br />
                            <br />
                            <asp:Label ID="lbNote" Visible="false" runat="server" ForeColor="Gray" Text=""></asp:Label>
                        </div>
                        <div class="form-group" runat="server" visible="false" id="rejectwithstatus">
                            <asp:Label ID="Lbtemp" runat="server" ForeColor="Gray" Text="Remarks from District Registrar for Rejection of society"> </asp:Label><br />
                            <asp:Label ID="Label11" runat="server" Font-Bold="true" ForeColor="Gray" Text=""> </asp:Label><br />
                            <br />
                            <asp:HiddenField ID="HFMongo" runat="server" />
                            <asp:LinkButton ID="LkBtnViewDoc" OnClick="LkBtnViewDoc_Click" CssClass="btn btn-danger" runat="server"><i class="fa fa-file" aria-hidden="true"></i>&nbsp;View Rejected Document</asp:LinkButton>
                        </div>
                        <div class="form-group" runat="server" id="paymentDIV">
                            <div class="table-responsive">
                                <div class="table table-bordered table-hover">
                                    <table style="width: 100%">
                                        <thead>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label2" runat="server" Text="Application ID:"> </asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:Label ID="lblappid" runat="server"> </asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label3" runat="server" Text="Society Name"> </asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:Label ID="lblsocietyname" runat="server"> </asp:Label></td>
                                            </tr>
                                            <tr runat="server" id="showchallan" visible="false">
                                                <td>
                                                    <asp:Label ID="Label4" runat="server" Text="Echallan No"> </asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:Label ID="lblshowchallan" runat="server"> </asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td>Total Fee</td>
                                                <td>₹
                                                    <asp:Label ID="lbltotalfee" runat="server"> </asp:Label></td>
                                                <td>(<asp:Label ID="Label8" runat="server" Text="Price inclusive of Registration Fee ₹"></asp:Label>
                                                    <asp:Label ID="regis_fees" runat="server" Text=""></asp:Label>)</td>
                                                <td>
                                                    <asp:Label ID="paymentpaid" runat="server" Font-Bold="true"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4"> <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-primary" Visible="false" OnClientClick="CoverClickLK('LinkButton1');" Enabled="false" OnClick="LinkButton1_Click"><i class="fa fa-cc-visa"></i>&nbsp;<span class="mobhide">Payment Receipt</span></asp:LinkButton></li></td>
                                            </tr>
                                             <tr runat="server" id="onrenewal" visible="false">
                                                 <td>Payment Remarks</td>
                                                 <td colspan="2">
                                                     <asp:Label ID="Label9" runat="server" Text="" ></asp:Label></td>
                                             </tr>
                                        </thead>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class="form-group text-right  ">
                            <asp:LinkButton ID="btnReject" CssClass="btn btn-danger" Visible="false" OnClick="btnReject_Click" runat="server"><i class="fa fa-ban" aria-hidden="true"></i>&nbsp;Application Closed</asp:LinkButton>
                            <asp:LinkButton ID="btnPayment" CssClass="btn btn-primary" OnClick="btnPayment_Click" runat="server"><i class="fa fa-credit-card" ></i>&nbsp;Make Payment/Verify Status</asp:LinkButton>
                            <img src="../assets/images/loading.gif" id="imgLoading" style="z-index: 101; left: 424px; visibility: hidden; position: absolute; top: 240px" />
                            <asp:LinkButton ID="btnpaymentstatus" OnClientClick="CoverClickLK('btnpaymentstatus');" CssClass="btn btn-primary" OnClick="btnpaymentstatus_Click" CausesValidation="false" Visible="false" runat="server"><i class="fa fa-check" aria-hidden="true"></i>&nbsp;Check Payment Status</asp:LinkButton>

                        </div>
                        <div class="form-group">
                            <asp:Label ID="gotooffice" runat="server"></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="card" style="width: 100%" id="paymentgridview" runat="server" visible="false">
                    <div class="card-body>">
                        <br />
                        <asp:Label ID="Label41" CssClass="alert" runat="server" Text="Old eChallan Summary "></asp:Label>
                        <div class="table-responsive">
                            <asp:GridView ID="gridviewPayment" runat="server" OnRowDataBound="gridviewPayment_RowDataBound" CellPadding="5" AutoGenerateColumns="false" CssClass="Grid marginer" AlternatingRowStyle-CssClass="alt" EmptyDataText="No eChallan Summary Found. Please Click on Proceed to Payment." ShowHeaderWhenEmpty="true" Width="80%">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr. No">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lbpaymentapp_id" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                            <asp:Label ID="lbstatus" runat="server" Text='<%# Eval("status") %>'></asp:Label>
                                            <asp:Label ID="lbechallan" runat="server" Text='<%# Eval("echallan_no") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField HeaderText="eChallan No" DataField="echallan_no" />
                                    <asp:BoundField HeaderText="eChallan Generated On" DataField="echallangeneratedon" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField HeaderText="Status" DataField="status" />
                                    <asp:TemplateField HeaderText="Update Status">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lkUpdateStatus" OnClick="lkUpdateStatus_Click" CssClass="btn btn-primary" CausesValidation="false" runat="server"><i class="fas fa-sync"></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete Old eChallan">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LkDeleteold" OnClick="LkDeleteold_Click" CssClass="btn btn-danger" CausesValidation="false" runat="server"><i class="fa fa-trash" aria-hidden="true"></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        <br />
                     &nbsp;   <asp:Label ID="Label40" CssClass="alert-danger text-center" runat="server" Text="Click on Update Status Button if your amount is deducted to get the Status. It will take some time to update."></asp:Label><br />

                        <div class="row">
                            
                            <asp:LinkButton ID="LkBtnMasterRefresh" OnClientClick="CoverClickLK('LkBtnMasterRefresh');" OnClick="LkBtnMasterRefresh_Click" CssClass="btn btn-warning marginer" runat="server" Visible="false"><i class="fa fa-rupee"></i>&nbsp;Check Payment Status</asp:LinkButton>

                        </div>
                        <div>
                            <br />
                        </div>
                    </div>
                </div>
                <div class="card" style="width: 100%">
                    <div class="card-body">
                        <asp:Label ID="lblinfo1" runat="server">केवल आवेदक के लिए /For Applicant Only:<br />
                            1.   कृपया क्षेत्रों को ध्यान से भरें।/ Dear Applicant, please fill the fields carefully.<br />                          
                            2.   पद भरने के बाद, आप भरा प्रोफ़ाइल को संपादित करने में सक्षम नहीं होंगे।/ After filling the Form, you will not be able to edit the filled data.</asp:Label>
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


    
        <div id="cover" style="display: none;"></div>
            <div class="row">
                <div id="CoverDoubleClick" class="opac_divLoader overlayLoader" style="display: none;">
                    <asp:Image ID="wait" runat="server" ImageUrl="../assets/images/loading.gif" AlternateText="w a i t"
                        Height="80%" Width="20%" Style="vertical-align: middle;" />
                </div>
            </div>


    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="deletechallanModal" class="modal fade">
            <div class="modal-dialog  modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">

                        <asp:HiddenField ID="hdechallan" runat="server" />
                        <asp:HiddenField ID="hdapplicationid" runat="server" />
                        <asp:Label ID="Label43" runat="server" ForeColor="Red" Text="If your payment is deducted then kindly wait and click on refresh button after some time. If you delete echallan after your payment is dedcuted, then your dedcuted payment will be lost."></asp:Label>
                        <br />
                        <asp:CheckBox ID="CheckBox1" OnCheckedChanged="CheckBox1_CheckedChanged" Visible="true" runat="server" CausesValidation="false" AutoPostBack="true" Text="I confirm that my payment is not deducted and I want to delete my old challan and create new." />
                        <asp:Label ID="Label45" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="lkdeleteoldechallan" Enabled="false" CausesValidation="false" OnClick="lkdeleteoldechallan_Click" CssClass="btn btn-success" runat="server"><i class="fas fa-check"></i>&nbsp;&nbsp;Confirm</asp:LinkButton>
                        <asp:LinkButton ID="LinkButton3" CssClass="btn btn-danger" runat="server"><i class="fas fa-times"></i>&nbsp;&nbsp;Close</asp:LinkButton>
                    </div>
                </div>
            </div>
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
                        <asp:Label ID="lblMSG1" runat="server" Text="" ForeColor="Green"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnCancel" runat="server" class="btn btn-secondary" data-dismiss="modal"></asp:Button>
                        <%--  <asp:Button ID="ViewChallan" runat="server" CssClass="btn btn-warning" Visible="false" OnClick="ViewChallan_Click" />--%>
                        <asp:Button ID="RedirecttoLoginBtn" CssClass="btn btn-primary" runat="server" OnClick="RedirecttoLoginBtn_Click" />
                        <asp:Button ID="errorpage" runat="server" OnClick="errorpage_Click" class="btn btn-primary"></asp:Button>

                        <asp:LinkButton ID="btnrefresh" OnClick="btnrefresh_Click" CssClass="btn btn-primary" Visible="false" runat="server"></asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
