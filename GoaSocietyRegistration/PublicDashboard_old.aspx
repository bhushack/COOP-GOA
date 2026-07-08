<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublicDashboard.aspx.cs" Inherits="GoaSocietyRegistration.PublicDashboard" %>

<!DOCTYPE html>

<html lang="en-US" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta http-equiv="content-type" content="text/html;charset=UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="Online Society Registration Portal" />
    <meta name="keywords" content="Society Registration Portal,Society  Registration" />
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <link rel="icon" href="favicon.ico" type="image/gif" sizes="16x16" />
    <title>Public Dashboard for Society registration portal</title>
   
    <script src="Scripts/jquery-3.5.0.min.js"></script>
    <script src="Scripts/popper.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <link href="assets/css/bootstrappart.css" rel="stylesheet" />
    <style>
        menu-item menu-item-type-post_type menu-item-object-page menu-item-2503, .menu {
            text-transform: none !important;
        }

        #topBar .govBranding ul li a {
            padding: 8px 10px !important;
        }

        #chartdiv {
            width: 100%;
            height: 400px;
        }

        .noc {
            padding: 8px;
            background-color: #001c6b !important;
            color: #ffffff;
        }

        .buttons {
            background-color: #0d6efd !important;
        }
    </style>

    
    <script src="Organization/assets/js/jquery.countTo.js"></script>
    <link href="Organization/assets/css/animate.css" rel="stylesheet" />
    
     
    <script src="Scripts/loader.js"></script>
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
    <script src="chart/societychart.js"></script>
    <script type="text/javascript"> 
        $(function () {

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

    <link href="AssestsLogin/CSS/fontawsome.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/base.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/extra.css" rel="stylesheet" media='all' />
</head>
<body class="home page-template-default page page-id-3371 mva7-thc-activetheme-district-theme lang-en wpb-js-composer js-comp-ver-5.4.7 vc_responsive" style="font-size: 14px !important">
    <form id="form1" runat="server">
        <header>
            <div id="topBar" class="wrapper">
                <div class="container">
                    <div class="push-right" aria-label="Primary">
                        <div id="accessibility">
                            <ul id="accessibilityMenu">
                                <li><a href="#SkipContent" class="skip-to-content" title="Skip to main content"><span class="icon-skip-to-main responsive-show"></span><strong class="responsive-hide">SKIP TO MAIN CONTENT</strong></a></li>
                                <li>
                                    <a href="ScreenReaderAccess.html" title="Screen Reader Access">
                                        <i class="fa fa-desktop" aria-hidden="true"></i>
                                        <span class="off-css">Screen Reader Access</span>
                                    </a>
                                </li>
                                <li>
                                    <a href="site-map.html" title="Sitemap">
                                        <i class="fa fa-sitemap" aria-hidden="true"></i>
                                        <span class="off-css">Site Map</span>
                                    </a>
                                </li>

                                <li>
                                    <a href="javascript:void(0);" title="Accessibility Links" aria-label="Accessibility Links" class="mobile-show accessible-icon"><span class="off-css">Accessibility Links</span><span class="icon-accessibility" aria-hidden="true"></span></a>
                                    <div class="accessiblelinks textSizing">
                                        <ul>
                                            <li><a href="javascript:void(0);" aria-label="Font Size Increase" title="Font Size Increase"><span aria-hidden="true">A+</span><span class="off-css"> Font Size Increase</span></a></li>
                                            <li><a href="javascript:void(0);" aria-label="Normal Font" title="Normal Font"><span aria-hidden="true">A</span><span class="off-css"> Normal Font</span></a></li>
                                            <li><a href="javascript:void(0);" aria-label="Font Size Decrease" title="Font Size Decrease"><span aria-hidden="true">A-</span><span class="off-css"> Font Size Decrease</span></a></li>
                                            <%--<li class="highContrast dark tog-con">
                                                <a href="javascript:void(0);" aria-label="High Contrast" title="High Contrast"><span aria-hidden="true">A</span> <span class="tcon">High Contrast</span></a>
                                            </li>
                                            <li class="highContrast light">
                                                <a href="javascript:void(0);" aria-hidden="true" aria-label="Normal Contrast" title="Normal Contrast"><span aria-hidden="true">A</span> <span class="tcon">Normal Contrast</span></a>
                                            </li>--%>
                                        </ul>
                                    </div>
                                </li>
                                <li><a href="javascript:void(0);" class="change-language" aria-label="English" title="English">English</a></li>
                            </ul>
                        </div>
                    </div>
                    <div class="push-left">
                        <div class="govBranding">
                            <ul>
                                <li><a href="https://www.goa.gov.in/">गोयें सरकार </a></li>
                                <li><a lang="grt" href="https://www.goa.gov.in/">Government of Goa</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
            <div class="wrapper header-wrapper">
                <div class="container header-container">
                    <div class="logo">
                        <a href="Default.aspx" aria-label="Go to home" class="emblem" rel="home">
                            <img class="site_logo" height="100" id="logo" src="AssestsLogin/img/goa.png" alt="Goa State Emblem" />

                            <div class="logo-text">
                                <strong lang="grt" class="site_name_regional" style="font-weight: 700!important">Registration Of Societies</strong>
                                <h1 class="site_name_english">Department of Registration ,Goa</h1>

                            </div>
                        </a>
                    </div>

                    <%--    <div class="header-right clearfix">
                        <div class="right-content clearfix">
                            <div class="float-element">
                                <a aria-label="Digital India - External site that opens in a new window" href="http://digitalindia.gov.in/" target="_blank" title="Digital India">
                                    <img class="sw-logo" height="95" src="AssestsLogin/img/digital-india.png" alt="Digital India">
                                </a>
                            </div>
                        </div>
                    </div>--%>
                    <a class="menuToggle" href="javascript:void(0);" aria-label="Mobile Menu"><span class="icon-menu"></span><span class="tcon">Menu Toggle</span></a>
                </div>
            </div>
            <div class="menuWrapper" runat="server" visible="false">
                <div class="menuMoreText hide">More</div>
                <div class="container">
                    <nav class="menu">
                        <ul id="menu-header-en" class="nav clearfix">
                            <li id="menu-item-26" class="menu-item menu-item-type-custom menu-item-object-custom current-menu-item current_page_item menu-item-home menu-item-2658 active ">
                                <a href="#">Home</a>
                            </li>
                            <li id="menu-item-2659" class="menu-item menu-item-type-custom menu-item-object-custom current-menu-item current_page_item menu-item-home menu-item-2658">
                                <a href="aboutus.html">About Us</a></li>
                            <li id="menu-item-2660" class="menu-item menu-item-type-custom menu-item-object-custom current-menu-item current_page_item menu-item-home menu-item-2660">
                                <a href="leaflet.pdf" target="_blank">Help</a></li>

                            <li id="menu-item-2494" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2494"><a href="contactus.html">Contact Us</a></li>
                        </ul>
                    </nav>
                </div>
            </div>
            <div class="clearfix"></div>
            <div id="overflowMenu">
                <div class="ofMenu">
                    <ul>
                    </ul>
                </div>
                <a title="Close" href="javascript:void(0);" class="closeMenu"><span class="icon-close" aria-hidden="true"></span>Close</a>
            </div>
        </header>
        <div class="noc row">
            <p style="text-align: center!important; padding-bottom: 0px!important; line-height: 1.5em!important;"><strong>Dashboard ( Last Updation:<asp:Label ID="Label5" runat="server" Text=""></asp:Label></strong> )</p>
        </div>
        <div class="row">
            <a style="float: right!important" href="Default.aspx" class="btn btn-primary buttons"><i class="fa fa-home"></i>&nbsp;Home</a>
        </div>
        <br />
        <div class="row" id="chart_card" runat="server">


            <div class="col-12 ">
                <br />
                <div class="row">
                    <div class="col-6 ">
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
                    <div class="col-6  ">
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

                <br />
            </div>
            <div class="col-6 " runat="server" visible="false">
                <br />
                <div class="row">
                    <div class="card">
                        <div class="card-header" style="height: auto">
                            <h5 class="card-title">Societies Registered</h5>
                        </div>
                        <!-- /.card-header -->
                        <div class="card-body">

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

                            <div class="col-12">
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
                            <div class="col-3" runat="server" visible="false">
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
                            <div class="col-3" runat="server" visible="false">

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
                            <div class="col-lg-4" runat="server" visible="false">
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
                                                                <asp:Label ID="Label1" runat="server" Text="North Goa : " CssClass="align-content-lg-start"></asp:Label>
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

                            <!-- /.row -->
                        </div>

                    </div>
                </div>
            </div>
            <!-- /.card -->
            <br />

        </div>
        <br />
        <br />
        <footer id="footer" class="footer-home">
            <div class="container">
                <div class="footerMenu">
                    <ul id="menu-footer-en" class="menu">

                        <li id="menu-item-2500" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a href="Feedback.aspx">Feedback</a></li>
                        <li id="menu-item-2501" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a href="website-policies.html">Website Policies</a></li>
                        <li id="menu-item-2502" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a target="_blank" href="Download/faq.pdf">FAQ&nbsp;<i class="fa fa-star-o fa-spin" aria-hidden="true"></i></a></li>
                        <%-- <li id="menu-item-2503" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a href="Guidelines.html">Guidelines</a></li>--%>
                        <li id="menu-item-2504" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2503"><a target="_blank" href="Download/user_manual.pdf">User Manual</a></li>
                        <li id="menu-item-2506" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2506"><a href="contactus.html">Contact Us</a></li>

                    </ul>
                </div>

                <div class="copyRights">
                    <div class="pd-bottom5 color-white ctnt-ownd-dis">Content Owned by Registration Department, Government of Goa</div>
                    <div class="copyRightsText">
                        <p>
                            Developed and hosted by <a href="https://nicgoa.nic.in/" rel="noopener noreferrer" target="_blank">National Informatics Centre, Goa</a>,<br />
                            <a href="http://meity.gov.in/" rel="noopener noreferrer" target="_blank">Ministry of Electronics & Information Technology</a>, Government of India
                        </p>

                        <div class="certification-logo">
                            <!-- <div class="certification-cont"> -->
                            <!-- <img src="wp-content/plugins/common_utility/images/certification-logo.png" alt="STQC Accessible Logo" /> -->
                            <!-- <strong>Verified Accessible Theme</strong> -->
                            <!-- </div> -->
                        </div>
                    </div>
                    <div class="copyRightsLogos">
                        <a href="#">
                            <img src="AssestsLogin/img/makeinindia.png" style="width: 127px; height: 45px" alt="Make IN India opens a new window" /></a>
                        <a href="http://www.nic.in/">
                            <img src="AssestsLogin/img/nicLogo.png" alt="National Informatics Centre opens a new window" /></a>
                        <a href="http://www.digitalindia.gov.in/">
                            <img src="AssestsLogin/img/digitalIndia.png" alt="Digital India opens a new window" /></a>
                        <!-- <a href="#" class="stqc-logo"><img src="/common_utility/images/STQC-approved.png"  alt="STQC"></a> -->
                    </div>
                </div>
            </div>
        </footer>
    </form>
</body>
</html>
