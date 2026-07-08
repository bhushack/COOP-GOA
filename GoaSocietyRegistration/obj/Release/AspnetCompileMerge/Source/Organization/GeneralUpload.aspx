<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="GeneralUpload.aspx.cs" Inherits="GoaSocietyRegistration.Organization.GeneralUpload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons" />
    <link href="../Admin/assets/css/font-awesome.min.css" rel="stylesheet" />
    <style type="text/css">
        .card {
            background-color: #ffffff;
            border: 1px solid rgba(0, 34, 51, 0.1);
            box-shadow: 2px 4px 10px 0 rgba(0, 34, 51, 0.05), 2px 4px 10px 0 rgba(0, 34, 51, 0.05);
            border-radius: 0.15rem;
        }

        .search {
            padding: 8px;
            background-color: #6c53dc !important;
            color: #ffffff;
            font-weight: bold;
        }

        .tg {
            border-collapse: collapse;
            border-spacing: 0;
            width: 80%;
        }

            .tg td {
                border-color: black;
                border-style: solid;
                border-width: 1px;
                font-family: Arial, sans-serif;
                font-size: 16px;
                overflow: hidden;
                padding: 10px 5px;
                word-break: normal;
            }

            .tg th {
                border-color: black;
                border-style: solid;
                border-width: 1px;
                font-family: Arial, sans-serif;
                font-size: 16px;
                font-weight: normal;
                overflow: hidden;
                padding: 10px 5px;
                word-break: normal;
            }

            .tg .tg-0lax {
                text-align: left;
                vertical-align: top;
            }

        .center {
            margin-left: auto;
            margin-right: auto;
        }

        label {
            margin-bottom: 0rem !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row ">
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <div class="col-12">
                <br />
                <div style="border: solid 1px #ddd">
                    <div class="search">Search</div>
                    <div class="card-header tab-card-header ">
                        <ul class="nav nav-tabs card-header-tabs" id="myTab" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link" id="one-tab" data-toggle="tab" href="#one" role="tab" aria-controls="One" aria-selected="true">Search</a>
                            </li>
                        </ul>
                    </div>
                         <div class="tab-content table-responsive" id="myTabContent">
                        <div class="tab-pane fade show active p-3" id="one" role="tabpanel" aria-labelledby="one-tab">
                            <div class="table-responsive-lg">
                                <span class="text-danger">**</span><span class="text-dark">Note:Enter Atleast one from below</span>
                                <table class="tg center">
                                    <tbody>
                                       <tr>
                                            <td class="tg-0lax">
                                                <label id="labelAppID" class="col-sm-12 col-xs-12 ">Application ID</label></td>
                                            <td class="tg-0lax">
                                                <asp:TextBox ID="TxtBxAppID" CssClass="form-control " ToolTip="Application ID" placeholder="Application ID" MaxLength="50" AutoCompleteType="Disabled" runat="server"></asp:TextBox></td>
                                        </tr>

                                        <tr>
                                            <td class="tg-0lax">
                                                <label id="labelSocName" class="col-sm-12 col-xs-12 ">Society Name</label></td>
                                            <td class="tg-0lax">
                                                <asp:TextBox ID="SocietyName" CssClass="form-control " ToolTip="Society Name" placeholder="Society Name" MaxLength="50" AutoCompleteType="Disabled" runat="server"></asp:TextBox></td>
                                        </tr>

                                        <tr>
                                            <td class="tg-0lax">
                                                <label id="labelregid" class="col-sm-12 col-xs-12">Search by Registration ID</label></td>
                                            <td class="tg-0lax">
                                               <asp:TextBox ID="TxtBxRegID" CssClass="form-control " ToolTip="Registration ID" placeholder="Registration ID" MaxLength="50" AutoCompleteType="Disabled" runat="server"></asp:TextBox></td>
                                     
                                        </tr>
                                    </tbody>
                                </table>
                                <%--<asp:Label ID="lberror" runat="server" Text=""></asp:Label>--%>
                                <div class="text-center mt-4">
                                    <asp:LinkButton ID="btnSearch" OnClick="btnSearch_Click" CssClass="btn btn-info" CausesValidation="false" Visible="true" runat="server"><i class="fa fa-search"></i>&nbsp;Search</asp:LinkButton>
                            
                                </div>
                             </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
