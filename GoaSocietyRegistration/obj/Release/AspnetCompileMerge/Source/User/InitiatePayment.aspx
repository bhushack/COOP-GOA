<%@ Page Title="" Language="C#" MasterPageFile="~/User/Society.Master" AutoEventWireup="true" CodeBehind="InitiatePayment.aspx.cs" Inherits="GoaSocietyRegistration.InitiatePayment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <script src="assets/js/jquery.min.js"></script>
  
    <style>
        .bs-example {
            margin: 20px;
        }
    </style>

     <script type="text/javascript">
        window.onload = function () {
            noBack();
        }
        function noBack() {
            window.history.forward();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" OnClientClick="CoverClick();" />

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="myModal" class="modal fade">
            <div class="modal-dialog  modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="lblMSG1" runat="server" Text="" ForeColor="Green"></asp:Label>
                    </div>
                    <div class="modal-footer">                                                                      
                          <asp:Button ID="btnCancel" runat="server" class="btn btn-primary" data-dismiss="modal" ></asp:Button>
                          <asp:Button ID="RedirecttoLoginBtn" CssClass="btn btn-primary" runat="server" OnClick="RedirecttoLoginBtn_Click"  />

                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

