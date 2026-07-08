<%@ Page Title="" Language="C#" MasterPageFile="~/User/Society.Master" AutoEventWireup="true" CodeBehind="ApplyCertifiedCopy.aspx.cs" Inherits="GoaSocietyRegistration.User.ApplyCertifiedCopy" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <style type="text/css">
        .mauto{
            margin:auto
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
        function openInNewTab() {
            window.document.forms[0].target = '_blank';
            setTimeout(function () { window.document.forms[0].target = ''; }, 0);
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <br />
      <div class="container-fluid">
        <div class="row">
            <div class="col-lg-12  col-md-12  col-sm-12 col-xs-12">   
                                 
                <div class="card" style="width: 100%;">
                    <h6 class="card-header text-white" style="background-color:#4582c3"><strong>Application for Certified Copies</strong> </h6>
                       <div class="card-body">
                        
                         <ul class="nav nav-tabs">
                            <li class="nav-item">
                                <a href="#home" class="nav-link active" data-toggle="tab">Apply</a>
                            </li>
                            <li class="nav-item">
                                <a href="#profile" class="nav-link" data-toggle="tab">Application History</a>
                            </li>
                        </ul>
                       
                        <div class="tab-content">
                            <br />
                            <div class="tab-pane fade show active" id="home">
                                
                                <asp:Label ID="label7" runat="server" ForeColor="OrangeRed" Text="NOTE: Fees for Certified Copies of Documents Pertaining to Registered Societies is ₹5/- PER PAGE and ₹10/- for Certificate "></asp:Label>
                                <div class="row mt-4">
                                    <label id="label2" class="col-sm-3 col-xs-12 form-group" style="font-weight:bold">Society Name:</label>
                                    <div class="col-sm-6 col-xs-12">
                                        <asp:Label ID="lblSocname" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="row ">
                                    <label id="label3" class="col-sm-3 col-xs-12 form-group" style="font-weight:bold">Society Registration No.</label>
                                    <div class="col-sm-6 col-xs-12">
                                         <asp:HiddenField ID="HiddenField1" runat="server" />
                                        <asp:Label ID="lblsocregno" runat="server" Text=""></asp:Label> <asp:HiddenField ID="hfcertmongo" runat="server" />
                                    </div>
                                </div>                            

                                <div class="form-group">
                                    <label id="label6"  style="font-weight:bold" >Apply for Certified Copies of:<span class="text-danger">*</span></label><br />
                                    <div class="table-responsive">
                                         <asp:GridView ID="gv_docs" OnRowDataBound="gv_docs_RowDataBound" Width="90%" Visible="true" runat="server" CellPadding="5" AutoGenerateColumns="false" CssClass="Grid" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true">
                                             <HeaderStyle BackColor="Wheat" Font-Bold="true" ForeColor="Black" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr. No">
                                                    <ItemTemplate>
                                                       <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="File Name" DataField="docname" />                                                
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbldocname" runat="server"  Text='<%# Eval("docname") %>'></asp:Label>
                                                        <asp:Label ID="lblid" runat="server"  Text='<%# Eval("myid") %>'></asp:Label>
                                                        <asp:Label ID="lblpagecount" runat="server"  Text='<%# Eval("pagecount") %>'></asp:Label>
                                                        <asp:Label ID="lblamt" runat="server"  Text='<%# Eval("amt") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="View" Visible="false" >
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="LbView" OnClick="LbView_Click"  CausesValidation="false" runat="server"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton>
                                                    <asp:HiddenField ID="hfobjectID" Value='<%# Eval("object_id") %>' runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Select" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>                                                 
                                                    
                                                    <asp:CheckBox ID="chkbx" OnCheckedChanged="chkbx_CheckedChanged" AutoPostBack="true" Visible="true" runat="server" CausesValidation="false" Text=" " Font-Size="Small"/>
                                                </ItemTemplate>
                                            </asp:TemplateField> 
                                                <asp:TemplateField HeaderText="No of Copies" ItemStyle-HorizontalAlign="Center" >
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlnoofcopies" CssClass="text-center" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlnoofcopies_SelectedIndexChanged" Width="50%" >
                                                             <asp:ListItem Value="1" Text="1" Selected="True"></asp:ListItem>
                                                             <asp:ListItem Value="2" Text="2"></asp:ListItem>
                                                        </asp:DropDownList>
                                       
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                
                                                   
                                                 <asp:TemplateField HeaderText="No. of Pages" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate> 
                                                   <asp:Label ID="noofpages" runat="server"  Text="0"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>    
                                                <asp:TemplateField HeaderText="Amount to be Paid" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate> 
                                                   <asp:Label ID="amountperdoc" runat="server"  Text="0"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>    
                                              
                                               
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <div align="center">No records found.</div>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                         <asp:HiddenField ID="hfcertselected_flag" runat="server" />
                                       
                                    </div>
                                  
                                </div>
                                   

                                  <div class="row mt-2">
                                   <asp:Label CssClass="col-sm-3 col-xs-12" ID="Label3" runat="server" Font-Bold="true" Text="Total Amount to be Paid : "></asp:Label>
                                    <div class="col-sm-4 col-xs-12 text-left">
                                        <asp:Label ID="Label4" runat="server" Text="₹ " ForeColor="Green" Font-Bold="true"></asp:Label><asp:Label ID="lblamt" runat="server" Text="0" ForeColor="Green" Font-Bold="true"></asp:Label>
                                         <asp:HiddenField ID="hftotalnoofpages" runat="server" />
                                        <asp:HiddenField ID="hftotalamt" runat="server" />
                                    </div>
                                </div>   

                                <div class="table-responsive mt-3 row p-3" id="div_fir" runat="server" visible="false">
                                    <asp:Label ID="Label1" runat="server" Text="Upload FIR Copy" Font-Bold="true"></asp:Label><span id="upload" class="text-danger" runat="server">*</span>
                                    <asp:FileUpload CssClass="ml-4" ID="FileUpload1" runat="server" />
                                     <asp:RegularExpressionValidator ID="revfileupload1" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload1" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File." Display="Dynamic" />
                                     <asp:LinkButton ID="Lb_fir_upload" CssClass="btn btn-info ml-5" CausesValidation="false" runat="server" ToolTip="Upload File" ><i class="fa fa-upload" ></i>&nbsp;Upload</asp:LinkButton>
                                     <asp:LinkButton ID="Lb_fir_delete"  CssClass="btn btn-danger ml-5" CausesValidation="false" runat="server" ToolTip="Delete File" ><i class="fa fa-trash"></i>&nbsp;Delete</asp:LinkButton>
                                    <asp:LinkButton ID="LB_fir_view"  CssClass="btn btn-secondary ml-5" CausesValidation="false" runat="server" ToolTip="View File"><i class="fa fa-file-pdf" aria-hidden="true"></i>&nbsp;View</asp:LinkButton>
                                   
                                     <asp:Label ID="lbfu1status" CssClass="ml-5" runat="server"></asp:Label>

                                  
                                </div>

                                <div class="form-group row text-center mt-2" id="buttonrow" runat="server">
                                    <asp:LinkButton ID="LkSave" OnClick="LkSave_Click"  CssClass="btn btn-primary mauto" CausesValidation="false" runat="server">&nbsp;Save</asp:LinkButton>
                                     <asp:LinkButton ID="LkDiscard" OnClick="LkDiscard_Click" CssClass="btn btn-danger mauto" runat="server" Visible="false">&nbsp;<i class="fa fa-trash" aria-hidden="true"></i>&nbsp;Reset</asp:LinkButton>
                                      <asp:LinkButton ID="LkProceedtoPayment" OnClick="LkProceedtoPayment_Click" CssClass="btn btn-info mauto" runat="server" Visible="false">&nbsp;<i class="fa fa-arrow-right" aria-hidden="true"></i>&nbsp;Proceed to Payment</asp:LinkButton>

                                </div> 

                                        
                                
                                    <div class="card-body" runat="server" id="approvedandgotopaymentstatus" visible="true">
                                         <div class="row ">
                                    <label id="label5" class="col-sm-3 col-xs-12 form-group" style="font-weight:bold">Payment Details :</label>
                                   
                                </div> 
                                           
                                            <div class="row" id="paymentgridview" runat="server">
                                                <div class="text-center col-12" style="background-color:dimgray; color:white; font-weight:bold">
                                                <asp:Label ID="Label41" CssClass="alert" runat="server" Text="Old eChallan Summary "></asp:Label>
                                                    </div>
                                                <div class="table-responsive">
                                                    <asp:GridView ID="gridviewPayment"  OnRowDataBound="gridviewPayment_RowDataBound"  PageSize="5" AllowPaging="true" runat="server" CellPadding="5" AutoGenerateColumns="false" CssClass="Grid" AlternatingRowStyle-CssClass="alt" EmptyDataText="No eChallan Summary Found. Please Click on Proceed to Payment." ShowHeaderWhenEmpty="true" Width="100%">
                                                         <HeaderStyle BackColor="#d4e3fd" Font-Bold="true" ForeColor="Black" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Sr. No">
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Edit" Visible="false">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbpaymentapp_id" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                                                    <asp:Label ID="lbstatus" runat="server" Text='<%# Eval("status") %>'></asp:Label>
                                                                    <asp:Label ID="lbechallan" runat="server" Text='<%# Eval("echallan_no") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:BoundField HeaderText="eChallan No" DataField="echallan_no" />
                                                            <asp:BoundField HeaderText="eChallan Generated On" DataField="echallangeneratedon" DataFormatString="{0:dd/MM/yyyy}" />
                                                            <asp:BoundField HeaderText="Status" DataField="status" />
                                                            <asp:TemplateField HeaderText="Update Status">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lkUpdateStatus" OnClick="lkUpdateStatus_Click" CssClass="btn btn-primary" CausesValidation="false" runat="server"><i class="fas fa-sync"></i></asp:LinkButton>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Delete Old eChallan">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="LkDeleteold" OnClick="LkDeleteold_Click" CssClass="btn btn-danger" CausesValidation="false" runat="server"><i class="fa fa-trash" aria-hidden="true"></i></asp:LinkButton>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                    <asp:Label ID="Label8" CssClass="alert-danger" runat="server" Text="Click on Update Status Button if your amount is deducted to get the Status. It will take some time to update."></asp:Label>
                                                
                                                </div>
                                                
                                            </div>
                                        </div>
                                                                             
                                            
                                           
                                           <div class="card-body" runat="server" id="paymentdone_details" visible="false">
                                                <div class="row ">
                                    <label id="label10" class="col-sm-3 col-xs-12 form-group" style="font-weight:bold">Payment Details :</label>
                                   
                                </div> 
                                            <div class="form-group" id="print">
                                                <div class="table-responsive">
                                                    <div class="table table-bordered table-hover">
                                                        <table style="width: 100%">
                                                            <thead>
                                                                 <tr>
                                                                        <td>
                                                                            <asp:Label ID="Label9" runat="server" Text="Application ID:"> </asp:Label>
                                                                        </td>
                                                                        <td colspan="1">
                                                                            <asp:Label ID="lblappid" runat="server"> </asp:Label></td>
                                                                    </tr>

                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="Label10" runat="server" Text="Society Name"> </asp:Label>
                                                                        </td>
                                                                        <td colspan="1">
                                                                            <asp:Label ID="lblsocietyname" runat="server"> </asp:Label></td>
                                                                    </tr>

                                                                    <tr runat="server" id="showchallan" visible="false">
                                                                        <td>
                                                                            <asp:Label ID="Label11" runat="server" Text="Echallan No"> </asp:Label>
                                                                        </td>
                                                                        <td colspan="1">
                                                                            <asp:Label ID="lblshowchallan" runat="server"> </asp:Label></td>
                                                                    </tr>

                                                                    <tr>
                                                                    <td>Total Fee</td>
                                                                    <td colspan="1">₹
                                                    <asp:Label ID="lbltotalfee" runat="server"> </asp:Label></td>


                                                                </tr>
                                                                <tr>
                                                                    <td>Fee Status</td>

                                                                    <td colspan="1">
                                                                        <asp:Label ID="paymentpaid" runat="server" Font-Bold="true"></asp:Label>
                                                                    </td>

                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" style="text-align: center">

                                                                        <%--<asp:LinkButton ID="" OnClick="" runat="server"><i class="fa fa-print" aria-hidden="true"></i>&nbsp;Print </asp:LinkButton>--%>
                                                                        <asp:LinkButton ID="LinkButton14" runat="server" CssClass="btn btn-primary" Visible="false"  Enabled="false" OnClick="LinkButton14_Click"><i class="fa fa-cc-visa"></i>&nbsp;Payment Receipt</asp:LinkButton>

                                                                    </td>
                                                                    <%--<td colspan="2" style="text-align: center">
                                                    <asp:LinkButton Visible="false" ID="LinkButton1" OnClick="viewmarrdecform_Click" runat="server" Enabled="false"><i class="fa fa-download"></i>&nbsp;Download Marriage Declaration Form</asp:LinkButton></td>--%>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="5">
                                                                        <asp:Label ID="LbMessage" Text="Payment is successful. Please visit office and collect the copy" CssClass="alert-info" runat="server" Font-Bold="true"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </thead>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                     
                                   

                                                             
                            </div>  
                            
                             <div class="tab-pane fade" id="profile">

                                <asp:GridView ID="GridviewApplicationHistory" OnRowDataBound="GridviewApplicationHistory_RowDataBound" runat="server" CellPadding="5" AutoGenerateColumns="false" CssClass="Grid" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" AlternatingRowStyle-CssClass="alt" Width="100%">
                                    <AlternatingRowStyle BackColor="White" />
                                    <FooterStyle BackColor="#CCCC99" />
                                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />                                    
                                    <RowStyle BackColor="#edeac9" />
                                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />                                   
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sr. No">
                                            <ItemTemplate>
                                               <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Application No" DataField="app_id" />                                   
                                      
                                         <asp:BoundField HeaderText="Document Name" DataField="docname" />
                                        <asp:BoundField HeaderText="Copies Applied" DataField="noofcopies" />
                                        <asp:BoundField HeaderText="Applied on" DataField="applieddate" DataFormatString="{0:dd/MM/yyyy}" />
                                          <asp:BoundField HeaderText="Status" DataField="status" />
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
        <div id="alertmodal" class="modal fade">
            <div class="modal-dialog  modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="lblMSG1" runat="server" Text="" ForeColor="Green"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnCancel" runat="server" class="btn btn-secondary" data-dismiss="modal"></asp:Button>
                        
                        <asp:Button ID="RedirecttoLoginBtn" CssClass="btn btn-primary" runat="server" OnClick="RedirecttoLoginBtn_Click" />
                        <asp:Button ID="errorpage" runat="server"  class="btn btn-primary"></asp:Button>

                        <asp:LinkButton ID="btnrefresh" OnClick="btnrefresh_Click" CssClass="btn btn-primary" Visible="false" runat="server"></asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>

   <div class="bs-example">
        <!-- Modal HTML -->
        <div id="myModal" class="modal fade">
            <div class="modal-dialog  modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label20" runat="server" Text="" ForeColor="Green"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <div id="pdfModal" class="modal fade" role="dialog">
        <div class="modal-dialog modal-lg">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">

                    <embed id="embed1" runat="server" frameborder="0" width="100%" height="500px" />
                    <div class="modal-footer">
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <div class="bs-example">
            <!-- Modal HTML -->
            <div id="redirectModal" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label64" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label65" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnRedirect" OnClick="btnRedirect_Click" CssClass="btn btn-primary" runat="server" Text="Close" />
                           
                        </div>
                    </div>
                </div>
            </div>
        </div>

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="deleteconfirmation" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label14" runat="server" Text="Are you Sure that you want to delete this entry?" ForeColor="OrangeRed"></asp:Label>
                    </div>
                    <div class="modal-footer">                     
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="deleteappln_modalbtn" CssClass="btn btn-primary" runat="server" OnClick="deleteappln_modalbtn_Click" Text="Confirm" />
                    </div>
                </div>
            </div>
        </div>
    </div>

     <div class="bs-example">
            <!-- Modal HTML -->
            <div id="confirmModal" class="modal fade">
                <div class="modal-dialog  modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title">Confirmation</h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label5" runat="server" Text="Do you want to Save the changes? Click Save to proceed" ForeColor="Green"></asp:Label>
                          </div>
                        <div class="modal-footer">
                              <asp:Button ID="confirmmodalbutton" runat="server" OnClick="confirmmodalbutton_Click" Text="Save" CssClass="btn btn-primary" CausesValidation="true" />
                            <button type="button" class="btn btn-danger"  data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="bs-example">
        <!-- Modal HTML -->
        <div id="deletechallanModal" class="modal fade">
            <div class="modal-dialog  modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">

                        <asp:HiddenField ID="hdechallan" runat="server" />
                        <asp:HiddenField ID="hdapplicationid" runat="server" />
                        <asp:Label ID="Label43" runat="server" ForeColor="Red" Text="If your payment is deducted then kindly wait and click on refresh button after some time. If you delete echallan after your payment is dedcuted, then your dedcuted payment will be lost."></asp:Label>
                        <br />
                        <asp:CheckBox ID="CheckBox1" OnCheckedChanged="CheckBox1_CheckedChanged" Visible="true" runat="server" CausesValidation="false" AutoPostBack="true" Text="I confirm that my payment is not deducted and I want to delete my old challan and create new." />
                        <asp:Label ID="Label45" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton ID="lkdeleteoldechallan" Enabled="false" CausesValidation="false" OnClick="lkdeleteoldechallan_Click" CssClass="btn btn-success" runat="server"><i class="fas fa-check"></i>&nbsp;&nbsp;Confirm</asp:LinkButton>
                        <asp:LinkButton ID="LinkButton3" CssClass="btn btn-danger" runat="server"><i class="fas fa-times"></i>&nbsp;&nbsp;Close</asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>



</asp:Content>

