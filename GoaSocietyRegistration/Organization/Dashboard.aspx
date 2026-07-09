<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="GoaSocietyRegistration.Organization.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
        <script src="../Scripts/jquery-3.5.0.min.js"></script>
    <script src="../Scripts/aes.js"></script>
    <script src="../Scripts/encrypt.js"></script>
    <script src="assets/js/jquery.countTo.js"></script>
    <link href="assets/font-awesome.css" rel="stylesheet" />
    <link href="assets/css/animate.css" rel="stylesheet" />
    <!-- Google Fonts -->
    <style>
        /*Google Fonts*/
        @font-face {
            font-family: 'Material Icons';
            font-style: normal;
            font-weight: 400;
            src: url(Login_Assets\fonts\flUhRq6tzZclQEJ-Vdg-IuiaDsNc.woff2) format('woff2');
        }

        .material-icons {
            font-family: 'Material Icons';
            font-weight: normal;
            font-style: normal;
            font-size: 24px;
            line-height: 1;
            letter-spacing: normal;
            text-transform: none;
            display: inline-block;
            white-space: nowrap;
            word-wrap: normal;
            direction: ltr;
            -webkit-font-feature-settings: 'liga';
            -webkit-font-smoothing: antialiased;
        }


        @font-face {
            font-family: 'Roboto';
            font-style: normal;
            font-weight: 400;
            src: url(Login_Assets\fonts\KFOmCnqEu92Fr1Mu72xKOzY.woff2) format('woff2');
            unicode-range: U+0460-052F, U+1C80-1C88, U+20B4, U+2DE0-2DFF, U+A640-A69F, U+FE2E-FE2F;
        }
        /* cyrillic */
        @font-face {
            font-family: 'Roboto';
            font-style: normal;
            font-weight: 400;
            src: url(Login_Assets\fonts\KFOmCnqEu92Fr1Mu72xKOzY.woff2) format('woff2');
            unicode-range: U+0301, U+0400-045F, U+0490-0491, U+04B0-04B1, U+2116;
        }

        @font-face {
            font-family: 'Roboto';
            font-style: normal;
            font-weight: 700;
            src: url(Login_Assets\fonts\KFOlCnqEu92Fr1MmWUlfCRc4EsA.woff2) format('woff2');
            unicode-range: U+0460-052F, U+1C80-1C88, U+20B4, U+2DE0-2DFF, U+A640-A69F, U+FE2E-FE2F;
        }
        /* cyrillic */
        @font-face {
            font-family: 'Roboto';
            font-style: normal;
            font-weight: 700;
            src: url(Login_Assets\fonts\KFOlCnqEu92Fr1MmWUlfCRc4EsA.woff2) format('woff2');
            unicode-range: U+0301, U+0400-045F, U+0490-0491, U+04B0-04B1, U+2116;
        }

        /* latin-ext */
        @font-face {
            font-family: 'Roboto';
            font-style: normal;
            font-weight: 400;
            src: url(Login_Assets\fonts\KFOmCnqEu92Fr1Mu7GxKOzY.woff2) format('woff2');
            unicode-range: U+0100-02AF, U+0304, U+0308, U+0329, U+1E00-1E9F, U+1EF2-1EFF, U+2020, U+20A0-20AB, U+20AD-20CF, U+2113, U+2C60-2C7F, U+A720-A7FF;
        }
        /* latin */
        @font-face {
            font-family: 'Roboto';
            font-style: normal;
            font-weight: 400;
            src: url(Login_Assets\fonts\KFOmCnqEu92Fr1Mu4mxK.woff2) format('woff2');
            unicode-range: U+0000-00FF, U+0131, U+0152-0153, U+02BB-02BC, U+02C6, U+02DA, U+02DC, U+0304, U+0308, U+0329, U+2000-206F, U+2074, U+20AC, U+2122, U+2191, U+2193, U+2212, U+2215, U+FEFF, U+FFFD;
        }

        /* latin-ext */
        @font-face {
            font-family: 'Roboto';
            font-style: normal;
            font-weight: 700;
            src: url(Login_Assets\fonts\KFOlCnqEu92Fr1MmWUlfChc4EsA.woff2) format('woff2');
            unicode-range: U+0100-02AF, U+0304, U+0308, U+0329, U+1E00-1E9F, U+1EF2-1EFF, U+2020, U+20A0-20AB, U+20AD-20CF, U+2113, U+2C60-2C7F, U+A720-A7FF;
        }
        /* latin */
        @font-face {
            font-family: 'Roboto';
            font-style: normal;
            font-weight: 700;
            src: url(Login_Assets\fonts\KFOlCnqEu92Fr1MmWUlfBBc4.woff2) format('woff2');
            unicode-range: U+0000-00FF, U+0131, U+0152-0153, U+02BB-02BC, U+02C6, U+02DA, U+02DC, U+0304, U+0308, U+0329, U+2000-206F, U+2074, U+20AC, U+2122, U+2191, U+2193, U+2212, U+2215, U+FEFF, U+FFFD;
        }
    </style>
    <script src="../Scripts/loader.js"></script>
    <script src="../chart/societychart.js"></script>
    <script type="text/javascript"> 
        $(document).ready(function () {

            'use strict'          
            var nshg = <%=this.nsgh%>;
            var nmm = <%=this.nmm%>;
            var nngo = <%=this.nngo%>;
            var nscc = <%=this.nscc%>;
            var nother = <%=this.nother%>;
           
            var pieChartCanvas1 = $('#pieChart1').get(0).getContext('2d')
            var pieData2 = {
                labels: [
                    ' Women Self Help Group',
                                 'Mahila Mandals',
                                'NGO',
                                   'Sports and Cultural Club',
                                  'Other Societies',                    
                ],
                datasets: [
                  {
                      data: [nshg,nmm,nngo,nscc,nother ],
                      backgroundColor: ['#f56954', '#00a65a', '#f39c12', '#00c0ef', '#d2d6de'],
                  }
                ]
            }
            var pieOptions1 = {
                legend: {
                    display: false
                }
            }
            var sshg = <%=this.shgh%>;
            var smm = <%=this.smm%>;
            var sngo = <%=this.sngo%>;
            var sscc = <%=this.sscc%>;
            var sother = <%=this.sother%>;
           
            var pieChartCanvas2 = $('#pieChart2').get(0).getContext('2d')
            var pieData3 = {
                labels: [
                    ' Women Self Help Group',
                                 'Mahila Mandals',
                                'NGO',
                                   'Sports and Cultural Club',
                                  'Other Societies',
                   
                    
                ],
                datasets: [
                  {
                      data: [sshg,smm,sngo,sscc,sother ],
                      backgroundColor: ['#f56954', '#00a65a', '#f39c12', '#00c0ef', '#d2d6de'],
                  }
                ]
            }
            var pieOptions2 = {
                legend: {
                    display: false
                }
            }
            //Create pie or douhnut chart
            // You can switch between pie and douhnut using the method below.
            //var pieChart = new Chart(pieChartCanvas, {
            //    type: 'doughnut',
            //    data: pieData,
            //    options: pieOptions
            //})
            var pieChart1 = new Chart(pieChartCanvas1, {
                type: 'doughnut',
                data: pieData2,
                options: pieOptions1
            })
            var pieChart2 = new Chart(pieChartCanvas2, {
                type: 'doughnut',
                data: pieData3,
                options: pieOptions2
            })
        })
     
    </script>
    <link href="assets/css/infoboxcss.css" rel="stylesheet" />
    <style>
        .donut-size {
            font-size: 7em;
        }

        .pie-wrapper {
            position: relative;
            width: 1em;
            height: 1em;
            margin: 0px auto;
        }

            .pie-wrapper .pie {
                position: absolute;
                top: 0px;
                left: 0px;
                width: 100%;
                height: 100%;
                clip: rect(0, 1em, 1em, 0.5em);
            }

            .pie-wrapper .half-circle {
                position: absolute;
                top: 0px;
                left: 0px;
                width: 100%;
                height: 100%;
                border: 0.1em solid #1abc9c;
                border-radius: 50%;
                clip: rect(0em, 0.5em, 1em, 0em);
            }

            .pie-wrapper .right-side {
                transform: rotate(0deg);
            }

            .pie-wrapper .label {
                position: absolute;
                top: 0.52em;
                right: 0.4em;
                bottom: 0.4em;
                left: 0.4em;
                display: block;
                background: none;
                border-radius: 50%;
                color: #7F8C8D;
                font-size: 0.25em;
                line-height: 2.6em;
                text-align: center;
                cursor: default;
                z-index: 2;
            }

            .pie-wrapper .smaller {
                padding-bottom: 20px;
                color: #BDC3C7;
                font-size: .45em;
                vertical-align: super;
            }

            .pie-wrapper .shadow {
                width: 100%;
                height: 100%;
                border: 0.1em solid #BDC3C7;
                border-radius: 50%;
            }



        .w3-badge, .w3-tag {
            background-color: #3c8dbc;
            color: #fff;
            display: inline-block;
            padding-left: 8px;
            padding-right: 8px;
            text-align: center;
        }

        .w3-badge {
            border-radius: 50%;
        }

        .w3-jumbo {
            font-size: 64px !important;
        }

        .verify_society {
            background-color: #6c53dc !important;
            color: #ffffff;
            padding: 8px;
        }

        .w3-padding-small {
            padding: 4px 8px !important;
        }

        .w3-padding {
            padding: 8px 16px !important;
        }

        .w3-padding-large {
            padding: 12px 24px !important;
        }

        .w3-padding-16 {
            padding-top: 16px !important;
            padding-bottom: 16px !important;
        }

        .w3-padding-24 {
            padding-top: 24px !important;
            padding-bottom: 24px !important;
        }

        .w3-padding-32 {
            padding-top: 32px !important;
            padding-bottom: 32px !important;
        }

        .w3-padding-48 {
            padding-top: 48px !important;
            padding-bottom: 48px !important;
        }

        .w3-padding-64 {
            padding-top: 64px !important;
            padding-bottom: 64px !important;
        }

        .nav-tabs > li > a {
            margin-right: 2px;
            line-height: 1.42857143;
            border: 1px solid transparent;
            border-radius: 4px 4px 0 0;
        }

        .nav > li > a {
            position: relative;
            display: block;
            padding: 2px 13px;
        }

        .nav-tabs > li {
            float: left;
            margin-bottom: 1px;
        }

        .bs-example {
            margin: 20px;
        }

        .bg-wood {
            background-color: #ab7d44 !important;
        }

        .mytable {
            margin-bottom: 0.10rem !important;
        }

        .k {
            margin-bottom: 0px !important;
            font-size: 13px;
            padding: 0px;
        }

        .p-3 {
            padding: 0rem !important;
        }

        .notice {
            padding: 8px;
            background-color: #00c0ef !important;
            color: #ffffff;
        }

        .badge {
            border-radius: 20px;
            padding: 3px 7px !important;
        }

        .calendar {
            background-color: #f39c12 !important;
            color: #ffffff;
            padding: 8px;
        }

        .declaration {
            background-color: #dd4b39 !important;
            color: #ffffff;
            padding: 8px;
        }

        .appointment {
            background-color: #00a65a !important;
            color: #ffffff;
            padding: 8px;
        }

        .mybtn {
            padding: 5px !important;
            letter-spacing: 0 !important;
        }




        .font_family {
            font-family: "Source Sans Pro",-apple-system,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif,"Apple Color Emoji","Segoe UI Emoji","Segoe UI Symbol";
        }
    </style>
    <style>
        body {
            font-family: Arial, Helvetica, sans-serif;
        }

        .flip-card {
            background-color: transparent;
            width: 191px;
            height: 59px;
            perspective: 1000px;
        }

        .flip-card-inner {
            position: relative;
            width: 100%;
            text-align: center;
            transition: transform 0.6s;
            transform-style: preserve-3d;
            box-shadow: 0 4px 8px 0 rgba(0,0,0,0.2);
        }

        .flip-card:hover .flip-card-inner {
            transform: rotateY(180deg);
        }

        .flip-card-front, .flip-card-back {
            position: absolute;
            width: 100%;
            height: 100%;
            -webkit-backface-visibility: hidden;
            backface-visibility: hidden;
        }

        .flip-card-front {
            background-color: #bbb;
            color: #fd7e14;
        }

        .flip-card-back {
            background-color: #2980b9;
            color: #00a65a;
            transform: rotateY(180deg);
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
    <script>
        $(document).ready(function () 
        {  
            var total = document.getElementById("<%=total_society.ClientID %>").innerHTML;
            var north = document.getElementById("<%=lbnorthgoa.ClientID %>").innerHTML;
            var south = document.getElementById("<%=lbsouth.ClientID %>").innerHTML;
            var northwidth = (north/total)*100;
            var southwidth = (south/total)*100;
            $(".northid").width(northwidth);
            $(".southid").width(southwidth);
          
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

    <div class="row clearfix" runat="server" id="upper_row">
        <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
            <div class="info-box bg-pink hover-expand-effect">
                <div class="icon">
                    <i class="fa fa-users" aria-hidden="true"></i>
                </div>
                <div class="content">
                    <div class="text">Verify Society</div>
                    <div class="number count-to" data-from="0" data-to="125" data-speed="15" data-fresh-interval="20">
                        <asp:Label ID="verifycountlabel" runat="server" Text=""></asp:Label>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
            <div class="info-box bg-cyan hover-expand-effect">
                <div class="icon">
                    <i class="fa fa-check" aria-hidden="true"></i>
                </div>
                <div class="content">
                    <div class="text">Verify Observation Society --</div>
                    <div class="number count-to" data-from="0" data-to="257" data-speed="1000" data-fresh-interval="20">
                        <asp:Label ID="observationcountlabel" runat="server" Text=""></asp:Label>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
            <div class="info-box bg-orange hover-expand-effect">
                <div class="icon">
                    <i class="fa fa-certificate" aria-hidden="true"></i>
                </div>
                <div class="content">
                    <div class="text">Generate Certificate</div>
                    <div class="number count-to" data-from="0" data-to="1225" data-speed="1000" data-fresh-interval="20">0</div>
                </div>
            </div>
        </div>
        <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
            <div class="info-box bg-light-green hover-expand-effect">
                <div class="icon">
                    <i class="fa fa-repeat" aria-hidden="true"></i>
                </div>
                <div class="content">
                    <div class="text">Renewal</div>
                    <div class="number count-to" data-from="0" data-to="243" data-speed="1000" data-fresh-interval="20">
                        <asp:Label ID="Label1" runat="server" Text="0"></asp:Label>
                        <asp:HiddenField ID="HiddenField1" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%--    <div class="row">
        <div class="col-lg-3 col-6">
            <!-- small box -->
            <div class="small-box bg-info">
                <div class="inner">
                    <h3 class="whitetext">
                        <asp:Label ID="Label4" runat="server" Text="Label"></asp:Label></h3>

                    <p class="whitetext font_family">For Declaration</p>
                </div>
                <div class="icon">
                    <i class="icon ion-document"></i>
                </div>
                <a href="Registration.aspx" class="small-box-footer">More info <i class="fa fa-arrow-circle-right"></i></a>
            </div>
        </div>
        <!-- ./col -->
        <div class="col-lg-3 col-6">
            <!-- small box -->
            <div class="small-box bg-success">
                <div class="inner">
                    <h3 class="whitetext">53<sup style="font-size: 20px">%</sup></h3>

                    <p class="whitetext font_family">Final Registration</p>
                </div>
                <div class="icon">
                    <i class="icon ion-document-text"></i>
                </div>
                <a href="#" class="small-box-footer">More info <i class="fa fa-arrow-circle-right"></i></a>
            </div>
        </div>
        <!-- ./col -->
        <div class="col-lg-3 col-6">
            <!-- small box -->
            <div class="small-box bg-warning">
                <div class="inner">
                    <h3 class="whitetext">
                        <asp:Label ID="Label5" runat="server" Text="Label"></asp:Label></h3>


                    <p class="whitetext font_family">Notice of Marriage</p>
                </div>
                <div class="icon">
                    <i class="icon ion-clipboard"></i>
                </div>
                <a href="NoticeMarriage.aspx" class="small-box-footer">More info <i class="fa fa-arrow-circle-right"></i></a>
            </div>
        </div>
        <!-- ./col -->
        <div class="col-lg-3 col-6">
            <!-- small box -->
            <div class="small-box bg-wood">
                <div class="inner">
                    <h3 class="whitetext">
                        <asp:Label ID="Label6" runat="server" Text="lABEL"></asp:Label></h3>

                    <p class="whitetext font_family">NOC</p>
                </div>
                <div class="icon">
                    <i class="ion ion-pie-graph"></i>
                </div>
                <a href="NOC.aspx" class="small-box-footer">More info <i class="fa fa-arrow-circle-right"></i></a>
            </div>
        </div>
        <!-- ./col -->
    </div>--%>
    <br />

    <div class="row" id="labelverify" runat="server">
        <div class="col-lg-9 ">
            <div class="verify_society">Verify Society</div>
            <div style="border: solid 1px #ddd" runat="server" id="hide_dashboard_grid">
                <div class="card-header tab-card-header">
                    <ul class="nav nav-tabs card-header-tabs" id="myTab1" role="tablist">
                        <li class="nav-item">
                            <a class="nav-link" id="verify_tab" data-toggle="tab" href="#verifysociety" role="tab" aria-controls="VerifySociety" aria-selected="true">Verify Society (
                                <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                                )</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="obs_tab" data-toggle="tab" href="#obssociety" role="tab" aria-controls="ObsSociety" aria-selected="false">Verify Observation Society (
                                <asp:Label ID="Lbcount" runat="server" Text=""></asp:Label>
                                )</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="renewal_tab" data-toggle="tab" href="#verifysociety_renewal" role="tab" aria-controls="VerifySociety_Renewal" aria-selected="false">Renewal Society (
                                <asp:Label ID="lbrenewalcount" runat="server" Text=""></asp:Label>
                                )</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="renewalobs_tab" data-toggle="tab" href="#obssociety_renewal" role="tab" aria-controls="ObsSociety_Renewal" aria-selected="false">Verify Observation Renewal Society (
                                <asp:Label ID="lbrenewalobscount" runat="server" Text=""></asp:Label>
                                )</a>
                        </li>
                    </ul>
                </div>
                <div class="tab-content table-responsive" id="societyverifytabcontent" style="overflow: scroll; height: 400px">
                    <div class="tab-pane fade show active p-3" id="verifysociety" role="tabpanel" aria-labelledby="verify_tab">
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
                                <%--<asp:BoundField HeaderText="Submission Time" DataField="application_submission_time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />--%>
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
                    <div class="tab-pane fade show  p-3" id="obssociety" role="tabpanel" aria-labelledby="obs_tab">
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
                                <asp:BoundField HeaderText="Submission Time" DataField="application_obs_submission_time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
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

                    <div class="tab-pane fade show p-3" id="verifysociety_renewal" role="tabpanel" aria-labelledby="renewal_tab">
                        <asp:GridView runat="server" ID="gv_ApplicationsRenewal" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%">
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
                                <%--<asp:BoundField HeaderText="Submission Time" DataField="application_submission_time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />--%>
                                <%-- <asp:HyperLinkField HeaderText="View Details Information" DataNavigateUrlFields="App_ID" DataNavigateUrlFormatString="VerifyForm.aspx?" Text="View Profile"></asp:HyperLinkField>--%>
                                <asp:TemplateField HeaderText="Edit" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="LbApp_id" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="View">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="ENameLinkBtn_renewal" runat="server" Style="font-weight: 700; font-size: 8pt;" OnClick="ENameLinkBtn_renewal_Click"><i class="fa fa-arrow-circle-right" style="font-size:20px;"></i></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                        </asp:GridView>
                    </div>

                    <div class="tab-pane fade show  p-3" id="obssociety_renewal" role="tabpanel" aria-labelledby="renewalobs_tab">
                        <asp:GridView runat="server" ID="gv_obsrenewal" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%">
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
                                <asp:BoundField HeaderText="Submission Time" DataField="application_obs_submission_time" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                <%-- <asp:HyperLinkField HeaderText="View Details Information" DataNavigateUrlFields="App_ID" DataNavigateUrlFormatString="VerifyForm.aspx?" Text="View Profile"></asp:HyperLinkField>--%>
                                <asp:TemplateField HeaderText="Edit" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="LbApp_id_obs" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="View">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="ENameLinkBtn_obs_renewal" runat="server" Style="font-weight: 700; font-size: 8pt;" OnClick="ENameLinkBtn_obs_renewal_Click"><i class="fa fa-arrow-circle-right" style="font-size:20px;"></i></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                        </asp:GridView>
                    </div>
                </div>
            </div>

        </div>

        <div class="col-lg-3">

            <div class="embed-responsive">
                <div class="calendar" style="font-size: 18px">Calendar</div>
                <div id="calendar" style="height: auto">
                    <%-- height: 200px--%>
                    <asp:Calendar ID="Calendar" runat="server" OtherMonthDayStyle-ForeColor="#b5afa8"></asp:Calendar>
                </div>
            </div>
            <div>
                <br />
                <asp:LinkButton ID="LkAdvSeach1" CssClass="float-lg-right btn btn-info" CausesValidation="false" Visible="false" OnClick="LkAdvSeach_Click" runat="server">Advance Search</asp:LinkButton>
                <br />

            </div>
        </div>
    </div>
    <div class="row" id="chart_card" runat="server">

        <div class="col-lg-12 ">
            <div class="card">
                <div class="card-header" style="height: auto">
                    <h5 class="card-title">Societies Registered</h5>
                </div>
                <!-- /.card-header -->
                <div class="card-body">
                    <div class="row">
                        <%--  <div class="col-lg-4">
                            <div class="chart-responsive">
                                <canvas id="pieChart" height="150"></canvas>
                                <ul class="chart-legend clearfix">
                                    <li><i class="fa fa-circle text-danger"></i>North Goa</li>
                                    <li><i class="fa fa-circle text-success"></i>South Goa</li>
                                </ul>
                            </div>
                            <!-- ./chart-responsive -->
                        </div>--%>
                        <!-- /.col -->

                        <div class="col-lg-4">
                            <p class="text-center">
                                <strong>Total Societies</strong>
                            </p>
                            <!-- /.progress-group -->
                            <div class="progress-group">
                                Total Societies Registered (GOA)
                      <span class="float-right"><b>
                          <asp:Label ID="total_society" runat="server" Text=""></asp:Label></b></span>
                                <div class="progress progress-sm">
                                    <div class="progress-bar bg-danger" style="width: 100%; height: 100px"></div>
                                </div>
                            </div>
                            <!-- /.progress-group -->
                            <div class="progress-group">
                                <span class="progress-text">North Goa Societies</span>
                                <span class="float-right"><b>
                                    <asp:Label ID="lbnorthgoa" runat="server" Text=""></asp:Label></b>/<b><asp:Label ID="total_societys" runat="server" Text=""></asp:Label></b></span>
                                <div class="progress progress-sm" id="ngoawidth" runat="server">
                                    <div class=" northid progress-bar bg-success" style="height: 100px"></div>
                                </div>
                            </div>
                            <!-- /.progress-group -->
                            <div class="progress-group">
                                South Goa Societies
                      <span class="float-right"><b>
                          <asp:Label ID="lbsouth" runat="server" Text=""></asp:Label></b>/<b><asp:Label ID="total_societyss" runat="server" Text=""></asp:Label></b></span>
                                <div class="progress progress-sm" runat="server" id="sgoawidth">
                                    <div class=" southid progress-bar bg-warning" style="height: 100px"></div>
                                </div>
                            </div>
                            <!-- /.progress-group -->
                        </div>
                        <div class="col-lg-2">
                            <p class="text-center">
                                <strong>In Process</strong>
                            </p>
                            <div id="specificChart" class="donut-size">

                                <div class="pie-wrapper">

                                    <span class="label">
                                        <span class="num">
                                            <asp:Label ID="procesedlabel" runat="server" Text=""></asp:Label>
                                        </span>
                                    </span>
                                    <div class="pie" style="clip: rect(auto, auto, auto, auto);">
                                        <div class="left-side half-circle" style="border-width: 0.1em; transform: rotate(360deg);"></div>
                                        <div class="right-side half-circle" style="transform: rotate(180deg); border-width: 0.1em;"></div>
                                    </div>

                                    <div class="shadow"></div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-2">

                            <p class="text-center">
                                <strong>Rejected</strong>
                            </p>
                            <div id="specificChart1" class="donut-size">

                                <div class="pie-wrapper">
                                    <span class="label">
                                        <span class="num">
                                            <asp:Label ID="rejectedLable" runat="server" Text=""></asp:Label>
                                        </span>
                                    </span>
                                    <div class="pie" style="clip: rect(auto, auto, auto, auto);">
                                        <div class="left-side half-circle" style="border-width: 0.1em; transform: rotate(360deg); border: 0.1em solid crimson !important"></div>
                                        <div class="right-side half-circle" style="transform: rotate(180deg); border-width: 0.1em; border: 0.1em solid crimson !important"></div>
                                    </div>

                                    <div class="shadow"></div>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4">
                            <div class="card shadow">
                                <div class="card-header">
                                    <h3 class="greeting-text">Total Revenue</h3>
                                </div>
                                <div class="card-content" style="overflow: auto">
                                    <div class="card-body">
                                        <div class="d-flex justify-content-between align-items-end">
                                            <div class="dashboard-content-left">
                                                <h1 class="text-primary  text-bold-300" style="font-size: 2.0rem !important;">₹
                                                    <asp:Label ID="lbtotalAmt" runat="server" Text=""></asp:Label></h1>
                                            </div>
                                            <div class="dashboard-content-right">
                                                <div class="flip-card">
                                                    <div class="flip-card-inner">
                                                        <div class="flip-card-front">
                                                            <asp:Label ID="Label2" runat="server" Text=" View District Wise Revenue" CssClass="align-content-lg-start"></asp:Label><br />

                                                        </div>
                                                        <div class="flip-card-back">
                                                            <asp:Label ID="Label5" runat="server" Text="North Goa : " CssClass="align-content-lg-start"></asp:Label>
                                                            <asp:Label ID="Label4" runat="server" Text="₹"></asp:Label><asp:Label ID="lbNorthAmt" runat="server" Text=""></asp:Label>
                                                            <br />
                                                            <asp:Label ID="Label6" runat="server" Text="South Goa : "></asp:Label>
                                                            <asp:Label ID="Label7" runat="server" Text="₹"></asp:Label><asp:Label ID="lbSouthAmt" runat="server" Text=""></asp:Label>

                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!-- /.col -->
                    </div>
                    <!-- /.row -->
                </div>

            </div>
        </div>

        <!-- /.card -->
        <br />
        <div class="col-lg-12 ">
            <br />
            <div class="row">
                <div class="col-lg-6 ">
                    <div class="card card-danger">
                        <div class="card-header" style="height: auto">
                            <h5 class="card-title">North Goa</h5>
                        </div>
                        <%-- <div class="card-header">
                <h3 class="card-title">Pie Chart</h3>

                <div class="card-tools">
                  <button type="button" class="btn btn-tool" data-card-widget="collapse"><i class="fas fa-minus"></i>
                  </button>
                  <button type="button" class="btn btn-tool" data-card-widget="remove"><i class="fas fa-times"></i></button>
                </div>
                            </div>--%>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-lg-5">
                                    <p class="text-center">
                                        <strong></strong>
                                    </p>
                                    <div class="chart-responsive">
                                        <canvas id="pieChart1" height="200"></canvas>

                                    </div>
                                    <!-- ./chart-responsive -->
                                </div>
                                <div class="col-lg-7">
                                    <ul class="chart-legend clearfix">
                                        <p class="text-center">
                                            <strong>Total</strong> &nbsp;<asp:Label ID="label8" runat="server"></asp:Label>
                                        </p>
                                        <li><i class="fa fa-circle text-danger"></i>Women Self Help Group -
                                            <asp:Label ID="labelneshg" runat="server"></asp:Label>
                                        </li>
                                        <li><i class="fa fa-circle text-success"></i>Mahila Mandals -
                                            <asp:Label ID="labelnmm" runat="server"></asp:Label>
                                        </li>
                                        <li><i class="fa fa-circle text-warning"></i>NGO -
                                            <asp:Label ID="labelnngo" runat="server"></asp:Label>
                                        </li>
                                        <li><i class="fa fa-circle text-info"></i>Sports and Cultural Club -
                                            <asp:Label ID="labelnnscc" runat="server"></asp:Label>
                                        </li>
                                        <li><i class="fa fa-circle text-secondary"></i>Other Societies -
                                            <asp:Label ID="labelnos" runat="server"></asp:Label>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <!-- /.card-body -->
                    </div>
                </div>
                <div class="col-lg-6 ">
                    <div class="card card-danger">
                        <div class="card-header" style="height: auto">
                            <h5 class="card-title">South Goa</h5>
                        </div>
                        <%-- <div class="card-header">
                <h3 class="card-title">Pie Chart</h3>

                <div class="card-tools">
                  <button type="button" class="btn btn-tool" data-card-widget="collapse"><i class="fas fa-minus"></i>
                  </button>
                  <button type="button" class="btn btn-tool" data-card-widget="remove"><i class="fas fa-times"></i></button>
                </div>
                            </div>--%>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-lg-5">
                                    <p class="text-center">
                                        <strong></strong>
                                    </p>
                                    <div class="chart-responsive">
                                        <canvas id="pieChart2" height="200"></canvas>

                                    </div>
                                    <!-- ./chart-responsive -->
                                </div>
                                <div class="col-lg-7">
                                    <ul class="chart-legend clearfix">
                                        <p class="text-center">
                                            <strong>Total</strong>&nbsp;<asp:Label ID="label9" runat="server"></asp:Label>
                                        </p>
                                        <li><i class="fa fa-circle text-danger"></i>Women Self Help Group -<asp:Label ID="labelwshg" runat="server"></asp:Label>
                                        </li>
                                        <li><i class="fa fa-circle text-success"></i>Mahila Mandals -
                                            <asp:Label ID="labelsmm" runat="server"></asp:Label>
                                        </li>
                                        <li><i class="fa fa-circle text-warning"></i>NGO -
                                            <asp:Label ID="labelsngo" runat="server"></asp:Label>
                                        </li>
                                        <li><i class="fa fa-circle text-info"></i>Sports and Cultural Club -
                                            <asp:Label ID="labelsscc" runat="server"></asp:Label>
                                        </li>
                                        <li><i class="fa fa-circle text-secondary"></i>Other Societies -
                                            <asp:Label ID="labelsos" runat="server"></asp:Label>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <!-- /.card-body -->
                    </div>
                </div>

            </div>
            <br />
            <asp:LinkButton ID="LkAdvSeach" CssClass="float-lg-right btn btn-info" OnClick="LkAdvSeach_Click" runat="server">Advance Search</asp:LinkButton>
            <br />
        </div>
    </div>

  

    <br />

    <script>
        jQuery(document).ready(function ($) {
            if (window.history && window.history.pushState) {
                $(window).on('popstate', function () {//when back is clicked popstate event executes                   
                    //code here will execute on back click
                });
            }
        });
    </script>





</asp:Content>
