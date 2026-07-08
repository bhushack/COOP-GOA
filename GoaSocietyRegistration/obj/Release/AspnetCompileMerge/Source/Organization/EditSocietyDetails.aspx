<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="EditSocietyDetails.aspx.cs" Inherits="GoaSocietyRegistration.Organization.EditSocietyDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://fonts.googleapis.com/icon?family=Material+Icons" />
  
  <script src="../Scripts/jquery-3.5.0.min.js"></script>
    <link href="../Admin/datepicker/jquery-ui.css" rel="stylesheet" />
   <script src="../Admin/datepicker/jquery-1.10.2.js"></script>
  <script src="../Admin/datepicker/jquery-ui.js"></script>

   <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />


    <style>
         .bs-example {
            margin: 20px;
        }

         .societydetails {
            background-color: #6c53dc !important;
            color: #ffffff;
            padding: 8px;
            font-weight:bold;
        }
         
         .ddl
         {
            margin-left:25px;
            color:#20824c;
            width:70px;
            font-weight:bold; 
            border-style:groove;
         }

    </style>

     <style type="text/css">   
              .cssPager td  
              { 
                  color:#0796fb; 
                  font-size:18px;
                  /*color:#fff;*/
                   padding-left: 4px;     
                  padding-right: 4px;    
              }    
        </style>

    
     <script type="text/javascript">   
        function CoverClickLK(val) {          
            if (val == "search_society") {
                document.getElementById("<%=search_society.ClientID %>").style.display = "none";
            }
           
            CoverClick1();
        }
    </script>

    <script type="text/javascript">
          jQuery(document).ready(function ($) {
              $("[id*=TxtBxRegDate]").datepicker({
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
              $("[id*=TxtBxSearchRegDate]").datepicker({
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



    <%--  <script type="text/javascript">
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

    </script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    
    <div class="container-fluid">
        <div class="row">
             <asp:HiddenField ID="HiddenField1" runat="server" />
            <div class="col-12">                
                <div class="societydetails">Society Details</div>
                <div style="border: solid 1px #ddd">
                    <div class="card-header tab-card-header">
                        <ul class="nav nav-tabs card-header-tabs" id="myTab" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link" id="one-tab" data-toggle="tab" href="#one" role="tab" aria-controls="One" aria-selected="true">Edit Society (<asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>) </a>
                            </li>
                        </ul>                       
                    </div>



                    <div class="tab-content table-responsive text-center" id="myTabContent">
                        <div class="tab-pane fade show active p-3" id="one" role="tabpanel" aria-labelledby="one-tab">

                            

                        <div class="row">
                            <div id="div_ddlyear" runat="server" class="col-sm-12 col-xs-12 form-group row " style="margin-left:5px;" >
                               <asp:Label ID="Label7" runat="server" Text="Displaying Records of the Year :" Font-Bold="true" ForeColor="#20824c"></asp:Label> 
                                <asp:DropDownList ID="ddlyear" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlyear_SelectedIndexChanged" CssClass="ddl">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="Rdbtn_search" ErrorMessage="Please Select an Option"> </asp:RequiredFieldValidator>
                                                         
                                
                            </div>
                            
                            <div class="col-sm-12 col-xs-12 form-group row " style="margin-left:5px;" >
                               <asp:Label ID="Label1" runat="server" Text="Search:" Font-Bold="true"></asp:Label>                                   
                                <div class="col-md-9">
                                   <asp:RadioButtonList RepeatDirection="Horizontal" ID="Rdbtn_search" Width="100%" OnSelectedIndexChanged="Rdbtn_search_SelectedIndexChanged" AutoPostBack="true" runat="server">
                                        <asp:ListItem Value="1" Selected="True">&nbsp All Records</asp:ListItem>
                                        <asp:ListItem Value="2">&nbsp By Registration No.</asp:ListItem>
                                        <asp:ListItem Value="3">&nbsp By Name</asp:ListItem>
                                       
                                    </asp:RadioButtonList>     
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="Rdbtn_search" ErrorMessage="Please Select an Option"> </asp:RequiredFieldValidator>
                                                         
                                </div>
                            </div>

                             <div class="col-sm-12 col-xs-12 form-group row form-inline" style="margin-left:5px;">
                               <div id="searchregno" runat="server" visible="false">
                                    <asp:Label ID="Label5" runat="server" Text="">Registration No.<span class="text-danger">*</span></asp:Label><asp:HiddenField ID="HiddenField3" runat="server" />
                                    <asp:TextBox ID="TxtBxSearchRegno"  placeholder="Registration No." CssClass=" form-control" AutoCompleteType="Disabled" style="margin-left:5px;" runat="server"></asp:TextBox>
                                </div>
                                 <div id="searchregdate" runat="server" visible="false">
                                    <asp:Label ID="Label2" runat="server" Text="">Registered Year<span class="text-danger">*</span></asp:Label><asp:HiddenField ID="HiddenField2" runat="server" />
                                    <asp:TextBox ID="TxtBxSearchRegYear"  placeholder="Registered Year" CssClass=" form-control" AutoCompleteType="Disabled" style="margin-left:5px;" runat="server"></asp:TextBox>
                                </div>
                                 <div id="searchregname"  runat="server" visible="false">
                                    <asp:Label ID="Label6" runat="server" Text="">Society Name<span class="text-danger">*</span></asp:Label><asp:HiddenField ID="HiddenField4" runat="server" />
                                    <asp:TextBox ID="TxtBxSearchRegName"  placeholder="Registered Society Name" CssClass=" form-control" AutoCompleteType="Disabled" style="margin-left:2px;" runat="server" ></asp:TextBox>
                                </div>                                                                 
                                <div id="searchbtn" runat="server" visible="false">
                                    <asp:LinkButton ID="search_society" CssClass="btn btn-info" OnClick="search_society_Click" OnClientClick="CoverClickLK('search_society');" CausesValidation="false" runat="server" style="margin-left:30px;"><i class="fa fa-search"></i>&nbsp;Search</asp:LinkButton>
                                       
                                </div>
                            </div>

                            
                        </div>

                            <div id="SocietyList" runat="server" visible="false">
                               <asp:GridView ID="GridviewSocietyList" runat="server" CellPadding="5" Font-Size="Small" AutoGenerateColumns="false" AllowPaging="true" PageSize ="100" OnPageIndexChanging="GridviewSocietyList_PageIndexChanging" CssClass="Grid" OnRowDataBound="GridviewSocietyList_RowDataBound" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%" OnDataBound="gdSearch_DataBound"><%-- OnSorting="GridviewSocietyList_Sorting"--%>
                                <AlternatingRowStyle BackColor="White" />
                                <FooterStyle BackColor="#CCCC99" />
                                <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                              <%--  <PagerStyle CssClass="cssPager" BackColor="#d0d8dc" ForeColor="Black" HorizontalAlign="Right" /> --%>

                                    <PagerStyle CssClass="cssPager" BackColor="#d0d8dc" ForeColor="Black" HorizontalAlign="Right" />
                               <%-- <PagerSettings Mode="Numeric" PageButtonCount="5"/>--%>
                                <RowStyle BackColor="#c8caf1" />
                                <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                <SortedAscendingHeaderStyle BackColor="#848384" />
                                <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                <SortedDescendingHeaderStyle BackColor="#575357" />

                                <PagerTemplate>
                                  <table width="100%">                   
                                    <tr>                       
                                        <td style="width:70%" class="text-center">
                                            <asp:Label runat="server" Text="Showing Page" ForeColor="#3a79c7" ></asp:Label>
                                            <asp:DropDownList ID="ddlPaging" runat="server" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlPaging_SelectedIndexChanged" style="width:auto; text-align:center" />&nbsp;
                                            <asp:Label ID="totalpages" runat="server" Text="" ForeColor="#3a79c7"></asp:Label>

                                        </td>
                                    </tr>
                                  </table>
                                </PagerTemplate>
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr. No">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Society Name" DataField="socname"  /> <%--SortExpression="socname"--%>
                                    <asp:BoundField HeaderText="Registration No." DataField="socregid" /><%-- SortExpression="socregid"--%>
                                    <asp:BoundField HeaderText="Registration Date" DataField="reg_date" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField HeaderText="Registered District" DataField="socdist" />
                                    <asp:BoundField HeaderText="Registered Year" DataField="society_year"  /> <%--SortExpression="society_year" --%>
                                    <asp:BoundField HeaderText="Taluka" DataField="old_village_name" />
                                    <asp:BoundField HeaderText="Society Address" DataField="socaddr" />
                                   
                                    <asp:TemplateField HeaderText="Edit" Visible="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LbEditSociety" CssClass="btn btn-info" OnClick="LbEditSociety_Click" OnClientClick="CoverClickLK('LbEditSociety');" CausesValidation="false" runat="server"><i class="fa fa-edit"></i>&nbsp;Edit</asp:LinkButton>
                                            <asp:HiddenField ID="hfdistrict" Value='<%# Eval("socdistrict") %>' runat="server" />
                                             </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                     <ItemTemplate>
                                           <asp:Label ID="LbRegId" runat="server" Text='<%# Eval("socregid") %>'></asp:Label>                                          
                                           <asp:Label ID="LbDate" runat="server" Text='<%# Eval("regdate") %>'></asp:Label>
                                           
                                            <asp:HiddenField ID="hfdatemodify" Value='<%# Eval("datemodified") %>' runat="server" />
                                            <%--<asp:HiddenField ID="hfregdate" Value='<%# Eval("reg_date") %>' runat="server" />--%>
                                     </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    
                                </Columns>
                            </asp:GridView>      
                             </div>                      
                              
                        </div>                       
                        
                    </div>
                </div>
            </div>
        </div>
    </div>



     <div class="bs-example">
            <!-- Modal HTML -->
            <div id="editsocietymodal" class="modal">
                <div class="modal-dialog modal-lg" >
            <div class="modal-content">
                <div class="modal-header">
                            <h4 class="modal-title">Society Details</h4>
                        </div>
                        <div class="modal-body">


                            <div class="table-responsive">
                                <div class="table table-bordered table-hover">
                                    <table style="width: 100%">
                                        <thead>
                                            
                                            <tr>
                                                <td class="a">
                                                    <asp:Label runat="server" ID="lblSocName" Text="Society Name" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                </td>
                                                <td class="b">
                                                    <asp:TextBox runat="server" ID="TxtBxSocName" MaxLength="250" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvsocname" Display="Dynamic" runat="server" ControlToValidate="TxtBxSocName" CssClass="text-danger" ErrorMessage="Enter society Name"></asp:RequiredFieldValidator>
                                                     <asp:RegularExpressionValidator ID="revsocname" runat="server" Display="Dynamic" ControlToValidate="TxtBxSocName" ForeColor="Red" ValidationExpression="[\sa-zA-Z0-9-',.`_()-]+$" ErrorMessage="Alphabet A-Z, a-z, 0-9 and Special Characters -,._()` only are allowed" />
                                                </td>
                                            </tr>                                           
                                            <tr>
                                                <td class="a">
                                                    <asp:Label runat="server" ID="lblRegNo" Text="Registration No." CssClass="control-label" Font-Bold="true" ></asp:Label><span class="text-danger">*</span>
                                                </td>
                                                <td class="b">
                                                    <asp:TextBox runat="server" ID="TxtBxRegNo" CssClass="form-control" AutoCompleteType="Disabled" Enabled="false"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9" Display="Dynamic" runat="server" ControlToValidate="TxtBxRegNo" CssClass="text-danger" ErrorMessage="Enter Registration No"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="a">
                                                    <asp:Label runat="server" ID="LblRegDate" Text="Registration Date" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                </td>
                                                <td class="b">
                                                    <asp:TextBox runat="server" ID="TxtBxRegDate" CssClass="form-control"  AutoCompleteType="Disabled"></asp:TextBox>
                                                    <%-- <asp:Label ID="Label9" runat="server" Text="Please enter date in DD-MM-YYYY format only" CssClass="control-label" ForeColor="OrangeRed" ></asp:Label>--%>
                                                 
                                                         </td>
                                            </tr>
                                            <tr >
                                                <td class="a">
                                                    <asp:Label runat="server" ID="LblRegDistrict" Text="Registered District" CssClass="control-label" Font-Bold="true" ></asp:Label><span class="text-danger">*</span>
                                                </td>
                                                <td class="b">
                                                    <asp:DropDownList ID="ddl_district" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_district_SelectedIndexChanged" CssClass="form-control" Enabled="false"></asp:DropDownList>
                                                    <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator2" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="ddl_district" ErrorMessage="Please Select Designation"> </asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            
                                            
                                            <tr>
                                                <td class="a">
                                                    <asp:Label runat="server" ID="lbl_socvillage" Text="Society Taluka Name" CssClass="control-label" Font-Bold="true" ></asp:Label><span class="text-danger"></span>
                                                </td>
                                                <td class="b">
                                                     <asp:TextBox ID="TxtBxVillage" CssClass="form-control" runat="server" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                                                      <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="TxtBxVillage" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\sa-zA-Z]*$" ErrorMessage="No special characters allowed." />
                                                --%> </td>
                                            </tr>
                                            <tr>
                                                <td class="a">
                                                    <asp:Label ID="lblSocaddr" runat="server" Text="Society Address" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger"></span>
                                                </td>
                                                <td class="b">
                                                    <asp:TextBox ID="TxtBxAddr" CssClass="form-control" runat="server" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                                                     <asp:RegularExpressionValidator ID="revbuilding" runat="server" ControlToValidate="TxtBxAddr" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\sa-zA-Z0-9()-,._/:@-]+$" ErrorMessage="No special characters allowed." />
                                                </td>
                                            </tr>
                                            
                                           
                                            
                                            <tr>
                                                <td></td>
                                                <td>
                                                     <asp:Label runat="server" ID="lblError" Text="" Width="100%" ForeColor="Red" CssClass="alert-danger" Font-Bold="true"></asp:Label>
                                                    <asp:Button ID="btnUpdate"  CssClass="btn btn-primary" OnClick="btnUpdate_Click"  runat="server"  Text="Update" />  <%--CausesValidation="false"--%>
                                                    
                                              
                                                </td>
                                            </tr>
                                   
                                        
                                  
                                        </thead>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="close_modal"  CausesValidation="false" OnClick="close_modal_Click" CssClass="btn btn-danger" runat="server" Text="Close" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

    
 <div class="bs-example">
        <div id="myerrorModal" class="modal fade">
            <div class="modal-dialog modal-confirm" id="confirm">
                <div class="modal-content" id="content1">
                    <div class="modal-header" id="header"  style="background-color: #607D8B">
                        <div class="icon-box" id="box" runat="server">
                            <div id="sorry" runat="server">
                                <i class="material-icons">&#xE5CD;</i>
                            </div>
                        </div>
                        <p id="hfour" runat="server" class="text-center modal-title text-white">Sorry!</p>
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
                           <asp:Label ID="Label8" runat="server" ForeColor="White"></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="permission" OnClick="permission_Click" CausesValidation="false"  CssClass="btn btn-primary" runat="server" Text="Ok" />
                           <%-- <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                        </div>
                    </div>
                </div>
            </div>
        </div>


    <%-- <div id="cover" style="display: none;"></div>
            <div class="row">
                <div id="CoverDoubleClick" class="opac_divLoader overlayLoader" style="display: none;">
                    <asp:Image ID="wait" runat="server" ImageUrl="assets/images/loading.gif" AlternateText="w a i t"
                        Height="40%" Width="40%" Style="vertical-align: middle;" />
                </div>
            </div>--%>

    

</asp:Content>
