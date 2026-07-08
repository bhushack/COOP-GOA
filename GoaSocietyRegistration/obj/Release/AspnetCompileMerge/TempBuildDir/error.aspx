<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="error.aspx.cs" Inherits="GoaSocietyRegistration.error" %>

<!DOCTYPE html>

<html lang="en-US" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Error Page</title>
    <link rel="icon" href="assets/images/favicon.ico" type="image/x-icon" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <link href="assets/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .center {
            margin: auto;
            width: 50%;
            padding: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <div class="col-12">
                    <img src="assets/images/error.png" class="img-fluid" style="background-position: center; background-repeat: no-repeat; width: 100%; height: auto; background-size: cover;" />
                    <div class="center">
                        <asp:Button ID="Button1" CssClass="btn btn-primary mx-auto d-block"  runat="server" Text="Logout" Style="width: 100px;" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
<script src="assets/js/jquery-3.2.1.slim.min.js"></script>
<script src="assets/js/popper.min.js"></script>
<script src="assets/js/bootstrap.min.js"></script>
</html>
