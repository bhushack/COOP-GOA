<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="momo.aspx.cs" Inherits="GoaSocietyRegistration.momo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <asp:GridView ID="vegmomo" runat="server" OnRowDataBound="vegmomo_RowDataBound" AutoGenerateColumns="false" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:TemplateField HeaderText="Sr. No">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblRowNumber" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="socname" HeaderText="Name of Society" />
                <asp:BoundField DataField="applicant_name" HeaderText="Name of person" />
                <asp:BoundField DataField="applicant_mobile_no" HeaderText="Contact No" />
                <asp:BoundField DataField="applicant_email" HeaderText="Contact Email" />
                <asp:BoundField DataField="applicant_address" HeaderText="Address" />
                <asp:BoundField DataField="application_submission_time" HeaderText="Date of service avialed"  DataFormatString="{0:dd/MM/yyyy}" />
            </Columns>
        </asp:GridView>
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
    </form>
</body>
</html>
