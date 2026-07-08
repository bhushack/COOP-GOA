<%@ Page Title="" Language="C#" MasterPageFile="~/Organization/admin.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="GoaSocietyRegistration.Organization.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../Scripts/jquery-3.5.0.min.js"></script>
    <script src="../Scripts/aes.js"></script>
    <script src="../Scripts/encrypt.js"></script>
    <style>
        .right {
            float: right;
        }

        .bs-example {
            margin: 20px;
        }

        .society_list {
            background-color: #6c53dc !important;
            color: #ffffff;
            padding: 8px;
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />

    <div class="container-fluid">
        <section class="row">
            <div class="col-sm-12">
                <section class="row">
                    <div class="col-lg-12 mb-12 bg-default">
                        <div class="society_list">Change Password</div>
                        <div class="card">
                            <div class="card-block">

                                <div class="form-group row">
                                    <label class="col-md-3 col-form-label">
                                        <asp:Label ID="Labeluserid" runat="server" Text="User Id"></asp:Label></label>
                                    <div class="col-md-9">
                                        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>

                                    </div>
                                </div>

                                <div class="form-group row">
                                    <label class="col-md-3 col-form-label">
                                        <asp:Label ID="Labelcurrentpass" runat="server" Text="Current Password*"></asp:Label></label>
                                    <div class="col-md-9">
                                        <asp:TextBox ID="TxtBxPassword" AutoPostBack="true" TextMode="Password" CssClass="form-control" OnTextChanged="TxtBxPassword_TextChanged" ToolTip="Current Password" placeholder="Current Password" autocomplete="off" runat="server"></asp:TextBox>

                                        <%--<asp:HiddenField runat="server" ID="HdCurrentPassword" />--%>
                                        <asp:HiddenField runat="server" ID="HdShaCurrentPass" />

                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-md-3 col-form-label">New Password<span class="text-danger">*</span></label>
                                    <div class="col-md-9">
                                        <asp:TextBox ID="TxtbxNewPassword" Enabled="false" CssClass="form-control" TextMode="Password" ToolTip="New Password" placeholder="New Password" autocomplete="off" runat="server"></asp:TextBox>
                                        <%-- <asp:HiddenField runat="server" ID="HDPassword" />--%>
                                        <asp:HiddenField runat="server" ID="HDShaPass" />
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <label class="col-md-3 col-form-label">Confirm Password<span class="text-danger">*</span></label>
                                    <div class="col-md-9">
                                        <asp:TextBox ID="TxtBxConfirmPassowrd" Enabled="false" CssClass="form-control" TextMode="Password" ToolTip="Confirm Password" placeholder="Confirm Password" autocomplete="off" runat="server"></asp:TextBox>
                                        <%--   <asp:HiddenField runat="server" ID="HDCnfPassword" />--%>
                                        <asp:HiddenField runat="server" ID="HDShaCnfPass" />
                                        <span id="cphMain_lblNote">Password should contians<br />
                                            At least one upper case letter (A – Z)<br />
                                            At least one lower case letter: (a - z)<br />
                                            At least one number: (0 - 9)<br />
                                            At least one Special Characters: ( @ ! # $ % &amp; ^ + = * ( ) - . : ? [ ] _ | { } ~ )<br />
                                            Optimum length is between 8 and 15<br />
                                            Consecutive Digits are not allowed
                                        </span>
                                        <br />
                                    </div>
                                </div>
                                <asp:Button ID="BtnChangePassword" OnClick="BtnChangePassword_Click" CssClass="btn btn-success right" runat="server" Text="Update Password" />
                            </div>

                        </div>
                    </div>
                </section>
            </div>
        </section>
    </div>
       <script type="text/javascript">
            function CheckPassword(inputtxt) {
                var passw = /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@!#$%&^+=*()-.:?[\]_|{}~])[a-zA-Z0-9@!#$%&^+=*()-.:?[\]_|{}~]{8,15}$/;
                if (inputtxt.match(passw)) {
                    if (hasConsecutiveDigits(inputtxt)) {
                        alert('Password should not contain 2 consecutive digits');
                        return false;
                    }
                    else {
                        return true;
                    }
                }
                else {
                    alert('Password should contians At least 1 upper case, 1 lower case, 1 special character and 1 number required. Optimum length is between 8 and 15');
                    return false;
                }
            };
            function isSequential(digits) {
                for (let i = 0; i < digits.length - 1; i++) {
                    if (parseInt(digits[i + 1]) !== parseInt(digits[i]) + 1) {
                        return false;
                    }
                }
                return true;
            }
            function hasConsecutiveDigits(password) {
                // Regular expression to match two consecutive digits
                const regex = /\d{2}/;
                let match;

                while ((match = regex.exec(password)) !== null) {
                    let digits = match[0];

                    // Check if the digits are sequential
                    if (isSequential(digits)) {
                        return true;
                    }
                    else {
                        return false;
                    }

                }
            }
        </script>
    <script>
        $(document).ready(function () {

            $('#<%=TxtBxPassword.ClientID %>').click(function () {
                this.select();
            });



            $('#<%=TxtBxPassword.ClientID %>').change(function () {
                var txtcurrentpassword = document.getElementById("<%=TxtBxPassword.ClientID %>").value.trim();
                var errUname = $('#<%=Label1.ClientID %>');
                if (txtcurrentpassword == "") errUname.html("Required !");
                else {
                    errUname.html("");
                    EncryptUserIdentity("LN");
                }
            });

            $('#<%=TxtbxNewPassword.ClientID %>').click(function () {
                this.select();
            });

            $('#<%=TxtbxNewPassword.ClientID %>').change(function () {
                var txtpassword1 = document.getElementById("<%=TxtbxNewPassword.ClientID %>").value.trim();
                var errPass = $('#<%=Label1.ClientID %>');
                if (txtpassword1 == "") errPass.html("Required !");
                else {
                    errPass.html("");
                    EncryptUserIdentity("PW");
                }
            });
            $('#<%=TxtBxConfirmPassowrd.ClientID %>').click(function () {
                this.select();
            });

            $('#<%=TxtBxConfirmPassowrd.ClientID %>').change(function () {
                var txtpassword2 = document.getElementById("<%=TxtBxConfirmPassowrd.ClientID %>").value.trim();
                var errPass = $('#<%=Label1.ClientID %>');
                if (txtpassword2 == "") errPass.html("Required !");
                else {
                    errPass.html("");
                    EncryptUserIdentity("CPW");
                }
            });
        });//End ready()

    </script>
    <script type="text/javascript">

        function EncryptUserIdentity(inputIdentifier) {
            //Encrypt Credentials
            var key = CryptoJS.enc.Utf8.parse('<%=Session["Enc_Random"]%>');
                    var iv = CryptoJS.enc.Utf8.parse('<%=Session["Enc_Vector"]%>');


                    if (inputIdentifier == "LN") {
                        var txtcurrentpassword = document.getElementById("<%=TxtBxPassword.ClientID %>").value.trim();
                $('#<%=HdShaCurrentPass.ClientID %>').val(SHA512('<%=Session["Sess_rndNo"]%>' + SHA512(txtcurrentpassword)));
                var rndU = Math.random().toString(36) + Math.random().toString(36) + Math.random().toString(36) + Math.random().toString(36);
                document.getElementById("<%=TxtBxPassword.ClientID %>").value = rndU;
            }


            else if (inputIdentifier == "PW") {
                var txtpassword1 = document.getElementById("<%=TxtbxNewPassword.ClientID %>").value.trim();
                $('#<%=HDShaPass.ClientID %>').val(SHA512(txtpassword1));

                var rndP = Math.random().toString(36) + Math.random().toString(36) + Math.random().toString(36) + Math.random().toString(36);
                document.getElementById("<%=TxtbxNewPassword.ClientID %>").value = rndP;
            }
            else if (inputIdentifier == "CPW") {
                var txtpassword2 = document.getElementById("<%=TxtBxConfirmPassowrd.ClientID %>").value.trim();
                var temp1 = CheckPassword(txtpassword2);
                if (Boolean(temp1)) {
                    $('#<%=HDShaCnfPass.ClientID %>').val(SHA512(txtpassword1));

                    var rndC = Math.random().toString(36) + Math.random().toString(36) + Math.random().toString(36) + Math.random().toString(36);
                    document.getElementById("<%=TxtBxConfirmPassowrd.ClientID %>").value = rndC;
                } else {

                    document.getElementById("<%=TxtBxConfirmPassowrd.ClientID %>").value = "";
                }
            } 
}
    </script>



    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="myModal" class="modal fade">
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
</asp:Content>
