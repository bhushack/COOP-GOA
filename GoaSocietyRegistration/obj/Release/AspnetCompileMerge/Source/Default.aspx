<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GoaSocietyRegistration.Default" %>

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
    <title>Society Registration Portal</title>
    <style>
        @font-face {
            font-family: 'icomoon';
            src: url("AssestsLogin/fonts/icomooncb5a.eot?y6palq");
            src: url("AssestsLogin/fonts/icomooncb5a.eot?y6palq#iefix") format("embedded-opentype"), url("AssestsLogin/fonts/icomooncb5a.ttf?y6palq") format("truetype"), url("AssestsLogin/fonts/icomooncb5a.woff?y6palq") format("woff"), url("AssestsLogin/fonts/icomooncb5a.svg?y6palq#icomoon") format("svg");
            font-weight: normal;
            font-style: normal;
        }

        .modal {
            overflow-y: auto;
        }

        .modal-open {
            overflow: auto;
        }

            .modal-open[style] {
                padding-right: 0px !important;
            }
    </style>
    <style type="text/css">
        img.wp-smiley,
        img.emoji {
            display: inline !important;
            border: none !important;
            box-shadow: none !important;
            height: 1em !important;
            width: 1em !important;
            margin: 0 .07em !important;
            vertical-align: -0.1em !important;
            background: none !important;
            padding: 0 !important;
        }

        .text-danger {
            color: red !important;
        }

        .btn-success {
            color: #ffffff !important;
            background-color: #5cb85c !important;
            border-color: #4cae4c !important;
        }

        .btn-danger, .btn-danger:active:hover, .btn-danger.active:hover {
            color: #ffffff !important;
            background-color: #ac2925 !important;
            border-color: #761c19 !important;
        }

        .blue-bg {
            background-color: #3f4db8 !important;
        }

        .location-info {
            border-bottom: none !important;
        }

        .mya {
            text-decoration: none !important;
        }

        table {
            border-collapse: collapse;
            border-spacing: 0;
            width: 100%;
            overflow-x: hidden;
        }

        th, td {
            text-align: left;
            padding: 8px;
        }
    </style>
    <style>
        table {
            font-family: arial, sans-serif;
            border-collapse: collapse;
            width: 100%;
            overflow-x: hidden;
        }

        td, th {
            border: 1px solid #dddddd;
            text-align: left;
            padding: 8px;
        }

        tr:nth-child(Odd) {
            background-color: #dddddd;
        }
        .hoveron{
           text-decoration:none!important;
        }
    </style>
    <script src="Scripts/jquery-3.5.0.min.js"></script>
    <script src="Scripts/popper.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>

    <link href="AssestsLogin/CSS/thememylogin.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/style.min.css" rel="stylesheet" media='all' />

    <link href="AssestsLogin/CSS/extra.features.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/style.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/style1.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/js_composer.min.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/fontawsome.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/base.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/extra.css" rel="stylesheet" media='all' />
    <link href="AssestsLogin/CSS/sliderhelper.css" rel="stylesheet" media='all' />


    <link href="AssestsLogin/CSS/styles.css" rel="stylesheet" media='all' />
    <script src='AssestsLogin/JS/jquery-migrate.min.js'></script>
    <script src='AssestsLogin/JS/themed-profiles.js'></script>
    <script src='AssestsLogin/JS/external.js'></script>

    <noscript>
        <style>
            #topBar #accessibility ul li .goiSearch, #topBar1 #accessibility ul li .goiSearch {
                visibility: visible;
            }

            #topBar #accessibility ul li .socialIcons ul, #topBar1 #accessibility ul li .socialIcons ul {
                background: #fff !important;
            }

            #topBar #accessibility ul li .goiSearch, #topBar1 #accessibility ul li .goiSearch {
                right: 0;
                left: inherit;
            }

            .nav li a:focus > ul {
                left: 0;
                opacity: 0.99;
            }

            a:focus, button:focus, .carasoleflex-wrap .flexslider .slides > li a:focus, .flexslider .slides > li a:focus {
                outline: 3px solid #ff8c00 !important;
            }

            .flexslider .slides > li {
                display: block;
            }

            .nav li.active > a, .nav li > a:hover, .nav li > a:focus, .nav ul li a:hover,
            .mva7-thc-activetheme-district-theme-13 .nav li:hover > a, .mva7-thc-activetheme-district-theme-13 .nav li.active > a, .home-13 .nav li:hover > a, .home-13 .nav li.active > a {
                color: #ffffff;
            }

            .nav li:hover > a {
                border-top: none;
                color: #ffffff;
            }

            .nav li.active > a {
                border: 0;
            }

            .nav ul {
                opacity: 1;
                left: 0;
                position: static !important;
                width: auto;
                border: 0;
            }

            .nav li {
                position: static !important;
                display: block;
                float: none;
                border: 0 !important;
            }

                .nav li > a {
                    float: none;
                    display: block;
                    background-color: rgba(146,38,4,0.75) !important;
                    color: #ffffff;
                    margin: 0;
                    padding: 12px 20px !important;
                    border-radius: 0;
                    border-bottom: 1px solid #ffffff !important;
                    position: static !important;
                    border-top: 0;
                    font-size: 14px !important;
                }

            .nav ul.sub-menu li > a {
                background-color: rgba(146,38,4,1);
                font-size: 12px !important;
            }

            ul li .socialIcons {
                visibility: visible !important;
            }

            .nav li > a,
            .nav li.active > a {
                background-color: #9e6b22 !important;
            }

            .nav ul.sub-menu li > a {
                background-color: #f3b45b !important;
            }

            .mva7-thc-activetheme-district-theme-15 .menuWrapper {
                background-color: #ffffff;
            }

            .mva7-thc-activetheme-district-theme-2 .nav li > a,
            .mva7-thc-activetheme-district-theme-2 .nav li.active > a {
                background-color: rgba(63,77,184,0.75) !important;
            }

            .mva7-thc-activetheme-district-theme-2 .nav ul.sub-menu li > a {
                background-color: rgba(63,77,184,1) !important;
            }

            .mva7-thc-activetheme-district-theme-3 .nav li > a,
            .mva7-thc-activetheme-district-theme-3 .nav li.active > a,
            .mva7-thc-activetheme-district-theme-5 .nav li > a,
            .mva7-thc-activetheme-district-theme-5 .nav li.active > a {
                background-color: rgba(212,60,60,0.75) !important;
            }

            .mva7-thc-activetheme-district-theme-3 .nav ul.sub-menu li > a,
            .mva7-thc-activetheme-district-theme-5 .nav ul.sub-menu li > a {
                background-color: rgba(212,60,60,1) !important;
            }

            .mva7-thc-activetheme-district-theme-4 .nav li > a,
            .mva7-thc-activetheme-district-theme-4 .nav li.active > a {
                background-color: rgba(184,48,88,0.75) !important;
            }

            .mva7-thc-activetheme-district-theme-4 .nav ul.sub-menu li > a {
                background-color: rgba(184,48,88,1) !important;
            }

            .mva7-thc-activetheme-district-theme-6 .nav li > a,
            .mva7-thc-activetheme-district-theme-6 .nav li.active > a {
                background-color: rgba(16,91,122,0.75) !important;
            }

            .mva7-thc-activetheme-district-theme-6 .nav ul.sub-menu li > a {
                background-color: rgba(16,91,122,1) !important;
            }

            .mva7-thc-activetheme-district-theme-7 .nav li > a,
            .mva7-thc-activetheme-district-theme-7 .nav li.active > a {
                background-color: rgba(2,20,80,0.75) !important;
            }

            .mva7-thc-activetheme-district-theme-7 .nav ul.sub-menu li > a {
                background-color: rgba(2,20,80,1) !important;
            }

            .mva7-thc-activetheme-district-theme-8 .nav li > a,
            .mva7-thc-activetheme-district-theme-8 .nav li.active > a {
                background-color: rgba(0,144,145,0.65) !important;
            }

            .mva7-thc-activetheme-district-theme-8 .nav ul.sub-menu li > a {
                background-color: rgba(0,144,145,1) !important;
            }

            .mva7-thc-activetheme-district-theme-9 .nav li > a,
            .mva7-thc-activetheme-district-theme-9 .nav li.active > a {
                background-color: rgba(60,125,20,0.75) !important;
            }

            .mva7-thc-activetheme-district-theme-9 .nav ul.sub-menu li > a {
                background-color: rgba(60,125,20,1) !important;
            }

            .mva7-thc-activetheme-district-theme-10 .nav li > a,
            .mva7-thc-activetheme-district-theme-10 .nav li.active > a {
                background-color: rgba(233,13,65,0.70) !important;
            }

            .mva7-thc-activetheme-district-theme-10 .nav ul.sub-menu li > a {
                background-color: rgba(233,13,65,1) !important;
            }

            .mva7-thc-activetheme-district-theme-11 .nav li > a,
            .mva7-thc-activetheme-district-theme-11 .nav li.active > a {
                background-color: rgba(104,57,127,0.75) !important;
            }

            .mva7-thc-activetheme-district-theme-11 .nav ul.sub-menu li > a {
                background-color: rgba(104,57,127,1) !important;
            }

            .mva7-thc-activetheme-district-theme-13 .nav li > a,
            .mva7-thc-activetheme-district-theme-13 .nav li.active > a {
                background-color: rgba(0,0,0,0.75) !important;
            }

            .mva7-thc-activetheme-district-theme-13 .nav ul.sub-menu li > a {
                background-color: rgba(0,0,0,1) !important;
            }

            .mva7-thc-activetheme-district-theme-14 .nav li > a,
            .mva7-thc-activetheme-district-theme-14 .nav li.active > a {
                background-color: rgba(0,120,175,0.75) !important;
            }

            .mva7-thc-activetheme-district-theme-14 .nav ul.sub-menu li > a {
                background-color: rgba(0,120,175,1) !important;
            }

            .mva7-thc-activetheme-district-theme-15 .nav li > a,
            .mva7-thc-activetheme-district-theme-15 .nav li.active > a {
                background-color: rgba(150,86,104,0.75) !important;
            }

            .mva7-thc-activetheme-district-theme-15 .nav ul.sub-menu li > a {
                background-color: rgba(150,86,104,1) !important;
            }
        </style>
    </noscript>
    <%--<script type="text/javascript">

        $(document).ready(function () {
            $(".test").click(function () {
                $("#contact_and_all").show();
                $("#slider_and_all").hide();
                $("#contact_us_all").hide();
                $('.menu-item').removeClass('active');
                var $this = $(this);
                if (!$this.hasClass('active')) {
                    $this.addClass('active');
                }
            });

        });

        $(document).ready(function () {
            $(".test0").click(function () {
                $("#contact_and_all").hide();
                $("#slider_and_all").show();
                $("#contact_us_all").hide();
            });

        });
        $(document).ready(function () {
            $(".test1").click(function () {
                $("#contact_and_all").hide();
                $("#slider_and_all").hide();
                $("#contact_us_all").show();
                $('.menu-item').removeClass('active');
                var $this = $(this);
                if (!$this.hasClass('active')) {
                    $this.addClass('active');
                }
            });

        });
        $(document).ready(function () {
            $("#contact_and_all").hide();
            $("#contact_us_all").hide();
        });
    </script>--%>
    <link rel="alternate" href="Default.aspx" hreflang="x-default" />
    <link rel="alternate" href="Default.aspx" hreflang="en" />
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
                                    <a href="ScreenReaderAccess.html"   title="Screen Reader Access">
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
            <div class="menuWrapper">
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
                            <li id="menu-item-2495" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2494"><a href="PublicDashboard.aspx">Public Dashboard</a></li>
                             <li id="menu-item-2496" class="menu-item menu-item-type-post_type menu-item-object-page menu-item-2494"><a href="VerifyCertificate.aspx">Verifiy Certificate</a></li>
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

        <main>
            <div class="wrapper bodyWrapper no_padding">
                <div class="container home-2">
                    <div id="SkipContent"></div>
                    <div class="row">
                        <div class="col-12" id="slider_and_all" runat="server">
                            <div data-vc-full-width="true" data-vc-full-width-init="true" data-vc-stretch-content="true" class="vc_row wpb_row vc_row-fluid vc_row-no-padding" style="position: relative; left: -149.297px; box-sizing: border-box; width: 1583px;">
                                <div class="wpb_column vc_column_container vc_col-sm-12">
                                    <div class="vc_column-inner ">
                                        <div class="wpb_wrapper">
                                            <div id="slide" class="home-slider full-cntrl-center-caption-bottom nav-white  flexslider ">

                                                <div class="flex-viewport" style="overflow: hidden; position: relative;">
                                                    <ul class="slides" style="width: 800%; transition-duration: 0s; transform: translate3d(-1583px, 0px, 0px);">

                                                        <li style="width: 1583px; float: left; display: block;" class="flex-active-slide">
                                                            <img src="assets/images/bg1.png" alt="Society registration banner" draggable="false" />
                                                            <div class="container">
                                                                <div class="slide-caption">
                                                                    <p class="heading3">SOCIETY REGISTRATION</p>
                                                                </div>

                                                            </div>
                                                        </li>

                                                    </ul>
                                                </div>
                                                <ul class="flex-direction-nav">
                                                    <li><a class="flex-prev" href="#" title="Previous" aria-label="Previous"><span class="hide">Previous</span></a></li>
                                                    <li><a class="flex-next" href="#" title="Next" aria-label="Next"><span class="hide">Next</span></a></li>
                                                </ul>
                                                <div class="flex-pauseplay"><a class="flex-play" href="javascript:void(0)" title="Play/Pause" aria-label="Play/Pause"><span class="hide">Play</span></a></div>
                                            </div>
                                            <script>
                                                jQuery(document).ready(function ($) {
                                                    // Slider

                                                    $('.home-slider').flexslider({
                                                        animation: ($('body').hasClass('rtl')) ? "fade" : "slide",
                                                        directionNav: true,
                                                        prevText: "<span class='hide'>Previous</span>",
                                                        nextText: "<span class='hide'>Next</span>",
                                                        pausePlay: true,
                                                        pauseText: "<span class='hide'>Pause</span>",
                                                        playText: "<span class='hide'>Play</span>",
                                                        controlNav: false,
                                                        start: function (slider) {
                                                            $('body').find('.flexslider').resize();
                                                            if (slider.count == 1) {
                                                                slider.pausePlay.parent().remove();
                                                            }
                                                        }

                                                    });
                                                });

                                            </script>
                                            <div class="wrapper" id="skipCont"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="vc_row-full-width vc_clearfix" style="padding-bottom: 2%;"></div>
                            <div class="vc_row wpb_row vc_row-fluid vc_custom_1501846501584 vc_row-o-equal-height vc_row-flex">
                                <div class="wpb_column vc_column_container vc_col-sm-4">
                                    <div class="vc_column-inner ">
                                        <div class="wpb_wrapper">
                                            <h3 style="padding-bottom: 12px;">SOCIETY REGISTRATION</h3>
                                            <p align="justify">A society is an organized group of persons whose main objective is to serve society and not to earn a profit. A society is registered under The Societies Registration Act, 1860 to become a legal entity. A society exists for the purpose of charitable activities such as poverty relief, arts, education, cultures and sports. The Governing body members are entrusted with the management of the affairs of the society. </p>
                                        </div>
                                    </div>
                                </div>
                                <div class="wpb_column vc_column_container vc_col-sm-2">
                                    <div class="vc_column-inner">
                                        <div class="wpb_wrapper">
                                            <img class="sw-logo" height="286px" width="auto" src="AssestsLogin/img/goa_map.png" alt="Digital India" />
                                        </div>
                                    </div>
                                </div>

                                <div class="wpb_column vc_column_container vc_col-sm-3">
                                    <div class="vc_column-inner ">
                                        <div class="wpb_wrapper">
                                            <div class="gen-list no-border no-bg padding-0 border-radius-none iconTop-textBottom-list services-eight  normal-font ">
                                                <center><h2 class="heading3">SERVICES</h2></center>
                                                <ul>
                                                    <li class="  ">
                                                        <a href="Applicant.aspx" title="New User">
                                                            <span class="list-icon icon-group-fill border-radius-round"></span>
                                                            <div class="list-text">Apply Online</div>
                                                            <%-- &nbsp;&nbsp;&nbsp;<div class="list-text">New User</div>--%>
                                                        </a>

                                                    </li>

                                                    <li class="  ">
                                                        <a href="User/LoginModule.aspx" title="Existing User">
                                                            <span class="list-icon fa fa-sign-in border-radius-round"></span>
                                                            <%-- <div class="list-text">Existing User</div>--%>
                                                            <div class="list-text">User Login</div>
                                                        </a>

                                                    </li>
                                                    <li class="  ">
                                                        <a href="OrganizationLogin.aspx" title="Office Login">
                                                            <span class="list-icon icon-magisterial border-radius-round"></span>
                                                            <div class="list-text">Office Login</div>
                                                        </a>
                                                    </li>
                                                </ul>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="wpb_column vc_column_container vc_col-sm-3">
                                    <div class="vc_column-inner ">
                                        <div class="wpb_wrapper">

                                            <div class="col-3 singlebox border office-bearers-nine">

                                                <div class="box-1 aside-box" style="float: none !important">
                                                    <div class="khowMinisterBox">
                                                        <div class="khowMinisterBoxImg">
                                                            <img class="" src="AssestsLogin/img/User.png" alt="Photo of Honourable Chief Minister of Goa" />
                                                        </div>
                                                        <div class="MinisterProfile">
                                                            <span class="Pname">Hon'ble Chief Minister</span>
                                                           <%-- <span class="Pdesg">Dr. Pramod Sawant</span>--%>

                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="box-1 aside-box" style="float: none !important">
                                                    <div class="khowMinisterBox">
                                                        <div class="khowMinisterBoxImg">
                                                            <img class="" src="AssestsLogin/img/User.png" alt="Photo of Honourable Minister for Law & Judiciary" />
                                                        </div>
                                                        <div class="MinisterProfile">
                                                            <span class="Pname">Minister for Law & Judiciary</span>
                                                           <%-- <span class="Pdesg">Shri Nilesh Cabral</span>--%>

                                                        </div>
                                                    </div>
                                                </div>
                                            </div>




                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                        <%--<div class="col-12" id="contact_and_all" runat="server">
                            <div class="vc_row-full-width vc_clearfix" style="padding-bottom: 2%;"></div>
                            <div class="vc_row wpb_row vc_row-fluid vc_custom_1501846501584 vc_row-o-equal-height vc_row-flex">
                                <div class="wpb_column vc_column_container vc_col-sm-10  vc_col-sm-offset-1">

                                    <div class="wpb_wrapper">
                                        <h3 style="padding-bottom: 12px;">About Us</h3>

                                        <p align="justify" style="font-size: 16px !important">
                                            The Registration Department with its twelve offices of Civil Registrars-Cum-Sub-Registrars one each in each Taluka and Office of the State Registrar-cum-Head of Notary Services at Panaji; and two offices of the District Registrars of North Goa and South Goa; basically deal with recording and preserving evidentiary matters relating to contracts, status, testamentary dispositions, etc., of individual citizens, like marriages, legitimations, adoptions, documents relating to contractual obligations, property transactions, Firms, Wills, Succession deeds, Societies, appointment of Notaries and similar.
                                            <br />
                                            <br />
                                            The Departmental work deals principally with the formal aspects of the transactions and creates or registers records with the special duty to permanently preserve the same for making authentic certified copies therefrom in future. Part of the work connected with the Personal law of the Goans, is peculiar to Goa, the registers in several cases requiring authentication by judicial Authorities and some work like Succession Deeds, Wills, etc., which is the exclusive domain of Courts and High Courts outside Goa, being also dealt with by the officers.
                                        </p>
                                    </div>

                                </div>

                            </div>
                        </div>--%>
                        <%--    <div class="col-12" id="contact_us_all" runat="server">
                            <div class="vc_row-full-width vc_clearfix" style="padding-bottom: 2%;"></div>
                            <div class="vc_row wpb_row vc_row-fluid vc_custom_1501846501584 vc_row-o-equal-height vc_row-flex">
                                <div class="wpb_column vc_column_container vc_col-sm-10 vc_col-sm-offset-1">

                                    <div class="wpb_wrapper">
                                        <h3 style="padding-bottom: 12px;">Contact Us</h3>

                                        <br />
                                        <br />
                                        <table>



                                            <tr>
                                                <td>CONTACT ADDRESS</td>
                                                <td>Office of State Registrar-cum-Head of Notary Services,
