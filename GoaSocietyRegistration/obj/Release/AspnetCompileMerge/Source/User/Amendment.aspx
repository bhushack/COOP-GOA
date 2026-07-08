<%@ Page Title="" Language="C#" MasterPageFile="~/User/Society.Master" AutoEventWireup="true" CodeBehind="Amendment.aspx.cs" Inherits="GoaSocietyRegistration.User.Amendment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Cache-control" content="no-cache" />
    <meta http-equiv="Cache-control" content="no-store" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="expires" content="0" />
       <%-- <script src="../Scripts/jquery-3.5.0.js"></script>--%>
  

    <style type="text/css">
        .bs-example {
            margin: 20px;
        }

        .a
        {
            width:20%;
        }

        .b
        {
            width:70%;
        }

        .c
        {
            width:30%;
        }
       
    </style>
      

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row">
            <div class="col-lg-12  col-md-12  col-sm-12 col-xs-12">   
                
                 <div class="card border-warning" style="width: 100%" id="edit_application" visible="true" runat="server">
                    
                        <h5 class="card-header text-white" style="background-color:#ebb434"><b>Amendment Application Status</b></h5>
                    
                    <div class="card-body">
                        <div class="form-group">
                            <asp:Label ID="lblimpnotice" runat="server" ForeColor="#0727bf" Font-Size="Large" Font-Bold="true" Text="Amendment form has been filled partially/not yet submitted. Please Upload the necessary Documents and Submit."></asp:Label><br />
                            <asp:Label ID="observation_remarks" runat="server" ForeColor="Red" Font-Size="Large" Font-Bold="true" Visible="false"></asp:Label>

                            <br />
                        </div>                       
                    </div>
                </div>             

                <div class="card" style="width: 100%;">
                    <h5 class="card-header text-white" style="background-color:#4d9f72; font-weight:bold">Amendment</h5>
                    <div class="card-body" >                        
                        
                         <ul class="nav nav-tabs mt-4">
                            <li class="nav-item">
                                <a href="#changesocname" class="nav-link active" data-toggle="tab"><strong>Change Society Name</strong> </a>
                            </li>
                            <li class="nav-item">
                                <a href="#changemangcomm" class="nav-link" data-toggle="tab"><strong>Change Managing Committee</strong></a>
                            </li>
                              <li class="nav-item">
                                <a href="#changeobjectives" class="nav-link" data-toggle="tab"><strong>Change Objectives</strong></a>
                            </li>
                             <li class="nav-item">
                                <a href="#docsupload" class="nav-link" data-toggle="tab"><strong>Documents Upload</strong></a>
                            </li>
                             <li class="nav-item">
                                <a href="#history" class="nav-link" data-toggle="tab"><strong>Application History</strong></a>
                            </li>
                        </ul>
                        <br />

                         <div class="row" id="socdetails" runat="server">
                             <table style="width: 100%; font-size:13px;" class=" table table-borderless">
                                         <tbody>
                                        <tr>
                                            <td class="a">
                                                    <asp:Label runat="server" ID="Label18"  Text="Registration ID :"  Font-Bold="true"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblregid" Text="" ></asp:Label>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="a">
                                                    <asp:Label runat="server" ID="Label20" Text="Registration Date :"  Font-Bold="true"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblregdate"  Text="" ></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="a">
                                                    <asp:Label runat="server" ID="Label9" Text="Society Name :"  Font-Bold="true"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblsocname"  Text=""></asp:Label>
                                            </td>
                                        </tr>

                                     <%-- <tr>
                                            <td class="a"  >
                                                    <asp:Label runat="server" ID="Label16"  Text="Society Address :"  Font-Bold="true"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblsocaddr"  Text="" ></asp:Label>
                                            </td>
                                        </tr>--%>

                                        <tr>
                                            <td class="a" >
                                                    <asp:Label runat="server" ID="Label17" Text="Society District :"  Font-Bold="true"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label runat="server" ID="lblsocdistrict" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        
                                    </tbody>
                                        </table>
                        </div>

                        <div class="tab-content">
                            <br />
                             <div class="tab-pane fade show active" id="changesocname"></div>

                            <div class="tab-pane fade" id="changemangcomm">                                 
                                <div class=" justify-content-center p-2" style="text-align: center; width: 100%; margin: auto">
                                    <div class="text-center" style="background-color:#c8e6d8">
                        <asp:Label ID="Label24" runat="server" Font-Bold="true" ForeColor="#484ba5" Font-Size="Large" Text="EXISTING MANAGING COMMITTEE DETAILS" ></asp:Label>
                     </div>

                                    <asp:GridView ID="gv_exisitingmangcomm" runat="server" Width="100%"  CellPadding="3" OnRowDataBound="gv_exisitingmangcomm_RowDataBound" AutoGenerateColumns="false"  Font-Size="Small"  CssClass="Grid mt-4" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true">
                                        <AlternatingRowStyle BackColor="White" />
                                        <FooterStyle BackColor="#CCCC99" />
                                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />                                       
                                        <RowStyle BackColor="#F7F7DE" />
                                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />                                        
                                        <Columns>
                                             <asp:TemplateField HeaderText="Select">
                                                <ItemTemplate>    
                                                    <asp:CheckBox ID="chkbx" Visible="true" runat="server" CausesValidation="false" Text=" " Font-Size="Small"/>
                                                </ItemTemplate>
                                            </asp:TemplateField>    
                                            <asp:TemplateField HeaderText="Sr. No">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblRowNumber2" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField HeaderText="Salutation" DataField="salutation" />
                                            <asp:BoundField HeaderText="Name" DataField="fname" />
                                            <asp:BoundField HeaderText="Gender" DataField="gender" />
                                            <asp:BoundField HeaderText="Age" DataField="age" />
                                            <asp:BoundField HeaderText="Designation" DataField="designtaion" />
                                            <asp:BoundField HeaderText="Occupation" DataField="occupation" />
                                            <asp:BoundField HeaderText="Address" DataField="address" />
                                            <asp:BoundField HeaderText="ID Proof" DataField="proofname" />                                            
                                            <asp:BoundField HeaderText="Proof Document No" DataField="proof_document_no" />
                                            <asp:BoundField HeaderText="Date of admission" DataField="dateofadmission" DataFormatString="{0:dd/MM/yyyy}" />
                                            <asp:BoundField HeaderText="Remarks" DataField="remarks" />                                          
                                           <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                     <asp:Label ID="lbmemberid" runat="server" Text='<%# Eval("member_id") %>'></asp:Label>
                                                    <asp:Label ID="lbAppid" runat="server" Text='<%# Eval("app_id") %>'></asp:Label>
                                                    <asp:Label ID="lbdesign" runat="server" Text='<%# Eval("design") %>'></asp:Label>
                                                    <asp:Label ID="lboccupatid" runat="server" Text='<%# Eval("occupatid") %>'></asp:Label>
                                                     <asp:Label ID="lbproofid" runat="server" Text='<%# Eval("proofid") %>'></asp:Label>
                                                    <asp:Label ID="lbmangcomm" runat="server" Text='<%# Eval("mangcomm") %>'></asp:Label>
                                                    <asp:Label ID="lbactive" runat="server" Text='<%# Eval("active") %>'></asp:Label>
                                                    <asp:Label ID="lbdocid" runat="server" Text='<%# Eval("doc_id") %>'></asp:Label>
                                                     <asp:Label ID="lbsalutationid" runat="server" Text='<%# Eval("salutation_id") %>'></asp:Label>
                                                    <asp:Label ID="lbdesigntaion_others" runat="server" Text='<%# Eval("designtaion_others") %>'></asp:Label>
                                                    <asp:Label ID="lboccupation_others" runat="server" Text='<%# Eval("occupation_others") %>'></asp:Label>
                                                    <asp:Label ID="lbdocument_mongoentry" runat="server" Text='<%# Eval("document_mongoentry") %>'></asp:Label>

                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="View ID">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="LbView_existing" CausesValidation="false" runat="server" OnClick="LbView_existing_Click"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton>
                                                    <asp:HiddenField ID="hfmemID" Value='<%# Eval("member_id") %>' runat="server" />
                                                    <asp:HiddenField ID="hfobjectID" Value='<%# Eval("document_mongoentry") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                                                                   
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <div align="center">No records found.</div>
                                        </EmptyDataTemplate>
                                    </asp:GridView>                                                                

                                  <asp:LinkButton ID="LKAddMembertoNew" CssClass="btn btn-outline-info mt-4" CausesValidation="false" OnClick="LKAddMembertoNew_Click" runat="server"><i class="fa fa-arrow-circle-down" aria-hidden="true"></i>&nbsp;Add Selected Members to New Committee</asp:LinkButton>
                                    
                                  <br />
                                
                                    <div class="text-center mt-4" style="background-color:#c8e6d8">
                        <asp:Label ID="Label25" runat="server" Font-Bold="true" ForeColor="#484ba5" Font-Size="Large" Text="NEW MANAGING COMMITTEE DETAILS" ></asp:Label>
                     </div>

                                <asp:LinkButton ID="LkAddNewMember" CssClass="btn btn-primary mt-4" CausesValidation="false" OnClick="LkAddNewMember_Click" runat="server"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add New Committee Member</asp:LinkButton>
                                 

                                <asp:GridView ID="gv_newmangcomm" runat="server" Width="100%" CellPadding="3" OnRowDataBound="gv_newmangcomm_RowDataBound" AutoGenerateColumns="false"  Font-Size="Small"  CssClass="Grid mt-4" AlternatingRowStyle-CssClass="alt" PagerStyle-CssClass="pgr" EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true">
                                        <AlternatingRowStyle BackColor="White" />
                                        <FooterStyle BackColor="#CCCC99" />
                                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />                                       
                                        <RowStyle BackColor="#dad4f9" />
                                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />                                        
                                        <Columns>
                                            <asp:TemplateField HeaderText="Sr. No">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblRowNumber2" Text='<%#Container.DataItemIndex + 1 %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField HeaderText="Salutation" DataField="salutation" />
                                            <asp:BoundField HeaderText="Name" DataField="fname" />
                                            <asp:BoundField HeaderText="Gender" DataField="gender" />
                                            <asp:BoundField HeaderText="Age" DataField="age" />
                                            <asp:BoundField HeaderText="Designation" DataField="designtaion" />
                                            <asp:BoundField HeaderText="Occupation" DataField="occupation" />
                                            <asp:BoundField HeaderText="Address" DataField="address" />
                                            <asp:BoundField HeaderText="ID Proof" DataField="proofname" />                                            
                                            <asp:BoundField HeaderText="Proof Document No" DataField="proof_document_no" />
                                            <asp:BoundField HeaderText="Date of admission" DataField="dateofadmission" DataFormatString="{0:dd/MM/yyyy}" />
                                            <asp:BoundField HeaderText="Remarks" DataField="remarks" />                                          
                                           <asp:TemplateField HeaderText="Edit">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="LBEdit" CausesValidation="false" runat="server" OnClick="LBEdit_Click"><i class="far fa-edit" ></i></asp:LinkButton>
                                                        <asp:HiddenField ID="hfdesignid" Value='<%# Eval("design") %>' runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Delete">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="LBDelete" CausesValidation="false" runat="server" OnClick="LBDelete_Click"><i class="fa fa-trash"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            <asp:TemplateField HeaderText="View ID">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="LbView_new" CausesValidation="false" runat="server" OnClick="LbView_new_Click"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton>
                                                    <asp:HiddenField ID="hfmemID" Value='<%# Eval("member_id") %>' runat="server" />
                                                    <asp:HiddenField ID="hfobjectID" Value='<%# Eval("document_mongoentry") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <div align="center">No records found.</div>
                                        </EmptyDataTemplate>
                                    </asp:GridView>   
                                    
                                   
                                
                                 
                              </div>      
                            </div>

                            
                            <div class="tab-pane fade" id="changeobjectives"></div>

                            <div class="tab-pane fade" id="docsupload">
                                 <div class="row p-2" id="documents" runat="server">
                                    <div class="table-responsive">
                                                       
                                      
                                            <h6 class="card-header" style="font-weight:bold;text-align:center;">Documents Upload</h6>
                                            <table style="width: 100%; font-size:13px" class="table table-borderless">
                                                <thead style="font-weight:bold; background-color:#919192; color:white">
                                                    <tr>
                                                        <th>
                                                            <asp:Label ID="lbsrno" runat="server" Text="Sr. No."></asp:Label></th>                                               
                                                        <th>
                                                            <asp:Label ID="lbheader" runat="server" Text="Name of Document"></asp:Label></th>
                                                        <th>
                                                            <asp:Label ID="LBUpload" runat="server" Text="Document Upload"></asp:Label></th>
                                                        <th>
                                                            <asp:Label ID="Label12" runat="server" Text=""></asp:Label></th>
                                                        <th>
                                                            <asp:Label ID="Label13" runat="server" Text=""></asp:Label></th>
                                                        <th>
                                                            <asp:Label ID="Labelpdf" runat="server" Text=""></asp:Label></th>
                                                        <th>
                                                            <asp:Label ID="lbStatus" runat="server" Text="Status"></asp:Label></th>
                                                    </tr>
                                                </thead>
                                                <tbody style="border-color:white">
                                                     <tr>
                                                        <td>
                                                            <asp:Label ID="lb1" runat="server" Text="1"></asp:Label></td>
                                                
                                                        <td>
                                                            <asp:Label ID="Label1" runat="server" Text="Original Byelaws"></asp:Label><span class="text-danger">*</span></td>

                                                        <td>
                                                            <asp:FileUpload ID="FileUpload1" runat="server" /></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_OrgByelaws_Upload" runat="server" ToolTip="Upload File" OnClick="LB_OrgByelaws_Upload_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_OrgByelaws_Delete" runat="server" ToolTip="Delete File" OnClick="LB_OrgByelaws_Delete_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_OrgByelaws_View" runat="server" OnClick="LB_OrgByelaws_View_Click"  ToolTip="View File" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:RegularExpressionValidator ID="revfileupload1" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload1" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbfu1status" runat="server"></asp:Label></td>
                                                    </tr>
                                            
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lb2" runat="server" Text="2"></asp:Label></td>
                                                
                                                        <td>
                                                            <asp:Label ID="Label4" runat="server" Text="Statement of Changes"></asp:Label><span class="text-danger">*</span></td>
                                                        <td>
                                                            <asp:FileUpload ID="FileUpload2" runat="server" /></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_StatementofChanges_Upload" runat="server" ToolTip="Upload File" OnClick="LB_StatementofChanges_Upload_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_StatementofChanges_Delete" runat="server" ToolTip="Delete File" OnClick="LB_StatementofChanges_Delete_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_StatementofChanges_View" runat="server" OnClick="LB_StatementofChanges_View_Click"  ToolTip="View File" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:RegularExpressionValidator ID="revfileupload2" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload2" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbfu2status" runat="server"></asp:Label></td>
                                                    </tr>
                                           
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="Label2" runat="server" Text="3"></asp:Label></td>
                                                
                                                        <td>
                                                            <asp:Label ID="Label3" runat="server" Text="AGM Notice"></asp:Label><span class="text-danger">*</span></td>
                                                        <td>
                                                            <asp:FileUpload ID="FileUpload3" runat="server" /></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_AgmNotice_Upload" runat="server" ToolTip="Upload File" OnClick="LB_AgmNotice_Upload_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_AgmNotice_Delete" runat="server" ToolTip="Delete File" OnClick="LB_AgmNotice_Delete_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_AgmNotice_View" runat="server" OnClick="LB_AgmNotice_View_Click"  ToolTip="View File" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:RegularExpressionValidator ID="revfileupload3" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload3" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbfu3status" runat="server"></asp:Label></td>
                                                    </tr>

                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="Label5" runat="server" Text="4"></asp:Label></td>
                                                
                                                        <td>
                                                            <asp:Label ID="Label6" runat="server" Text="Support of AGM Resolution"></asp:Label><span class="text-danger">*</span></td>
                                                        <td>
                                                            <asp:FileUpload ID="FileUpload4" runat="server" /></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_AgmResolution_Upload" runat="server" ToolTip="Upload File" OnClick="LB_AgmResolution_Upload_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_AgmResolution_Delete" runat="server" ToolTip="Delete File" OnClick="LB_AgmResolution_Delete_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_AgmResolution_View" runat="server" OnClick="LB_AgmResolution_View_Click"  ToolTip="View File" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:RegularExpressionValidator ID="revfileupload4" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload4" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbfu4status" runat="server"></asp:Label></td>
                                                    </tr>

                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="Label7" runat="server" Text="5"></asp:Label></td>
                                                
                                                        <td>
                                                            <asp:Label ID="Label8" runat="server" Text="Amendment Bye-laws"></asp:Label><span class="text-danger">*</span></td>
                                                        <td>
                                                            <asp:FileUpload ID="FileUpload5" runat="server" /></td>
                                                        <td>
                                                            <asp:LinkButton ID="Lb_AmendByelaws_Upload" runat="server" ToolTip="Upload File" OnClick="Lb_AmendByelaws_Upload_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:LinkButton ID="Lb_AmendByelaws_Delete" runat="server" ToolTip="Delete File" OnClick="Lb_AmendByelaws_Delete_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:LinkButton ID="Lb_AmendByelaws_View" runat="server" OnClick="Lb_AmendByelaws_View_Click"  ToolTip="View File" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload5" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbfu5status" runat="server"></asp:Label></td>
                                                    </tr>
                                           
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="Label10" runat="server" Text="6"></asp:Label></td>
                                                
                                                        <td>
                                                            <asp:Label ID="Label11" runat="server" Text="Revised Version"></asp:Label><span class="text-danger">*</span></td>
                                                        <td>
                                                            <asp:FileUpload ID="FileUpload6" runat="server" /></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_RevisedVersion_Upload" runat="server" ToolTip="Upload File" OnClick="LB_RevisedVersion_Upload_Click"><i class="fa fa-upload" aria-hidden="true"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_RevisedVersion_Delete" runat="server" ToolTip="Delete File" OnClick="LB_RevisedVersion_Delete_Click"><i class="fa fa-trash"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:LinkButton ID="LB_RevisedVersion_View" runat="server" OnClick="LB_RevisedVersion_View_Click"  ToolTip="View File" Enabled="false"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton></td>
                                                        <td>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload6" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><asp:Label ID="lbfu6status" runat="server"></asp:Label></td>
                                                    </tr>                                    
                                            
                                                    <tr>
                                                         <asp:Label runat="server" ID="lblError" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                    </tr>                                            
                                                    <tr id="tr_addamenddocs" runat="server">
                                                 
                                                         <td colspan="7" style="text-align:center;">
                                                            <h5 class="card-header" style="font-weight:bold">Additional Documents</h5>
                                                            <asp:GridView ID="GridView_AddAmendDocs" Width="100%" runat="server" OnRowDataBound="GridView_AddAmendDocs_RowDataBound" CellPadding="5" AutoGenerateColumns="false" AllowPaging="false"  EmptyDataText="No Record Found." ShowHeaderWhenEmpty="true" BackColor="White" BorderStyle="None"> <%-- AlternatingRowStyle-CssClass="alt"--%>
                                                            <AlternatingRowStyle BackColor="White" />
                                                            <FooterStyle BackColor="#CCCC99" />
                                                            <HeaderStyle BackColor="#919192" Font-Bold="True" ForeColor="White" />
                                                            <%--<PagerStyle BackColor="#dee3f7b0" ForeColor="Black" HorizontalAlign="Right" />--%>
                                                            <%--<RowStyle BackColor="#deeff7" />--%>
                                                            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                                                            <SortedAscendingCellStyle BackColor="#FBFBF2" />
                                                            <SortedAscendingHeaderStyle BackColor="#848384" />
                                                            <SortedDescendingCellStyle BackColor="#EAEAD3" />
                                                            <SortedDescendingHeaderStyle BackColor="#575357" />
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Sr. No">
                                                                    <ItemTemplate>
                                                                        <%#Container.DataItemIndex+1 %>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField HeaderText="Document Name" DataField="docname" />      
                                                                <asp:TemplateField HeaderText="Delete">
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="LBDelete_AddAmenddocs" OnClick="LBDelete_AddAmenddocs_Click"  CausesValidation="false" runat="server"><i class="fa fa-trash"></i></asp:LinkButton>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>                                                  
                                                                <asp:TemplateField HeaderText="View">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="LbAmendDocsView" CausesValidation="false" runat="server" OnClick="LbAmendDocsView_Click"><i class="fa fa-file-pdf" aria-hidden="true"></i></asp:LinkButton>
                                                                        <%-- <asp:ImageButton ID="View_adddocs" runat="server" OnClick="View_adddocs_Click" ImageUrl="~/assets/images/pdf.png" Width="30px" Height="30px" /></td>--%>
                                                                         <asp:HiddenField ID="hfobjectID" Value='<%# Eval("object_id") %>' runat="server" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                <div align="center">No Documents found.</div>
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                               
                                                    </td>
                                                    </tr>
                                           

                                                    <tr id="tradddocsbtn" runat="server">                                                
                                                        <td colspan="7" style="text-align:center">                                                  
                                                   
                                                            <asp:LinkButton ID="LkAddDocs" CssClass="btn btn-info" OnClick="LkAddDocs_Click" runat="server"><i class="fa fa-plus" aria-hidden="true"></i>&nbsp;Add Documents</asp:LinkButton>
                                           
                                                        </td>
                                                    </tr>
                                                    <tr>                                                
                                                        <td colspan="7" style="text-align:center">                                                  
                                                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnconfirmSubmit_Click" CssClass="btn btn-primary" />
                                                   
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                

                                
                                    </div>
                                </div>
                            </div>

                              <div class="tab-pane fade" id="history"></div>
                            </div>


                       

                       
                     

                         


                    </div>
                </div>
            </div>
        </div>
        </div>


    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="errormodal" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label15" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="myModal1" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Status" runat="server" Text="" ForeColor="Green"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--    <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                        <asp:Button ID="gotohomepage" CssClass="btn btn-primary" runat="server" Text="Ok" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="bs-example">
        <!-- Modal HTML -->
        <div id="errorModal" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #607D8B">
                        <h4 class="modal-title">
                            <asp:Label ID="Label47" runat="server" Text="Confirmation" ForeColor="White"></asp:Label>
                        </h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label48" CssClass="control-label" runat="server" Text=""></asp:Label>
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
        <div id="finalmodal" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Confirmation</h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label14" runat="server" Text="" ForeColor="OrangeRed"></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--    <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>--%>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="btnSubmitfinal" CssClass="btn btn-primary" runat="server" OnClick="btnSubmitfinal_Click" Text="Ok" />
                    </div>
                </div>
            </div>
        </div>
    </div>


     <div class="bs-example">
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
    </div>

    <div class="bs-example">
        <div class="modal" id="otherdocumentuploads">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Other Documents For Society</h4>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">
                    <div class="table-responsive" style="width: 100%">
                        <div class="table table-bordered table-hover">
                            <table style="width: 100%">
                                <thead>
                                    <tr>
                                        <td class="c">
                                            <asp:Label runat="server" ID="lbl_DocName" Text="Name of Document" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                        </td>
                                        <td class="b">
                                            <asp:TextBox runat="server" ID="txtbx_DocName" MaxLength="50" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" Display="Dynamic" ControlToValidate="txtbx_DocName" ForeColor="Red" ValidationExpression="[\sa-zA-Z]*$" ErrorMessage="Invalid Name" /><br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="a">
                                            <asp:Label ID="Label43" runat="server" CssClass="control-label" Text="Document Upload" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                        </td>
                                        <td class="c">
                                            <asp:FileUpload ID="FileUpload9" runat="server" /><br />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload9" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File file." Display="Dynamic" /><br />
                                            <asp:Label runat="server" ID="Label46" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label>
                                            <asp:Label runat="server" ID="Label49" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:Button ID="btnAddDocs" CausesValidation="false" OnClick="btnAddDocs_Click" CssClass="btn btn-primary" runat="server" Text="Add" />
                                        </td>
                                    </tr>
                                </thead>
                            </table>
                        </div>
                    </div>
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
        <div id="addmembersmodal" class="modal">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title">Member Details</h4>
                    </div>
                    <div class="modal-body">
                             <div class="table-responsive">
                                    <div class="table table-bordered table-hover">
                                        <table style="width: 100%">
                                            <thead>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="lbl_salutation" runat="server" Text="Salutation" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:DropDownList ID="ddl_salutation" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator7" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="ddl_salutation" ErrorMessage="Please Select Salutation"> </asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label runat="server" ID="lbl_MemName" Text="Full Name" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox runat="server" ID="txt_MemName" MaxLength="50" CssClass="form-control" AutoCompleteType="Disabled" onkeyup="name_changed('txt_MemName');"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvfullname" Display="Dynamic" runat="server" ControlToValidate="txt_MemName" CssClass="text-danger" ErrorMessage="Enter Full Name"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revfirstname" runat="server" Display="Dynamic" ControlToValidate="txt_MemName" ForeColor="Red" ValidationExpression="[\sa-zA-Z`]*$" ErrorMessage="Invalid Name" /><br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="lbl_gender" runat="server" Text="Gender" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:RadioButtonList ID="Rdbtngender" RepeatDirection="Horizontal" OnSelectedIndexChanged="Rdbtngender_SelectedIndexChanged" Width="100%" runat="server" AutoPostBack="true">
                                                            <asp:ListItem Value="M">Male</asp:ListItem>
                                                            <asp:ListItem Value="F">Female</asp:ListItem>
                                                            <asp:ListItem Value="T">Transgender</asp:ListItem>

                                                        </asp:RadioButtonList>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" CssClass="text-danger" Display="Dynamic" ControlToValidate="Rdbtngender" ErrorMessage="Select atleast One"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label runat="server" ID="lbl_age" Text="Age" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox runat="server" ID="txtbx_age" MaxLength="3" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" Display="Dynamic" runat="server" ControlToValidate="txtbx_age" CssClass="text-danger" ErrorMessage="Enter Age"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td class="a">
                                                        <asp:Label runat="server" ID="lbl_DesignMem" Text="Designation" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:DropDownList ID="ddl_DesignMem" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_DesignMem_SelectedIndexChanged" CssClass="form-control"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator2" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="ddl_DesignMem" ErrorMessage="Please Select Designation"> </asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr id="row_DesignOthers" runat="server" visible="false">
                                                    <td class="a">
                                                        <asp:Label runat="server" ID="lbl_DesignOthers" Text="Designation(Others)" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox ID="TxtBx_DesignOthers" CssClass="form-control" runat="server" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" Display="Dynamic" runat="server" ControlToValidate="TxtBx_DesignOthers" CssClass="text-danger" ErrorMessage="Enter Designation"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="TxtBx_DesignOthers" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\sa-zA-Z]*$" ErrorMessage="No special characters allowed." />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label runat="server" ID="lbl_MemOccup" Text="Occupation" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:DropDownList ID="ddl_MemOccup" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_MemOccup_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator1" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="ddl_MemOccup" ErrorMessage="Please Select Profession"> </asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr id="row_OccupOthers" runat="server" visible="false">
                                                    <td class="a">
                                                        <asp:Label runat="server" ID="lbl_OccupOthers" Text="Occupation(Others)" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox ID="TxtBx_OccupOthers" CssClass="form-control" runat="server" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" Display="Dynamic" runat="server" ControlToValidate="TxtBx_OccupOthers" CssClass="text-danger" ErrorMessage="Enter Other Occupation Name"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="TxtBx_OccupOthers" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\sa-zA-Z]*$" ErrorMessage="No special characters allowed." />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="lbl_MemAddress" runat="server" Text="Address:" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox ID="txt_MemAddress" CssClass="form-control" runat="server" MaxLength="100" AutoCompleteType="Disabled"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" Display="Dynamic" runat="server" ControlToValidate="txt_MemAddress" CssClass="text-danger" ErrorMessage="Enter Full Address"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revbuilding" runat="server" ControlToValidate="txt_MemAddress" CssClass="text-danger" Display="Dynamic" ValidationExpression="[\sa-zA-Z0-9()-,._/:@-]+$" ErrorMessage="No special characters allowed." />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="lbl_MemDocType" runat="server" Text="ID Proof" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>

                                                    </td>
                                                    <td class="b">
                                                        <asp:DropDownList ID="ddl_MemDocType" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddl_MemDocType_SelectedIndexChanged" CssClass="form-control"></asp:DropDownList>
                                                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator4" Display="Dynamic" runat="server" ControlToValidate="ddl_MemDocType" CssClass="text-danger" ErrorMessage="Please Select ID Proof "></asp:RequiredFieldValidator><br />
                                                        <asp:Label ID="Label16" runat="server" Text="Please do not Upload your Aadhaar Card Number as ID Proof." CssClass="control-label" ForeColor="OrangeRed"></asp:Label>

                                                    </td>
                                                </tr>
                                                <tr id="trvisible" runat="server" visible="false">
                                                    <td class="a">
                                                        <asp:Label ID="Label19" runat="server" Text="" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>

                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox runat="server" ID="TxtBxDocumentNo" AutoPostBack="true" AutoCompleteType="Disabled" OnTextChanged="TxtBxDocumentNo_TextChanged" MaxLength="50" CssClass="form-control"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" Display="Dynamic" runat="server" ControlToValidate="TxtBxDocumentNo" CssClass="text-danger" ErrorMessage="Enter Proof No"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="TxtBxDocumentNo" CssClass="text-danger" Display="Dynamic" ValidationExpression="[a-zA-Z0-9]*$" ErrorMessage="No special characters allowed." />
                                                     
                                             </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="lbl_ManComm" runat="server" Text="Managing Committee" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:DropDownList ID="ddlMcom" runat="server" CssClass="form-control" AppendDataBoundItems="True">
                                                            <asp:ListItem Text="Yes" Value="1" Selected="True"></asp:ListItem>
                                                            <%--          <asp:ListItem Text="No" Value="2"></asp:ListItem>--%>
                                                        </asp:DropDownList>
                                                        <asp:RequiredFieldValidator InitialValue="-1" ID="RequiredFieldValidator5" Display="Dynamic" runat="server" ControlToValidate="ddlMcom" CssClass="text-danger" ErrorMessage="Please Select part of Managing Committee "></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr id="tr_dateofadmission" runat="server" visible="true">
                                                    <td class="a">
                                                        <asp:Label ID="Label21" runat="server" Text="Date of Admission" CssClass="control-label" Font-Bold="true"></asp:Label><span class="text-danger">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <%--<input  name="party" min="2017-04-01" max="2017-04-20" >--%>
                                                        <asp:TextBox ID="txtbxdateadmission" type="date" ToolTip="Date of Admission" placeholder="Date of Admission" autocomplete="off" CssClass="form-control" runat="server"></asp:TextBox>

                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator12" Display="Dynamic" runat="server" CssClass="text-danger" ControlToValidate="txtbxdateadmission" ErrorMessage="Date of Admission is blank!" SetFocusOnError="True" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="Label22" runat="server" CssClass="control-label" Text="Remarks" Font-Bold="true"></asp:Label>
                                                    </td>
                                                    <td class="b">
                                                        <asp:TextBox ID="TxtBxRemarks" CssClass="form-control" runat="server" AutoCompleteType="Disabled"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="a">
                                                        <asp:Label ID="Label23" runat="server" CssClass="control-label" Text="Upload ID" Font-Bold="true"></asp:Label><span id="upload" class="text-danger" runat="server">*</span>
                                                    </td>
                                                    <td class="b">
                                                        <asp:FileUpload ID="fileuploadmember" runat="server" /><br />
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ValidationExpression="([a-zA-Z0-9\s_\\.\-:])+(.pdf)$" ControlToValidate="FileUpload6" runat="server" ForeColor="Red" ErrorMessage="Please select a valid PDF File." Display="Dynamic" /><br />
                                                        <asp:Label runat="server" ID="lbuploadmember" Text="" Width="100%" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                        <asp:Label runat="server" ID="lberrormember" Text="" Width="100%" ForeColor="Red" CssClass="alert-danger" Font-Bold="true"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:Button ID="btnUpdate" CssClass="btn btn-primary" Visible="false" runat="server" OnClick="btnUpdate_Click" Text="Update" />                                                        
                                                        <asp:Button ID="btnAdd" CssClass="btn btn-primary" runat="server" OnClick="btnAdd_Click" Text="Add" />
                                                        
                                              
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td style="color: orangered" colspan="2">* Only PDF File Type & Maximum File Size Allowed is 2 MB 
                                                    </td>

                                                </tr>

                                            </thead>
                                        </table>
                                    </div>
                                </div>
                            
                          
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="close_modal" OnClick="close_modal_Click" CausesValidation="false" CssClass="btn btn-danger" runat="server" Text="Close" />
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
