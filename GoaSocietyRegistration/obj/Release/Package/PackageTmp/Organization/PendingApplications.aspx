<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="PendingApplications.aspx.cs" Inherits="GoaSocietyRegistration.Organization.PendingApplications" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
     <script src="../Scripts/jquery-3.5.0.min.js"></script>
    <link href="../Admin/datepicker/jquery-ui.css" rel="stylesheet" />
     <script src="../Admin/datepicker/jquery-1.10.2.js"></script>
  <script src="../Admin/datepicker/jquery-ui.js"></script>
   


     <style>
        .slot {
            background-color: #75751a !important;
            color: #ffffff;
            padding: 8px;
        }

        .bs-example {
            margin: 20px;
        }

        .search {
            padding: 8px;
            background-color: #6c53dc  !important;
            color: #ffffff;
           
        }

        .left {
            float: left;
        }

        @media print {
            .pgBrk {
                page-break-inside: avoid;
                overflow: visible;
            }
        }

        #statusprint {
            margin-top: 20px;
            margin-bottom: 20px;
            margin-left: 50%;
        }

        #lbFromDate {
            margin-top: 10px;
            padding-left: 15px;
        }

        #LbTodate {
            margin-top: 10px;
            padding-left: 15px;
        }

        #labelDistrict{
            margin-top: 10px;
            padding-left: 15px;
        }

       
        #print_table{

            padding:15px;
        }

        .declaration {
            background-color: #75751a !important;
            color: #ffffff;
            padding: 8px;
        }

        td {
            border: 1px solid red;
            /*text-align: center;*/
            border-width: 2px;;
           
        }


        th {
            border-left-style: solid;
            border-right-style: solid;
            border-width: 2px;;
            text-align: center;
          
        }

        
    </style>
    <style type="text/css" media="print">
        @page {
            size: landscape;
        }
    </style>
   <%-- <script type="text/javascript">
        $.expr[":"].containsNoCase = function (el, i, m) {
            var search = m[3];
            if (!search) return false;
            return eval("/" + search + "/i").test($(el).text());
        };

        $(document).ready(function () {
            $('#txtSearch').keyup(function () {
                if ($('#txtSearch').val().length > 1) {
                    $('#ContentPlaceHolder1_dgadvancesearch tr').hide();
                    $('#ContentPlaceHolder1_dgadvancesearch tr:first').show();
                    $('#ContentPlaceHolder1_dgadvancesearch tr td:containsNoCase(\'' + $('#txtSearch').val() + '\')').parent().show();
                }
                else if ($('#txtSearch').val().length == 0) {
                    resetSearchValue();
                }

                if ($('#ContentPlaceHolder1_dgadvancesearch tr:visible').length == 1) {
                    $('.norecords').remove();
                    $('#ContentPlaceHolder1_dgadvancesearch').append('<tr class="norecords"><td colspan="6" class="Normal" style="text-align: center">No records were found</td></tr>');
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
            $('#ContentPlaceHolder1_dgadvancesearch tr').show();
            $('.norecords').remove();
            $('#txtSearch').focus(); location.reload();
        }

    </script>--%>
    <script>
        function printContent(el) {
            var restorepage = $('body').html();
            var printcontent = $('#' + el).clone();
            $('body').empty().html(printcontent);
            window.print();
            $('body').html(restorepage);
        }
    </script>
  

    <script type="text/javascript">
          jQuery(document).ready(function ($) {
              $("[id*=TxtBxFromDate]").datepicker({
                maxDate: 0,
                showAnim: "",
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                onSelect: function () {
                    $('#datepicker').val($(this).datepicker({
                        dateFormat: 'dd/mm/yyyy'
                    }).val());
                }
            });
        });
    </script>

    <script type="text/javascript">
          jQuery(document).ready(function ($) {
              $("[id*=TxtBxToDate]").datepicker({
                maxDate: 0,
                showAnim: "",
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                onSelect: function () {
                    $('#datepicker').val($(this).datepicker({
                        dateFormat: 'dd/mm/yyyy'
                    }).val());
                }
            });
        });
    </script>
    <script type="text/javascript">


        function CoverClickLK(val) {

            if (val == "SearchButton") {
                document.getElementById("<%=SearchButton.ClientID %>").style.display = "none";

            }
           
            CoverClick1();
        }
    </script>

   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div class="container-fluid">

        <div class="row ">
            
            <div class="col-12">
                <br />
                <asp:HiddenField ID="HiddenField1" runat="server" />
                <div style="border: solid 1px #ddd">
                    <div class="search"><b>Advance Search</b></div>

                    <div class="card-header tab-card-header ">
                        <ul class="nav nav-tabs card-header-tabs" id="myTab" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link" id="one-tab" data-toggle="tab" href="#one" role="tab" aria-controls="One" aria-selected="true">Pending Application at Office</a>
                            </li>
                        </ul>
                    </div>
                    <div class="tab-content table-responsive" id="myTabContent">
                        <div class="tab-pane fade show active p-3" id="one" role="tabpanel" aria-labelledby="one-tab">
                            <div class="table-responsive-lg">


                                <div class="form-group row">
                                   
                                    <div class="col-xl-4 col-lg-6 col-md-6 col-sm-12 col-xs-12 row">
                                        <label id="lbFromDate" class="form-group">From Date:</label>
                                        <asp:TextBox ID="TxtBxFromDate" CssClass="form-control form-group" ToolTip="From Date" Text="01/01/2021" placeholder="From Date" autocomplete="off" runat="server" style="width:60%; margin-left:5px;"></asp:TextBox>
                                        <asp:HiddenField ID="HiddenField2" runat="server" />
                                    </div>
                                   
                                    <div class="col-xl-4 col-lg-6 col-md-6 col-sm-12 col-xs-12 row">
                                         <label id="LbTodate" class="form-group">To Date:</label>
                                        <asp:TextBox ID="TxtBxToDate" CssClass="form-control form-group" ToolTip="To Date" placeholder="To Date" autocomplete="off" runat="server"  style="width:60%; margin-left:5px;"></asp:TextBox>
                                    </div>
                                    <div class="col-xl-4 col-lg-6 col-md-6 col-sm-12 col-xs-12 row">
                                          <label id="labelDistrict" class="form-group">District:</label>
                                         <asp:DropDownList ID="ddlDistrict" CssClass="form-control custom-select mr-sm-2" runat="server" style="width:60%; margin-left:5px;"></asp:DropDownList>
                                    </div>
                                </div>
                              
                                <asp:Label ID="lberror" runat="server" Text=""></asp:Label>
                                <div class="form-group row">
                                    <div class="col-sm-12 col-xs-12 text-center">
                                        <asp:LinkButton ID="SearchButton" OnClick="SearchButton_Click" OnClientClick="CoverClickLK('SearchButton');" CausesValidation="false" CssClass="btn btn-dark" runat="server"><i class="fa fa-filter"></i>&nbsp;Search</asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <%--     <div class="input-group input-group-sm mb-3">
                            <div class="input-group-prepend">
                                <span class="input-group-text" id="basic-addon1">Application ID</span>
                            </div>
                            <input type="text" class="form-control" id="txtSearch" name="txtSearch" placeholder="Search" maxlength="50">
                        </div>--%>

                                <div class="table-responsive" id="print_table" runat="server" visible="false" style="padding:15px;">
                                  
                                    <div runat="server" id="Renewallabel" visible="true" class="text-center" style="background-color:#94949466; margin-bottom:10px;">
                                        <asp:Label ID="Label11" runat="server" Font-Bold="true" ForeColor="#0410a7" Font-Size="Large" Text="New Applications" ></asp:Label>
                                     </div>
                                    <asp:GridView ID="gvnewappln" runat="server" OnRowDataBound="gvnewappln_RowDataBound" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
                                        <AlternatingRowStyle BackColor="White" />
                                        <FooterStyle BackColor="#CCCC99" />
                                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                        <RowStyle BackColor="#eef1c8" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                        <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                        <SortedAscendingHeaderStyle BackColor="#848384" />
                                        <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                        <SortedDescendingHeaderStyle BackColor="#575357" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Sr. No">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="app_id" HeaderText="Application ID" />
                                            <asp:BoundField DataField="login_id" HeaderText="Token ID" />
                                            <asp:BoundField DataField="socname" HeaderText="Society Name" />   
                                            <asp:BoundField DataField="new_or_renewal" HeaderText="New or Renewal" />
                                            <asp:BoundField DataField="status_id" HeaderText="Application at" />
                                            <%--<asp:BoundField HeaderText="Application Type" /> --%>                                                                               
                                            <asp:BoundField DataField="application_submission_time" HeaderText="Submission Date" />
                                        </Columns>
                                    </asp:GridView>
                                    <br />
                                    <div runat="server" id="Div1" visible="true" class="text-center" style="background-color:#94949466; margin-bottom:10px;">
                                        <asp:Label ID="Label3" runat="server" Font-Bold="true" ForeColor="#0410a7" Font-Size="Large" Text="Submitted After Observation" ></asp:Label>
                                     </div>
                                     <asp:GridView ID="gvobsappln" OnRowDataBound="gvobsappln_RowDataBound" runat="server" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
                                        <AlternatingRowStyle BackColor="White" />
                                        <FooterStyle BackColor="#CCCC99" />
                                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />
                                        <RowStyle BackColor="#eef1c8" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                        <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                        <SortedAscendingHeaderStyle BackColor="#848384" />
                                        <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                        <SortedDescendingHeaderStyle BackColor="#575357" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Sr. No">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="app_id" HeaderText="Application ID" />
                                            <asp:BoundField DataField="login_id" HeaderText="Token ID" />
                                            <asp:BoundField DataField="socname" HeaderText="Society Name" />   
                                            <asp:BoundField DataField="new_or_renewal" HeaderText="New or Renewal" />
                                            <asp:BoundField DataField="status_id" HeaderText="Application at" />
                                            <%--<asp:BoundField HeaderText="Application Type" /> --%>                                                                               
                                            <asp:BoundField DataField="application_obs_submission_time" HeaderText="Submission Date" />
                                            <asp:TemplateField HeaderText="View History" >
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="LkViewDetails"  OnClick="LkViewDetails_Click" CssClass="btn" CausesValidation="false" runat="server" Font-Size="20px"><i class="fa fa-arrow-circle-right" aria-hidden="true"></i></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField> 
                                            <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="LkobsAppID" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>

                                    
                                </div>

                                 <div id="printbtn" runat="server" visible="false" style="margin-left:auto; margin-right:auto; margin-top:10px;">
                                       
                                          <asp:LinkButton ID="LinkButton1"   OnClientClick="printContent('ContentPlaceHolder1_print_table');"  CssClass="btn-success btn-lg " runat="server"><i class="fa fa-print" aria-hidden="true"></i>&nbsp;Print</asp:LinkButton>
                                         
                                 </div>

                               
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
                 
    </div>


    <%--<div id="cover" style="display: none;"></div>
    <div class="row">
        <div id="CoverDoubleClick" class="opac_divLoader overlayLoader" style="display: none;">
            <asp:Image ID="wait" runat="server" ImageUrl="../assets/Images/Loader.gif" AlternateText="w a i t"
                Height="100%" Style="vertical-align: middle;" />
        </div>
    </div>--%>
    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="permission_error_modal" class="modal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label25" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label26" runat="server"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="permission" OnClick="permission_Click" CausesValidation="false" CssClass="btn btn-primary" runat="server" Text="Ok" />

                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="myerrorModal" class="modal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label1" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button id="btn1" class="btn btn-danger" data-dismiss="modal">OK</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

     <div id="remarkshistory" class="modal fade" role="dialog">
            <div class="modal-dialog modal-xl mymodal1 modal-lg">

                <!-- Modal content-->
                <div class="modal-content ">
                    <div class="modal-header">
                        <b>Observation History</b>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div>                               

                                 <div class="table-responsive p-1">
                                      <div class="panel-heading" style="background-color: grey; padding: 5px 10px;">
                                    <div class="panel-title" style="color: white;">Submit Time</div>
                                </div>
                                <br />
                                    <asp:GridView ID="gvobssubmittime" runat="server" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="80%" style="margin-left:auto;margin-right:auto;">
                                        <HeaderStyle BackColor="#c8caf1" Font-Bold="True" /> 
                                        <RowStyle HorizontalAlign="Center" />
                                        <Columns>
                                           <%-- <asp:TemplateField HeaderText="Edit" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbAppID" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                            <asp:BoundField DataField="submitted_at" HeaderText="Application Submitted by User at" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                      
                               
                                <div class="table-responsive p-1">
                                     <div class="panel-heading" style="background-color: grey; margin-top: 15px; padding: 5px 10px;">
                                        <div class="panel-title" style="color: white;">Application Observation History</div>
                                    </div>
                                    <br />
                                    <asp:GridView ID="gvobsremarks" runat="server" OnRowDataBound="gvobsremarks_RowDataBound" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" Width="100%">
                                        <HeaderStyle BackColor="#c8caf1" Font-Bold="True" /> 
                                        <RowStyle HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Edit" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbAppID_gvhistory" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                         
                                            <asp:BoundField DataField="observation_by_dh" HeaderText=" Remarks by the Dealing Hand  " ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="remarks_sendobservation" HeaderText=" Observation by District Registrar " />
                                            <asp:BoundField DataField="submit_time_remarkssendobservation" HeaderText="Observation Made by DRO at" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                                             
                               
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>

    <div class="bs-example">
            <!-- Modal HTML -->
            <div id="MyErrorModal" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label66" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label69" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

</asp:Content>