Shramshakti Bhavan, 7th Floor,
Patto, Panaji, Tiswadi - Goa
Pincode: 403001</td>

                                            </tr>
                                            <tr>
                                                <td>PHONE</td>
                                                <td>+91 832 2437136</td>

                                            </tr>
                                            <tr>
                                                <td>FAX</td>
                                                <td>+91 832 2437133</td>

                                            </tr>

                                            <tr>
                                                <td>WEBLINK</td>
                                                <td><a href="http://registration.goa.gov.in/">http://registration.goa.gov.in</a></td>

                                            </tr>
                                        </table>


                                    </div>
                                </div>
                            </div>
                        </div>--%>
                    </div>
                </div>

                <br />
            </div>

            <div class="row">
                <div class="vc_row-full-width vc_clearfix"></div>
                <div data-vc-full-width="true" data-vc-full-width-init="false" data-vc-stretch-content="true" class="vc_row wpb_row vc_row-fluid vc_row-no-padding">
                    <div class="wpb_column vc_column_container vc_col-sm-12">
                        <div class="vc_column-inner vc_custom_1499931344939">
                            <div class="wpb_wrapper">
                                <section id="footerScrollbarWrapper" class="footerlogocarousel withbg withborder" aria-label="Other Important Links">
                                    <div class="footerlogocarousel-outer item-count-8">
                                        <div id="footerScrollbar" class="flexslider">
                                            <ul class="slides" aria-label="Important Sites">
                                                <li>
                                                    <a href="https://data.gov.in/" target="_blank" rel="noopener noreferrer" title="Open Government Data (OGD)  Platform India">
                                                        <img src="AssestsLogin/img/minislider/2017053014.png" alt="data.gov.in" />
                                                    </a>
                                                </li>
                                                <li>
                                                    <a href="https://incredibleindia.org/" rel="noopener noreferrer" target="_blank" title="Incredible India">
                                                        <img src="AssestsLogin/img/minislider/2017053094.png" alt="Incredible India Site" />
                                                    </a>
                                                </li>
                                                <li>
                                                    <a href="http://www.makeinindia.com/home" rel="noopener noreferrer" target="_blank" title="Make in India">
                                                        <img src="AssestsLogin/img/minislider/2017053052.png" alt="make in India" />
                                                    </a>
                                                </li>
                                                <li>
                                                    <a href="https://www.mygov.in/" rel="noopener noreferrer" target="_blank" title="My Government">
                                                        <img src="AssestsLogin/img/minislider/2017053017.png" alt="mygov" />
                                                    </a>
                                                </li>
                                                <li>
                                                    <a href="https://www.pmnrf.gov.in/" rel="noopener noreferrer" target="_blank" title="Prime Minister&#8217;s National Relief Fund">
                                                        <img src="AssestsLogin/img/minislider/2017053039.png" alt="PMNRF" />
                                                    </a>
                                                </li>
                                                <li>
                                                    <a href="http://www.pmindia.gov.in/en/" rel="noopener noreferrer" target="_blank" title="Prime Minister of India">
                                                        <img src="AssestsLogin/img/minislider/2017110781.png" alt="pmindia" />
                                                    </a>
                                                </li>
                                                <li>
                                                    <a href="https://www.india.gov.in/" rel="noopener noreferrer" target="_blank" title="The National Portal of India">
                                                        <img src="AssestsLogin/img/minislider/2019052293.png" alt="india.gov.in" />
                                                    </a>
                                                </li>
                                                <li>
                                                    <a href="http://www.digitalindia.gov.in/" rel="noopener noreferrer" target="_blank" title="Digital India">
                                                        <img src="AssestsLogin/img/minislider/2017072418.png" alt="digital-india" />
                                                    </a>
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                </section>
                                <script type="text/javascript">
                                    jQuery(document).ready(function () {
                                        jQuery("#footerScrollbar").flexslider({
                                            animation: "slide",
                                            animationLoop: true,
                                            itemWidth: 201,
                                            minItems: 2,
                                            slideshow: 1,
                                            move: 1,
                                            controlNav: false,
                                            pausePlay: true,
                                            prevText: "<span class='hide'>Previous</span>",
                                            nextText: "<span class='hide'>Next</span>",
                                            pauseText: "<span class='hide'>Pause</span>",
                                            playText: "<span class='hide'>Play</span>",
                                        })
                                    });
                                </script>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="vc_row-full-width vc_clearfix"></div>
            </div>
        </main>
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
                        <p>Last Updated: <strong>August 18, 2025</strong></p>
                        <div class="certification-logo">
                        </div>
                    </div>
                    <div class="copyRightsLogos">
                        <a href="#">
                            <img src="AssestsLogin/img/makeinindia.png" style="width: 127px; height: 45px" alt="Make IN India opens a new window" /></a>
                        <a href="http://www.nic.in/">
                            <img src="AssestsLogin/img/nicLogo.png" alt="National Informatics Centre opens a new window" /></a>
                        <a href="http://www.digitalindia.gov.in/">
                            <img src="AssestsLogin/img/digitalIndia.png"  alt="Digital India opens a new window" /></a>
                        <!-- <a href="#" class="stqc-logo"><img src="/common_utility/images/STQC-approved.png"  alt="STQC"></a> -->
                    </div>
                </div>
            </div>
        </footer>

        <script>
            jQuery(document).ready(function ($) {
                jQuery('.vc_tta-tabs-list').attr('role', 'tablist');
                jQuery('.vc_tta-panel').attr('role', 'tabpanel');
                jQuery('.vc_tta-tab a').attr('role', 'tab');
                jQuery('[data-vc-tabs]').each(function () {
                    var id = jQuery(this).attr('href');
                    id = id.replace('#', '');
                    jQuery(this).attr('aria-controls', id);
                    if (jQuery(this).parent().hasClass('vc_active')) {
                        jQuery(this).attr('aria-selected', true)
                    } else {
                        jQuery(this).attr('aria-selected', false)
                    }
                });
                jQuery('[data-vc-tabs]').click(function () {
                    jQuery(this).parent().siblings().find('a').attr('aria-selected', false);
                    jQuery(this).attr('aria-selected', true);
                });
                if (!String.prototype.startsWith) {
                    String.prototype.startsWith = function (searchString, position) {
                        position = position || 0;
                        return this.indexOf(searchString, position) === position;
                    };
                }
                jQuery('body').on('targetExternalLinks', function () {
                    var isExternal = function (url) {
                        if (!url.match('^(https?:)?(\\/\\/).*$')) return false;
                        return !(location.href.replace("http://", "").replace("https:///", "").split("Default.aspx")[0] === url.replace("http://", "").replace("https:///", "").split("Default.aspx")[0]);
                    }
                    jQuery('a').each(function () {
                        var href = jQuery(this).attr('href');
                        if (typeof href == 'undefined') {
                            jQuery(this).attr('href', 'javascript:void(0)');
                            href = '#';
                        }
                        if ($(this).attr('hreflang') !== undefined) {
                            if (jQuery(this).attr('aria-label') !== typeof undefined) {
                                jQuery(this).attr('aria-label', jQuery(this).text()).attr('title', jQuery(this).text());
                            }
                        } else if (isExternal(href)) {
                            if (href.indexOf('https://nicgoa.nic.in/') == -1 && href.indexOf('https://nicgoa.nic.in/') == -1) {
                                if (typeof jQuery(this).attr('onclick') === "undefined") {
                                    jQuery(this).attr("onclick", "return confirm('You are being redirected to an external website. Please note that District Preview Template cannot be held responsible for external websites content & privacy policies.');");
                                }
                            }
                            if (typeof jQuery(this).attr('aria-label') === "undefined" || typeof jQuery(this).attr('title') === "undefined") {
                                var text = '';
                                if (jQuery(this).text().trim() !== '') {
                                    text = jQuery(this).text().trim() + ' - ';
                                } else {
                                    text = jQuery(this).attr('href') + ' - ';
                                }
                                if (href.indexOf('https://nicgoa.nic.in/') == -1 && href.indexOf('auth.s3waas.gov.in') == -1) {
                                    if (typeof jQuery(this).attr('aria-label') === "undefined") {
                                        jQuery(this).attr('aria-label', text + 'External site that opens in a new window');
                                    }
                                    if (typeof jQuery(this).attr('title') === "undefined") {
                                        jQuery(this).attr('title', text + 'External site that opens in a new window');
                                    }
                                }
                            }
                            if (href.indexOf('https://nicgoa.nic.in/') == -1) {
                                jQuery(this).prop('target', '_blank');
                            }
                        }
                    });
                })
                jQuery('body').trigger('targetExternalLinks');
                jQuery('body iframe').each(function () {
                    var attrSrc = $(this).attr('src');
                    if (attrSrc.indexOf('map') > 0) {
                        $(this).attr('title', 'District Map');
                    }
                });
                $('.flex-direction-nav a.flex-prev').attr({ 'title': 'Previous', 'aria-label': 'Previous' });
                $('.flex-pauseplay a.flex-pause').attr({ 'title': 'Play/Pause', 'aria-label': 'Play/Pause' });
                $('.flex-direction-nav a.flex-next').attr({ 'title': 'Next', 'aria-label': 'Next' });
                $('a[download]').each(function () {
                    var ariaLabelPrevious = $(this).prev().attr('aria-label');
                    if (typeof ariaLabelPrevious !== typeof undefined) {
                        var ariaLabel = $(this).prev().attr('aria-label').split('-')[0];
                        ariaLabel = 'Download ' + ariaLabel;
                        $(this).attr('aria-label', ariaLabel).removeAttr('aria-hidden');
                    }
                });
            });
        </script>




        <%--<link rel='stylesheet' id='services-style-css' href='Assests/Default/CSS/1list-style.min.css' media='all' />--%>


        <link href="AssestsLogin/CSS/js_composer.min.css" rel="stylesheet" media='all' />
        <link href="AssestsLogin/CSS/flexslider.min.css" rel="stylesheet" media='all' />
        <link href="AssestsLogin/CSS/customflexslider.css" rel="stylesheet" media='all' />
        <link href="AssestsLogin/CSS/style.min.css" rel="stylesheet" media='all' />
        <link href="AssestsLogin/CSS/componenthelper.css" rel="stylesheet" media='all' />
        <link href="AssestsLogin/CSS/servicetabs.css" rel="stylesheet" media='all' />
        <link href="AssestsLogin/CSS/fontawesome.min.css" rel="stylesheet" media='all' />
        <link href="AssestsLogin/CSS/events.css" rel="stylesheet" media='all' />
        <link href="AssestsLogin/CSS/footerlogocarousel.css" rel="stylesheet" media='all' />
        <link href="AssestsLogin/CSS/photogalleryhome.css" rel="stylesheet" media='all' />


        <script src='AssestsLogin/JS/common.js'></script>
        <script src='AssestsLogin/JS/core.min.js'></script>
        <script src='AssestsLogin/JS/jquery.flexslider.js'></script>
        <script src='AssestsLogin/JS/easyResponsiveTabs.js'></script>
        <script src='AssestsLogin/JS/jquery.fancybox.js'></script>
        <script src='AssestsLogin/JS/style.switcher.js'></script>
        <script src='AssestsLogin/JS/menu.js'></script>
        <script src='AssestsLogin/JS/table.min.js'></script>
        <script src='AssestsLogin/JS/custom.js'></script>
        <script src='AssestsLogin/JS/extra.js'></script>
        <script src='AssestsLogin/JS/wp-embed.min.js'></script>
        <script src='AssestsLogin/JS/js_composer_front.min.js'></script>
        <script src='AssestsLogin/JS/jquery.flexslider-min.js'></script>
        <script src='AssestsLogin/JS/vc-accordion.min.js'></script>
        <script src='AssestsLogin/JS/vc-tta-autoplay.min.js'></script>
        <script src='AssestsLogin/JS/vc-tabs.min.js'></script>
    </form>
</body>
</html>
