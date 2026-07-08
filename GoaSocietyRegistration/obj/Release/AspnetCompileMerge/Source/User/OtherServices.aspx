<%@ Page Title="" Language="C#" MasterPageFile="~/User/Society.Master" AutoEventWireup="true" CodeBehind="OtherServices.aspx.cs" Inherits="GoaSocietyRegistration.User.OtherServices" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
     <script src="../assets/js/jquery-1.12.4.min.js"></script>
    <script src="../Scripts/popper.min.js"></script>
     <script src="../assets/js/bootstrap-3.3.7.min.js"></script>
   
    <%-- <script src="../Scripts/jquery-3.5.0.js"></script>
    <script src="../assets/js/jquery.min.js"></script>
   --%>
    <style>
        .bs-example {
            margin: 20px;
        }

       a {
            color: #7460EE;
        }

       
         .modal-lg {
            max-width: 800px !important;
        }
         
    </style>
   
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="container-fluid">
        <div class="row ">
             <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                  <div class="card" style="width: 100%;">
                    <div class="card-body">
                            <div class="table table-responsive">
                                <table style="width: 100%; font-size:small">
                                    <thead>
                                        <tr>
                                            <td >
                                                    <asp:Label runat="server" ID="Label3" CssClass="control-label" Text="Society Name :" Font-Size="Medium" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblsocname" CssClass="control-label" Text="" Font-Size="Medium"></asp:Label>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td >
                                                    <asp:Label runat="server" ID="Label4" CssClass="control-label" Text="Society Address :" Font-Size="Medium" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblsocaddr" CssClass="control-label" Text="" Font-Size="Medium"></asp:Label>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td >
                                                    <asp:Label runat="server" ID="Label5" CssClass="control-label" Text="Society District :" Font-Size="Medium" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblsocdistrict" CssClass="control-label" Text="" Font-Size="Medium"></asp:Label>
                                            </td>
                                        </tr>
                                    </thead>
                                </table>
                               
                            </div>
                       
                    </div>
                </div>

                <div class="card" style="width: 100%;">
                    <h5 class="card-header text-white" style="background-color:#17a2b8; font-weight:bold">Other Services</h5>                    
                    <div class="card-body">                      
                                            
                        
                            <div class="table-responsive">
                                
                                     <table class="table table-striped">
                                       
                                        <tbody>
                                            <tr >
                                                <td>1.</td>
                                                <td>Amendment</td>
                                                <td>
                                                    <asp:LinkButton ID="LkAmendment" CssClass="btn btn-primary" OnClick="LkAmendment_Click" runat="server"><i class="fa fa-arrow-right" aria-hidden="true"></i>&nbsp;Goto</asp:LinkButton></td>
                                            </tr>
                                            <tr>
                                                <td>2.</td>
                                                <td>Return Filing</td>
                                                <td><asp:LinkButton ID="LkReturnFiling" CssClass="btn btn-primary" OnClick="LkReturnFiling_Click" runat="server"><i class="fa fa-arrow-right" aria-hidden="true"></i>&nbsp;Goto</asp:LinkButton></td>                                
                                            </tr>                                            
                                        </tbody>
                                     </table>

                                   <%-- <table style="width: 100%; font-size:small">
                                        <thead>
                                            
                                            <tr id="fetcholdmems" runat="server">
                                                <td class="a">
                                                     <asp:Label runat="server" ID="LblFetchmembers" CssClass="col-sm-3 col-form-label mylabel" Text="Old Ammend" Font-Size="Medium" Font-Bold="true"></asp:Label></td>
                                                <td>
                                                    <asp:FileUpload ID="FileUpload2" runat="server" />
                                                </td>
                                                 
                                            </tr>
                                           
                                            <tr>
                                               <td class="a" >
                                                     <asp:Label runat="server" ID="Label1" CssClass="col-sm-3 col-form-label mylabel" Text="New Ammend" Font-Size="Medium" Font-Bold="true"></asp:Label></td>
                                                <td>
                                                    <asp:FileUpload ID="FileUpload3" runat="server" />
                                                </td>
                                            </tr>
                                        </thead>
                                    </table>--%>

                              
                                   



                                     
                               
                            </div>
                       
                    </div>
                </div>

                
            </div>
        </div>
    </div>
</asp:Content>
