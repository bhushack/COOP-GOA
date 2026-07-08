<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DSCRegister.aspx.cs" Inherits="GoaSocietyRegistration.Organization.Digital.DSCRegister" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DSC Register</title>
    <link rel="icon" href="../../Images/favicon.ico" type="image/gif" sizes="16x16" />
    <script src="../../Scripts/jquery-3.5.0.js"></script>
    <script src="../../Scripts/dscsignRegister.js"></script>
    <script src="../../Admin/assets/adminstyle/vendor/bootstrap/js/popper.js"></script>
    <link href="../../AssestsLogin/CSS/fontawesome.min.css" rel="stylesheet" />
    <script src="../../Admin/assets/adminstyle/vendor/bootstrap/js/bootstrap.js"></script> 
    <link href="../../AssestsLogin/CSS/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript">
        $(document).ready(function () {
            $(".sub").click(function () {
                $('input:checkbox').not(this).prop('checked', this.checked);
            });
            $(".btn-sign").attr("disabled", "true");
            $(".btn-sign").text("Please wait...");
            $(".sign-status").hide();
            DSCChecker(function (exists, newversion) {
                if (exists) {
                    if (newversion == "") {
                        $(".btn-sign").removeAttr("disabled");
                        $(".sign-status").hide();
                        $(".sign-status").removeClass("callout").removeClass("callout-success");
                    }
                    else {
                        $(".btn-sign").attr("disabled", "true");
                        $(".sign-status").show();
                        $(".sign-status").html("A new version of DSC Sign software is available. Please click <a href=\"../../Download/DSCSign.zip\">here to download and install</a>.");
                        $(".sign-status").addClass("callout").addClass("callout-danger");
                    }
                }
                else {
                    $(".btn-sign").attr("disabled", "true");
                    $(".sign-status").show();
                    $(".sign-status").text("DSC Sign Service is not available. Check whether DSC Sign software is intalled on your system. If already installed, double click the 'DSC Sign' icon on your desktop and refresh this page. If not installed, go to 'About & Help' section on the left side menu of this application and download the setup and install. Note that this feature will work only on computer systems and not on mobile and tablet devices.");
                    $(".sign-status").addClass("callout").addClass("callout-danger");
                }
                $(".btn-sign").text("DSC-Register");
                $(".btn-sign").removeAttr("disabled");
            });
            $(".btn-sign").click(function (e) {
                if ($(".sub:checked").length == 0) {
                    alert("Please select I accept.");
                    e.preventDefault();
                    return;
                }
                e.preventDefault();
                if (confirm("I hereby certify that, I have checked all the details in the applications and no other changes are required and can proceed for registering my DSC.")) {
                    $("#signModal").modal({
                        backdrop: 'static',
                        keyboard: false
                    });
                    $("#signModal").modal("show");
                    $(".sign-status").hide();
                    $(this).attr("disabled", "true");
                    stack = $(".sub:checked").map(function () {
                        return $(this).val();
                    }).toArray();
                    stackTotal = stack.length;
                    startTask();
                }
            });
        });

    </script>

    <style>
        .container_abc {
            padding-left: 5px !important;
            padding-right: 5px !important;
        }

        .all {
            visibility: hidden;
        }
    </style>
   
    <script type="text/javascript">
        window.history.forward();
        function noBack() {
            window.history.forward();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">

        <div class="container-fluid" id="wrapper">

            <div class="row">

              
                <main class="col-xs-12 col-sm-8 col-lg-9 col-xl-10 ml-auto">
                            
                    <header class="page-header row justify-center">
                        <div class="col-md-6 col-lg-8">
                            <%--<h1 class="float-left text-center text-md-left">Cards</h1>--%>
                            <h4 style="align-content: center; color: #343a40">SOCIETY REGISTRATION (Department of Registration, Goa)</h4>

                        </div>

                        <div class="clear"></div>
                    </header>

                    <div class="container_abc">
                        <div class="row"> <table class="table">
                                <tbody class="">
                                    <tr>
                                        <th scope="col">Username</th>
                                        <th scope="col"><asp:TextBox ID="TextBox1" CssClass="form-control"  Enabled="false" runat="server"></asp:TextBox></th>
                                         <th scope="col">Designation</th>
                                        <th scope="col"><asp:TextBox ID="TextBox2" CssClass="form-control" Text=""  Enabled="false" runat="server"></asp:TextBox></th>
                                    </tr>
                                </tbody>
                               
                            </table>
                            <div class="col-md-5" style="height: 600px;">
                                <asp:Repeater ID="Repeater1" runat="server">
                                    <HeaderTemplate>
                                        <table class="table  dataTable table-hover table-borderless" role="grid" aria-describedby="dt-table_info" style="table-layout: fixed; word-wrap: break-word;">
                                            <thead>
                                                <tr>
                                                    <th>
                                                        <input type="checkbox" class="all" />
                                                    </th>
                                                    <th></th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <input type="checkbox" class="sub" value="<%#DataBinder.Eval(Container,"DataItem.Id")%>" />

                                            </td>
                                            <td>
                                                <%#DataBinder.Eval(Container,"DataItem.Name")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </tbody>
                    </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                            <div class="col-md-7" style="height: 600px;">
                                <div class="table table-borderless">

                                    <div>
                                        <br />
                                        <br />
                                        <div class="sign-status"></div>
                                        <br />
                                        <div class="row">
                                        <button class="btn  btn-flat btn-success btn-sign">DSC-Register</button><br />
                                             <asp:LinkButton ID="LinkButton2"  Visible="false" CssClass="btn btn-info" style="text-decoration:none;margin-left:3px!important"  runat="server"><i class="fa fa-home" aria-hidden="true"></i>&nbsp;Dashboard</asp:LinkButton><br />
                                         <a href="../Dashboard.aspx" class="btn btn-warning">Dashboard</a>
                                            <asp:LinkButton ID="LinkButton1" OnClick="LinkButton1_Click" CssClass="btn btn-primary" style="text-decoration:none;margin-left:3px!important"  runat="server"><i class="fa fa-sign-out" aria-hidden="true"></i>&nbsp;Signout</asp:LinkButton>
                                         
                                            </div>
                                        <br />
                                    </div>
                                </div>
                            </div>
                            <br /> 
                        </div>
                    </div>
                </main>
            </div>
        </div> 
    </form>
</body>
</html>
