<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="SocietyAmendment.aspx.cs" Inherits="GoaSocietyRegistration.Organization.SocietyAmendment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <style>
        .card {
            background-color: #ffffff;
            border: 1px solid rgba(0, 34, 51, 0.1);
            box-shadow: 2px 4px 10px 0 rgba(0, 34, 51, 0.05), 2px 4px 10px 0 rgba(0, 34, 51, 0.05);
            border-radius: 0.15rem;
        }

        .search {
            padding: 8px;
            background-color: #0aa600 !important;
            color: #ffffff;
        }
        .linka
        {
            /*text-decoration:underline;*/
            color:blue;
        }
        .center {
            margin: auto;
        }

        .left {
            float: left;
        }

        .right {
            float: right;
        }
    </style>

     <script type="text/javascript">
        $.expr[":"].containsNoCase = function (el, i, m) {
            var search = m[3];
            if (!search) return false;
            return eval("/" + search + "/i").test($(el).text());
        };

        $(document).ready(function () {
            $('#txtSearch').keyup(function () {
                if ($('#txtSearch').val().length > 1) {
                    $('#ContentPlaceHolder1_GridviewSocietyList tr').hide();
                    $('#ContentPlaceHolder1_GridviewSocietyList tr:first').show();
                    $('#ContentPlaceHolder1_GridviewSocietyList tr td:containsNoCase(\'' + $('#txtSearch').val() + '\')').parent().show();
                }
                else if ($('#txtSearch').val().length == 0) {
                    resetSearchValue();
                }

                if ($('#ContentPlaceHolder1_GridviewSocietyList tr:visible').length == 1) {
                    $('.norecords').remove();
                    $('#ContentPlaceHolder1_GridviewSocietyList').append('<tr class="norecords"><td colspan="6" class="Normal" style="text-align: center">No records were found</td></tr>');
                }
            });

            $('#txtSearch').keyup(function (event) {
                if (event.keyCode == 27) {
                    resetSearchValue();
                }
            });
        });

        function resetSearchValue() {
            $('#txtSearch').val('');
            $('#ContentPlaceHolder1_GridviewAccepted tr').show();
            $('.norecords').remove();
            $('#txtSearch').focus(); location.reload();
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="container-fluid">
        <div class="row ">
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <div class="col-12">
                <br />
                <div style="border: solid 1px #ddd">
                    <div class="search"><b>Amendment</b></div>
                    <div class="card-header tab-card-header ">
                        <ul class="nav nav-tabs card-header-tabs" id="myTab" role="tablist">
                           <li class="nav-item">
                                <a class="nav-link" id="one-tab" data-toggle="tab" href="#one" role="tab" aria-controls="One" aria-selected="true">New Applications (<asp:Label ID="Label1" runat="server" Text=""></asp:Label>)</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" id="two-tab" data-toggle="tab" href="#two" role="tab" aria-controls="Two" aria-selected="false">Applications Submitted after observation (<asp:Label ID="Label2" runat="server" Text=""></asp:Label>)</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" id="three-tab" data-toggle="tab" href="#three" role="tab" aria-controls="Three" aria-selected="false">Accepted (<asp:Label ID="Label3" runat="server" Text=""></asp:Label>)</a>
                            </li>
                        </ul>
                       
                    </div>
                    <div class="tab-content table-responsive" id="myTabContent">
                        <%--<div class="form-inline">
                                <div class="form-group mb-2" style="margin-top:27px">
                                    <label for="staticEmail2" class="sr-only">Application No</label>
                                    <input type="text" readonly class="form-control-plaintext" id="staticEmail2" value="Application No"/>
                                </div>
                                <div class="form-group mx-sm-3 mb-2" style="margin-top:27px">
                                    <asp:HiddenField ID="hdAppID" runat="server" />
                                    <asp:TextBox ID="TxtBxAppID" CssClass="form-control " ToolTip="Application ID" placeholder="Application No" MaxLength="20" autocomplete="off" runat="server"></asp:TextBox>
                                </div>
                                <asp:LinkButton ID="btnSearch" CssClass="btn btn-info" CausesValidation="false" Visible="true" runat="server" style="margin-top:20px;margin-left:30px;"><i class="fa fa-search"></i>&nbsp;Search</asp:LinkButton>  
                            </div>--%>

                       
                        <div class="input-group input-group-sm p-3" style="margin-top:15px;">
                                <div class="input-group-prepend">
                                    <span class="input-group-text" id="basic-addon1">Find</span>
                                </div>
                                <input type="text" class="form-control" id="txtSearch" name="txtSearch" placeholder="Find Match" autocomplete="off" maxlength="50"/>
                            </div>

                        <div class="tab-pane fade show active p-3" id="one" role="tabpanel" aria-labelledby="one-tab">
                            

                            <div id="newamendappl" runat="server" visible="true">
                              
                                <div class="table-responsive">
                                <asp:GridView ID="gvamend" runat="server" OnRowDataBound="gvamend_RowDataBound" CellPadding="5" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
                                <AlternatingRowStyle BackColor="White" />
                                <FooterStyle BackColor="#CCCC99" />
                                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                <RowStyle BackColor="#f1eac8" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                <SortedAscendingHeaderStyle BackColor="#848384" />
                                <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                <SortedDescendingHeaderStyle BackColor="#575357" />
                                <Columns>   
                                                           
                                    <%--<asp:BoundField HeaderText="Application ID" DataField="app_id" />--%>
                                    <asp:BoundField HeaderText="Application ID" DataField="app_id" />
                                    <asp:BoundField HeaderText="Registration ID" DataField="socregid" />
                                    <asp:BoundField HeaderText="Society Name" DataField="socname" /> 
                                    <%--<asp:BoundField HeaderText="Submission Date" DataField="regdate" DataFormatString="{0:dd/MM/yyyy}" />--%>
                                    <asp:BoundField HeaderText="Submission Time" DataField="amend_submittime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                    <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LbApp_id" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                           <%-- <asp:Label ID="LbAmendStatus" runat="server" Text='<%# Eval("amend_status") %>'></asp:Label>
                                            <asp:Label ID="LbAmendSubmittime" runat="server" Text='<%# Eval("amendsubmittime") %>'></asp:Label>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lkviewapplicant" OnClick="lkviewapplicant_Click" CausesValidation="false"  CssClass="linka text-center" runat="server"><i class="fa fa-arrow-circle-right" style="font-size:20px;"></i>&nbsp;</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>       
                                        
                                        
                                </Columns>
                                </asp:GridView>
                                </div>
                                <br />
                            </div>


                        </div>

                        <div class="tab-pane fade p-3" id="two" role="tabpanel" aria-labelledby="two-tab">
                            <asp:GridView ID="gvamendobservation" runat="server" CellPadding="5" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
                                 <AlternatingRowStyle BackColor="White" />
                                <FooterStyle BackColor="#CCCC99" />
                                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                <RowStyle BackColor="#f1eac8" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                <SortedAscendingHeaderStyle BackColor="#848384" />
                                <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                <SortedDescendingHeaderStyle BackColor="#575357" />
                                <Columns>   
                                                           
                                    <%--<asp:BoundField HeaderText="Application ID" DataField="app_id" />--%>
                                    <asp:BoundField HeaderText="Application ID" DataField="app_id" />
                                    <asp:BoundField HeaderText="Registration ID" DataField="socregid" />
                                    <asp:BoundField HeaderText="Society Name" DataField="socname" />                                    
                                    <asp:BoundField HeaderText="Submission Time" DataField="amend_obssubmittime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                     <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LbApp_id" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                           <%-- <asp:Label ID="LbAmendStatus" runat="server" Text='<%# Eval("amend_status") %>'></asp:Label>
                                            <asp:Label ID="LbAmendSubmittime" runat="server" Text='<%# Eval("amendsubmittime") %>'></asp:Label>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lkviewobsapplicant" OnClick="lkviewobsapplicant_Click" CausesValidation="false"  CssClass="linka text-center" runat="server"><i class="fa fa-arrow-circle-right" style="font-size:20px;"></i>&nbsp;</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>       
                                        
                                        
                                </Columns>
                            </asp:GridView>
                        </div>

                        <div class="tab-pane fade p-3" id="three" role="tabpanel" aria-labelledby="three-tab">
                           
                            <div id="acceptedamendappl" runat="server" visible="true">
                               <div class="table-responsive">
                                    <asp:GridView ID="gvamendaccepted" runat="server" CellPadding="5" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
                                <AlternatingRowStyle BackColor="White" />
                                <FooterStyle BackColor="#CCCC99" />
                                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                <RowStyle BackColor="#f1eac8" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                <SortedAscendingHeaderStyle BackColor="#848384" />
                                <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                <SortedDescendingHeaderStyle BackColor="#575357" />
                                <Columns>   
                                                           
                                    <%--<asp:BoundField HeaderText="Application ID" DataField="app_id" />--%>
                                    <asp:BoundField HeaderText="Application ID" DataField="app_id" />                                    
                                    <asp:BoundField HeaderText="Society Name" DataField="socname" /> 
                                    <asp:BoundField HeaderText="Registration ID" DataField="socregid" />
                                    <asp:BoundField HeaderText="Registration Date" DataField="regdate" DataFormatString="{0:dd/MM/yyyy}" />
                                     <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LbApp_id" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                           <%-- <asp:Label ID="LbAmendStatus" runat="server" Text='<%# Eval("amend_status") %>'></asp:Label>
                                            <asp:Label ID="LbAmendSubmittime" runat="server" Text='<%# Eval("amendsubmittime") %>'></asp:Label>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lkviewacceptedapplicant" OnClick="lkviewacceptedapplicant_Click" CausesValidation="false"  CssClass="linka" runat="server"><i class="fa fa-arrow-circle-right" style="font-size:20px;"></i>&nbsp;</asp:LinkButton>
                                        
                                        </ItemTemplate>
                                    </asp:TemplateField>       
                                        
                                        
                                </Columns>
                                </asp:GridView>
                                </div>
                                <br />
                            </div>


                        </div>
                    </div>


                </div>
            </div>
        </div>
    </div>
</asp:Content>
