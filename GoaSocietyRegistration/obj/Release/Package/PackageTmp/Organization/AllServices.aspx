<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="AllServices.aspx.cs" Inherits="GoaSocietyRegistration.Organization.AllServices" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <style>
        .declaration {
            background-color: #0062cc !important;
            color: #ffffff;
            padding: 8px;
            font-weight: bold;
        }

        .bs-example {
            margin: 20px;
        }

        .left {
            float: left;
        }
    </style>


    <style>
        .box {
            background-color: #ffffff;
            display: flex;
            width: 90%;
            height: 75px;
            position: relative;
            z-index: 10;
            border-radius: 2px 25px;
            transition: .5s;
            -moz-box-shadow: 9px 9px 9px -2px rgba(143, 154, 149, 0.72);
            -webkit-box-shadow: 7px 7px 7px -2px rgba(143, 154, 149, 0.72);
        }

        .left {
            width: 80px;
            height: 75px;
            position: relative;
            display: flex;
            justify-content: center;
            align-items: center;
            cursor: pointer;
            border-radius: 2px 25px;
            flex-shrink: 0;
            overflow: hidden;
        }

        .right {
            width: calc(100% - 80px);
            display: flex;
            align-items: center;
            overflow: hidden;
            cursor: pointer;
            justify-content: space-between;
            /*white-space: nowrap;*/
            transition: 0.3s;
        }


        .new {
            font-size: 17px;
            font-family: "Lexend Deca", sans-serif;
            margin-left: 20px;
            margin-top: auto;
            margin-bottom: auto;
            color: #696969;
        }

        .info {
            font-size: 14px;
            font-family: "Lexend Deca", sans-serif;
            /*margin-left: 20px;*/
        }


        .box:hover {
            transform: scale(1.1);
            margin: auto;
        }


        .left .fa {
            font-size: 32px;
            color: white;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />

    <div class="container-fluid">
        <div class="row">
            <div class="col-lg-12 col-xs-12">
                <div class="declaration">All Services</div>
                <div class="card-header tab-card-header table-responsive">
                    <div class="row">

                        <%-- Search --%>
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-xl-4">
                            <a href="Search.aspx">
                                <div class="box m-2" title="Click to Goto Search Page">
                                    <div class="left" style="background-color: #836f9e;">
                                        <i class="fa fa-search " aria-hidden="true"></i>
                                    </div>
                                    <div class="right">
                                        <div class="new">
                                            <b>Search Societies </b>
                                            <div class="info">(Online Registered)</div>
                                        </div>
                                    </div>
                                </div>
                            </a>
                        </div>

                        <%-- Edit Society Details--%>
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-xl-4">
                            <a href="EditSocietyDetails.aspx">
                                <div class="box m-2" title="Click to Goto Society Details Page">
                                    <div class="left" style="background-color: violet;">
                                        <i class="fa fa-edit " aria-hidden="true"></i>
                                    </div>
                                    <div class="right">
                                        <div class="new">
                                            <b>Edit / Search Offline Registered Societies</b>
                                            <div class="info"></div>
                                        </div>
                                    </div>
                                </div>
                            </a>
                        </div>

                        <asp:HiddenField ID="HiddenField1" runat="server" />

                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-xl-4">
                            <a>
                                <div class="box m-2" title="Click to Goto Add Society Details">
                                    <div class="left" style="background-color: slateblue">
                                        <i class="fa fa-plus-square" aria-hidden="true"></i>
                                    </div>
                                    <div class="right">
                                        <div class="new">
                                            <b>Add Old Society</b>
                                            <div class="info">(Offline Registered)</div>
                                        </div>
                                    </div>
                                </div>
                            </a>
                        </div>



                        <%-- Echallan Status --%>
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-xl-4">
                            <a href="EchallanStatus.aspx">
                                <div class="box m-2">
                                    <div class="left" style="background-color: mediumseagreen">
                                        <i class="fa fa-money " aria-hidden="true"></i>
                                    </div>
                                    <div class="right">
                                        <div class="new"><b>Check Echallan Status</b></div>
                                    </div>
                                </div>
                            </a>
                        </div>

                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-xl-4">
                            <a href="CertifiedCopy.aspx">
                                <div class="box m-2">
                                    <div class="left" style="background-color: dodgerblue">
                                        <i class="fa fa-files-o" aria-hidden="true"></i>
                                    </div>
                                    <div class="right">
                                        <div class="new"><b>Certified Copies</b></div>
                                    </div>
                                </div>
                            </a>
                        </div>


                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-xl-4">
                            <a href="ChangeofFees.aspx">
                                <div class="box m-2">
                                    <div class="left" style="background-color: #4c7b56;">
                                        <i class="fa fa-inr " aria-hidden="true"></i>
                                    </div>
                                    <div class="right">
                                        <div class="new"><b>Change of Fees</b></div>
                                    </div>
                                </div>
                            </a>
                        </div>


                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-xl-4">
                            <a href="DisableLogin.aspx">
                                <div class="box m-2">
                                    <div class="left" style="background-color: tomato;">
                                        <i class="fa fa-ban" aria-hidden="true"></i>
                                    </div>
                                    <div class="right">
                                        <div class="new"><b>Disable Login / Society Name</b></div>
                                    </div>
                                </div>
                            </a>
                        </div>


                        
                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-xl-4">
                            <a href="ChangeMobile.aspx">
                                <div class="box m-2">
                                    <div class="left" style="background-color: #E8C872;">
                                        <i class="fa fa-mobile" aria-hidden="true"></i>
                                    </div>
                                    <div class="right">
                                        <div class="new"><b>Change Mobile</b></div>
                                    </div>
                                </div>
                            </a>
                        </div>


                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-xl-4">
                            <a href="ApproveDSC.aspx">
                                <div class="box m-2">
                                    <div class="left" style="background-color: #68e39f;";">
                                        <i class="fa fa-mobile" aria-hidden="true"></i>
                                    </div>
                                    <div class="right">
                                        <div class="new"><b>Approve DSC</b></div>
                                    </div>
                                </div>
                            </a>
                        </div>

                        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6 col-xl-4">
                            <a href="CancelRegistration.aspx">
                                <div class="box m-2">
                                    <div class="left" style="background-color: #ff000c;">
                                        <i class="fa fa-ban" aria-hidden="true"></i>
                                    </div>
                                    <div class="right">
                                        <div class="new"><b>Cancel Last Registration</b></div>
                                    </div>
                                </div>
                            </a>
                        </div>

                        
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
                        <asp:Label ID="Label48" runat="server" ></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="permission" OnClick="permission_Click" CausesValidation="false" CssClass="btn btn-primary" runat="server" Text="Ok" />

                    </div>
                </div>
            </div>
        </div>
    </div>


</asp:Content>
