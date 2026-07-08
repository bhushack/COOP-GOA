<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="AddOldSociety.aspx.cs" Inherits="GoaSocietyRegistration.Organization.AddOldSociety" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <script src="../Scripts/jquery-3.5.0.min.js"></script>
    <script src="../Scripts/aes.js"></script>
    <script src="../Scripts/encrypt.js"></script>

     <link href="../Admin/datepicker/jquery-ui.css" rel="stylesheet" />
     <script src="../Admin/datepicker/jquery-1.10.2.js"></script>
     <script src="../Admin/datepicker/jquery-ui.js"></script>

     <style>
         .uppercase {
            text-transform: uppercase;
        }

        .right {
            float:right;
        }

        .left {
            float:left;
        }

        .bs-example {
            margin: 20px;
        }
       
        .society_details {
            background-color: #6c53dc !important;
            color: #ffffff;
            padding: 8px;
            font-weight:bold;
        }

         .a {
            width: 25%;
        }

        .b {
            width: 75%;
        }

        
    </style>

     <script type="text/javascript">
          jQuery(document).ready(function ($) {
              $("[id*=TxtBxSocRegDate]").datepicker({
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

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    
    <div class="container-fluid">
      <section class="row">
        <div class="col-sm-12">
            <section class="row">            
                <div class="col-lg-12 mb-12 bg-default">
                    <div class="society_details">Add Society Details</div>
                    <div class="card">                     
                        <div class="card-block">

                            <div class="table-esponsive">
                                <div class="table table-borderless">
                                    <table style="width: 100%">
                                        <tbody>
                                            
                                            <tr>
                                                <td class="a">
                                                    <asp:Label runat="server" ID="lblSocNAme" Text="Society Name" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                </td>
                                                <td class="b">
                                                   <asp:TextBox ID="TxtBxSocName" CssClass="form-control uppercase"  AutoCompleteType="Disabled" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="TxtBxSocName" ErrorMessage="Enter Society Name"> </asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Display="Dynamic" ControlToValidate="TxtBxSocName" ForeColor="Red" ValidationExpression="[\sa-zA-Z0-9-',.`_()-]+$" ErrorMessage="Alphabet A-Z, a-z, 0-9 and Special Characters -.,_()` only are allowed" />
                                   
                                                  </td>
                                            </tr>

                                            <tr>
                                                <td class="a">
                                                    <asp:Label runat="server" ID="lblSocRegNo" Text="Registration No." CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                </td>
                                                <td class="b">
                                                   <asp:TextBox ID="TxtBxRegNo" CssClass="form-control uppercase" AutoCompleteType="Disabled" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="TxtBxRegNo" ErrorMessage="Enter Registration Number"> </asp:RequiredFieldValidator>
                                                    <asp:Label runat="server" ID="errorsocregno" Visible="false" CssClass="control-label" Font-Bold="true"></asp:Label>
                                              
                                                  </td>
                                            </tr>

                                            <tr>
                                                <td class="a">
                                                    <asp:Label runat="server" ID="lblSocRegYear" Text="Registered Year" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                </td>
                                                <td class="b">
                                                   <asp:TextBox ID="TxtBxSocRegYear" TextMode="Number" min="1947" max="2200" CssClass="form-control"  AutoCompleteType="Disabled" runat="server" MaxLength="4"></asp:TextBox>
                                                   <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="TxtBxSocRegYear" ErrorMessage="Enter Registered Year"> </asp:RequiredFieldValidator>
                                                  <%--<asp:RangeValidator ID="RangeValidator1" Type="Integer" MinimumValue="1947" MaximumValue="2025" ControlToValidate="TxtBxSocRegYear" runat="server" ErrorMessage="Enter Valid Year"></asp:RangeValidator>
                                               --%>

                                                </td>
                                            </tr>

                                            <tr>
                                                <td class="a">
                                                    <asp:Label runat="server" ID="lblSocRegDate" Text="Date of Registration" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                
                                                      </td>
                                                <td class="b">
                                                   <asp:TextBox ID="TxtBxSocRegDate" CssClass="form-control"  AutoCompleteType="Disabled" runat="server"></asp:TextBox>
                                                   <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="TxtBxSocRegDate" ErrorMessage="Enter Registration Date"> </asp:RequiredFieldValidator>
                                                   
                                                  </td>
                                            </tr>
                                           
                                            <tr>
                                                <td class="a">
                                                    <asp:Label runat="server" ID="Label3" Text="District" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                   
                                                    </td>
                                                <td class="b">
                                                    <asp:DropDownList ID="ddlSocDistrict" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSocDistrict_SelectedIndexChanged" CssClass="form-control"></asp:DropDownList>
                                                   
                                                    <asp:RequiredFieldValidator InitialValue="-1" ID="Req_ID" Display="Dynamic" runat="server" ControlToValidate="ddlSocDistrict" CssClass="text-danger" ErrorMessage="Please Select District"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="a">
                                                    <asp:Label runat="server" ID="lblSocTaluka" Text="Taluka" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger"></span>
                                                     <asp:HiddenField ID="HiddenField1" runat="server" />
                                      
                                                </td>
                                                <td class="b">
                                                   <asp:DropDownList ID="ddlSocTaluka" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    
                                                    <%--<asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator5" Display="Dynamic" runat="server" ControlToValidate="ddlSocTaluka" CssClass="text-danger" ErrorMessage="Please Select District"></asp:RequiredFieldValidator>
                                               --%>
                                                     </td>
                                            </tr>
                                             <tr>
                                                <td class="a">
                                                    <asp:Label runat="server" ID="lblSocAddr" Text="Society Address" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger"></span>
                                                </td>
                                                 <td class="b">
                                                     <asp:TextBox ID="TxtBxSocAddr" TextMode="MultiLine" CssClass="form-control"  AutoCompleteType="Disabled" runat="server"></asp:TextBox>
                                   
                                                 </td>
                                            </tr>                                        
                                           
                                            
                                            <tr>
                                                
                                                <td colspan="2">
                                                    <asp:Label runat="server" ID="lblError" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label><br /><br />
                                                    <asp:LinkButton ID="LkBtnClear" OnClick="LkBtnClear_Click" CssClass="btn btn-warning left ml-3"  runat="server" CausesValidation="false"><span class="btn-label"><i class="fa fa-refresh"></i></span>&nbsp <b>Clear</b></asp:LinkButton>
                                                    <asp:LinkButton ID="LkBtnAddSoc" OnClick="LkBtnAddSoc_Click"  CssClass="btn btn-success right mr-3" runat="server"><span class="btn-label"><i class="fa fa-plus"></i></span>&nbsp <b>Add Society</b></asp:LinkButton>
                                
                                                    
                                                  </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            
                           

                           
                        </div>
                       
                    </div>
                </div>
            </section>
        </div>
          </section>    
        </div>
         

    
    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="errormodal" class="modal">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label49" runat="server" Text="Alert" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label50" runat="server" ForeColor="red"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

      <div class="bs-example">
        <!-- Modal HTML -->
        <div id="successmodal" class="modal fade">
            <div class="modal-dialog  modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Success</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label11" runat="server" Text="" ForeColor="Green"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
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
</asp:Content>
