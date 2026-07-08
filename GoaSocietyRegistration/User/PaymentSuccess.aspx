<%@ Page Title="" Language="C#" MasterPageFile="~/User/Society.Master" AutoEventWireup="true" CodeBehind="PaymentSuccess.aspx.cs" Inherits="GoaSocietyRegistration.User.PaymentSuccess" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
     <script type="text/javascript">
        window.onload = function () {
            noBack();
        }
        function noBack() {
            window.history.forward();
        }
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            //Disable mouse right click
            $(document).bind("contextmenu", function (e) {
                return false;
            });
        });
    </script>
      <script>
        function printContent(el) {
            var restorepage = $('body').html();
            var printcontent = $('#' + el).clone();
            $('body').empty().html(printcontent);
            window.print();
            $('body').html(restorepage);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div class="col-lg-12" >      
        <div class="card card-container col-lg-12">
            <br />
            <div id="cphMain_updRegistration center-block">
                <div class="container-fluid" id="print">
                    <div runat="server" id="cphMain_pnlRegistration">
                        <div>
                            <div class="alert alert-success alertsuccess" style="text-align: center;" role="alert" id="alertsuccess" runat="server">
                                <strong>Payment successful !</strong>
                            </div>
                            <div class="row">
                                <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                    <div class="row form-row">
                                        <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
                                            <span>Transaction</span>
                                        </div>
                                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                                            <asp:Label runat="server" ID="lblTransaction" class="label-control" />
                                        </div>
                                    </div>
                                </div>
                            <%--    <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                    <div class="row form-row">
                                        <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
                                            <span>Receipt No.</span>
                                        </div>
                                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                                            <asp:Label runat="server" ID="lblReceiptNo" class="label-control" />
                                        </div>
                                    </div>
                                </div>--%>
                            </div>
                            <div class="row">
                                <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                    <div class="row form-row">
                                        <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
                                            <span>eChallan No</span>
                                        </div>
                                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                                            <asp:Label runat="server" ID="lbleChallanNo" class="label-control" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                    <div class="row form-row">
                                        <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
                                            <span>Amount</span>
                                        </div>
                                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                                            <asp:Label runat="server" ID="lblAmount" class="label-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                    <div class="row form-row">
                                        <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
                                            <span>Bank Ref. No.</span>
                                        </div>
                                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                                            <asp:Label runat="server" ID="lblBankRefNo" class="label-control" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-6 col-sm-6 col-md-6 col-lg-6">
                                    <div class="row form-row">
                                        <div class="col-xs-12 col-sm-12 col-md-4 col-lg-4">
                                            <span>Bank Received</span>
                                        </div>
                                        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-8">
                                            <asp:Label runat="server" ID="lblBankRcvdDate" class="label-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br /> <br />   <br />
                            <div class="row" id="divbuttons" runat="server">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                    <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12 text-center">
                                        <asp:Button ID="BtnPrint" runat="server" CausesValidation="true" Text="Print" class="btn btn-success" OnClientClick="printContent('print');"  />
                                        <asp:Button ID="btnBack" runat="server" CausesValidation="false" Text="Back" class="btn  btn-danger" OnClick="btnBack_Click" OnClientClick="window.onbeforeunload = null;"  />
                                    </div>
                                </div>
                            </div>
                            <br />   <br />   <br />   <br />   <br /><br />   <br />  
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
