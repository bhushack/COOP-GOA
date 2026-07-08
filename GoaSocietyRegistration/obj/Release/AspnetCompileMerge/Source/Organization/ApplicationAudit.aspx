<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="ApplicationAudit.aspx.cs" Inherits="GoaSocietyRegistration.Organization.ApplicationAudit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <script src="assets/js/jquery.js"></script>
    <link href="../Admin/datepicker/jquery-ui.css" rel="stylesheet" />
    <script src="../Admin/datepicker/jquery-1.10.2.js"></script>
    <script src="../Admin/datepicker/jquery-ui.js"></script>
    <script type="text/javascript">
        $(function () {
            var tabName = $("[id*=TabName]").val() != "" ? $("[id*=TabName]").val() : "one";
            $('#myTab a[href="#' + tabName + '"]').tab('show');
            $("#myTab a").click(function () {
                $("[id*=TabName]").val($(this).attr("href").replace("#", ""));
            });
        });
    </script>
    <style>
        .society_list {
            background-color: #6c53dc !important;
            color: #ffffff;
            padding: 8px;
        }

        .abc {
            height: 38px !important;
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
        jQuery(document).ready(function ($) {
            $("[id*=TxtBxDate]").datepicker({
                maxDate: 0,
                showAnim: "",
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                onSelect: function () {
                    $('#datepicker').val($(this).datepicker({
                        dateFormat: 'dd/mm/yy'
                    }).val());
                }
            });
        });

        jQuery(document).ready(function ($) {
            $("[id*=TextBox1]").datepicker({
                maxDate: 0,
                showAnim: "",
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                onSelect: function () {
                    $('#datepicker').val($(this).datepicker({
                        dateFormat: 'dd/mm/yy'
                    }).val());
                }
            });
        });


    </script>

    <style type="text/css">
        .cssPager td {
            color: #0796fb;
            font-size: 18px;
            /*color:#fff;*/
            padding-left: 4px;
            padding-right: 4px;
        }

        .nav-tabs {
            border-bottom: 1px solid #ddd;
        }

            .nav-tabs > li {
                float: left;
                margin-bottom: -1px;
            }

                .nav-tabs > li > a {
                    margin-right: 2px;
                    line-height: 1.42857143;
                    border: 1px solid transparent;
                    border-radius: 4px 4px 0 0;
                }

                    .nav-tabs > li > a:hover {
                        border-color: #eee #eee #ddd;
                    }

                .nav-tabs > li.active > a, .nav-tabs > li.active > a:focus, .nav-tabs > li.active > a:hover {
                    color: #555;
                    cursor: default;
                    background-color: #fff;
                    border: 1px solid #ddd;
                    border-bottom-color: transparent;
                }

            .nav-tabs.nav-justified {
                width: 100%;
                border-bottom: 0;
            }

                .nav-tabs.nav-justified > li {
                    float: none;
                }

                    .nav-tabs.nav-justified > li > a {
                        margin-bottom: 5px;
                        text-align: center;
                    }

                .nav-tabs.nav-justified > .dropdown .dropdown-menu {
                    top: auto;
                    left: auto;
                }

        @media (min-width:768px) {
            .nav-tabs.nav-justified > li {
                display: table-cell;
                width: 1%;
            }

                .nav-tabs.nav-justified > li > a {
                    margin-bottom: 0;
                }
        }

        .nav-tabs.nav-justified > li > a {
            margin-right: 0;
            border-radius: 4px;
        }

        .nav-tabs.nav-justified > .active > a, .nav-tabs.nav-justified > .active > a:focus, .nav-tabs.nav-justified > .active > a:hover {
            border: 1px solid #ddd;
        }

        @media (min-width:768px) {
            .nav-tabs.nav-justified > li > a {
                border-bottom: 1px solid #ddd;
                border-radius: 4px 4px 0 0;
            }

            .nav-tabs.nav-justified > .active > a, .nav-tabs.nav-justified > .active > a:focus, .nav-tabs.nav-justified > .active > a:hover {
                border-bottom-color: #fff;
            }
        }

        .nav-pills > li {
            float: left;
        }

            .nav-pills > li > a {
                border-radius: 4px;
            }

            .nav-pills > li + li {
                margin-left: 2px;
            }

            .nav-pills > li.active > a, .nav-pills > li.active > a:focus, .nav-pills > li.active > a:hover {
                color: #fff;
                background-color: #337ab7;
            }

        .nav-stacked > li {
            float: none;
        }

            .nav-stacked > li + li {
                margin-top: 2px;
                margin-left: 0;
            }

        .nav-justified {
            width: 100%;
        }

            .nav-justified > li {
                float: none;
            }

                .nav-justified > li > a {
                    margin-bottom: 5px;
                    text-align: center;
                }

            .nav-justified > .dropdown .dropdown-menu {
                top: auto;
                left: auto;
            }

        @media (min-width:768px) {
            .nav-justified > li {
                display: table-cell;
                width: 1%;
            }

                .nav-justified > li > a {
                    margin-bottom: 0;
                }
        }

        .nav-tabs-justified {
            border-bottom: 0;
        }

            .nav-tabs-justified > li > a {
                margin-right: 0;
                border-radius: 4px;
            }

            .nav-tabs-justified > .active > a, .nav-tabs-justified > .active > a:focus, .nav-tabs-justified > .active > a:hover {
                border: 1px solid #ddd;
            }

        @media (min-width:768px) {
            .nav-tabs-justified > li > a {
                border-bottom: 1px solid #ddd;
                border-radius: 4px 4px 0 0;
            }

            .nav-tabs-justified > .active > a, .nav-tabs-justified > .active > a:focus, .nav-tabs-justified > .active > a:hover {
                border-bottom-color: #fff;
            }
        }

        .tab-content > .tab-pane {
            display: none;
        }

        .tab-content > .active {
            display: block;
        }

        .nav-tabs .dropdown-menu {
            margin-top: -1px;
            border-top-left-radius: 0;
            border-top-right-radius: 0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row">
            <div class="col-12">
                <div class="society_list">Audit Logs</div>
                <div style="border: solid 1px #ddd">
                    <br />
                    <ul class="nav nav-tabs">
                        <li class="active"><a data-toggle="tab" href="#home">Audit Logs</a></li>
                        <li><a data-toggle="tab" href="#menu1">Log In & Out</a></li>
                    </ul>
                    <br />
                    <div class="tab-content">
                        <div id="home" class="tab-pane fade in active show">
                            <div class="container">
                                <div class="row">
                                    <div class="col-12 form-row">
                                        <div class="form-group col-md-4 col-xs-12">
                                            <asp:Label ID="Label10" runat="server" Text="">Date<span class="text-danger">*</span></asp:Label>
                                            <asp:HiddenField ID="HiddenField1" runat="server" />
                                            <asp:TextBox ID="TxtBxDate" placeholder="Date" CssClass="abc form-control" AutoCompleteType="Disabled" runat="server"></asp:TextBox>
                                        </div>
                                        <div class="form-group col-md-4 col-xs-12">
                                            <asp:Label ID="Label1" runat="server" Text="User Id"></asp:Label>
                                            <asp:DropDownList ID="ddlrole" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>


                                        <div class="form-group col-md-4 col-xs-12">
                                            <asp:Label ID="Label2" runat="server"></asp:Label>
                                            <asp:LinkButton ID="seach_adudit_logs" OnClick="seach_adudit_logs_Click" runat="server"><i class="fa fa-search" style="font-size:24px; margin-top: 30px; "></i></asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <asp:GridView runat="server" ID="GridviewAuditLogs" OnRowDataBound="GridviewAuditLogs_RowDataBound" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" OnPageIndexChanging="GridviewAuditLogs_PageIndexChanging" AllowPaging="true" PageSize="10" PagerStyle-CssClass="pgr" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%">
                                <AlternatingRowStyle BackColor="White" />
                                <FooterStyle BackColor="#CCCC99" />
                                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                <PagerStyle CssClass="cssPager" BackColor="#d0d8dc" ForeColor="Black" HorizontalAlign="Right" />
                                <%--#c8caf1    #919fa7--%>
                                <PagerSettings Mode="Numeric" PageButtonCount="5" LastPageText="Last" />
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
                                    <asp:BoundField HeaderText="DateTime" DataField="tracked_datetime" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" />
                                    <asp:BoundField HeaderText="UserLoginId" DataField="admin_login_id" />
                                    <asp:BoundField HeaderText="Action on App_Id" DataField="app_id" />
                                    <asp:BoundField HeaderText="Action Performed" DataField="action_performed" />
                                    <asp:BoundField HeaderText="Action Description" DataField="action_description" />
                                    <asp:BoundField HeaderText="Accessed Module" DataField="accessed_module" />
                                    <asp:BoundField HeaderText="Action Status" DataField="action_status" />
                                    <asp:BoundField HeaderText="IP Address" DataField="ipaddress" />
                                </Columns>
                            </asp:GridView>

                            <asp:HiddenField ID="TabName" runat="server" />
                            <div class="form-group row p-3">
                                <%--<div class="col-sm-3 col-xs-12">
                                <asp:LinkButton ID="export_logs_pdf"  CausesValidation="false" CssClass="btn btn-info" runat="server"><i class="fa fa-file-pdf-o"></i>&nbsp;Export to PDF</asp:LinkButton>
                            </div>--%>
                                <div class="col-sm-3 col-xs-12">
                                    <asp:LinkButton ID="export_logs_excel" OnClick="export_logs_excel_Click" CausesValidation="false" CssClass="btn btn-warning" runat="server"><i class="fa fa-file-excel-o"></i>&nbsp;Export to Excel</asp:LinkButton>
                                </div>
                            </div>
                        </div>
                        <div id="menu1" class="tab-pane fade">
                            <div class="container">
                                <div class="row">
                                    <div class="col-12 form-row">
                                        <div class="form-group col-md-4 col-xs-12">
                                            <asp:Label ID="Label3" runat="server" Text="">Date<span class="text-danger">*</span></asp:Label>
                                            <asp:HiddenField ID="HiddenField2" runat="server" />
                                            <asp:TextBox ID="TextBox1" placeholder="Date" CssClass="abc form-control" AutoCompleteType="Disabled" runat="server"></asp:TextBox>
                                        </div>
                                        <div class="form-group col-md-4 col-xs-12">
                                            <asp:Label ID="Label4" runat="server" Text="User Id"></asp:Label>
                                            <asp:DropDownList ID="DropDownList1" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>


                                        <div class="form-group col-md-4 col-xs-12">
                                            <asp:Label ID="Label6" runat="server"></asp:Label>
                                            <asp:LinkButton ID="LinkButton1" OnClick="LinkButton1_Click" runat="server"><i class="fa fa-search" style="font-size:24px; margin-top: 30px; "></i></asp:LinkButton>
                                        </div>
                                    </div>
                                    <br />
                                    <asp:GridView runat="server" ID="Gridview1" OnRowDataBound="Gridview1_RowDataBound" OnPageIndexChanging="Gridview1_PageIndexChanging" CssClass="Grid" AutoGenerateColumns="false" CellPadding="5" AllowPaging="true" PageSize="10" PagerStyle-CssClass="pgr" EmptyDataText="No Record Found" ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%">
                                        <AlternatingRowStyle BackColor="White" />
                                        <FooterStyle BackColor="#CCCC99" />
                                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle CssClass="cssPager" BackColor="#d0d8dc" ForeColor="Black" HorizontalAlign="Right" />
                                        <%--#c8caf1    #919fa7--%>
                                        <PagerSettings Mode="Numeric" PageButtonCount="5" LastPageText="Last" />
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
                                            <asp:BoundField HeaderText="User Login Id" DataField="user_loginname" />
                                            <asp:BoundField HeaderText="Name" DataField="user_fullname" />
                                            <asp:BoundField HeaderText="Login" DataField="user_logintime" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" />
                                            <asp:BoundField HeaderText="Logout" DataField="user_logouttime" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" />
                                            <asp:BoundField HeaderText="IP Address" DataField="ipaddress" />
                                        </Columns>

                                    </asp:GridView>
                                </div>

                                <div class="form-group row p-3">
                                    <%--<div class="col-sm-3 col-xs-12">
                                <asp:LinkButton ID="export_logs_pdf"  CausesValidation="false" CssClass="btn btn-info" runat="server"><i class="fa fa-file-pdf-o"></i>&nbsp;Export to PDF</asp:LinkButton>
                            </div>--%>
                                    <div class="col-sm-3 col-xs-12">
                                        <asp:LinkButton ID="LinkButton2" OnClick="LinkButton2_Click" CausesValidation="false" CssClass="btn btn-warning" runat="server"><i class="fa fa-file-excel-o"></i>&nbsp;Export to Excel</asp:LinkButton>
                                    </div>
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
        <div id="permission_error_modal" class="modal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label19" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label5" runat="server" ForeColor="White"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="permission" OnClick="permission_Click" CssClass="btn btn-primary" runat="server" Text="Ok" />
                        <%-- <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
