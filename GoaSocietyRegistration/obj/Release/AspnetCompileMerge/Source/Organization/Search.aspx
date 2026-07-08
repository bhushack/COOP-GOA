<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="GoaSocietyRegistration.Organization.Search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
   
    <link href="../Admin/assets/css/font-awesome.min.css" rel="stylesheet" />
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
            font-weight:bold;
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
        .modal-lg {
    max-width: 850px!important;
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
                                            <th class="tg-0lax">
                                                <label id="labelTOKENID" class="col-sm-12 col-xs-12 ">Token ID</label></th>
                                            <th class="tg-0lax">
                                                <asp:TextBox ID="TxtBxLoginID" CssClass="form-control " ToolTip="Token ID" placeholder="Token ID" MaxLength="50" AutoCompleteType="Disabled" runat="server"></asp:TextBox></th>

                                        </tr>

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
                                                <label id="labeldistrict" class="col-sm-12 col-xs-12">Search by District</label></td>
                                            <td class="tg-0lax">
                                                <asp:DropDownList ID="ddldistrict" CssClass="form-control custom-select mr-sm-2" runat="server"></asp:DropDownList></td>
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

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="searchModal" class="modal">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label49" runat="server" Text="Search Result" ForeColor="White"></asp:Label>
                        </h4>
                         <button type="button" class="close text-white" data-dismiss="modal">&times;</button>
                        
                    </div>

                    <div class="modal-body  table-responsive ">
                        <div class="row">
                            <div class="container">
                                <asp:GridView ID="gdSearch" runat="server" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
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
                                        <asp:TemplateField HeaderText="Edit" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="LbSearchApp_id" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="login_id" HeaderText="Token ID" />
                                        <asp:BoundField DataField="app_id" HeaderText="Application ID" />
                                        <asp:BoundField DataField="socname" HeaderText="Society Name" />
                                        <asp:BoundField DataField="DistrictName" HeaderText="Registered At" />
                                        <asp:BoundField DataField="societytype" HeaderText="Society Type" />
                                        <asp:BoundField DataField="societystatus" HeaderText="Society Status" />
                                        <asp:TemplateField HeaderText="View Applicant">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="LbView" OnClick="LbView_Click" CssClass="btn btn-info" CausesValidation="false" runat="server"><i class="fa fa-search" aria-hidden="true"></i></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <div class="bs-example">
        <div id="myerrorModal" class="modal fade">
            <div class="modal-dialog modal-confirm" id="confirm">
                <div class="modal-content" id="content1">
                    <div class="modal-header" id="header">
                        <div class="icon-box" id="box" runat="server">
                            <div id="sorry" runat="server">
                                <i class="material-icons">&#xE5CD;</i>
                            </div>
                        </div>
                        <p id="hfour" runat="server" class="text-center modal-title">Sorry!</p>
                        <br />
                    </div>
                    <div class="modal-body">
                        <p class="text-center">
                            <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                        </p>
                    </div>
                    <div class="modal-footer" id="footer">
                        <button id="btn1" class="btn btn-danger" data-dismiss="modal">OK</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
