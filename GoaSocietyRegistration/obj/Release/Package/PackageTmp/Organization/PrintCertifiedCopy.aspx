<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="PrintCertifiedCopy.aspx.cs" Inherits="GoaSocietyRegistration.Organization.PrintCertifiedCopy" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
      <style>
         .uppercase {
            text-transform: uppercase;
        }

        .right {
            float:right;
            margin-right:10%;
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

      

        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

   <br />
    
     <div class="container-fluid">
      <section class="row">
        <div class="col-sm-12">
            <section class="row">            
                <div class="col-lg-12 mb-12 bg-default">
                    <div class="society_details">Print</div>
                    <div class="card">                     
                        <div class="card-block">

                            <div class="row mt-4">
                                    <label id="label2" class="col-sm-3 col-xs-12 form-group" style="font-weight:bold">Society Name:</label>
                                    <div class="col-sm-6 col-xs-12">
                                        <asp:Label ID="lblSocname" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="row mt-2 ">
                                    <label id="label3" class="col-sm-3 col-xs-12 form-group" style="font-weight:bold">Society Registration No.</label>
                                    <div class="col-sm-6 col-xs-12">
                                        <asp:Label ID="lblsocregno" runat="server" Text=""></asp:Label> <asp:HiddenField ID="hfcertmongo" runat="server" />
                                    </div>
                                </div>        

                            <div class="table-responsive">
                                  <asp:GridView ID="gv_docs" Width="90%" Visible="true" runat="server" CellPadding="5" AutoGenerateColumns="false" CssClass="Grid" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true">
                                        <HeaderStyle BackColor="Wheat" Font-Bold="true" ForeColor="Black" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sr. No"  ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="File Name" DataField="docname" />                                                
                                        <asp:TemplateField Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAppid" runat="server"  Text='<%# Eval("app_id") %>'></asp:Label>
                                                <asp:Label ID="lbldocname" runat="server"  Text='<%# Eval("docname") %>'></asp:Label>
                                                <asp:Label ID="lblid" runat="server"  Text='<%# Eval("myid") %>'></asp:Label>
        
                                            </ItemTemplate>
                                        </asp:TemplateField>                                        
                                                
                                        <asp:TemplateField HeaderText="No of Copies" ItemStyle-HorizontalAlign="Center"  >
                                            <ItemTemplate>
                                                <asp:Label ID="lblcopies" runat="server"  Text='<%# Eval("noofcopies") %>'></asp:Label>
                                       
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Download" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                
                                                <asp:LinkButton ID="LkDownload" CssClass="btn btn-info" OnClick="LbDownload_Click" CausesValidation="false" runat="server" ><i class="fa fa-download"></i></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                                
                                                   
                                               
                                               
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div align="center">No records found.</div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                                 <asp:HiddenField ID="HiddenField1" runat="server" />


                               
                            </div>

                            <div class="mt-4">
                               
                                <asp:LinkButton ID="LkBack" OnClick="LkBack_Click" CssClass="btn btn-warning left" CausesValidation="false" runat="server" ><i class="fa fa-arrow-left">&nbsp;BACK</i></asp:LinkButton>
                                <asp:LinkButton ID="Lkissue" OnClick="Lkissue_Click" CssClass="btn btn-primary right" CausesValidation="false" runat="server" ><i class="fa fa-check">&nbsp;ISSUED</i></asp:LinkButton>
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
            <div id="issue_confirmation_modal" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #607D8B">
                            <h4 class="modal-title">
                                <asp:Label ID="Label10" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="Label11" runat="server" Text="I confirm that I have Downloaded and Issued the Certified Copies to the Applicant."></asp:Label>
                            </div>
                        <asp:Label ID="Label12" CssClass="alert-danger" runat="server" Text=""></asp:Label>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-danger left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="issue_confirm_modal_btn" OnClick="issue_confirm_modal_btn_Click"  CssClass="right btn btn-success" runat="server">Confirm</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
</asp:Content>
